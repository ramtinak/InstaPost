using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace InstaPost.Views
{
    /// <summary>
    /// Interaction logic for AboutView.xaml
    /// </summary>
    public partial class AboutView : UserControl
    {
        public AboutView()
        {
            InitializeComponent();
        }

        private void MailHyperClick(object sender, RoutedEventArgs e)
        {
            "mailto:ramtinak@live.com".OpenUrl();
        }

        private void TelegramHyperClick(object sender, RoutedEventArgs e)
        {
            "https://t.me/Ramtinak".OpenUrl();
        }

        private void HyperlinkClick(object sender, RoutedEventArgs e)
        {
            "https://github.com/ramtinak/InstagramApiSharp".OpenUrl();
        }

        private void MahAppHyperClick(object sender, RoutedEventArgs e)
        {
            "https://github.com/MahApps/MahApps.Metro".OpenUrl();
        }

        private void MaterialDesignHyperClick(object sender, RoutedEventArgs e)
        {
            "https://github.com/ButchersBoy/MaterialDesignInXamlToolkit".OpenUrl();
        }

        private void DragablzHyperClick(object sender, RoutedEventArgs e)
        {
            "http://dragablz.net".OpenUrl();
        }

        private void JsonNETHyperClick(object sender, RoutedEventArgs e)
        {
            "https://github.com/JamesNK/Newtonsoft.Json".OpenUrl();
        }

        private void FFmpegFaHyperClick(object sender, RoutedEventArgs e)
        {
            "https://github.com/ramtinak/FFmpegFa".OpenUrl();
        }
    }
}
