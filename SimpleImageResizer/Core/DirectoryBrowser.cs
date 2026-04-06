using Microsoft.Win32;

namespace SimpleImageResizer.Core;

public static class DirectoryBrowser
{
    /// <summary>
    /// Allows user to choose a folder by using the windows browser.
    /// </summary>
    /// <remarks>
    /// Legacy Shell32 COM interop was removed during .NET 10 modernization.
    /// Uses built-in OpenFolderDialog for WPF on modern .NET.
    /// </remarks>
    /// <param name="caption">Caption required for the Folder Browser Dialog.</param>
    /// <returns>Directory path.</returns>
    public static string GetDirectory(string caption)
    {
        OpenFolderDialog folderDialog = new()
        {
            Title = caption,
            Multiselect = false,
        };

        return folderDialog.ShowDialog() == true
            ? folderDialog.FolderName
            : string.Empty;
    }
}
