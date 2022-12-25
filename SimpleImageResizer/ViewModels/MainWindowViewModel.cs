using System.Windows.Media;

namespace SimpleImageResizer.ViewModels;

public sealed class MainWindowViewModel
{
	/// <summary>
	/// Constructor.
	/// </summary>
	public MainWindowViewModel()
	{

	}

    /* PROPERTIES */

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
}
