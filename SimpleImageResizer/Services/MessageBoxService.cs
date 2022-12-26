using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleImageResizer.Services;

public interface IMessageService
{
    void ShowMessage(string message,
        string title,
        Views.MessageBoxWindow.MessageBoxButton messageBoxButton,
        Views.MessageBoxWindow.MessageBoxIcon messageBoxIcon,
        Window? owner);
}

public class MessageBoxService : IMessageService
{
    public void ShowMessage(string message,
        string title,
        Views.MessageBoxWindow.MessageBoxButton messageBoxButton,
        Views.MessageBoxWindow.MessageBoxIcon messageBoxIcon,
        Window? owner)
    {
        new Views.MessageBoxWindow(
                message,
                title,
                messageBoxButton,
                messageBoxIcon,
                owner).ShowDialog();
    }
}
