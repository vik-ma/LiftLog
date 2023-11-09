using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    Dictionary<int, string> weekdayDictionary = new()
                    {
                        { 0, "Monday" },
                        { 1, "Tuesday" },
                        { 2, "Wednesday" },
                        { 3, "Thursday" },
                        { 4, "Friday" },
                        { 5, "Saturday" },
                        { 6, "Sunday" }
                    };

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
