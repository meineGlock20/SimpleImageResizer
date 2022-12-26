using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SimpleImageResizer.Views;

/// <summary>
/// Interaction logic for MessageBoxWindow.xaml
/// </summary>
public partial class MessageBoxWindow : Window
{
    // Bold, Warning (RED), Italics. These can not be nested. To bold a word in the message surround it with tags <b>Make me bold</b>
    private readonly List<string> commands = new() { "<b>", "<w>", "<i>" };

    public MessageBoxWindow(string text, string title, MessageBoxButton messageBoxButton, MessageBoxIcon messageBoxIcon, Window? owner)
    {
        Owner = owner;

        InitializeComponent();

        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentNullException(nameof(text), "Argument TEXT can not be null!");
        }

        if (owner is not null)
        {
            owner.Opacity = 0.8;
        }
        else
        {
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        TextBlockTitle.Text = string.IsNullOrWhiteSpace(title) ? "My Movies" : title;
        TextBlockMessage.Text = string.Empty;

        // Check if the passed text contains any valid commands from the readonly list.
        // If it does, then process it otherwise just add the text to the textblock.
        FormatMessageText(text);

        ResourceDictionary myResourceDictionary = new()
        {
            Source = new Uri("pack://application:,,,/Resources/icons.messagebox.xaml",
                   UriKind.RelativeOrAbsolute),
        };

        switch (messageBoxIcon)
        {
            case MessageBoxIcon.AccessDenied:
                ImageMessageBoxIcon.Content = myResourceDictionary["access-denied"];
                System.Media.SystemSounds.Exclamation.Play();
                break;
            case MessageBoxIcon.CheckMark:
                ImageMessageBoxIcon.Content = myResourceDictionary["check-mark"];
                System.Media.SystemSounds.Beep.Play();
                break;
            case MessageBoxIcon.Error:
                ImageMessageBoxIcon.Content = myResourceDictionary["error"];
                System.Media.SystemSounds.Exclamation.Play();
                break;
            case MessageBoxIcon.Information:
                ImageMessageBoxIcon.Content = myResourceDictionary["information"];
                System.Media.SystemSounds.Exclamation.Play();
                break;
            case MessageBoxIcon.NetworkConnectionError:
                ImageMessageBoxIcon.Content = myResourceDictionary["network-connection-error"];
                System.Media.SystemSounds.Exclamation.Play();
                break;
            case MessageBoxIcon.Question:
                ImageMessageBoxIcon.Content = myResourceDictionary["question"];
                System.Media.SystemSounds.Question.Play();
                break;
            case MessageBoxIcon.Thinking:
                ImageMessageBoxIcon.Content = myResourceDictionary["thinking-face"];
                System.Media.SystemSounds.Exclamation.Play();
                break;
            case MessageBoxIcon.Warning:
                ImageMessageBoxIcon.Content = myResourceDictionary["warning"];
                System.Media.SystemSounds.Exclamation.Play();
                break;
            case MessageBoxIcon.Robot:
                ImageMessageBoxIcon.Content = myResourceDictionary["robot"];
                System.Media.SystemSounds.Exclamation.Play();
                break;
            default:
                break;
        }

        switch (messageBoxButton)
        {
            case MessageBoxButton.Ok:
                ButtonCancel.Visibility = Visibility.Collapsed;
                ButtonNo.Visibility = Visibility.Collapsed;
                ButtonYes.Visibility = Visibility.Collapsed;
                ButtonOk.IsDefault = true;
                break;
            case MessageBoxButton.YesNo:
                ButtonCancel.Visibility = Visibility.Collapsed;
                ButtonNo.IsCancel = true;
                ButtonYes.IsDefault = true;
                ButtonOk.Visibility = Visibility.Collapsed;
                break;
            case MessageBoxButton.YesNoCancel:
                ButtonCancel.IsCancel = true;
                ButtonYes.IsDefault = true;
                ButtonOk.Visibility = Visibility.Collapsed;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Provides button options for message box.
    /// </summary>
    public enum MessageBoxButton
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

    /// <summary>
    /// Provides icon options for message box.
    /// </summary>
    public enum MessageBoxIcon
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
        /// Robot.
        /// </summary>
        Robot,
    }

    private void FormatMessageText(string text)
    {
        // Check if any of the text has formatting commands and add text inlines for them.
        if (commands.Any(x => text.Contains(x, StringComparison.OrdinalIgnoreCase)))
        {
            string temp = string.Empty;

            for (int i = 0; i < text.Length; i++)
            {
                string s = text.Substring(i, 1);

                if (s == "<")
                {
                    if (temp.Length > 0)
                    {
                        TextBlockMessage.Inlines.Add(new Run(temp));
                        temp = string.Empty;
                    }

                    i++;
                    string cmd = $"<{text.Substring(i, 1)}>";
                    i += 2;

                    string endCmd = cmd.Insert(1, "/");
                    int iEndCmd = text.IndexOf(endCmd, i, StringComparison.OrdinalIgnoreCase);

                    do
                    {
                        s = text.Substring(i, 1);
                        temp += s;
                        i++;
                    }
                    while (i < iEndCmd);

                    i += 3;

                    switch (cmd)
                    {
                        case "<b>":
                            TextBlockMessage.Inlines.Add(new Run(temp) { FontWeight = FontWeights.Bold });
                            break;
                        case "<w>":
                            TextBlockMessage.Inlines.Add(new Run(temp) { Foreground = Brushes.Red, FontWeight = FontWeights.Medium });
                            break;
                        case "<i>":
                            TextBlockMessage.Inlines.Add(new Run(temp) { FontStyle = FontStyles.Italic });
                            break;
                        default:
                            break;
                    }

                    temp = string.Empty;
                }
                else
                {
                    temp += s;
                }
            }

            // if there is anything left, add the last inline.
            if (temp.Length > 0)
            {
                TextBlockMessage.Inlines.Add(new Run(temp));
                temp = string.Empty;
            }
        }
        else
        {
            TextBlockMessage.Text = text;
        }
    }

    private void ButtonCancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }

    private void ButtonOk_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }

    private void ButtonNo_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }

    private void ButtonYes_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }

    private void Window_Closed(object sender, EventArgs e)
    {
        if (Owner is not null)
        {
            Owner.Opacity = 1;
        }
    }

    private void Border_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        DragMove();
    }
}