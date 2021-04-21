using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace ReinicioProcesos
{
    public class EmailSender
    {
        private SmtpClient smtpClient;
        private MailMessage mailMessage; 

        public EmailSender(string subject)
        {
            if (Utils.GetValueAppSettings("MUST_SEND_EMAIL").Equals("0"))
            {
                smtpClient = new SmtpClient();
                mailMessage = new MailMessage();
                return;
            }

            string email = Utils.GetValueAppSettings("ADDRESS_EMAIL_CONFIRM");
            string displayName = Utils.GetValueAppSettings("DISPLAY_NAME_EMAIL_CONFIRM");
            string pswd = Utils.GetValueAppSettings("PSWD_EMAIL_CONFIRM");
            string port = Utils.GetValueAppSettings("SMTP_PORT_EMAIL_CONFIRM");
            string server = Utils.GetValueAppSettings("SMTP_SERVER_EMAIL_CONFIRM");

            smtpClient = new SmtpClient
            {
                Credentials = new NetworkCredential(email, pswd),
                Port = Convert.ToInt32(port), 
                Host = server
            };

            mailMessage = new MailMessage
            {
                From = new MailAddress(email, displayName),
                IsBodyHtml = true,
                Subject = subject,
                BodyEncoding = Encoding.UTF8,
                HeadersEncoding = Encoding.UTF8
            };

            List<string> receipts = GetEmailRecipients();
            if (receipts == null)
                return;

            foreach (string a in receipts)
                mailMessage.To.Add(a);
        }

        public void Send()
        {
            try
            {
                if(smtpClient != null)
                    smtpClient.Send(mailMessage);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public EmailSender SetEmailTextBody(string textBody)
        {
            if (mailMessage != null && smtpClient != null)
            {
                mailMessage.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(textBody, new System.Net.Mime.ContentType("text/plain;charset=utf-8;")));
                return this;
            }
            return null;
        }

        public EmailSender SetEmailHtmlBody(string htmlBody)
        {
            if(mailMessage != null && smtpClient != null)
            {
                mailMessage.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(htmlBody, new System.Net.Mime.ContentType("text/html;charset=utf-8;")));
                return this; 
            }
            return null;
        }

        private List<string> GetEmailRecipients()
        {
            string addresses = Utils.GetValueAppSettings("RECIPIENTS_EMAIL_INFO");
            if(string.IsNullOrEmpty(addresses)){
                Console.WriteLine("No existen direcciones de correo electrónico para informar");
                return null; 
            }

            List<string> adds = new List<string>();
            foreach(string a in addresses.Split(','))
            {
                string aux = a.Replace(" ", "");
                adds.Add(aux);
            }

            return adds;
        }
    }
}
