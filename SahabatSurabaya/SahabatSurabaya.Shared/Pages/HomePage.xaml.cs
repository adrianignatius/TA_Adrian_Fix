﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SahabatSurabaya.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Xamarin.Essentials;
#if __ANDROID__
using Com.OneSignal;
using Com.OneSignal.Abstractions;
#endif

namespace SahabatSurabaya
{

    public sealed partial class HomePage : Page
    {
        Session session;
        User userLogin;
        HttpObject httpObject;
        ObservableCollection<LaporanLostFound> listLaporanLostFound;
        ObservableCollection<LaporanKriminalitas> listLaporanKriminalitas;
        ObservableCollection<User> listEmergencyContact;
        public HomePage()
        {
            this.InitializeComponent();
            listLaporanLostFound = new ObservableCollection<LaporanLostFound>();
            listLaporanKriminalitas = new ObservableCollection<LaporanKriminalitas>();
            listEmergencyContact = new ObservableCollection<User>();
            httpObject = new HttpObject();
            session = new Session();
#if NETFX_CORE
            btnEmergency.Visibility=Visibility.Collapsed;
#endif
        }

        private async Task<string> getUserAddress()
        {
            var location = await Geolocation.GetLastKnownLocationAsync();
            if (location == null)
            {
                location = await Geolocation.GetLocationAsync(new GeolocationRequest
                {
                    DesiredAccuracy = GeolocationAccuracy.Best,
                    Timeout = TimeSpan.FromSeconds(30)
                });
            }
            string lat = location.Latitude.ToString().Replace(",", ".");
            string lng = location.Longitude.ToString().Replace(",", ".");
            string latlng = lat + "," + lng;
            string reqUri = "https://maps.googleapis.com/maps/api/geocode/json?latlng=" + latlng + "&key=AIzaSyA9rHJZEGWe6rX4nAHTGXFxCubmw-F0BBw";
            string responseData = await httpObject.GetRequest(reqUri,session.getTokenAuthorization());
            JObject json = JObject.Parse(responseData);
            string address = json["results"][0]["formatted_address"].ToString();
            return address;
            //using (var client = new HttpClient())
            //{
            //    string latlng = lat + "," + lng;
            //    string reqUri = "https://maps.googleapis.com/maps/api/geocode/json?latlng=" + latlng + "&key=AIzaSyA9rHJZEGWe6rX4nAHTGXFxCubmw-F0BBw";
            //    HttpResponseMessage response = await client.GetAsync(reqUri);
            //    if (response.IsSuccessStatusCode)
            //    {
            //        var jsonString = response.Content.ReadAsStringAsync().Result;
            //        JObject json = JObject.Parse(jsonString);
            //        string address = json["results"][0]["formatted_address"].ToString();
            //        return address;
            //    }
            //    else
            //    {
            //        return null;
            //    }
            //}
        }

        private void goToAllReportPage(object sender,RoutedEventArgs e)
        {
            string tag = (sender as TextBlock).Tag.ToString();
            session.setAllReportParam(tag);
            this.Frame.Navigate(typeof(AllReportPage));
        }

        

        public async void HomePageLoaded(object sender, RoutedEventArgs e)
        {
            userLogin = session.getUserLogin();
            txtNamaUser.Text = "Selamat Datang, " + userLogin.nama_user + "!";
            if (userLogin.status_user == 1)
            {
                txtStatusUser.Text = "Premium Account";
            }
            else
            {
                txtStatusUser.Text = "Free Account";
            }
            string responseData = await httpObject.GetRequest("/getHeadlineLaporanKriminalitas",session.getTokenAuthorization());
            listLaporanKriminalitas = JsonConvert.DeserializeObject<ObservableCollection<LaporanKriminalitas>>(responseData);
            responseData = await httpObject.GetRequest("/getHeadlineLaporanLostFound",session.getTokenAuthorization());
            listLaporanLostFound = JsonConvert.DeserializeObject<ObservableCollection<LaporanLostFound>>(responseData);
#if __ANDROID__
            lvHeadline.ItemsSource = listLaporanLostFound;
            btnSelectionLaporanKriminalitas.IsEnabled = true;
            btnSelectionLaporanLostFound.IsEnabled = false;
            lvHeadline.Tag = "lvLostfound";
            if (userLogin.status_user == 1)
            {
                btnEmergency.Visibility = Visibility.Visible;
            }
            else
            {
                btnEmergency.Visibility = Visibility.Collapsed;
            }
#elif NETFX_CORE
            lvLaporanKriminalitas.ItemsSource = listLaporanKriminalitas;
            lvLaporanLostFound.ItemsSource = listLaporanLostFound;
#endif 
        }

