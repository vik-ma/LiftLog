namespace LocalLiftLog.Converters
{
    public class CurrentSetBorderColorMultiBindingConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            ResourceDictionary ColorResource = Application.Current.Resources.MergedDictionaries.FirstOrDefault();

            Color falseBorderColor = ColorResource["MediumGray"] as Color;
            Color trueBorderColor = ColorResource["Primary"] as Color;

            if (values[0] is SetExercisePackage set1 && values[1] is SetExercisePackage set2)
            {
                if (set1 == set2) return trueBorderColor;
            }

            return falseBorderColor;
        }

        object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
