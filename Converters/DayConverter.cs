using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocalLiftLog.Helpers;

namespace LocalLiftLog.Converters
{
    public class DayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is string context)
            {
                if (context == "Weekly")
                {
                    var weekdayDictionary = WeekdayHelper.WeekdayDict;

                    if (value is int selectedDay && weekdayDictionary.ContainsKey(selectedDay))
                    {
                        return weekdayDictionary[selectedDay];
                    }
                }
                else if (context == "Custom")
                {
                    if (value is int selectedDay && selectedDay >= 0 && selectedDay < 14)
                    {
                        return $"Day {selectedDay + 1}";
                    }
                }

            }

            return "Unknown";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