        private void emergencyAction(object sender, HoldingRoutedEventArgs e)
        {
#if __ANDROID__
            if(e.HoldingState==HoldingState.Completed){
                sendNotification();
            }
#endif
        }


        private async void sendEmergencyChat(User u, string address)
        {
            string message = "Saya sedang dalam keadaan darurat! Lokasi terakhir saya di " + address;
            string responseData=await httpObject.GetRequest("checkHeaderChat/" + userLogin.id_user + "/" + u.id_user,session.getTokenAuthorization());
            JObject json = JObject.Parse(responseData);
            var content = new FormUrlEncodedContent(new[]{
                new KeyValuePair<string, string>("id_chat", json["id_chat"].ToString()),
                new KeyValuePair<string, string>("id_user_pengirim", userLogin.id_user.ToString()),
                new KeyValuePair<string, string>("id_user_penerima", u.id_user.ToString()),
                new KeyValuePair<string, string>("isi_chat", message),
            });
            await httpObject.PostRequestUrlEncodedWithAuthorization("insertDetailChat", content,session.getTokenAuthorization());
            //using (var client=new HttpClient())
            //{
            //    client.BaseAddress = new Uri(session.getApiURL());
            //    client.DefaultRequestHeaders.Accept.Clear();
            //    HttpResponseMessage response = await client.GetAsync("checkHeaderChat/" + userLogin.id_user+"/"+u.id_user);
            //    if (response.IsSuccessStatusCode)
            //    {
            //        var responseData = response.Content.ReadAsStringAsync().Result;
            //        JObject json = JObject.Parse(responseData);  
            //        MultipartFormDataContent form = new MultipartFormDataContent();
            //        form.Add(new StringContent(json["id_chat"].ToString()), "id_chat");
            //        form.Add(new StringContent(userLogin.id_user.ToString()), "id_user_pengirim");
            //        form.Add(new StringContent(u.id_user.ToString()), "id_user_penerima");
            //        form.Add(new StringContent(content), "isi_chat");
            //        await client.PostAsync("insertDetailChat", form);
            //    }
            //}
        }

#if __ANDROID__
        private void changeSource(object sender,RoutedEventArgs e)
        {
            string tag = (sender as Button).Tag.ToString();
            if (tag == "1")
            {
                lvHeadline.ItemsSource = listLaporanLostFound;
                btnSelectionLaporanKriminalitas.IsEnabled = true;
                btnSelectionLaporanLostFound.IsEnabled = false;
                lvHeadline.Tag = "lvLostfound";
                txtTagLine.Tag="lostfound";
            }
            else
            {
                lvHeadline.ItemsSource = listLaporanKriminalitas;
                btnSelectionLaporanKriminalitas.IsEnabled = false;
                btnSelectionLaporanLostFound.IsEnabled = true;
                lvHeadline.Tag = "lvKriminalitas";
                txtTagLine.Tag="kriminalitas";
            }
        }

