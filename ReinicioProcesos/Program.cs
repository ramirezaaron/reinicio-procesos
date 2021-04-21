using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ReinicioProcesos
{
    class Program
    {
        private string KillProcess(Process proc)
        {
            string result;
            try
            {
                result = $"'{proc.ProcessName}': OK";
                proc.Kill();
            }
            catch (Exception ex)
            {
                result = $"No se pudo matar el proceso {proc.ProcessName}. Causa: {ex.Message}";
            }

            Console.WriteLine(result);
            return result;
        }

        private void GiveLiveToProcess(List<string> processList)
        {
            try
            {
                // TODO: iniciar de nuevo el proceso (o procesos) 
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private Process[] GetProcessCollectionByName(string name)
        {
            Process[] ps = Process.GetProcessesByName(name);
            if (ps == null || ps.Length == 0)
                return null;

            return ps;
        }

        static void Main(string[] args)
        {
            Program program = new Program();

            Console.Title = "Reinicio de procesos - Por Aarón Ramírez";
            string processListString = Utils.GetValueAppSettings("LIST_KILLABLES");
            processListString = processListString.Replace(" ", "");

            string[] tokensProcessNames = processListString.Split(',');
            List<string> resultMessages = new List<string>();

            StringBuilder messageBody = new StringBuilder("");
            messageBody.Append("Hola :) ");
            messageBody.Append("\n\n");
            messageBody.Append("Se han reiniciado los procesos: ");
            messageBody.Append("\n\n");
            messageBody.Append("=================\n");

            byte counter = 0;
            bool hasKilledSomeProcess = false; 
            foreach (string p in tokensProcessNames)
            {
                Process[] targetProcess = program.GetProcessCollectionByName(p);
                if (targetProcess == null)
                    continue;

                foreach(Process proc in targetProcess)
                {
                    string result = program.KillProcess(proc);
                    resultMessages.Add(result);
                    messageBody.Append($"{++counter}: {result}");
                    messageBody.Append("\n");
                    hasKilledSomeProcess = true; 
                }
            }

            if (!hasKilledSomeProcess)
            {
                Console.WriteLine("No se encontraron procesos activos. Nada por hacer.");
                Console.ReadLine();
                return;
            }

            messageBody.Append("\n\n");
            messageBody.Append($"Esta operación fue realizada el {DateTime.Now:yyy-MM-dd HH:mm:ss}\n\n");

            try
            {
                EmailSender emailSender = new EmailSender($"Terminación de procesos");
                emailSender
                    .SetEmailHtmlBody(messageBody.ToString())
                    .SetEmailTextBody(messageBody.ToString())
                    .Send();
                Console.WriteLine("Se envió correo electrónico");
            }
            catch (Exception)
            {
                Console.WriteLine("No se envió el correo electrónico");
            }

            Console.ReadLine();
        }
    }
}
