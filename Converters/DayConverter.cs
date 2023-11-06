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
            Dictionary<int, string> dayDictionary = new()
            {
                { 0, "Monday" },
                { 1, "Tuesday" },
                { 2, "Wednesday" },
                { 3, "Thursday" },
                { 4, "Friday" },
                { 5, "Saturday" },
                { 6, "Sunday" }
            };

            if (value is int selectedDay && dayDictionary.ContainsKey(selectedDay))
            {
                return dayDictionary[selectedDay];
            }

            return "Unknown";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
