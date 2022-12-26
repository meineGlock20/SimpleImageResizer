using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleImageResizer.Services;

public enum MessageBoxServiceButton
{
    /// <summary>
    /// Use only the Ok button.
    /// </summary>
    Ok,

    /// <summary>
    /// Use Yes and No buttons.
    /// </summary>
    YesNo,

    /// <summary>
    /// User Yes, No, and Cancel buttons.
    /// </summary>
    YesNoCancel,
}

public enum MessageBoxServiceIcon
{
    /// <summary>
    /// Access Denied icon.
    /// </summary>
    AccessDenied,

    /// <summary>
    /// Check mark icon.
    /// </summary>
    CheckMark,

    /// <summary>
    /// Error or Exception icon.
    /// </summary>
    Error,

    /// <summary>
    /// Information icon.
    /// </summary>
    Information,

    /// <summary>
    /// Network Connection Error icon.
    /// </summary>
    NetworkConnectionError,

    /// <summary>
    /// Question icon.
    /// </summary>
    Question,

    /// <summary>
    /// Thinking emoji icon.
    /// </summary>
    Thinking,

    /// <summary>
    /// Warning icon.
    /// </summary>
    Warning,

    /// <summary>
    /// Robot. It's a robot.
    /// </summary>
    Robot,
}

public interface IMessageService
{
    bool? ShowMessage(string message,
        string title,
        MessageBoxServiceButton button,
        MessageBoxServiceIcon icon,
        Window? owner);
}

public class MessageBoxService : IMessageService
{
    /// <summary>
    /// Shows a message to the user and returns a bool value based on button selection.
    /// </summary>
    /// <remarks>
    /// This ensures that the View Model has no idea of the existence of the View used to show a message. 
    /// You could easily switch the message box here to the built-in windows message box or another custom one.
    /// </remarks>
    /// <param name="message">Message to display to the user.</param>
    /// <param name="title">Title of the message box.</param>
    /// <param name="button">Message box button(s) to display.</param>
    /// <param name="icon">Message box icon to display.</param>
    /// <param name="owner">The owner of the message box. Usually a window but can be null.</param>
    /// <returns>False if No or Cancelled was selected, otherwise True is returned.</returns>
    public bool? ShowMessage(string message,
        string title,
        MessageBoxServiceButton button,
        MessageBoxServiceIcon icon,
        Window? owner)
    {
        Views.MessageBoxWindow.MessageBoxButton b = button switch
        {
            MessageBoxServiceButton.Ok => Views.MessageBoxWindow.MessageBoxButton.Ok,
            MessageBoxServiceButton.YesNo => Views.MessageBoxWindow.MessageBoxButton.YesNo,
            MessageBoxServiceButton.YesNoCancel => Views.MessageBoxWindow.MessageBoxButton.YesNoCancel,
            _ => Views.MessageBoxWindow.MessageBoxButton.Ok,
        };

        Views.MessageBoxWindow.MessageBoxIcon i = icon switch
        {
            MessageBoxServiceIcon.AccessDenied => Views.MessageBoxWindow.MessageBoxIcon.AccessDenied,
            MessageBoxServiceIcon.CheckMark => Views.MessageBoxWindow.MessageBoxIcon.CheckMark,
            MessageBoxServiceIcon.Error => Views.MessageBoxWindow.MessageBoxIcon.Error,
            MessageBoxServiceIcon.Information => Views.MessageBoxWindow.MessageBoxIcon.Information,
            MessageBoxServiceIcon.NetworkConnectionError => Views.MessageBoxWindow.MessageBoxIcon.NetworkConnectionError,
            MessageBoxServiceIcon.Question => Views.MessageBoxWindow.MessageBoxIcon.Question,
            MessageBoxServiceIcon.Thinking => Views.MessageBoxWindow.MessageBoxIcon.Thinking,
            MessageBoxServiceIcon.Warning => Views.MessageBoxWindow.MessageBoxIcon.Warning,
            MessageBoxServiceIcon.Robot => Views.MessageBoxWindow.MessageBoxIcon.Robot,
            _ => Views.MessageBoxWindow.MessageBoxIcon.Information,
        };

        return new Views.MessageBoxWindow(
                 message,
                 title,
                 b,
                 i,
                 owner).ShowDialog();
    }
}