﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace SimpleImageResizer.Core.Imaging;

/// <summary>
/// Class for saving an image to various supported file formats.
/// </summary>
public class Save
{
    /// <summary>
    /// Choose the image type to Save As.
    /// </summary>
    public enum SaveAs
    {
        /// <summary>
        /// Bitmap.
        /// </summary>
        BMP,

        /// <summary>
        /// Gif.
        /// </summary>
        GIF,

        /// <summary>
        /// JFIF.
        /// </summary>
        JFIF,

        /// <summary>
        /// Jpeg.
        /// </summary>
        JPG,

        /// <summary>
        /// Png.
        /// </summary>
        PNG,

        /// <summary>
        /// TIFF.
        /// </summary>
        TIF,

        /// <summary>
        /// Not valid.
        /// </summary>
        NotValid,
    }

    /// <summary>
    /// Gets or sets a value for the PngInterlaceOption. The Default is set to OFF.
    /// </summary>
    public static PngInterlaceOption PngInterlaceOption { get; set; } = PngInterlaceOption.Off;

    /// <summary>
    /// Gets or sets a value for the TiffCompressOption. The default is set to NONE.
    /// </summary>
    public static TiffCompressOption TiffCompressOption { get; set; } = TiffCompressOption.None;

    /// <summary>
    /// Gets or sets a value for the Full Path.
    /// </summary>
    private static string? FullPath { get; set; }

    /// <summary>
    /// Saves an image to the passed path as any valid type selected. Quality is only for JPG/JFIF and default to 70.
    /// You may optionally set additional properties for PNG and TIF files.
    /// </summary>
    /// <param name="bitmapFrame">The BitmapFrame of the image to be saved.</param>
    /// <param name="fileToSave">The path to save the image.</param>
    /// <param name="saveAs">Image type to Save As.</param>
    /// <param name="overwrite">True to overwrite an existing file of the same name.</param>
    /// <param name="quality">Optional. Only for JPG/JFIF. Default = 70.</param>
    /// <returns>Bool.</returns>
    public static bool Image(BitmapFrame bitmapFrame, string fileToSave, SaveAs saveAs, bool overwrite, int quality = 70)
    {
        if (string.IsNullOrWhiteSpace(fileToSave))
        {
            throw new ArgumentNullException(nameof(fileToSave));
        }

        FullPath = fileToSave;
        FileInfo fileInfo = new(FullPath);
        string extension = fileInfo.Extension;
        string extensionSaveAs = ExtensionSaveAs(saveAs);

        FullPath = FullPath.Replace(extension, extensionSaveAs);

        if (File.Exists(FullPath) && !overwrite)
        {
            return false;
        }

        switch (saveAs)
        {
            case SaveAs.BMP:
                Bmp(bitmapFrame);
                break;
            case SaveAs.GIF:
                Gif(bitmapFrame);
                break;
            case SaveAs.JFIF:
                Jfif(bitmapFrame, quality);
                break;
            case SaveAs.JPG:
                Jpg(bitmapFrame, quality);
                break;
            case SaveAs.PNG:
                Png(bitmapFrame);
                break;
            case SaveAs.TIF:
                Tif(bitmapFrame);
                break;
        }

        return true;
    }

    private static string ExtensionSaveAs(SaveAs saveAs)
    {
        return saveAs switch
        {
            SaveAs.BMP => ".bmp",
            SaveAs.GIF => ".gif",
            SaveAs.JFIF => ".jfif",
            SaveAs.JPG => ".jpg",
            SaveAs.PNG => ".png",
            SaveAs.TIF => ".tif",
            SaveAs.NotValid => throw new ArgumentException(nameof(ExtensionSaveAs) + " : " + saveAs + " is not a valid argument."),
            _ => ".jpg",
        };
    }

    private static void Bmp(BitmapFrame bitmapFrame)
    {
        /* Other possibilities:

        //RenderTargetBitmap bitmap = new RenderTargetBitmap(bitmapFrame.PixelWidth, bitmapFrame.PixelHeight,
        //    96, 96, System.Windows.Media.PixelFormats.Bgra32);

        //TransformedBitmap transformedBitmap = new TransformedBitmap();
        //transformedBitmap.BeginInit();
        //transformedBitmap.Source = bitmap;
        //transformedBitmap.Transform = new System.Windows.Media.RotateTransform(90);
        //transformedBitmap.EndInit();
        */
        using FileStream stream = new(FullPath!, FileMode.Create);
        BmpBitmapEncoder encoder = new();
        encoder.Frames.Add(BitmapFrame.Create(bitmapFrame));
        encoder.Save(stream);
    }

    private static void Gif(BitmapFrame bitmapFrame)
    {
        using FileStream stream = new(FullPath!, FileMode.Create);
        GifBitmapEncoder encoder = new();
        encoder.Frames.Add(BitmapFrame.Create(bitmapFrame));
        encoder.Save(stream);
    }

    private static void Jfif(BitmapFrame bitmapFrame, int quality = 70)
    {
        using FileStream stream = new(FullPath!, FileMode.Create);
        JpegBitmapEncoder encoder = new()
        {
            QualityLevel = quality,
        };
        encoder.Frames.Add(BitmapFrame.Create(bitmapFrame));
        encoder.Save(stream);
    }

    private static void Jpg(BitmapFrame bitmapFrame, int quality = 70)
    {
        using FileStream stream = new(FullPath!, FileMode.Create);
        JpegBitmapEncoder encoder = new()
        {
            QualityLevel = quality,

            // you could rotate on encoding ,Rotation = Rotation.Rotate180
        };
        encoder.Frames.Add(BitmapFrame.Create(bitmapFrame));
        encoder.Save(stream);
    }

    private static void Png(BitmapFrame bitmapFrame)
    {
        using FileStream stream = new(FullPath!, FileMode.Create);
        PngBitmapEncoder encoder = new()
        {
            Interlace = PngInterlaceOption,
        };
        encoder.Frames.Add(BitmapFrame.Create(bitmapFrame));
        encoder.Save(stream);
    }

    private static void Tif(BitmapFrame bitmapFrame)
    {
        using FileStream stream = new(FullPath!, FileMode.Create);
        TiffBitmapEncoder encoder = new()
        {
            Compression = TiffCompressOption,
        };
        encoder.Frames.Add(BitmapFrame.Create(bitmapFrame));
        encoder.Save(stream);
    }
}
