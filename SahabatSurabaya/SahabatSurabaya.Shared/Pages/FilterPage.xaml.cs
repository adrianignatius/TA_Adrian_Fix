﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
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

namespace SahabatSurabaya.Shared.Pages
{

    public sealed partial class FilterPage : Page
    {
        ObservableCollection<Kecamatan> listKecamatan;
        List<Kecamatan> listKecamatanSelected;
        Session session;
        HttpObject httpObject;
        public FilterPage()
        {
            this.InitializeComponent();
            session = new Session();
            httpObject = new HttpObject();
            listKecamatan = new ObservableCollection<Kecamatan>();
            listKecamatanSelected = new List<Kecamatan>();
        }

        private async void pageLoaded(object sender,RoutedEventArgs e)
        {
            string responseData = await httpObject.GetRequest("getKecamatan");
            listKecamatan = JsonConvert.DeserializeObject<ObservableCollection<Kecamatan>>(responseData);
            gvKecamatan.ItemsSource = listKecamatan;
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

        private void setFilter(object sender,RoutedEventArgs e)
        {

        }

        private void flyoutDone(object sender,RoutedEventArgs e)
        {
            flyoutKecamatan.Hide();
            int count = listKecamatanSelected.Count;
            string kecamatan = "";
            for (int i = 0; i < count; i++)
            {
                kecamatan += listKecamatanSelected[i] + ",";
            }
            txtStackKecamatan.Text = kecamatan.Substring(0,kecamatan.Length-1);
        }

        private void kecamatanUnchecked(object sender, RoutedEventArgs e)
        {
            string kecamatan = (sender as CheckBox).Content.ToString();
            //listKecamatanSelected.Remove(kecamatan);
        }

        private void kecamatanChecked(object sender,RoutedEventArgs e)
        {
            int id_kecamatan = Convert.ToInt32((sender as CheckBox).Tag.ToString());
            Kecamatan selected = listKecamatan.Single(i => i.id_kecamatan == id_kecamatan);
            var message = new MessageDialog(selected.nama_kecamatan.ToString());
        }
    }
}
