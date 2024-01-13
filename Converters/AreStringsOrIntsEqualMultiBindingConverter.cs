namespace LocalLiftLog.Converters
{
    public class AreStringsOrIntsEqualMultiBindingConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is string stringValue0 && values[1] is string stringValue1)
            {
                return stringValue0 == stringValue1;
            }

            if (values[0] is int intValue0 && values[1] is int intValue1)
            {
                return intValue0 == intValue1;
            }

            return false;
        }

        object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
