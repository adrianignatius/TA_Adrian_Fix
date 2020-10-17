using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;

namespace SahabatSurabaya.Shared.Helper
{
    class StatusLaporanConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int status = (int)value;
            if (status == 0)
            {
                return "Belum diverifikasi";
            }
            else if (status == 99)
            {
                return "Dibatalkan";
            }else
            {
                return "Sudah diverifikasi";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
