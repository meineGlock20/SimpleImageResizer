using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleImageResizer.Models;

public class ImageType
{
    public Core.Imaging.ImageTypes.ImageType? Type { get; set; }

    public string? TypeName { get; set; }
}