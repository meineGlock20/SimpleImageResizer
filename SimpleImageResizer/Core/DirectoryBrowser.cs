using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleImageResizer.Core;

public static class DirectoryBrowser
{
    /// <summary>
    /// Allows user to choose a folder by using the windows browser.
    /// </summary>
    /// <remarks>
    /// Requires a reference to C:\Windows\System32\Shell32.dll.
    /// You need to set the property Embed Interop Types to NO for the Interop.Shell32 com object.
    /// Interestingly, in 2022 there is still no folder browser in WPF.
    /// You alternatively could use a reference to WinForms or a nuget like ookii dilogs. 
    /// </remarks>
    /// <param name="caption">Caption required for the Folder Browser Dialog.</param>
    /// <returns>Directory path.</returns>
    public static string GetDirectory(string caption)
    {
        Shell32.ShellClass shl = new();
        Shell32.Folder2 fld = (Shell32.Folder2)shl.BrowseForFolder(0, caption, 0, System.Reflection.Missing.Value);

        if (fld != null)
        {
            return fld.Self.Path;
        }
        else
        {
            return string.Empty;
        }
    }
}
