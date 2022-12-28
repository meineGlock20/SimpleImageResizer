using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleImageResizer.Models;

public class ScalingOption
{
    public Core.Imaging.Resize.ScalingOption Option { get; set; }

    public string? OptionDisplay { get; set; }
}
