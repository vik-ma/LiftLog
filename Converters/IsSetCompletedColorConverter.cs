namespace LocalLiftLog.Converters
{
    public class IsSetCompletedColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ResourceDictionary ColorResource = Application.Current.Resources.MergedDictionaries.FirstOrDefault();

            Color mediumGreenColor = ColorResource["MediumGreen"] as Color;
            Color mediumRedColor = ColorResource["MediumRed"] as Color;

            if (value is bool boolValue && boolValue)
            {
                return mediumGreenColor;   
            }
            return mediumRedColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
