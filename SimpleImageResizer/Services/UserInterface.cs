using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace SimpleImageResizer.Services;

/// <summary>
/// Contains User Interface Services.
/// To set the cursor to busy inside of a view model in a long running operation on the UI thread
/// simply call Services.UserInterface.SetBusyState().
/// Then perform your processing. The state will return the default cursor when done.
/// </summary>
public static class UserInterface
{
    /// <summary>
    /// A value indicating whether the UI is currently busy.
    /// </summary>
    private static bool isBusy;

    /// <summary>
    /// Sets the busystate as busy.
    /// </summary>
    public static void SetBusyState()
    {
        SetBusyState(true);
    }

    /// <summary>
    /// Sets the busystate to busy or not busy.
    /// </summary>
    /// <param name="busy">if set to <c>true</c> the application is now busy.</param>
    private static void SetBusyState(bool busy)
    {
        if (busy != isBusy)
        {
            isBusy = busy;
            Mouse.OverrideCursor = busy ? Cursors.Wait : null;

            if (isBusy)
            {
                _ = new DispatcherTimer(
                    TimeSpan.FromSeconds(0),
                    DispatcherPriority.ApplicationIdle,
                    DispatcherTimer_Tick,
                    System.Windows.Application.Current.Dispatcher);
            }
        }
    }

    /// <summary>
    /// Handles the Tick event of the dispatcherTimer control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private static void DispatcherTimer_Tick(object? sender, EventArgs e)
    {
        if (sender is not null && sender is DispatcherTimer dispatcherTimer)
        {
            SetBusyState(false);
            dispatcherTimer.Stop();
        }
    }
}