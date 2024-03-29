﻿namespace LocalLiftLog.Converters
{
    public class DateToWeekdayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string dateString)
            {
                if (parameter is string parameterString && parameterString == "IncludeDate")
                {
                    return $"{dateString} ({DateTimeHelper.GetWeekdayOfYmdDateString(dateString)})";
                }

                return DateTimeHelper.GetWeekdayOfYmdDateString(dateString);
            }

            return "No Date Set";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
