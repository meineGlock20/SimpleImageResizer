using SimpleImageResizer.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Xps;

namespace SimpleImageResizer.ViewModels;

public sealed class MainWindowViewModel : Models.BaseModel
{
    private readonly IMessageService MessageService;

    // Backing fields.
    private string? destinationDirectory;
    private ObservableCollection<Models.Image>? images;
    private int imageCount;
    private string? imagesTotalSize;

    /// <summary>
    /// Constructor.
    /// </summary>
    public MainWindowViewModel()
    {
        MessageService = new MessageBoxService();

        DestinationDirectory = Properties.Settings.Default.DestinationDirectory;

        // Commands.
        CommandClearImages = new Commands.Relay(ClearImages, p => true);
        CommandSetDestination = new Commands.Relay(SetDestination, p => true);
        CommandSettings = new Commands.Relay(Settings, p => true);
        CommandBatchProcess = new Commands.Relay(BatchProcess, p => true);
        CommandProcessImages = new Commands.Relay(ProcessImages, p => true);
        CommandOpenDestination = new Commands.Relay(OpenDestination, p => true);
    }

    /* COMMANDS */

    public ICommand CommandClearImages { get; set; }
    public ICommand CommandSetDestination { get; set; }
    public ICommand CommandSettings { get; set; }
    public ICommand CommandBatchProcess { get; set; }
    public ICommand CommandProcessImages { get; set; }
    public ICommand CommandOpenDestination { get; set; }

    /// <summary>
    /// Clears all images from the collection.
    /// </summary>
    /// <param name="o">Command Parameter, not used.</param>
    private void ClearImages(object o)
    {
        Images = null;
    }

    /// <summary>
    /// Sets the destination directory for the resized images.
    /// </summary>
    /// <param name="o">Command Parameter, not used.</param>
    private void SetDestination(object o)
    {
        string? d = Core.DirectoryBrowser.GetDirectory(Localize.MainWindow.DestinationDialogTitle);
        if (!string.IsNullOrWhiteSpace(d))
        {
            Properties.Settings.Default.DestinationDirectory = d;
            Properties.Settings.Default.Save();
            DestinationDirectory= d;
        }
    }

    private void Settings(object o) { }
    private void BatchProcess(object o) { }
    private void ProcessImages(object o) { }

    private void OpenDestination(object o)
    {
        if (!Directory.Exists(DestinationDirectory))
        {
            MessageService.ShowMessage(Localize.MainWindow.ButtonDestinationNotFoundMessage,
                       Localize.MainWindow.ButtonDestinationNotFoundTitle,
                       MessageBoxServiceButton.Ok,
                       MessageBoxServiceIcon.Information,
                       Window);
            return;
        }

        ProcessStartInfo psi = new()
        {
            FileName = DestinationDirectory,
            UseShellExecute = true,
        };

        Process.Start(psi);
    }

    /* METHODS */

    public void AddImagesToCollection(Models.Image i)
    {
        Images ??= new();
        Images.Add(i);
    }

    /* PROPERTIES */

    /// <summary>
    /// Gets or sets a value indicating the calling window.
    /// </summary>
    /// <remarks>
    /// This is used to properly position another window such as the message box by owner. Example: WindowStartupLocation="CenterOwner".
    /// This does not break MVVM because if its null it does not matter - it does not rely on the View in any way.
    /// </remarks>
    public Window? Window { get; set; }

    /// <summary>
    /// Gets a value to set the background of the element to show the Welcome message
    /// if there are no images to display.
    /// </summary>
    public DrawingBrush? BackgroundDrawingBrush
    {
        get
        {
            if (Images is null || Images.Count == 0)
            {
                return new DrawingBrush(Core.Draw.WelcomeText(Core.MyApplication.DpiScale));
            }
            else
            {
                return null;
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating the directory used to save resized images.
    /// </summary>
    public string? DestinationDirectory
    {
        get => destinationDirectory;
        set
        {
            destinationDirectory = value;
            NotifyPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating the number of images in the collection.
    /// </summary>
    public int ImageCount
    {
        get => imageCount;
        set
        {
            imageCount = value;
            NotifyPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating the total size of all selected images to be processed.
    /// </summary>
    public string? ImagesTotalSize
    {
        get => imagesTotalSize;
        set
        {
            imagesTotalSize = value;
            NotifyPropertyChanged();
        }
    }

    /* COLLECTION PROPERTIES */

    public ObservableCollection<Models.Image>? Images
    {
        get => images;
        set
        {
            if (value is not null) value.CollectionChanged += Images_CollectionChanged;
            images = value;
            NotifyPropertyChanged();
            NotifyPropertyChanged(nameof(BackgroundDrawingBrush));
            if (value is null)
            {
                ImageCount = 0;
                ImagesTotalSize = null;
            }
        }
    }

    /* EVENTS */

    private void Images_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (Images is not null)
        {
            ImageCount = Images.Count;
            ImagesTotalSize = Core.Calculate.CalculateSpace(Images.Sum(x => x.ImageBytes),
                CultureInfo.CurrentCulture.Name, Core.RoundToDecimalPlaces.One);
        }

        NotifyPropertyChanged(nameof(BackgroundDrawingBrush));
    }
}
