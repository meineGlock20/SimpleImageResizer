using SimpleImageResizer.Services;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Xps;

namespace SimpleImageResizer.ViewModels;

public sealed class MainWindowViewModel : Models.BaseModel, INotifyDataErrorInfo
{
    private readonly Dictionary<string, List<string>> errorList = new();

    private readonly IMessageService MessageService;

    private const int jpgDefault = 70;

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
    private string? resizeAspect;
    private bool optionOverwrite;
    private bool optionAddNumericSuffix;
    private bool optionShowMessageBox;
    private bool optionClearImages;
    private bool optionUseAllProcessors;
    private string? optionJpgQuality;
    private bool cancelProcess;
    private bool processingInProgress;

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
        OptionOverwrite = Properties.Settings.Default.OptionOverwrite;
        OptionAddNumericSuffix = Properties.Settings.Default.OptionAddNumericSuffix;
        OptionShowMessageBox = Properties.Settings.Default.OptionShowMessageBox;
        OptionClearImages = Properties.Settings.Default.OptionClearImages;
        OptionUseAllProcessors = Properties.Settings.Default.OptionUseAllProcessors;
        OptionJpgQuality = Properties.Settings.Default.OptionJpgQuality.ToString();

        ScalingOptions = new()
        {
            new Models.ScalingOption() { Option = Core.Imaging.Resize.ScalingOption.Width, OptionDisplay = Localize.MainWindow.ScalingOptionWidth },
            new Models.ScalingOption() { Option = Core.Imaging.Resize.ScalingOption.Height, OptionDisplay = Localize.MainWindow.ScalingOptionHeight },
        };

        ImageTypes = new()
        {
           new Models.ImageType(){Type= null, TypeName=Localize.MainWindow.ImageTypeOriginal},
           new Models.ImageType(){Type=Core.Imaging.ImageTypes.ImageType.bmp, TypeName=Localize.MainWindow.ImageTypeBitmap},
           new Models.ImageType(){Type=Core.Imaging.ImageTypes.ImageType.gif, TypeName=Localize.MainWindow.ImageTypeGif},
           new Models.ImageType(){Type=Core.Imaging.ImageTypes.ImageType.jfif, TypeName=Localize.MainWindow.ImageTypeJfif},
           new Models.ImageType(){Type=Core.Imaging.ImageTypes.ImageType.jpg, TypeName=Localize.MainWindow.ImageTypeJpg},
           new Models.ImageType(){Type=Core.Imaging.ImageTypes.ImageType.png, TypeName=Localize.MainWindow.ImageTypePng},
           new Models.ImageType(){Type=Core.Imaging.ImageTypes.ImageType.tif, TypeName=Localize.MainWindow.ImageTypeTif},
        };

