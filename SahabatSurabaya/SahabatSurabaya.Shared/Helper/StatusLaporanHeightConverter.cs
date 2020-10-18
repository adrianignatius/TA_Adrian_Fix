using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;

namespace SahabatSurabaya.Shared.Helper
{
    class StatusLaporanHeightConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int status = (int)value;
            if (status == 0)
            {
                return 110;
            }
            else
            {
                return 80;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
