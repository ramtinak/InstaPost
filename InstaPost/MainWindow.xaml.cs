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
            //Media.MediaOpened += Media_MediaOpened;
        }

        private async void Media_MediaOpened(object sender, RoutedEventArgs e)
        {
            //var info = Media.MediaInfo;

            //var json = Newtonsoft.Json.JsonConvert.SerializeObject(info, Newtonsoft.Json.Formatting.Indented);
            ////json.Output();
            //var sx = "";
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SettingsHelper.Save();
            CachedHelper.DeleteCachedFiles();
        }

        private /*async*/ void WindowLoaded(object sender, RoutedEventArgs e)
        {
            new Thread(() => { Helper.FFmpeg = new FFmpegFa.FFmpeg(); }).Start();
            Helper.LocalPath.Output();
            Helper.CreateDirectory();
            SettingsHelper.Load();
            SettingsView.Load();
            SignInView.Load();
            if (Helper.InstaApi != null && Helper.InstaApi.IsUserAuthenticated)
                PostView.IsEnabled = true;
            //ImageHelper.ConvertToJPEG(@"I:\Images\RMT.png");
            //ImageHelper.ConvertToJPEG(@"C:\Users\Ramti\Downloads\Wrong.bmp");
            //Helper.Theme.IsAlternate = true;
            //Media.Source = new Uri(@"D:\Videos\Ahllam - Behtarin(FazMusic)(720).mp4");
            //Media.Source = new Uri(@"I:\Images\spider-man-homecoming-1280x768-4k-poster-14293.jpg");

                //Media.Source = new Uri(@"I:\Images\RMT.png");
                //await  Media.Play();
                //FileSelectorHelper.SelectFiles();

        }
    }
}
