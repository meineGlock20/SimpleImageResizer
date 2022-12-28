using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace SimpleImageResizer.Core.Imaging;

/// <summary>
/// Generates a bitmap thumbnail from image files.
/// </summary>
/// <remarks>
/// https://docs.microsoft.com/en-us/dotnet/framework/wpf/graphics-multimedia/imaging-overview
/// EXIF!
/// Some photos taken with digital camera can have rotation information in the exif meta data.
/// This can cause the image to appear rotated the wrong way.
/// Most OS and image software check this meta data and correct automatically.
/// WPF and windows forms images do not so it was a bit of a surprise. Corrected April 05 2019
/// PERFORMANCE!
/// The reason this is used over the Resize class is;  bitmapImage.DecodePixelWidth (or height)
/// is significantly faster for smaller images such as thumbnails. But using these methods for
/// resizing larger than thumbnail size (128x128) loses performance so you need ScaleTransform.
/// </remarks>
/// <history>
/// May 03 2010 :: Created.
/// </history>
/* THIS IS OLD DATA from 2010 when I was playing with this stuff. fun.
 *
 * NOTE : WPF classes have better performace and less code for
 * creating small bitmaps like thumbs, however, this is only the case if you are saving to
 * file because the returned bitmap is not compatible with windows forms controls like the ListView.
 * It's not the same as the System.Drawing.Bitmap
 * November 28 2009
 * TESTING: create 100x75 thumbnail
 * JPEG image original details: 3073x2304 @ 2.89 MB
 * GDI+ DrawImage = 0.340 seconds (slowest)
 * GDI+ GetThumbnailImage = 0.210 seconds (62% faster)
 * WPF Bitmap.DecodePixelWidth = 0.089 seconds (WOW) (74%) (42%)
 * ---- THIS IS ONLY FOR THUMBNAILS ----
 * For resizing more than 120x120 GetThumbnailImage is useless
 * and the larger the image resize, the better the performance of GDI+ DrawImage
 * to a point where it overtakes WPF.
 * 2020-12-10 ^ this is no longer the case.
 */
public class Thumbnail
{
    /// <summary>
    /// Generates a thumbnail by Width. Height aspect ratio is maintained.
    /// </summary>
    /// <param name="width">Width.</param>
    /// <param name="file">File.</param>
    /// <returns>BitmapImage.</returns>
    public static BitmapImage GenerateByWidth(int width, string file)
    {
        // Create source
        BitmapImage bitmapImage = new();

        // BitmapImage.UriSource must be in a BeginInit/EndInit block
        // Signals the start of the BitmapImage initialization.
        bitmapImage.BeginInit();
        bitmapImage.UriSource = new Uri(file);

        // This will prevent image locking.
        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        bitmapImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;

        // To save significant application memory, set the DecodePixelWidth or
        // DecodePixelHeight of the BitmapImage value of the image source to the desired
        // height or width of the rendered image. If you don't do this, the application will
        // cache the image as though it were rendered as its normal size rather then just
        // the size that is displayed.
        // Note: In order to preserve aspect ratio, set DecodePixelWidth
        // or DecodePixelHeight but not both.
        bitmapImage.DecodePixelWidth = width;

        // Rotate the image if necessary.
        bitmapImage.Rotation = Exif.GetOrientation(file);
        bitmapImage.EndInit();

        // Set image source.
        return bitmapImage;
    }

    /// <summary>
    /// Generate a thumbnail by height. Width aspect ratio is maintained.
    /// </summary>
    /// <param name="height">Height.</param>
    /// <param name="file">File.</param>
    /// <returns>BitmapImage.</returns>
    public static BitmapImage GenerateByHeight(int height, string file)
    {
        BitmapImage bitmapImage = new();

        bitmapImage.BeginInit();
        bitmapImage.UriSource = new Uri(file);

        // This will prevent image locking.
        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        bitmapImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;

        bitmapImage.DecodePixelHeight = height;
        bitmapImage.Rotation = Exif.GetOrientation(file);
        bitmapImage.EndInit();

        return bitmapImage;
    }

    /// <summary>
    /// Generate a fixed width and height thumbnail. No aspect ratio.
    /// </summary>
    /// <param name="width">Width.</param>
    /// <param name="height">Height.</param>
    /// <param name="file">File.</param>
    /// <returns>BitmapImage.</returns>
    public static BitmapImage GenerateFixed(int width, int height, string file)
    {
        BitmapImage bitmapImage = new();

        bitmapImage.BeginInit();
        bitmapImage.UriSource = new Uri(file);

        // This will prevent image locking.
        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        bitmapImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;

        bitmapImage.DecodePixelWidth = width;
        bitmapImage.DecodePixelHeight = height;
        bitmapImage.Rotation = Exif.GetOrientation(file);
        bitmapImage.EndInit();

        return bitmapImage;
    }
}
