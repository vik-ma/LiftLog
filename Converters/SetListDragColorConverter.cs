namespace LocalLiftLog.Converters
{
    public class SetListDragColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ResourceDictionary ColorResource = Application.Current.Resources.MergedDictionaries.FirstOrDefault();

            Color isDraggedColor = ColorResource["VeryLightGray"] as Color;
            Color transparentColor = ColorResource["Transparent"] as Color;

            if (value is bool boolValue && boolValue)
            {
                return isDraggedColor;
            }

            return transparentColor;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
