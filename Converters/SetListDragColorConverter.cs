namespace LocalLiftLog.Converters
{
    public class SetListDragColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ResourceDictionary ColorResource = Application.Current.Resources.MergedDictionaries.FirstOrDefault();

            Color mediumGreenColor = ColorResource["MediumGreen"] as Color;

            Color transparentColor = Color.FromArgb("#00FFFFFF");

            if (value is bool boolValue && boolValue)
            {
                return mediumGreenColor;
            }

            return transparentColor;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