        private async void sendNotification()
        {
            string responseData=await httpObject.GetRequest("user/getEmergencyContact/"+userLogin.id_user);
            listEmergencyContact = JsonConvert.DeserializeObject<ObservableCollection<User>>(responseData);
            if(listEmergencyContact.Count<1){
                var message = new MessageDialog("Tidak ada kontak darurat yang terdaftar");
                await message.ShowAsync();
            }
                
            else
            {
                string address = await getUserAddress();
                string heading=userLogin.nama_user+" sedang dalam keadaan darurat!";
                string messageContent="Salah satu kontakmu, "+userLogin.nama_user+" sedang dalam keadaan darurat. Lokasi terakhirnya adalah "+address;
                foreach (User user in listEmergencyContact)
                {
            
                    var content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("number", user.telpon_user),
                        new KeyValuePair<string, string>("heading", heading),
                        new KeyValuePair<string, string>("content", messageContent)
                    });
                    await httpObject.PostRequestWithUrlEncoded("user/sendEmergencyNotification", content);
                    //response = await client.PostAsync("user/sendEmergencyNotification", content);
                    sendEmergencyChat(user, address);
                }
                var message = new MessageDialog("Pesan darurat telah dikirimkan ke semua kontak darurat anda");
                await message.ShowAsync();
            }
            //using (var client = new HttpClient())
            //{
            //    client.BaseAddress = new Uri(session.getApiURL());
            //    client.DefaultRequestHeaders.Accept.Clear();
            //    string address = await getUserAddress();
            //    HttpResponseMessage response = await client.GetAsync("user/getEmergencyContact/" + userLogin.id_user);
            //    if (response.IsSuccessStatusCode)
            //    {
            //        var responseData = response.Content.ReadAsStringAsync().Result;
            //        listEmergencyContact = JsonConvert.DeserializeObject<ObservableCollection<User>>(responseData);
            //        foreach(User user in listEmergencyContact){
            //            var content = new FormUrlEncodedContent(new[]
            //            {
            //                new KeyValuePair<string, string>("number", user.telpon_user),
            //            });
            //            response = await client.PostAsync("user/sendEmergencyNotification", content);
            //            sendEmergencyChat(user,address);
            //        }
            //        var message = new MessageDialog("Pesan darurat telah dikirimkan ke semua kontak darurat anda");
            //        await message.ShowAsync();
            //    }
            //}
        }
#endif

        public void goToDetailPage(object sender, ItemClickEventArgs e)
        {
            string tag = (sender as ListView).Tag.ToString();
            if (tag == "lvKriminalitas")
            {
                LaporanKriminalitas selected = (LaporanKriminalitas)e.ClickedItem;
                ReportDetailPageParams param = new ReportDetailPageParams(selected.id_user_pelapor, selected.nama_user_pelapor, selected.id_laporan, selected.alamat_laporan, selected.tanggal_laporan, selected.waktu_laporan, selected.judul_laporan, selected.jenis_kejadian, selected.deskripsi_kejadian, selected.lat_laporan, selected.lng_laporan, "kriminalitas", selected.thumbnail_gambar,selected.status_laporan) ;
                session.setReportDetailPageParams(param);
            }
            else if (tag == "lvLostfound")
            {
                LaporanLostFound selected = (LaporanLostFound)e.ClickedItem;
                string jenis_laporan = "";
                if (selected.jenis_laporan == 0)
                {
                    jenis_laporan = "Penemuan barang";
                }
                else
                {
                    jenis_laporan = "Kehilangan barang";
                }
                ReportDetailPageParams param = new ReportDetailPageParams(selected.id_user_pelapor, selected.nama_user_pelapor, selected.id_laporan, selected.alamat_laporan, selected.tanggal_laporan, selected.waktu_laporan, selected.judul_laporan,jenis_laporan, selected.deskripsi_barang, selected.lat_laporan, selected.lng_laporan, "lostfound",selected.thumbnail_gambar,selected.status_laporan);
                session.setReportDetailPageParams(param);
            }
            this.Frame.Navigate(typeof(ReportDetailPage));
        }
    }
}