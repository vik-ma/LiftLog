namespace LocalLiftLog.Converters
{
    public class BoolToShowOrHideTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                if (parameter is string context)
                {
                    if (boolValue) return $"Hide {context}";
                    else return $"Show {context}";
                }
                return "Invalid Parameter";
            }
            return "Invalid Input";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
