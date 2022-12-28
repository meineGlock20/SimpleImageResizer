using SimpleImageResizer.Services;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Xps;

namespace SimpleImageResizer.ViewModels;

public sealed class MainWindowViewModel : Models.BaseModel, INotifyDataErrorInfo
{
    private readonly Dictionary<string, List<string>> errorList = new();

    private readonly IMessageService MessageService;

    // Backing fields.
    private ObservableCollection<Models.Image>? images;
    private ObservableCollection<Models.ScalingOption>? scalingOptions;
    private Models.ScalingOption? selectedScalingOption;
    private ObservableCollection<Models.ImageType>? imageTypes;
    private Models.ImageType? selectedImageType;

    private string? destinationDirectory;
    private int imageCount;
    private string? imagesTotalSize;
    private bool showSettings;
    private int imageProcessingProgress;
    private string? imageProcessingProgressText;
    private int simpleResizeSetting;
    private string? simpleResizeSettingDisplay;
    private bool useSimple;
    private bool usePercentage;
    private bool useAbsolute;
    private bool useAspect;

    private string? resizePercentage;
    private string? resizeAbsoluteX;
    private string? resizeAbsoluteY;

    /// <summary>
    /// Constructor.
    /// </summary>
    public MainWindowViewModel()
    {
        MessageService = new MessageBoxService();

        DestinationDirectory = Properties.Settings.Default.DestinationDirectory;
        SimpleResizeSetting = Properties.Settings.Default.SimpleResizeSetting;

        // Default use to simple.
        UseSimple = true;

        ScalingOptions = new()
        {
            new Models.ScalingOption() { Option = Core.Imaging.Resize.ScalingOption.Width, OptionDisplay = Localize.MainWindow.ScalingOptionWidth },
            new Models.ScalingOption() { Option = Core.Imaging.Resize.ScalingOption.Height, OptionDisplay = Localize.MainWindow.ScalingOptionHeight },
        };

        ImageTypes = new()
        {
           new Models.ImageType(){Type= Core.Imaging.ImageTypes.ImageType.original, TypeName=Localize.MainWindow.ImageTypeOriginal},
           new Models.ImageType(){Type=Core.Imaging.ImageTypes.ImageType.bmp, TypeName=Localize.MainWindow.ImageTypeBitmap},
           new Models.ImageType(){Type=Core.Imaging.ImageTypes.ImageType.gif, TypeName=Localize.MainWindow.ImageTypeGif},
           new Models.ImageType(){Type=Core.Imaging.ImageTypes.ImageType.jfif, TypeName=Localize.MainWindow.ImageTypeJfif},
           new Models.ImageType(){Type=Core.Imaging.ImageTypes.ImageType.jpg, TypeName=Localize.MainWindow.ImageTypeJpg},
           new Models.ImageType(){Type=Core.Imaging.ImageTypes.ImageType.png, TypeName=Localize.MainWindow.ImageTypePng},
           new Models.ImageType(){Type=Core.Imaging.ImageTypes.ImageType.tif, TypeName=Localize.MainWindow.ImageTypeTif},
        };

        // Commands.
        CommandClearImages = new Commands.Relay(ClearImages, p => true);
        CommandSetDestination = new Commands.Relay(SetDestination, p => true);
        CommandSettings = new Commands.Relay(Settings, p => true);
        CommandBatchProcess = new Commands.Relay(BatchProcess, p => true);
        CommandProcessImages = new Commands.RelayAsync(ProcessImages, p => true);
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
            DestinationDirectory = d;
        }
    }

    /// <summary>
    /// Shows/hides the settings pane.
    /// </summary>
    /// <param name="o">Command Parameter, not used.</param>
    private void Settings(object o)
    {
        ShowSettings = !ShowSettings;
    }

    /// <summary>
    /// Opens the window responsible for performing a batch process.
    /// </summary>
    /// <param name="o">Command Parameter, not used.</param>
    private void BatchProcess(object o)
    {
        // TODO: Open another window for the batch process.
    }

    /// <summary>
    /// Processes the images selected for resizing.
    /// </summary>
    /// <param name="o">Command Parameter, not used.</param>
    private async Task ProcessImages(object o)
    {
        await ProcessImagesAsync();
    }

    /// <summary>
    /// Opens the destination directory for resized images.
    /// </summary>
    /// <param name="o">Command Parameter, not used.</param>
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

    private async Task ProcessImagesAsync()
    {
        if (string.IsNullOrWhiteSpace(DestinationDirectory) || Images is null)
            throw new ArgumentNullException();

        if (!Directory.Exists(DestinationDirectory))
            Directory.CreateDirectory(DestinationDirectory);

        // This allows progress to be reported back to the UI when a long running TASK is run on another thread in an await.
        IProgress<int> progress = new Progress<int>(percentCompleted =>
        {
            ImageProcessingProgress = percentCompleted;
            ImageProcessingProgressText = $"{percentCompleted} %";
        });




        await Task.Run(() => Parallel.ForEach(Images, new ParallelOptions { MaxDegreeOfParallelism = 0 }, image =>
        {

        }));

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

    /// <summary>
    /// Gets or sets a value indicating whether to show or hide the settings pane.
    /// </summary>
    public bool ShowSettings
    {
        get => showSettings;
        set
        {
            showSettings = value;
            NotifyPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating the progress of the processing of images.
    /// </summary>
    public int ImageProcessingProgress
    {
        get => imageProcessingProgress;

        set
        {
            if (imageProcessingProgress != value)
            {
                imageProcessingProgress = value;
                NotifyPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating the progress of the processing of images using percentage.
    /// </summary>
    public string? ImageProcessingProgressText
    {
        get => imageProcessingProgressText;

        set
        {
            if (imageProcessingProgressText != value)
            {
                imageProcessingProgressText = value;
                NotifyPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating the value used to resize images using the simple method.
    /// </summary>
    public int SimpleResizeSetting
    {
        get => simpleResizeSetting;
        set
        {
            simpleResizeSetting = value;
            Properties.Settings.Default.SimpleResizeSetting = value;
            Properties.Settings.Default.Save();
            SimpleResizeSettingDisplay = value switch
            {
                0 => Localize.MainWindow.SimpleImageResizeThumbnail,
                25 => Localize.MainWindow.SimpleImageResizeSmall,
                50 => Localize.MainWindow.SimpleImageResizeMedium,
                75 => Localize.MainWindow.SimpleImageResizeLarge,
                _ => Localize.MainWindow.SimpleImageResizeUnknown,
            };
            NotifyPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating the simple resize mode; thumbnail, small, medium, or large.
    /// </summary>
    public string? SimpleResizeSettingDisplay
    {
        get => simpleResizeSettingDisplay;
        set
        {
            simpleResizeSettingDisplay = value;
            NotifyPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating if Simple resizing should be used.
    /// </summary>
    public bool UseSimple
    {
        get => useSimple;
        set
        {
            useSimple = value;
            NotifyPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating if resizing by Percentage should be used.
    /// </summary>
    public bool UsePercentage
    {
        get => usePercentage;
        set
        {
            usePercentage = value;
            NotifyPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating if resizing by Absolute Size should be used.
    /// </summary>
    public bool UseAbsolute
    {
        get => useAbsolute;
        set
        {
            useAbsolute = value;
            NotifyPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating if resizing by Aspect Ratio should be used.
    /// </summary>
    public bool UseAspect
    {
        get => useAspect;
        set
        {
            useAspect = value;
            NotifyPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating the amount to resize by percentage.
    /// </summary>
    public string? ResizePercentage
    {
        get => resizePercentage;
        set
        {
            resizePercentage = value;
            ValidateProperties();
            NotifyPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating the width for an absolute resize.
    /// </summary>
    public string? ResizeAbsoluteX
    {
        get => resizeAbsoluteX;
        set
        {
            resizeAbsoluteX = value;
            ValidateProperties();
            NotifyPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating the height for an absolute resize.
    /// </summary>
    public string? ResizeAbsoluteY
    {
        get => resizeAbsoluteY;
        set
        {
            resizeAbsoluteY = value;
            ValidateProperties();
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

    public ObservableCollection<Models.ScalingOption>? ScalingOptions
    {
        get => scalingOptions;
        set
        {
            scalingOptions = value;
            NotifyPropertyChanged();
        }
    }

    public Models.ScalingOption? SelectedScalingOption
    {
        get => selectedScalingOption;
        set
        {
            selectedScalingOption = value;
            NotifyPropertyChanged();
        }
    }

    public ObservableCollection<Models.ImageType>? ImageTypes
    {
        get => imageTypes;
        set
        {
            imageTypes = value;
            NotifyPropertyChanged();
        }
    }

    public Models.ImageType? SelectedImageType
    {
        get => selectedImageType;
        set
        {
            selectedImageType = value;
            NotifyPropertyChanged();
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

    /* ERROR HANDLING */

    /// <summary>
    /// Gets a value indicating whether there are any errors in the error list.
    /// </summary>
    public bool HasErrors => errorList.Any();

    /// <summary>
    /// Event fires when a validation error occurs for a property or an entire entity.
    /// </summary>
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    /// <summary>
    /// Validates passed properties for errors and if any, adds them to the dictionary of errorList.
    /// </summary>
    /// <param name="propertyName">Property to check for errors.</param>
    private void ValidateProperties([CallerMemberName] string propertyName = "")
    {
        if (propertyName == nameof(ResizePercentage))
        {
            ClearErrors(propertyName);
            if (!int.TryParse(ResizePercentage, out int i) || i < 1 || i > 99)
                AddError(propertyName, "Percentage must be set between 1 and 99.");
        }

        if (propertyName == nameof(ResizeAbsoluteX))
        {
            ClearErrors(propertyName);
            if (!int.TryParse(ResizeAbsoluteX, out int i) || i < 1 || i > 100000)
                AddError(propertyName, "Percentage must be set between 1 and 100000.");
        }

        if (propertyName == nameof(ResizeAbsoluteY))
        {
            ClearErrors(propertyName);
            if (!int.TryParse(ResizeAbsoluteY, out int i) || i < 1 || i > 100000)
                AddError(propertyName, "Percentage must be set between 1 and 100000.");
        }
    }

    /// <summary>
    /// Adds an error to the dictionary.
    /// </summary>
    /// <param name="propertyName">Property name.</param>
    /// <param name="error">Error.</param>
    private void AddError(string propertyName, string error)
    {
        if (!errorList.ContainsKey(propertyName))
        {
            errorList[propertyName] = new List<string>();
        }

        if (!errorList[propertyName].Contains(error))
        {
            errorList[propertyName].Add(error);
            OnErrorsChanged(propertyName);
        }

        NotifyPropertyChanged(nameof(HasErrors));
    }

    /// <summary>
    /// Gets a list of errors by property.
    /// </summary>
    /// <param name="propertyName">Property name.</param>
    /// <returns>A list of errors for a property or null if none exist.</returns>
    public IEnumerable GetErrors([CallerMemberName] string? propertyName = "")
    {
        if (errorList.TryGetValue(propertyName ?? "", out List<string>? value))
            return value;
        else
            return Enumerable.Empty<string>();
    }

    /// <summary>
    /// Clears any errors from the dictionary for the passed property from ValidateProperties.
    /// </summary>
    /// <param name="propertyName"></param>
    private void ClearErrors(string propertyName)
    {
        errorList.Remove(propertyName);
        NotifyPropertyChanged(nameof(HasErrors));
    }

    /// <summary>
    /// Whenever the error condition of a property changes, invoke the DataErrorsChangedEventArgs.
    /// Called from AddError and ClearErrors.
    /// </summary>
    /// <param name="propertyName">propertyName.</param>
    private void OnErrorsChanged(string propertyName)
    {
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }

    /* END ERROR HANDLING */
}