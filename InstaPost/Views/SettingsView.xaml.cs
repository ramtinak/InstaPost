using CoreLib.Helpers;
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
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
        }
        public void Load()
        {
            if (Helper.Settings == null)
                return;
            IPText.Text = Helper.Settings.ProxyIP;
            PortText.Text = Helper.Settings.ProxyPort;
            UseProxyToggle.IsChecked = Helper.Settings.UseProxy;
        }
        private void UseProxyToggleChecked(object sender, RoutedEventArgs e)
        {
            UseProxyToggle.IsChecked.Value.Output();
            if (ProxyGrid != null)
                Helper.Settings.UseProxy = ProxyGrid.IsEnabled = UseProxyToggle.IsChecked.Value;
        }

        private async void TestConnectionButtonClick(object sender, RoutedEventArgs e)
        {
            IPText.Text = IPText.Text.Trim();
            PortText.Text = PortText.Text.Trim(); if (Helper.Settings == null)
                return;
            if (!Helper.CheckIPAddress(IPText.Text))
            {
                Helper.ShowMsg("Please type a valid IP address.", "ERR", MessageBoxImage.Error);
                return;
            }
            if (!Helper.CheckPortAddress(PortText.Text))
            {
                Helper.ShowMsg("Please type a valid Port.", "ERR", MessageBoxImage.Error);
                return;
            }
            var checkProxy = await HttpHelper.CheckProxyAsync(IPText.Text, PortText.Text);
            if (checkProxy)
            {

                Helper.ShowMsg("This proxy is working with Instagram.com.", "Success", MessageBoxImage.Information);
                if (Helper.Settings == null)
                    return;
                Helper.Settings.ProxyIP = IPText.Text;
                Helper.Settings.ProxyPort = PortText.Text;
                Helper.Settings.UseProxy = UseProxyToggle.IsChecked.Value;
            }
            else
                Helper.ShowMsg("This proxy is NOT working with Instagram.com.\n" +
                    "If you leave 'Use Proxy' enable, app cannot connect to Instagram.", "ERR", MessageBoxImage.Error);
        }
    }
}
