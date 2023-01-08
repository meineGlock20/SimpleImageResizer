using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleImageResizer.Core.Imaging;

public class ImageTypes
{
    /// <summary>
    /// Valid image types.
    /// </summary>
    public enum ImageType
    {
        /// <summary>
        /// What is this?
        /// </summary>
        unknown,

        /// <summary>
        /// Bitmap.
        /// </summary>
        bmp,

        /// <summary>
        /// GIF.
        /// </summary>
        gif,

        /// <summary>
        /// JFIF.
        /// </summary>
        jfif,

        /// <summary>
        /// JPEG.
        /// </summary>
        jpg,

        /// <summary>
        /// PNG.
        /// </summary>
        png,

        /// <summary>
        /// TIF.
        /// </summary>
        tif
    }

    public static ImageTypes.ImageType GetImageType(string extension)
    {
        if (string.IsNullOrWhiteSpace(extension))
        {
            throw new ArgumentNullException(nameof(extension));
        }

        // use original
        return (extension.ToLower()) switch
        {
            ".bmp" => ImageTypes.ImageType.bmp,
            ".gif" => ImageTypes.ImageType.gif,
            ".jfif" => ImageTypes.ImageType.jfif,
            ".jpg" => ImageTypes.ImageType.jpg,
            ".png" => ImageTypes.ImageType.png,
            ".tif" => ImageTypes.ImageType.tif,
            _ => ImageTypes.ImageType.unknown,
        };
    }

    /// <summary>
    /// A list of valid image extensions.
    /// </summary>
    public static readonly string[] ValidExtensions = new[] { ".bmp", ".gif", ".jfif", ".jpg", ".jpeg", ".png", ".tif", ".tiff" };
}