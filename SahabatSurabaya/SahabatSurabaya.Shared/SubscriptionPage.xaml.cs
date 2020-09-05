using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SahabatSurabaya
{

    public sealed partial class SubscriptionPage : Page
    {
        List<string> listMonthExpired, listYearExpired;
        public SubscriptionPage()
        {
            this.InitializeComponent();
            listMonthExpired = new List<string>();
            listYearExpired = new List<string>();
            for (int i = 1; i <= 12; i++)
            {
                string month = (i > 9) ? i.ToString() : month = "0" + i;
                listMonthExpired.Add(month);
            }
            for (int i = 20; i <= 40; i++)
            {
                listYearExpired.Add(i.ToString());
            }
            cbExpiredMonth.ItemsSource = listMonthExpired;
            cbExpiredYear.ItemsSource = listYearExpired;
        }

        private async void confirmUpgrade(object sender, RoutedEventArgs e)
        {
            if (cbExpiredYear.SelectedIndex == -1 || cbExpiredMonth.SelectedIndex == 1)
            {
                var message = new MessageDialog("Pastikan Semua Data Kartu Terisi");
                await message.ShowAsync();
            }
            else
            {
                using (var client = new HttpClient())
                {
                    string cardNumber, cardExpMonth, cardExpYear, clientKey;
                    clientKey = "SB-Mid-client-J4xpVwGv_HmNID-g";
                    cardNumber = "4811 1111 1111 1114";
                    cardExpMonth = cbExpiredMonth.SelectedItem.ToString();
                    cardExpYear = "20" + cbExpiredYear.SelectedItem.ToString();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                    client.DefaultRequestHeaders.Add("Authorization", "Basic U0ItTWlkLXNlcnZlci1GQjRNSERieVhlcFc5OFNRWjY0SHhNeEU=");
                    string reqUri = "https://api.sandbox.midtrans.com/v2/card/register?card_number=" + cardNumber + "&card_exp_month=" + cardExpMonth + "&card_exp_year=" + cardExpYear + "&client_key=" + clientKey;
                    HttpResponseMessage response = await client.GetAsync(reqUri);
                    if (response.IsSuccessStatusCode)
                    {
                        string idUser = "1";
                        var hasil = response.Content.ReadAsStringAsync().Result;
                        JObject json = JObject.Parse(hasil);
                        string tokenId = json["saved_token_id"].ToString();
                        using (var client2 = new HttpClient())
                        {
                            client2.BaseAddress = new Uri("http://localhost:8080/user/");
                            var content = new FormUrlEncodedContent(new[]
                                {
                                    new KeyValuePair<string, string>("id_user", idUser),
                                    new KeyValuePair<string, string>("credit_card_token", tokenId),
                                });
                            HttpResponseMessage response2 = await client2.PutAsync("updateCreditCardToken",content);
                            if (response2.IsSuccessStatusCode)
                            {
                                var content2 = new FormUrlEncodedContent(new[]
                                {
                                    new KeyValuePair<string, string>("id_user", idUser)
                                });
                                response2 = await client2.PostAsync("chargeUser",content2);
                                if (response2.IsSuccessStatusCode)
                                {
                                    var message = new MessageDialog("");
                                    string subscribeResponse = response2.Content.ReadAsStringAsync().Result;
                                    if (subscribeResponse == "200")
                                    {
                                        message.Content = "Berhasil Berlangganan";
                                    }
                                    else
                                    {
                                        message.Content = "Proses Berlangganan Bermasalah, Silahkan coba lagi beberapa saat";
                                    }
                                    await message.ShowAsync();
                                }
                                else
                                {
                                    var message = new MessageDialog("Gagal Berlangganan");
                                    await message.ShowAsync();
                                }
                            }
                            else
                            {
                                var message = new MessageDialog(response2.StatusCode.ToString());
                                await message.ShowAsync();
                            }
                        }   
                    }
                    else
                    {
                        var message = new MessageDialog(response.StatusCode.ToString());
                        await message.ShowAsync();
                    }
                }
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            On_BackRequested();
        }

        private bool On_BackRequested()
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
                return true;
            }
            return false;
        }
    }
}
