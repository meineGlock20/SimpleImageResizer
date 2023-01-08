using SimpleImageResizer.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace SimpleImageResizer.Converters;

/// <summary>
/// Converts a NULL value to visibility COLLAPSED.
/// </summary>
public class NullBitmapImageToVisibilityConverter : IValueConverter
{
    /// <summary>
    /// If the value of the collection count is zero return VISIBLE otherwise COLLAPSED.
    /// </summary>
    /// <param name="value">Value.</param>
    /// <param name="targetType">TargetType.</param>
    /// <param name="parameter">Parameter.</param>
    /// <param name="culture">Culture.</param>
    /// <returns>Bool.</returns>        
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not BitmapImage)
        {
            return Visibility.Collapsed;
        }
        else
        {
            return Visibility.Visible;
        }
    }

    /// <summary>
    /// Converts back.
    /// </summary>
    /// <param name="value">Value.</param>
    /// <param name="targetType">TargetType.</param>
    /// <param name="parameter">Parameter.</param>
    /// <param name="culture">Culture.</param>
    /// <returns>Bool.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
