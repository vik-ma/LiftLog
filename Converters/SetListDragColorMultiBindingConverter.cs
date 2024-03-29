﻿namespace LocalLiftLog.Converters
{
    public class SetListDragColorMultiBindingConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            ResourceDictionary ColorResource = Application.Current.Resources.MergedDictionaries.FirstOrDefault();

            Color isItemDraggedColor = ColorResource["Gray100"] as Color;
            Color isDraggedOverColor = ColorResource["VeryLightYellow"] as Color;
            Color isItemHoveredColor = ColorResource["VeryLightGray"] as Color;
            Color isWarmupSetColor = ColorResource["VeryLightOrange"] as Color;
            Color isWarmupSetHoverColor = ColorResource["MediumLightOrange"] as Color;
            Color defaultColor = ColorResource["White"] as Color;

            if (values[3] is bool isWarmupSet && isWarmupSet)
            {
                if (values[2] is bool isWarmupSetHoveredOver && isWarmupSetHoveredOver)
                    return isWarmupSetHoverColor;

                return isWarmupSetColor;
            }

            if (values[1] is bool isItemDragged && isItemDragged)
            {
                return isItemDraggedColor;
            }

            if (values[0] is bool isDraggedOver && isDraggedOver)
            {
                return isDraggedOverColor;
            }

            if (values[2] is bool isHoveredOver && isHoveredOver)
            {
                return isItemHoveredColor;
            }

            return defaultColor;

        }

        object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
