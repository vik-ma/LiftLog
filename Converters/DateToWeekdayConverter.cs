using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocalLiftLog.Helpers;

namespace LocalLiftLog.Converters
{
    public class DateToWeekdayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string dateString)
            {
                if (string.IsNullOrWhiteSpace(dateString)) return "No Date Set";

                return DateTimeHelper.GetWeekdayOfYmdDateString(dateString);
            }

            return "Unknown";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
