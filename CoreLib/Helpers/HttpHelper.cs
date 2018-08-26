using CoreLib.Helpers;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
namespace CoreLib.Helpers
{
    class HttpHelper
    {
        const string INSTAGRAM_USER_AGENT =
            "Instagram 35.0.0.20.96 Android (24/7.0; 640dpi; 1440x2560; samsung; SM-G935F; hero2lte; samsungexynos8890; en_US; 95414346)";

        const string USER_AGENT =
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.87 Safari/537.36 OPR/54.0.2952.71";

        public static async Task<bool> CheckProxyAsync(string ip, int port)
        {
            return await CheckProxyAsync(ip, port.ToString());
        }

        public static async Task<bool> CheckProxyAsync(string ip, string port)
        {
            try
            {
                var proxy = new WebProxy()
                {
                    Address = new Uri($"http://{ip}:{port}"),
                    BypassProxyOnLocal = false,
                    UseDefaultCredentials = false
                };
                var httpClientHandler = new HttpClientHandler()
                {
                    Proxy = proxy,
                };
                using (HttpClient client = new HttpClient(httpClientHandler))
                {
                    client.DefaultRequestHeaders.Add("User-Agent", INSTAGRAM_USER_AGENT);
                    using (var request = new HttpRequestMessage(HttpMethod.Get, new Uri("https://i.instagram.com")))
                    using (HttpResponseMessage response = await client.SendAsync(request))
                    {
                        response.EnsureSuccessStatusCode();
                        response.StatusCode.Output();
                        string responseBody = await response.Content.ReadAsStringAsync();
                        responseBody.Output();
                        return true;
                    }
                }
            }
            catch { }
            return false;
        }
        
        public static async Task<bool> CheckFacebookAsync()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", USER_AGENT);
                    using (var request = new HttpRequestMessage(HttpMethod.Get, new Uri("https://www.facebook.com")))
                    using (HttpResponseMessage response = await client.SendAsync(request))
                    {
                        response.EnsureSuccessStatusCode();
                        response.StatusCode.Output();
                        await response.Content.ReadAsStringAsync();                        
                        return response.StatusCode == HttpStatusCode.OK;
                    }
                }
            }
            catch { }
            return false;
        }
    }
}
