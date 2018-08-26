using CoreLib.Helpers;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace InstaPost
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public static MainWindow Current;
        public MainWindow()
        {
            InitializeComponent();
            Current = this;
            ThemeHelper.LoadTheme();
            Loaded += WindowLoaded;
            Closing += WindowClosing;
        }
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SettingsHelper.Save();
            CachedHelper.DeleteCachedFiles();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            new Thread(() => { Helper.FFmpeg = new FFmpegFa.FFmpeg(); }).Start();
            Helper.LocalPath.Output();
            Helper.CreateDirectory();
            SettingsHelper.Load();
            SettingsView.Load();
            SignInView.Load();
            if (Helper.InstaApi != null && Helper.InstaApi.IsUserAuthenticated)
                PostView.IsEnabled = true;
        }
    }
}
