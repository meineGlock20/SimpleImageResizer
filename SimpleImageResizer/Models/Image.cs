using SimpleImageResizer.Core.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace SimpleImageResizer.Models;

public class Image : BaseModel
{
    private string? fullPathToImage;
    private double imageWidth;
    private double imageHeight;
    private long imageBytes;
    private BitmapImage? thumbnail;

    /// <summary>
    /// Gets or sets a value indicating the full path to the image.
    /// </summary>
    public string? FullPathToImage
    {
        get => fullPathToImage;

        set
        {
            if (fullPathToImage != value)
            {
                fullPathToImage = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(ImageName));
                NotifyPropertyChanged(nameof(ImageType));
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating the width of the image.
    /// </summary>
    public double ImageWidth
    {
        get => imageWidth;

        set
        {
            if (imageWidth != value)
            {
                imageWidth = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(DimensionsDisplay);
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating the height of the image.
    /// </summary>
    public double ImageHeight
    {
        get => imageHeight;

        set
        {
            if (imageHeight != value)
            {
                imageHeight = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(DimensionsDisplay);
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating the file size of the image in bytes.
    /// </summary>
    public long ImageBytes
    {
        get => imageBytes;

        set
        {
            if (imageBytes != value)
            {
                imageBytes = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(FileSizeDisplay);
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating the thumbnail image of the image to be resized.
    /// </summary>
    public BitmapImage? Thumbnail
    {
        get => thumbnail;

        set
        {
            if (thumbnail != value)
            {
                thumbnail = value;
                NotifyPropertyChanged();
            }
        }
    }

    /* HELPER PROPERTIES */

    /// <summary>
    /// Gets a value indicating the name of the image without the extension.
    /// </summary>
    public string? ImageName
    {
        get
        {
            if (string.IsNullOrWhiteSpace(FullPathToImage)) return null;
            return new FileInfo(FullPathToImage).Name;
        }
    }

    /// <summary>
    /// Gets a value indicating the type of image file by returning the extension.
    /// </summary>
    public ImageTypes.ImageType ImageType
    {
        get
        {
            if (string.IsNullOrWhiteSpace(FullPathToImage)) return ImageTypes.ImageType.unknown;
            return ImageTypes.GetImageType(new FileInfo(FullPathToImage).Extension.ToLowerInvariant());
        }
    }

    /// <summary>
    /// Gets a value indicating the dimensions of the image.
    /// </summary>
    public string? DimensionsDisplay
    {
        get
        {
            return ImageWidth + "x" + imageHeight;
        }
    }

    /// <summary>
    /// Gets a value indicating the size of the image in a readable human friendly format.
    /// </summary>
    public string? FileSizeDisplay
    {
        get
        {
            return Core.Calculate.CalculateSpace(ImageBytes);
        }
    }
}