        // Commands.
        CommandClearImages = new Commands.Relay(ClearImages, p => !ProcessingInProgress);
        CommandSetDestination = new Commands.Relay(SetDestination, p => !ProcessingInProgress);
        CommandSettings = new Commands.Relay(Settings, p => !ProcessingInProgress);
        CommandBatchProcess = new Commands.Relay(BatchProcess, p => !ProcessingInProgress);
        CommandProcessImages = new Commands.RelayAsync(ProcessImages, p => Images is not null && Images.Count > 0 && !ProcessingInProgress);
        CommandOpenDestination = new Commands.Relay(OpenDestination, p => true);
        CommandResetJpgDefault = new Commands.Relay(ResetJpgDefault, p => OptionJpgQuality != jpgDefault.ToString());
        CommandCancelProcess = new Commands.Relay(ExecuteCancelProcess, p => ProcessingInProgress);
    }

    /* COMMANDS */

    public ICommand CommandClearImages { get; set; }
    public ICommand CommandSetDestination { get; set; }
    public ICommand CommandSettings { get; set; }
    public ICommand CommandBatchProcess { get; set; }
    public ICommand CommandProcessImages { get; set; }
    public ICommand CommandOpenDestination { get; set; }
    public ICommand CommandResetJpgDefault { get; set; }
    public ICommand CommandCancelProcess { get; set; }

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

    /// <summary>
    /// Resets the JPG default quality.
    /// </summary>
    /// <param name="o">Command Parameter, not used.</param>
    private void ResetJpgDefault(object o)
    {
        OptionJpgQuality = jpgDefault.ToString();
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

        // Determine the number of processors to use. If this computer only has 1 processor, then just use 1.
        // Otherwise, use all available less one.
        int processors = Environment.ProcessorCount == 1 ? 1 : Properties.Settings.Default.OptionUseAllProcessors ? Environment.ProcessorCount - 1 : 1;

        // Track bytes of resized images.
        long resizedBytes = 0;

        // This allows progress to be reported back to the UI when a long running TASK is run on another thread in an await.
        double counter = 0;
        IProgress<int> progress = new Progress<int>(percentCompleted =>
        {
            ImageProcessingProgress = percentCompleted;
            ImageProcessingProgressText = $"{percentCompleted} %";
        });

        ProcessingInProgress = true;

        // Each operation goes to a date time marked directory.
        string destination = Path.Combine(DestinationDirectory, DateTime.Now.ToString("yyyyMMddTHHmmss"));
        Directory.CreateDirectory(destination);

        using var cancellationTokenSource = new CancellationTokenSource();

        // Begin timing operation.
        var stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();

        try
        {
            await Task.Run(() => Parallel.ForEach(Images, new ParallelOptions { MaxDegreeOfParallelism = processors, CancellationToken = cancellationTokenSource.Token }, image =>
            {
                if (CancelProcess) cancellationTokenSource.Cancel();

                if (string.IsNullOrWhiteSpace(image.FullPathToImage) || string.IsNullOrWhiteSpace(image.ImageName))
                    throw new ArgumentNullException(nameof(image.FullPathToImage));

                // Resize based on options.
                BitmapFrame bitmapFrame;
                if (UseSimple || UsePercentage)
                {
                    bitmapFrame = Core.Imaging.Resize.Image(image.FullPathToImage, UseSimple ? SimpleResizeSetting : int.Parse(ResizePercentage!));
                }
                else if (UseAbsolute)
                {
                    bitmapFrame = Core.Imaging.Resize.Image(image.FullPathToImage, Core.Imaging.Resize.ScalingOption.None, int.Parse(ResizeAbsoluteX!), int.Parse(ResizeAbsoluteY!));
                }
                else if (UseAspect && SelectedScalingOption?.Option == Core.Imaging.Resize.ScalingOption.Width)
                {
                    bitmapFrame = Core.Imaging.Resize.Image(image.FullPathToImage, Core.Imaging.Resize.ScalingOption.Width, 0, int.Parse(ResizeAspect!));
                }
                else if (UseAspect && SelectedScalingOption?.Option == Core.Imaging.Resize.ScalingOption.Height)
                {
                    bitmapFrame = Core.Imaging.Resize.Image(image.FullPathToImage, Core.Imaging.Resize.ScalingOption.Height, int.Parse(ResizeAspect!), 0);
                }
                else
                {
                    // This should never happen but.
                    throw new ArgumentNullException("bitmapFrame", "The bitmapFrame could not be created!");
                }

                // Save.
                Core.Imaging.ImageTypes.ImageType saveAs;
                if (SelectedImageType?.Type is null)
                    saveAs = image.ImageType;
                else
                    saveAs = (Core.Imaging.ImageTypes.ImageType)SelectedImageType.Type;

                string imagename = image.ImageName;

                Core.Imaging.Save.Image(bitmapFrame, Path.Combine(destination, $"{imagename}"), saveAs, OptionOverwrite, int.Parse(OptionJpgQuality ?? "70"));

                // Track resized bytes.
                resizedBytes += new FileInfo(Path.Combine(destination, $"{imagename}")).Length;

                // Report progress.
                counter++;
                double pro = counter / ImageCount * 100d;
                progress.Report((int)pro);
            }));
        }
        catch (TaskCanceledException)
        {
            // There is nothing to do here. Finally will take over.
            Debug.Print("Canceled.");
        }
        catch (AggregateException ae)
        {
            // When Parallel processing, exceptions will aggregate and can be caught here.
            Debug.Print(ae.InnerException?.ToString());
        }
        finally
        {
            // On rare occassion the progress would end at 98% or 99% although all processing was complete.
            // So we will just make sure it says 100. This would also apply to a canceled operation as well.
            ImageProcessingProgress = 100;
            ImageProcessingProgressText = $"100 %";

            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;

            // Format the TimeSpan value for display.
            string elapsedTime = string.Format(
                "{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours,
                ts.Minutes,
                ts.Seconds,
                ts.Milliseconds / 10);

            if (OptionShowMessageBox)
            {
                MessageService.ShowMessage($"{Localize.MainWindow.ProcessingCompleteMsgImagesProcessed} {counter}\r\n{Localize.MainWindow.ProcessingCompleteMsgElapsedTime}: {elapsedTime}",
                    Localize.MainWindow.ProcessingCompleteMsgTitle, MessageBoxServiceButton.Ok, MessageBoxServiceIcon.Information, Window);
            }

            // Reset.
            progress.Report(0);
            ProcessingInProgress = false;
            CancelProcess = false;
            if (OptionClearImages) Images = null;
        }
    }

    private void ExecuteCancelProcess(object o)
    {
        CancelProcess = true;
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

    /// <summary>
    /// Gets or sets a value indicating the x or y value when resizing by aspect ratio.
    /// </summary>
    public string? ResizeAspect
    {
        get => resizeAspect;
        set
        {
            resizeAspect = value;
            ValidateProperties();
            NotifyPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether to overwrite existing images in the destination directory.
    /// </summary>
    public bool OptionOverwrite
    {
        get => optionOverwrite;
        set
        {
            optionOverwrite = value;
            Properties.Settings.Default.OptionOverwrite = value;
            Properties.Settings.Default.Save();
            NotifyPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether to add a numeric suffix to the end of the resized image file name.
    /// </summary>
    public bool OptionAddNumericSuffix
    {
        get => optionAddNumericSuffix;
        set
        {
            optionAddNumericSuffix = value;
            Properties.Settings.Default.OptionAddNumericSuffix = value;
            Properties.Settings.Default.Save();
            NotifyPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether to show a message box when image processing is complete.
    /// </summary>
    public bool OptionShowMessageBox
    {
        get => optionShowMessageBox;
        set
        {
            optionShowMessageBox = value;
            Properties.Settings.Default.OptionShowMessageBox = value;
            Properties.Settings.Default.Save();
            NotifyPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether to clear all images from the list box when processing is complete.
    /// </summary>
    public bool OptionClearImages
    {
        get => optionClearImages;
        set
        {
            optionClearImages = value;
            Properties.Settings.Default.OptionClearImages = value;
            Properties.Settings.Default.Save();
            NotifyPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether to use all available processors when resizing images.
    /// </summary>
    public bool OptionUseAllProcessors
    {
        get => optionUseAllProcessors;
        set
        {
            optionUseAllProcessors = value;
            Properties.Settings.Default.OptionUseAllProcessors = value;
            Properties.Settings.Default.Save();
            NotifyPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating the quality of the resized JPG image. Only for JPGs.
    /// </summary>
    public string? OptionJpgQuality
    {
        get => optionJpgQuality;
        set
        {
            optionJpgQuality = value;
            ValidateProperties();
            if (!GetErrors().Cast<object>().ToList().Any())
            {
                Properties.Settings.Default.OptionJpgQuality = int.Parse(value ?? "70");
                Properties.Settings.Default.Save();
            }
            NotifyPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the processing operation should be cancelled.
    /// </summary>
    public bool CancelProcess
    {
        get => cancelProcess;
        set
        {
            cancelProcess = value;
            NotifyPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the resizing process in active.
    /// </summary>
    /// <remarks>
    /// This is used to disable certain functions like adding or removing images etc while processing.
    /// </remarks>
    public bool ProcessingInProgress
    {
        get => processingInProgress;
        set
        {
            processingInProgress = value;
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
        ClearErrors(propertyName);

        switch (propertyName)
        {
            case nameof(ResizePercentage):
                {
                    if (!int.TryParse(ResizePercentage, out int i) || i < 1 || i > 99)
                        AddError(propertyName, Localize.MainWindow.ValidatePercentage);
                }
                break;
            case nameof(ResizeAbsoluteX):
                {
                    if (!int.TryParse(ResizeAbsoluteX, out int i) || i < 1 || i > 100000)
                        AddError(propertyName, Localize.MainWindow.ValidateValue1and100000);
                }
                break;
            case nameof(ResizeAbsoluteY):
                {
                    if (!int.TryParse(ResizeAbsoluteY, out int i) || i < 1 || i > 100000)
                        AddError(propertyName, Localize.MainWindow.ValidateValue1and100000);
                }
                break;
            case nameof(ResizeAspect):
                {
                    if (!int.TryParse(ResizeAspect, out int i) || i < 1 || i > 100000)
                        AddError(propertyName, Localize.MainWindow.ValidateValue1and100000);
                }
                break;
            case nameof(OptionJpgQuality):
                {
                    if (!int.TryParse(OptionJpgQuality, out int i) || i < 30 || i > 100)
                        AddError(propertyName, Localize.MainWindow.ValidateJpg);
                }
                break;
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
    /// <returns>A list of errors for a property or empty if none exist.</returns>
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