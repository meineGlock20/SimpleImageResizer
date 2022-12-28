using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Sir.Imaging;

public class Dimensions
{
    /// <summary>
    /// Returns the dimensions of an image in a Size object (w,h).
    /// </summary>
    /// <param name="file">File.</param>
    /// <returns>Size.</returns>
    public static Size GetImageSize(string file)
    {
        int width = 0;
        int height = 0;
        using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            var bitmapFrame = BitmapFrame.Create(stream, BitmapCreateOptions.DelayCreation, BitmapCacheOption.None);
            width = bitmapFrame.PixelWidth;
            height = bitmapFrame.PixelHeight;
        }

        return new Size(width, height);
    }
}
