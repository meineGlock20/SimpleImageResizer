using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleImageResizer.Services;

public interface IMessageService
{
    bool? ShowMessage(string message,
        string title,
        Views.MessageBoxWindow.MessageBoxButton messageBoxButton,
        Views.MessageBoxWindow.MessageBoxIcon messageBoxIcon,
        Window? owner);
}

public class MessageBoxService : IMessageService
{
    /// <summary>
    /// Shows a message to the user and returns a value based on button selection.
    /// </summary>
    /// <param name="message">Message to display to the user.</param>
    /// <param name="title">Title of the message box.</param>
    /// <param name="messageBoxButton">Message box button(s) to display.</param>
    /// <param name="messageBoxIcon">Message box icon to display.</param>
    /// <param name="owner">The owner of the message box. Usually a window but can be null.</param>
    /// <returns>False if No or Cancelled was selected, otherwise True is returned.</returns>
    public bool? ShowMessage(string message,
        string title,
        Views.MessageBoxWindow.MessageBoxButton messageBoxButton,
        Views.MessageBoxWindow.MessageBoxIcon messageBoxIcon,
        Window? owner)
    {
       return new Views.MessageBoxWindow(
                message,
                title,
                messageBoxButton,
                messageBoxIcon,
                owner).ShowDialog();
    }
}