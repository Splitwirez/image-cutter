using Avalonia;
using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ImageCutter
{
    public class DoublesToThicknessConverter : IMultiValueConverter
    {
        public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture)
        {
            double[] doubleValues = values.OfType<double>().ToArray();
            return new Thickness(doubleValues[0], doubleValues[1], doubleValues[2], doubleValues[3]);
        }
    }
}