using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleImageResizer.ViewModels;

public sealed class AboutWindowViewModel : Models.BaseModel
{
    private string? version;

    /// <summary>
    /// Constructor.
    /// </summary>
    public AboutWindowViewModel()
    {
        Version = Core.MyApplication.Version;
    }

    public string? Version
    {
        get => version;
        set
        {
            version = value;
            NotifyPropertyChanged();
        }
    }
}
