using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace SahabatSurabaya.Shared
{
    public class ChatAlignmentConverter: IValueConverter
    {
        public object Convert(object value, Type targetType,
        object parameter, string language)
        {
            bool isSender = (bool) value;
            if (isSender == true)
            {
                return HorizontalAlignment.Right;
            }
            else
            {
                return HorizontalAlignment.Left;
            }

            // Return the month value to pass to the target.
            
        }

        // ConvertBack is not implemented for a OneWay binding.
        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
