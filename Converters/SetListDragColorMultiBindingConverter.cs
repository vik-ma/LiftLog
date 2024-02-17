namespace LocalLiftLog.Converters
{
    public class SetListDragColorMultiBindingConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            ResourceDictionary ColorResource = Application.Current.Resources.MergedDictionaries.FirstOrDefault();

            Color isItemDraggedColor = ColorResource["VeryLightGray"] as Color;
            Color isDraggedOverColor = ColorResource["VeryLightYellow"] as Color;
            Color defaultColor = ColorResource["White"] as Color;

            if (values[1] is bool isItemDragged && isItemDragged)
            {
                return isItemDraggedColor;
            }

            if (values[0] is bool isDraggedOver && isDraggedOver)
            {
                return isDraggedOverColor;
            }

            return defaultColor;

        }

        object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
