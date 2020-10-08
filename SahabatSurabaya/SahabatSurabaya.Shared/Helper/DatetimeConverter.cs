using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;

namespace SahabatSurabaya.Shared
{
    class DatetimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null)
            {
                string date = (string)value;
                DateTime dateTime = DateTime.Parse(date);
                return dateTime.ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("id-ID"));
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
