using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleImageResizer.Models;

public sealed class Process
{
    public long ProcessId { get; set; }
    public DateTime ProcessStart { get; set; }
    public DateTime ProcessEnd { get; set; }
    public long ImageCount { get; set; }
    public long ImagesOriginalSize { get; set; }
    public long ImagesProcessedSize { get; set; }
}