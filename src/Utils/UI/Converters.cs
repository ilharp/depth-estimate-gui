using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace DepthEstimateGui.Utils.UI
{
    public static class Converters
    {
        public static StringBoolConverter StringBoolConverter = new();
        public static InverseBooleanValueConverter InverseBooleanValueConverter = new();
    }

    public class StringBoolConverter : IValueConverter
    {
        internal StringBoolConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string stringValue ||
                parameter is not string stringParameter)
                return false;

            return stringValue == stringParameter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not bool boolValue ||
                parameter is not string stringParameter)
                return BindingOperations.DoNothing;

            return boolValue ? stringParameter : BindingOperations.DoNothing;
        }
    }

    public class InverseBooleanValueConverter : IValueConverter
    {
        internal InverseBooleanValueConverter()
        {
        }

        public bool Default { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            value is bool b ? !b : Default;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            value is bool b ? !b : !Default;
    }
}
