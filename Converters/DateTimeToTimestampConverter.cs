namespace LocalLiftLog.Converters
{
    public class DateTimeToTimestampConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is string dateString && values[1] is UserPreferences userSettings)
            {
                if (!userSettings.ShowCompletedSetTimestamp) return "Completed";

                if (parameter is string parameterString && parameterString == "IncludeDate")
                {
                    string dateTimeString = DateTimeHelper.FormatDateTimeStringToYmdDateAndTimestampString(dateString, userSettings.IsUsing24HourClock);
                    return $"Completed {dateTimeString}";
                }

                string timeString = DateTimeHelper.FormatDateTimeStringToTimestamp(dateString, userSettings.IsUsing24HourClock);
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
