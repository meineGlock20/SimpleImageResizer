global using System;
global using System.Collections.Generic;
global using System.Configuration;
global using System.Data;
global using System.Linq;
global using System.Threading.Tasks;
global using System.Windows;
global using System.IO;
using System.Windows.Media;

namespace SimpleImageResizer;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
/// <remarks>
/// FREEWARE!
/// </remarks>
/*
 * HISTORY
 * 
 * 
 */
public partial class App : Application
{
    private void Application_Startup(object sender, StartupEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(SimpleImageResizer.Properties.Settings.Default.DestinationDirectory))
        {
            SimpleImageResizer.Properties.Settings.Default.DestinationDirectory =
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "SIR_ResizedImages");
            SimpleImageResizer.Properties.Settings.Default.Save();
        }
    }
}
