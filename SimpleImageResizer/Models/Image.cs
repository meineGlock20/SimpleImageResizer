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
                NotifyPropertyChanged(ImageName);
                NotifyPropertyChanged(FileType);
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

    /// <summary>
    /// Gets a value indicating the name of the image without the extension.
    /// </summary>
    public string? ImageName
    {
        get
        {
            if (string.IsNullOrWhiteSpace(FullPathToImage)) return null;

            FileInfo fileInfo = new(FullPathToImage);
            return fileInfo.Name.Replace(fileInfo.Extension, string.Empty);
        }
    }

    /// <summary>
    /// Gets a value indicating the type of image file by returning the extension.
    /// </summary>
    public string? FileType
    {
        get
        {
            if (string.IsNullOrWhiteSpace(FullPathToImage)) return null;

            FileInfo fileInfo = new(FullPathToImage);
            return fileInfo.Extension.ToUpper();
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
