using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace SimpleImageResizer.Core.Imaging;

public class Exif
{
    /// <summary>
    /// This is the EXIF orientation ID found in the meta data = 0x0112.
    /// </summary>
    private const string OrientationQuery = "System.Photo.Orientation";

    /// <summary>
    /// Possible EXIF orientations.
    /// </summary>
    public enum ExifOrientation
    {
        /// <summary>
        /// Unknown.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// TopLeft.
        /// </summary>
        TopLeft = 1,

        /// <summary>
        /// TopRight.
        /// </summary>
        TopRight = 2,

        /// <summary>
        /// BottomRight.
        /// </summary>
        BottomRight = 3,

        /// <summary>
        /// BottomLeft.
        /// </summary>
        BottomLeft = 4,

        /// <summary>
        /// LeftTop.
        /// </summary>
        LeftTop = 5,

        /// <summary>
        /// RightTop.
        /// </summary>
        RightTop = 6,

        /// <summary>
        /// RightBottom.
        /// </summary>
        RightBottom = 7,

        /// <summary>
        /// LeftBottom.
        /// </summary>
        LeftBottom = 8,
    }

    /// <summary>
    /// Returns the correct rotation for a BitmapImage based on the EXIF meta data of the image.
    /// </summary>
    /// <remarks>
    /// refer to http://www.impulseadventure.com/photo/exif-orientation.html for details on orientation values.
    /// </remarks>
    /// <param name="file">Image.</param>
    /// <returns>Rotation.</returns>
    public static Rotation GetOrientation(string file)
    {
        Rotation rotation = Rotation.Rotate0;

        if (System.IO.File.Exists(file))
        {
            using FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
            BitmapFrame bitmapFrame = BitmapFrame.Create(fileStream, BitmapCreateOptions.DelayCreation, BitmapCacheOption.None);
            if ((bitmapFrame.Metadata is BitmapMetadata bitmapMetadata) && bitmapMetadata.ContainsQuery(OrientationQuery))
            {
                var o = bitmapMetadata.GetQuery(OrientationQuery);

                if (o != null)
                {
                    switch ((ushort)o)
                    {
                        case (ushort)ExifOrientation.RightTop:
                            rotation = Rotation.Rotate90;
                            break;
                        case (ushort)ExifOrientation.BottomRight:
                            rotation = Rotation.Rotate180;
                            break;
                        case (ushort)ExifOrientation.LeftBottom:
                            rotation = Rotation.Rotate270;
                            break;
                    }
                }
            }
        }

        return rotation;
    }
}