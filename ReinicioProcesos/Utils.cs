using System;
using System.Configuration;

namespace ReinicioProcesos
{
    public static class Utils
    {
        public static string GetValueAppSettings(string key)
        {
            if (string.IsNullOrEmpty(key))
                return null;

            try
            {
                ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");

                if (ConfigurationManager.AppSettings[key] == null || string.IsNullOrEmpty(ConfigurationManager.AppSettings[key]))
                    throw new Exception($"¡Error!: la configuración {key} en App.config");

                return ConfigurationManager.AppSettings[key].ToString();
            }
            catch (Exception e)
            {
                throw new Exception($"Error: {e.Message}");
            }
        }
    }
}
