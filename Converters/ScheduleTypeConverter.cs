﻿namespace LocalLiftLog.Converters
{
    public class ScheduleTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool context)
            {
                // If IsWeeklySchedule is True
                if (context) return "Weekly";
                // If IsWeeklySchedule is False
                else return "Custom";
            }

            return "Unknown";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
