using CoreLib.Helpers;
using InstagramApiSharp.API.Builder;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for SignInView.xaml
    /// </summary>
    public partial class SignInView : UserControl
    {
        public SignInView()
        {
            InitializeComponent();
        }
        public async void Load()
        {
            var login = await SessionHelper.LoadAndLogin();
            if (login && Helper.InstaApi != null)
                Connected();
            if (Helper.InstaApi != null)
            {
                Connected();
                UsernameText.Text = Helper.InstaApi.GetLoggedUser().UserName;
                PasswordText.Password = Helper.InstaApi.GetLoggedUser().Password;
            }            
        }
        private async void LoginButtonClick(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(UsernameText.Text))
            {
                Helper.ShowMsg("Please type your username.", "ERR", MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrEmpty(PasswordText.Password))
            {
                Helper.ShowMsg("Please type your password.", "ERR", MessageBoxImage.Error);
                return;
            }
            Helper.AppName.ChangeAppTitle();
            try
            {
                var userSession = new UserSessionData
                {
                    UserName = UsernameText.Text,
                    Password = PasswordText.Password
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
                    "Using proxy".Output();
                }
                else
                {
                    Helper.InstaApi = InstaApiBuilder.CreateBuilder()
                        .SetUser(userSession)
                        .UseLogger(new DebugLogger(LogLevel.Exceptions))
                        .Build();
                }
                $"{Helper.AppName} - Connecting...".ChangeAppTitle();
                var loginResult = await Helper.InstaApi.LoginAsync();
                if (loginResult.Succeeded)
                {
                    Connected();
                    SessionHelper.SaveCurrentSession();
                }
                else
                {
                    switch (loginResult.Value)
                    {
                        case InstaLoginResult.InvalidUser:
                            Helper.AppName.ChangeAppTitle();
                            Helper.ShowMsg("Username is invalid.", "ERR", MessageBoxImage.Error);
                            break;
                        case InstaLoginResult.BadPassword:
                            Helper.AppName.ChangeAppTitle();
                            Helper.ShowMsg("Password is wrong.", "ERR", MessageBoxImage.Error);
                            break;
                        case InstaLoginResult.Exception:
                            Helper.AppName.ChangeAppTitle();
                            Helper.ShowMsg("Exception throws:\n" + loginResult.Info?.Message, "ERR", MessageBoxImage.Error);
                            break;
                        case InstaLoginResult.LimitError:
                            Helper.AppName.ChangeAppTitle();
                            Helper.ShowMsg("Limit error (you should wait 10 minutes).", "ERR", MessageBoxImage.Error);
                            break;
                        case InstaLoginResult.ChallengeRequired:
                            Helper.AppName.ChangeAppTitle();
                            LoginGrid.Visibility = Visibility.Collapsed;
                            HandleChallenge();
                            break;
                        case InstaLoginResult.TwoFactorRequired:
                            Helper.AppName.ChangeAppTitle();
                            LoginGrid.Visibility = Visibility.Collapsed;
                            TwoFactorGrid.Visibility = Visibility.Visible;
                            break;
                    }

                
                }
            }
            catch(Exception ex) { ex.PrintException("LoginButtonClick"); }
        }

        private void FacebookLoginButtonClick(object sender, RoutedEventArgs e)
        {
            Helper.ShowMsg("Will be availabe in future updates.");
        }


        void Connected()
        {
            if (Helper.InstaApi != null)
            {
                var username = Helper.InstaApi.GetLoggedUser().UserName;
                LoginText.Text = $"Logged in as '{username}'";
                $"{Helper.AppName} - {username} - Connected".ChangeAppTitle();
                LogoutGrid.Visibility = Visibility.Visible;
                LoginGrid.Visibility = Visibility.Collapsed;
                MainWindow.Current.PostView.IsEnabled = true;
            }
        }

        #region challenge require functions
        async void HandleChallenge(bool resend = false)
        {
            try
            {
                IResult<ChallengeRequireVerifyMethod> challenge = null;
                if (!resend)
                    challenge = await Helper.InstaApi.GetChallengeRequireVerifyMethodAsync();
                else
                    challenge = await Helper.InstaApi.ResetChallengeRequireVerifyMethodAsync();
                if (challenge.Succeeded)
                {
                    if (challenge.Value.StepData != null)
                    {
                        ChallengePhoneNumberRadio.Visibility =
                            ChallengeEmailRadio.Visibility = Visibility.Collapsed;
                        if (!string.IsNullOrEmpty(challenge.Value.StepData.PhoneNumber))
                        {
                            if (!resend)
                                ChallengePhoneNumberRadio.IsChecked = false;
                            ChallengePhoneNumberRadio.Visibility = Visibility.Visible;
                            ChallengePhoneNumberRadio.Content = challenge.Value.StepData.PhoneNumber;
                        }
                        if (!string.IsNullOrEmpty(challenge.Value.StepData.Email))
                        {
                            if (!resend)
                                ChallengeEmailRadio.IsChecked = false;
                            ChallengeEmailRadio.Visibility = Visibility.Visible;
                            ChallengeEmailRadio.Content = challenge.Value.StepData.Email;
                        }
                        if (!resend)
                            Challenge1Grid.Visibility = Visibility.Visible;
                    }
                }
                else
                    Helper.ShowMsg(challenge.Info.Message, "ERR", MessageBoxImage.Error);
            }
            catch (Exception ex) { ex.PrintException("HandleChallenge"); }
        }
        private async void ChallengeSendCodeButtonClick(object sender, RoutedEventArgs e)
        {
            if (Helper.InstaApi == null)
                return;
            bool isEmail = false;
            if (ChallengeEmailRadio.IsChecked.Value)
                isEmail = true;
            try
            {
                // Note: every request to this endpoint is limited to 60 seconds                 
                if (isEmail)
                {
                    var email = await Helper.InstaApi.RequestVerifyCodeToEmailForChallengeRequireAsync();
                    if (email.Succeeded)
                    {
                        SmsEmailText.Text = $"We sent verify code to this email:\n{email.Value.StepData.ContactPoint}";
                        Challenge1Grid.Visibility = Visibility.Collapsed;
                        Challenge2Grid.Visibility = Visibility.Visible;
                    }
                    else
                        Helper.ShowMsg(email.Info.Message, "ERR", MessageBoxImage.Error);
                }
                else
                {
                    var phoneNumber = await Helper.InstaApi.RequestVerifyCodeToSMSForChallengeRequireAsync();
                    if (phoneNumber.Succeeded)
                    {
                        SmsEmailText.Text = $"We sent verify code to this phone number(it's end with this):\n{phoneNumber.Value.StepData.ContactPoint}";
                        Challenge1Grid.Visibility = Visibility.Collapsed;
                        Challenge2Grid.Visibility = Visibility.Visible;
                    }
                    else
                        Helper.ShowMsg(phoneNumber.Info.Message, "ERR", MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                Helper.ShowMsg(ex.Message, "ERR", MessageBoxImage.Error);
                ex.PrintException("ChallengeSendCodeButtonClick");
            }

        }
        private async void ChallengeResendCodeButtonClick(object sender, RoutedEventArgs e)
        {
            if (Helper.InstaApi == null)
                return;
            try
            {
                var reset = await Helper.InstaApi.ResetChallengeRequireVerifyMethodAsync();
                reset.Succeeded.Output();
                HandleChallenge(true);
            }
            catch (Exception ex)
            {
                Helper.ShowMsg(ex.Message, "ERR", MessageBoxImage.Error);
                ex.PrintException("ChallengeResendCodeButtonClick");
            }
        }
        private async void ChallengeVerifyCodeButtonClick(object sender, RoutedEventArgs e)
        {
            if (Helper.InstaApi == null)
                return;
            try
            {
                ChallengeVerificationCodeText.Text = ChallengeVerificationCodeText.Text.Trim();
                ChallengeVerificationCodeText.Text = ChallengeVerificationCodeText.Text.Replace(" ", "");
                var regex = new Regex(@"^-*[0-9,\.]+$");
                if (!regex.IsMatch(ChallengeVerificationCodeText.Text))
                {
                    Helper.ShowMsg("Verification code is numeric!!!", "ERR", MessageBoxImage.Error);
                    return;
                }
                if (ChallengeVerificationCodeText.Text.Length != 6)
                {
                    Helper.ShowMsg("Verification code must be 6 digits!!!", "ERR", MessageBoxImage.Error);
                    return;
                }
                var verify = await Helper.InstaApi.VerifyCodeForChallengeRequireAsync(ChallengeVerificationCodeText.Text);
                if (verify.Succeeded)
                {
                    Challenge1Grid.Visibility = Challenge2Grid.Visibility = Visibility.Collapsed;
                    ChallengeVerificationCodeText.Text = "";
                    SessionHelper.SaveCurrentSession();
                    Connected();

                }
                else
                {
                    if (verify.Value == InstaLoginResult.TwoFactorRequired)
                    {
                        LoginGrid.Visibility = Challenge1Grid.Visibility = Challenge2Grid.Visibility = Visibility.Collapsed;
                        ChallengeVerificationCodeText.Text = "";

                        TwoFactorVerificationCodeText.Text = "";
                        Helper.AppName.ChangeAppTitle();
                        TwoFactorGrid.Visibility = Visibility.Visible;
                    }
                }
                    Helper.ShowMsg(verify.Info.Message, "ERR", MessageBoxImage.Error);
            }
            catch (Exception ex) { ex.PrintException("ChallengeVerifyCodeButtonClick"); }
        }


        #endregion challenge require functions

        private async void TwoFactorVerifyCodeButtonClick(object sender, RoutedEventArgs e)
        {
            if (Helper.InstaApi == null)
                return;
            if (string.IsNullOrEmpty(TwoFactorVerificationCodeText.Text))
            {
                Helper.ShowMsg("Please type your two factor code and then press Auth button.", "ERR", MessageBoxImage.Error);
                return;
            }
            try
            {
                // send two factor code
                var twoFactorLogin = await Helper.InstaApi.TwoFactorLoginAsync(TwoFactorVerificationCodeText.Text);
                if (twoFactorLogin.Succeeded)
                {
                    Connected();
                    SessionHelper.SaveCurrentSession();
                    TwoFactorGrid.Visibility = Visibility.Collapsed;
                    TwoFactorVerificationCodeText.Text = "";
                }
                else
                {
                    Helper.ShowMsg(twoFactorLogin.Info.Message, "ERR", MessageBoxImage.Error);
                }
            }
            catch (Exception ex) { ex.PrintException("TwoFactorVerifyCodeButton_Click"); }
        }

        private async void LogoutButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                await Helper.InstaApi.LogoutAsync();
                SessionHelper.DeleteCurrentSession();
                LogoutGrid.Visibility = Visibility.Collapsed;
                LoginGrid.Visibility = Visibility.Visible;
                MainWindow.Current.PostView.IsEnabled = false;

            }
            catch
            {
                SessionHelper.DeleteCurrentSession();
                LogoutGrid.Visibility = Visibility.Collapsed;
                LoginGrid.Visibility = Visibility.Visible;
                MainWindow.Current.PostView.IsEnabled = false;
            }
        }
    }
}
