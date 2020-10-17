using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace SahabatSurabaya.Shared.Helper
{
    class StatusLaporanButtonConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int status = (int)value;
            if (status == 0)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
