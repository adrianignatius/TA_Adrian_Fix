using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace SahabatSurabaya.Shared.Helper
{
    class StatusLaporanColorConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int statusLaporan = (int)value;
            if (statusLaporan == 1)
            {
                return new SolidColorBrush(Colors.Green);
            }
            else
            {
                return new SolidColorBrush(Colors.Red);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
