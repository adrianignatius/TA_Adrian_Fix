using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SahabatSurabaya
{
    public sealed partial class VerifyOtpPage : Page
    {
        Session session;
        User userRegister;
        DispatcherTimer timer;
        int countdown = 30;
        public VerifyOtpPage()
        {
            this.InitializeComponent();
            txtOtp.Focus(FocusState.Keyboard);
            session = new Session();
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += Timer_Tick;
            updateTxtTimer();
            timer.Start();
            
        }
        
        private void updateTxtTimer()
        {
            if (countdown < 10)
            {
                txtTimer.Text = "00:0" + countdown;
            }
            else
            {
                txtTimer.Text = "00:" + countdown;
            }
            
        }

        private void Timer_Tick(object sender, object e)
        {
            if (countdown == 0)
            {
                timer.Stop();
                txtTimer.Visibility = Visibility.Collapsed;
            }
            else
            {
                countdown--;
                updateTxtTimer();
            }
        }

        private void pageLoaded(object sender,RoutedEventArgs e)
        {
            userRegister = session.getUserLogin();
            sendOTP();
        }

        private async void sendOTP()
        {
            using (var client = new HttpClient())
            {
               client.BaseAddress = new Uri(session.getApiURL());
                //client.BaseAddress = new Uri("http://localhost:8080/");
               client.DefaultRequestHeaders.Accept.Clear();
               client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
               MultipartFormDataContent form = new MultipartFormDataContent();
               form.Add(new StringContent(userRegister.telpon_user), "number");
               await client.PostAsync("user/sendOTP", form);
            }
        }

        private async void sendOTP(object sender, RoutedEventArgs e)
        {
            if (countdown > 0)
            {
                var message = new MessageDialog("Tunggu beberapa saat lagi untuk request kode baru");
                await message.ShowAsync();
            }
            else
            {
                countdown = 30;
                timer.Start();
                txtTimer.Visibility = Visibility.Visible;
                sendOTP();
                updateTxtTimer();
            }
            
        }

        private async void confirmOTP(object sender,RoutedEventArgs e)
        {
            if (txtOtp.Text.Length != 0)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(session.getApiURL());
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    MultipartFormDataContent form = new MultipartFormDataContent();
                    form.Add(new StringContent(userRegister.telpon_user), "number");
                    form.Add(new StringContent(txtOtp.Text), "otp_code");
                    HttpResponseMessage response = await client.PostAsync("user/verifyOTP", form);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = response.Content.ReadAsStringAsync().Result;
                        JObject json = JObject.Parse(responseData);
                        if (json["status"].ToString() == "1")
                        {
                            var message = new MessageDialog(json["message"].ToString());
                            await message.ShowAsync();
                            this.Frame.Navigate(typeof(HomeNavigationPage));
                        }
                        else
                        {
                            var message = new MessageDialog(json["message"].ToString());
                            await message.ShowAsync();

                        }
                    }
                }
            }
            else
            {
                var message = new MessageDialog("Anda belum memasukkan kode");
                await message.ShowAsync();
            }
        }

        private void txtOtpBeforeChangingEvent(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            args.Cancel = args.NewText.Any(c => !char.IsDigit(c));
        }
        
    }
}
