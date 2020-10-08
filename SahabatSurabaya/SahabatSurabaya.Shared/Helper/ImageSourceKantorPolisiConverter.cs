using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;

namespace SahabatSurabaya.Shared
{
    class ImageSourceKantorPolisiConverter:IValueConverter
    {
        public object Convert(object value, Type targetType,
       object parameter, string language)
        {
            string fileGambar = (string)value;
            string baseUri = "http://adrian-assets.ta-istts.com/";
            return baseUri + fileGambar;
        }

        // ConvertBack is not implemented for a OneWay binding.
        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
