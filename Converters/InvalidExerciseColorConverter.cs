namespace LocalLiftLog.Converters
{
    public class InvalidExerciseColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ResourceDictionary ColorResource = Application.Current.Resources.MergedDictionaries.FirstOrDefault();

            Color normalTextColor = ColorResource["Black"] as Color;
            Color normalBorderColor = ColorResource["Primary"] as Color;
            Color invalidColor = ColorResource["MediumRed"] as Color;


            if (value is bool isInvalid && isInvalid)
                return invalidColor;

            if (parameter is string parameterString && parameterString == "Border")
            {
                return normalBorderColor;
            }

            return normalTextColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
