using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleImageResizer.ViewModels;

public sealed class ProcessLogWindowViewModel : Models.BaseModel
{
    private readonly SynchronizationContext uiContext = SynchronizationContext.Current!;
    private List<Models.Process>? processes;
    private long totalImages;
    private string? sizeDifference;

    /// <summary>
    /// Constructor.
    /// </summary>
    public ProcessLogWindowViewModel()
    {
        // To load data from the constructor, create a task that runs async on another thread.
        // It awaits until it gets the data and then sends it to the LoadProcessingLog method via
        // the uiContext. This allows the window to open immediately and keeps the UI responsive
        // while the data loads.
        // In addition, set the datagrid property IsAsync in the binding like this: ItemsSource="{Binding Processes, IsAsync=True}".
        Task.Run(async () =>
        {
            List<Models.Process>? p = await Core.Data.GetProcesses();
            uiContext.Send(x => LoadProcessingLog(p), null);
        });
    }

    private void LoadProcessingLog(List<Models.Process>? processes)
    {
        Processes = processes ?? new List<Models.Process>();

        TotalImages = Processes.Sum(x => x.ImageCount);
        long a = Processes.Sum(x => x.ImagesOriginalSize);
        long b = Processes.Sum(x => x.ImagesProcessedSize);
        SizeDifference = Core.Calculate.CalculateSpace(a - b, CultureInfo.CurrentCulture.Name, Core.RoundToDecimalPlaces.Two);
    }

    public List<Models.Process>? Processes
    {
        get => processes;
        set
        {
            processes = value;
            NotifyPropertyChanged();
        }
    }

    public long TotalImages
    {
        get => totalImages;
        set
        {
            totalImages = value;
            NotifyPropertyChanged();
        }
    }

    public string? SizeDifference
    {
        get => sizeDifference;
        set
        {
            sizeDifference = value;
            NotifyPropertyChanged();
        }
    }
}
