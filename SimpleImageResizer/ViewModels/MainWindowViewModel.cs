using SimpleImageResizer.Services;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Xps;

namespace SimpleImageResizer.ViewModels;

public sealed class MainWindowViewModel : Models.BaseModel
{
    private readonly IMessageService MessageService;

    // Backing fields.
    private string? destinationDirectory;

    /// <summary>
    /// Constructor.
    /// </summary>
    public MainWindowViewModel()
    {
        MessageService = new MessageBoxService();

        DestinationDirectory = Properties.Settings.Default.DestinationDirectory;

        // Commands.
        CommandSelectImages = new Commands.Relay(SelectImages, p => true);
        CommandClearImages = new Commands.Relay(ClearImages, p => true);
        CommandSetDestination = new Commands.Relay(SetDestination, p => true);
        CommandSettings = new Commands.Relay(Settings, p => true);
        CommandBatchProcess = new Commands.Relay(BatchProcess, p => true);
        CommandProcessImages = new Commands.Relay(ProcessImages, p => true);
        CommandOpenDestination = new Commands.Relay(OpenDestination, p => true);
    }

    /* COMMANDS */

    public ICommand CommandSelectImages { get; set; }
    public ICommand CommandClearImages { get; set; }
    public ICommand CommandSetDestination { get; set; }
    public ICommand CommandSettings { get; set; }
    public ICommand CommandBatchProcess { get; set; }
    public ICommand CommandProcessImages { get; set; }
    public ICommand CommandOpenDestination { get; set; }

    private void SelectImages(object o)
    {
        // TODO: Select Images.

    }

    private void ClearImages(object o) { }

    private void SetDestination(object o)
    {
        string caption = "Select the destination directory for your resized images.";
        DestinationDirectory = Core.DirectoryBrowser.GetDirectory(caption);
    }

    private void Settings(object o) { }
    private void BatchProcess(object o) { }
    private void ProcessImages(object o) { }

    private void OpenDestination(object o)
    {
        if (!Directory.Exists(DestinationDirectory))
        {
            MessageService.ShowMessage("The specified directory does not exist.\r\nHowever, it will be created when images are resized.",
                       "Directory Does Not Exist!",
                       Views.MessageBoxWindow.MessageBoxButton.Ok,
                       Views.MessageBoxWindow.MessageBoxIcon.Information,
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

    /* PROPERTIES */

    /// <summary>
    /// Gets or sets a value indicating the calling window.
    /// </summary>
    /// <remarks>
    /// This is used to properly position another window such as the message box by owner. Example: WindowStartupLocation="CenterOwner".
    /// This does not break MVVM because if its null it does not matter.
    /// </remarks>
    public Window? Window { get; set; }

    /// <summary>
    /// Gets a value to set the background of the element to show the Welcome message
    /// if there are no images to display.
    /// </summary>
    public DrawingBrush BackgroundDrawingBrush
    {
        get
        {
            return new DrawingBrush(Core.Draw.WelcomeText(Core.MyApplication.DpiScale));
            //if (this.ImageCount == 0)
            //{
            //}
            //else
            //{
            //    return null;
            //}
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
}
