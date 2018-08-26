using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace CoreLib.Helpers
{
    public class Settings
    {
        public bool UseProxy { get; set; } = false;
        public string ProxyIP { get; set; }
        public string ProxyPort { get; set; }
        

        public string LastUsername { get; set; }
    }
    class SettingsHelper
    {
        static readonly string SettingsPath = Path.Combine(Helper.LocalPath, "settings.bin");

        public static void Save()
        {
            if (Helper.Settings == null)
                return;

            try
            {
                var json = JsonConvert.SerializeObject(Helper.Settings);
                File.WriteAllText(SettingsPath, json);
            }
            catch { }

        }
        public static void Load()
        {
            if(!File.Exists(SettingsPath))
            {
                Helper.Settings = new Settings();
                return;
            }

            try
            {
                var json = File.ReadAllText(SettingsPath);
                var settings = JsonConvert.DeserializeObject<Settings>(json);
                Helper.Settings = settings;
            }
            catch { Helper.Settings = new Settings(); }
        }
    }
}
