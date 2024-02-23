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
                    string dateTimeString = DateTimeHelper.FormatDateTimeStringToYmdDateAndTimestampString(dateString, is24HourFormat);
                    return $"Completed {dateTimeString}";
                }

                string timeString = DateTimeHelper.FormatDateTimeStringToTimestamp(dateString, is24HourFormat);
                return $"Completed {timeString}";
            }

            return "Invalid DateTime";
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
