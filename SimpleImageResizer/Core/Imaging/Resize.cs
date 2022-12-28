using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace SimpleImageResizer.Core.Imaging;

/// <summary>
/// Class for resizing an image.
/// </summary>
/// <remarks>
/// BitmapFrames do not need to be disposed unlike GDI+. Apparenlty. GC just does its magic.
/// Without using bitmap.CacheOption = BitmapCacheOption.None; you can run out of memory very quickly.
/// </remarks>
public class Resize
{
    /// <summary>
    /// Scales by width or height or a fixed size.
    /// </summary>
    public enum ScalingOption
    {
        /// <summary>
        /// Scale by width.
        /// </summary>
        Width,

        /// <summary>
        /// Scale by height.
        /// </summary>
        Height,

        /// <summary>
        /// No scaling.
        /// </summary>
        None,
    }

    /// <summary>
    /// Resizes the passed image file and returns a BitmapFrame.
    /// </summary>
    /// <param name="file">The image file.</param>
    /// <param name="scalingOptions">How the image should be scaled.</param>
    /// <param name="x">New Width.</param>
    /// <param name="y">New Height.</param>
    /// <returns>Bitmap Frame.</returns>
    public static BitmapFrame Image(
        string file,
        ScalingOption scalingOptions,
        int x = 0,
        int y = 0
        )
    {
        BitmapImage bitmap = new();
        bitmap.BeginInit();
        bitmap.CacheOption = BitmapCacheOption.None; // this one is YUGE! Significantly reduces memory usage.
        bitmap.CreateOptions = BitmapCreateOptions.DelayCreation;
        bitmap.UriSource = new Uri(file);
        bitmap.EndInit();

        double newWidth = bitmap.PixelWidth;
        double newHeight = bitmap.PixelHeight;
        switch (scalingOptions)
        {
            case ScalingOption.Height:
                newHeight = ((double)x / bitmap.PixelWidth) * bitmap.PixelHeight;
                newWidth = x;
                break;
            case ScalingOption.Width:
                newHeight = y;
                newWidth = ((double)y / bitmap.PixelHeight) * bitmap.PixelWidth;
                break;
            case ScalingOption.None:
                newHeight = y;
                newWidth = x;
                break;
        }

        // Transform the bitmap frame
        TransformedBitmap transformedBitmap = new TransformedBitmap();
        transformedBitmap.BeginInit();
        transformedBitmap.Source = bitmap;
        transformedBitmap.Transform = new ScaleTransform(newWidth / bitmap.PixelWidth, newHeight / bitmap.PixelHeight);
        transformedBitmap.EndInit();

        return BitmapFrame.Create(transformedBitmap);
    }

    /// <summary>
    /// Resizes the passed image file by PERCENTAGE and returns a BitmapFrame.
    /// </summary>
    /// <param name="file">File.</param>
    /// <param name="percentage">Percentage.</param>
    /// <returns>Bitmap Frame.</returns>
    public static BitmapFrame Image(string file, int percentage)
    {
        BitmapImage bitmap = new();
        bitmap.BeginInit();
        bitmap.CacheOption = BitmapCacheOption.None; // this one is YUGE! Significantly reduces memory usage.
        bitmap.CreateOptions = BitmapCreateOptions.DelayCreation;
        bitmap.UriSource = new Uri(file);
        bitmap.EndInit();

        double d = (double)percentage / 100;

        // Transform the bitmap frame
        TransformedBitmap transformedBitmap = new();
        transformedBitmap.BeginInit();
        transformedBitmap.Source = bitmap;
        transformedBitmap.Transform = new ScaleTransform(d, d);
        transformedBitmap.EndInit();

        return BitmapFrame.Create(transformedBitmap);
    }
}
