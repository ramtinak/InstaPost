using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using CoreLib.Classes;
using MaterialDesignColors;
using Newtonsoft.Json;
namespace CoreLib.Helpers
{
    public static class ThemeHelper
    {
        static readonly string ThemePath = Path.Combine(Helper.LocalPath, "theme.bin");
        public static void SaveTheme()
        {
            if (Helper.Theme == null)
            {
                "SaveTheme(current theme is empty)".Output();
                return;
            }
            try
            {
                var json = JsonConvert.SerializeObject(Helper.Theme);
                File.WriteAllText(ThemePath, json, Encoding.UTF8);
            }
            catch { }
        }
        public static void LoadTheme()
        {
            try
            {
                ThemePath.Output();
                if (!File.Exists(ThemePath))
                {
                    "LoadTheme(theme file is not exist)".Output();
                    var swatch = new SwatchesProvider().Swatches.FirstOrDefault(sw => sw.Name == "pink");
                    Helper.Theme = new RmtTheme
                    {
                        PrimarySwatch = swatch,
                        IsAlternate = true,
                        //IsDarkMode = true
                    };
                    return;
                }
                var json = File.ReadAllText(ThemePath);
                Helper.Theme = JsonConvert.DeserializeObject<RmtTheme>(json);
            }
            catch { }
            finally
            {
                if (Helper.Theme == null)
                    Helper.Theme = new RmtTheme();
            }
        }
    }
}
