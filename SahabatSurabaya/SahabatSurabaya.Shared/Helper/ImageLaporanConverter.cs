using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;

namespace SahabatSurabaya.Shared.Helper
{
    class ImageLaporanConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string fileGambar = (string)value;
            if (fileGambar == "no-image.png")
            {
                return "/Assets/no-image.png";
            }
            else
            {
                string baseUri = "http://adrian-webservice.ta-istts.com/public/uploads/";
                return baseUri + fileGambar;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
