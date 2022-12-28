using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace SimpleImageResizer.Core.Imaging;

public class Size
{
    /// <summary>
    /// Returns the dimensions of an image in a Size object (w,h).
    /// </summary>
    /// <param name="file">The image.</param>
    /// <returns>Size of the image dimensions.</returns>
    public static System.Windows.Size GetImageSize(string file)
    {
        int width = 0;
        int height = 0;
        using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            var bitmapFrame = BitmapFrame.Create(stream, BitmapCreateOptions.DelayCreation, BitmapCacheOption.None);
            width = bitmapFrame.PixelWidth;
            height = bitmapFrame.PixelHeight;
        }

        return new System.Windows.Size(width, height);
    }
}
