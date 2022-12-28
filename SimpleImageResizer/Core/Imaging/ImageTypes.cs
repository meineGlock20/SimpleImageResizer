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
        /// Save as the original file type.
        /// </summary>
        original,

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

    /// <summary>
    /// A list of valid image extensions.
    /// </summary>
    public static readonly string[] ValidExtensions = new[] { ".bmp", ".gif", ".jfif", ".jpg", ".png", ".tif" };
}