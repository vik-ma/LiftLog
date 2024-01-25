namespace LocalLiftLog.Converters
{
    public class InvalidExerciseColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ResourceDictionary ColorResource = Application.Current.Resources.MergedDictionaries.FirstOrDefault();

            Color blackColor = ColorResource["Black"] as Color;
            Color redColor = ColorResource["MediumRed"] as Color;

            if (value is string stringValue && stringValue == "Invalid Exercise")
            {
                return redColor;
            }
            return blackColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
