using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleImageResizer.Core.Imaging;

public class Validation
{
    /// <summary>
    /// Tests if the Extention of the file is a valid image and returns
    /// The SaveAs parameter. If it fails, it returns NotValid.
    /// </summary>
    /// <param name="extension">Exentension.</param>
    /// <returns>Enum SaveAs.</returns>
    public static Save.SaveAs IsValid(string extension)
    {
        if (string.IsNullOrWhiteSpace(extension))
        {
            return Save.SaveAs.NotValid;
        }

        // use original
        return (extension.ToLower()) switch
        {
            ".bmp" => Save.SaveAs.BMP,
            ".gif" => Save.SaveAs.GIF,
            ".jfif" => Save.SaveAs.JFIF,
            ".jpg" => Save.SaveAs.JPG,
            ".png" => Save.SaveAs.PNG,
            ".tif" => Save.SaveAs.TIF,
            _ => Save.SaveAs.NotValid,
        };
    }
}
