using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace SahabatSurabaya.Shared
{
    class ChatBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool isSender = (bool)value;
            if (isSender == true)
            {
                return new SolidColorBrush(Colors.YellowGreen);
            }
            else
            {
                return new SolidColorBrush(Colors.AliceBlue);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
