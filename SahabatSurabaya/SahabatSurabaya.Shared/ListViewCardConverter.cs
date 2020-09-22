using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace SahabatSurabaya.Shared
{
    class ListViewCardConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string id_laporan = (string)value;
            if (id_laporan == "99")
            {
                return Visibility.Collapsed;
            }
            else
            {
                return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
