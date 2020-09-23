using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;

namespace SahabatSurabaya.Shared
{
    class ImageLaporanConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string fileGambar = (string)value;
            string baseUri = "http://adrian-webservice.ta-istts.com/public/uploads/";
            return baseUri + fileGambar;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
