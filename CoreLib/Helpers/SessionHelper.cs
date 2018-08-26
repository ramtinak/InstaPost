using InstagramApiSharp.API.Builder;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Logger;
using InstaPost;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Helpers
{
    public class SessionHelper
    {
        public static readonly string AccountPath = Path.Combine(Helper.LocalPath, "Accounts");
        static readonly string SessionPath = Path.Combine(Helper.LocalPath, "session.dat");
        public static void CreateInstaDeskDirectory()
        {
            if (!Directory.Exists(Helper.LocalPath))
                Directory.CreateDirectory(Helper.LocalPath);
        }
        public static async Task<bool> LoadAndLogin()
        {
            if (!File.Exists(SessionPath))
                return false;
            try
            {
                var userSession = new UserSessionData
                {
                    UserName = "Username",
                    Password = "Password"
                };
                if (Helper.Settings != null && Helper.Settings.UseProxy && !string.IsNullOrEmpty(Helper.Settings.ProxyIP) && !string.IsNullOrEmpty(Helper.Settings.ProxyPort))
                {
                    var proxy = new WebProxy()
                    {
                        Address = new Uri($"http://{Helper.Settings.ProxyIP}:{Helper.Settings.ProxyPort}"),
                        BypassProxyOnLocal = false,
                        UseDefaultCredentials = false,
                    };
                    var httpClientHandler = new HttpClientHandler()
                    {
                        Proxy = proxy,
                    };
                    Helper.InstaApi = InstaApiBuilder.CreateBuilder()
                        .SetUser(userSession)
                        .UseLogger(new DebugLogger(LogLevel.Exceptions))
                        .UseHttpClientHandler(httpClientHandler)
                        .Build();
                }
                else
                {
                    Helper.InstaApi = InstaApiBuilder.CreateBuilder()
                        .SetUser(userSession)
                        .UseLogger(new DebugLogger(LogLevel.Exceptions))
                        .Build();
                }
                var text = LoadSession();
                Helper.InstaApi.LoadStateDataFromString(text);
                if (!Helper.InstaApi.IsUserAuthenticated)
                    await Helper.InstaApi.LoginAsync();

                $"Us: {Helper.InstaApi.GetLoggedUser().UserName}".Output();
                return true;
            }
            catch (Exception ex)
            {
                ex.PrintException("LoadAndLogin");
                Helper.InstaApi = null;
            }
            return false;
        }
        public static void DeleteCurrentSession()
        {
            if (!File.Exists(SessionPath))
                return;
            try
            {
                File.Delete(SessionPath);
            }
            catch { }
        }
        public static void SaveCurrentSession()
        {
            if (Helper.InstaApi == null)
                return;
            if (!Helper.InstaApi.IsUserAuthenticated)
                return;
            try
            {
                var state = Helper.InstaApi.GetStateDataAsString();
                File.WriteAllText(SessionPath, state);
            }
            catch (Exception ex) { ex.PrintException("SaveCurrentSession"); }
        }

        public static string LoadSession()
        {
            try
            {
                var text = File.ReadAllText(SessionPath);

                return text;
            }
            catch (Exception ex) { ex.PrintException("LoadSession"); }
            return null;
        }
    }
}
