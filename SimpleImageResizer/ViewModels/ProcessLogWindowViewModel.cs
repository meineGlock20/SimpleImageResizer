using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

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

        CommandExportToCsv = new Commands.Relay(ExportToCsv, p => Processes is not null);
    }

    public ICommand CommandExportToCsv { get; set; }

    private void ExportToCsv(object o)
    {
        if (Processes is null)
            return;

        Services.UserInterface.SetBusyState();
        string temp = System.IO.Path.GetTempFileName().Replace(".tmp", ".csv");

        using var writer = new StreamWriter(temp);
        using var csv = new CsvHelper.CsvWriter(writer, CultureInfo.InvariantCulture);
        csv.WriteRecords(Processes);

        ProcessStartInfo psi = new()
        {
            FileName = temp,
            UseShellExecute = true,
        };

        Process.Start(psi);
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
