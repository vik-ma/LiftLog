namespace LocalLiftLog.Converters
{
    public class NullValueToEmptyTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || (value is string stringValue && string.IsNullOrEmpty(stringValue)) || (value is int intValue && intValue == 0))
            {
                if (parameter is string context && !string.IsNullOrEmpty(context))
                {
                    return $"No {context} Set";
                }
                return "Empty";
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
