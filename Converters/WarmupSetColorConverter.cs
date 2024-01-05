using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalLiftLog.Converters
{
    public class WarmupSetColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ResourceDictionary ColorResource = Application.Current.Resources.MergedDictionaries.FirstOrDefault();
            
            Color veryLightOrangeColor = ColorResource["VeryLightOrange"] as Color;
            Color mediumBrownColor = ColorResource["MediumBrown"] as Color;
            Color veryLightGrayColor = ColorResource["VeryLightGray"] as Color;
            Color veryDarkGrayColor = ColorResource["VeryDarkGray"] as Color;

            if (value is bool boolValue && boolValue)
            {
                // If Set is Warmup-Set
                if (parameter is string contextTrue && contextTrue == "Background")
                {
                    // Background Color
                    return veryLightOrangeColor;
                }
                // Text Color
                return mediumBrownColor;
            }

            // If Set is NOT Warmup-Set
            if (parameter is string contextFalse && contextFalse == "Background")
            {
                // Background Color
                return veryLightGrayColor;
            }
            // Text Color
            return veryDarkGrayColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
