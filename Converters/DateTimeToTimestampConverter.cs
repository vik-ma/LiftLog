using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocalLiftLog.Helpers;

namespace LocalLiftLog.Converters
{
    public class DateTimeToTimestampConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is string dateString && values[1] is bool is24HourFormat)
            {
                if (parameter is string parameterString && parameterString == "IncludeDate")
                {
                    return DateTimeHelper.FormatDateTimeStringToYmdDateAndTimestampString(dateString, is24HourFormat);
                }

                return DateTimeHelper.FormatDateTimeStringToTimestamp(dateString, is24HourFormat);
            }

            return "Invalid DateTime";
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
