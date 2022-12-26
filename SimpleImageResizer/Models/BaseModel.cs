using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SimpleImageResizer.Models;

/// <summary>
/// Class that provides a base model for all models and view models.
/// </summary>
public abstract class BaseModel : INotifyPropertyChanged
{
    /// <summary>
    /// Event for handling the PropertyChanged.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Create the NotifyPropertyChanged method to raise the event.
    /// The calling member's name will be used as the parameter.
    /// </summary>
    /// <param name="name">Name of the paramater.</param>
    protected void NotifyPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}