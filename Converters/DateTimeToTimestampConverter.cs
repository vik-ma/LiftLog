using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocalLiftLog.Helpers;

namespace LocalLiftLog.Converters
{
    public class DateTimeToTimestampConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string dateString)
            {
                if (parameter is string parameterString && parameterString == "IncludeDate")
                {
                    return DateTimeHelper.FormatDateTimeStringToYmdDateAndTimestampString(dateString);
                }

                return DateTimeHelper.FormatDateTimeStringToTimestamp(dateString);
            }

            return "Invalid DateTime";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
