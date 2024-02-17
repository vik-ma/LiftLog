namespace LocalLiftLog.Converters
{
    public class SetListDragColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ResourceDictionary ColorResource = Application.Current.Resources.MergedDictionaries.FirstOrDefault();

            Color isDraggedOverColor = ColorResource["VeryLightGray"] as Color;
            Color defaultColor = ColorResource["White"] as Color;

            if (value is bool boolValue && boolValue)
            {
                return isDraggedOverColor;
            }

            return defaultColor;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
