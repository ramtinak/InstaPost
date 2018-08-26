using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
namespace CoreLib.Classes
{ 
    public class RmtTheme
    {
        bool _isDarkMode = false;
        public bool IsDarkMode
        {
            get { return _isDarkMode; }
            set
            {
                _isDarkMode = value;
                SetDarkMode(value);
            }
        }
        bool _isAlternate = false;
        public bool IsAlternate
        {
            get { return _isAlternate; }
            set
            {
                _isAlternate = value;
                SetAlternate(value);
            }
        }
        Swatch _primarySwatch;
        public Swatch PrimarySwatch
        {
            get { return _primarySwatch; }
            set
            {
                _primarySwatch = value;
                SetPrimarySwatch(value);
            }
        }
        Swatch _accentSwatch;
        public Swatch AccentSwatch
        {
            get { return _accentSwatch; }
            set
            {
                _accentSwatch = value;
                SetAccentSwatch(value);
            }
        }

        public static void SetDarkMode(bool isDark)
        {
            new PaletteHelper().SetLightDark(isDark);
        }
        public static void SetAlternate(bool alternate)
        {
            var resourceDictionary = new ResourceDictionary
            {
                Source = new Uri(@"pack://application:,,,/Dragablz;component/Themes/materialdesign.xaml")
            };

            var styleKey = alternate ? "MaterialDesignAlternateTabablzControlStyle" : "MaterialDesignTabablzControlStyle";
            var style = (Style)resourceDictionary[styleKey];

            foreach (var tabablzControl in Dragablz.TabablzControl.GetLoadedInstances())
            {
                tabablzControl.Style = style;
            }
        }
        public static void SetPrimarySwatch(Swatch swatch)
        {
            if (swatch == null)
                return;
            new PaletteHelper().ReplacePrimaryColor(swatch);
        }
        public static void SetAccentSwatch(Swatch swatch)
        {
            if (swatch == null)
                return;
            new PaletteHelper().ReplaceAccentColor(swatch);
        }
    }
}
