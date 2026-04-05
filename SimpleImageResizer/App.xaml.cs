global using System;
global using System.Collections.Generic;
global using System.Data;
global using System.Linq;
global using System.Threading.Tasks;
global using System.Windows;
global using System.IO;
using System.Windows.Media;
using System.Globalization;
using SimpleImageResizer.Properties;

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
        // Setup Localization and Globalization.
        CultureInfo.CurrentCulture = new CultureInfo(string.IsNullOrWhiteSpace(AppSettings.Default.CurrentCulture) ? "en-US" : AppSettings.Default.CurrentCulture);
        CultureInfo.CurrentUICulture = new CultureInfo(string.IsNullOrWhiteSpace(AppSettings.Default.CurrentUICulture) ? "en-US" : AppSettings.Default.CurrentUICulture);

        if (string.IsNullOrWhiteSpace(AppSettings.Default.DestinationDirectory))
        {
            AppSettings.Default.DestinationDirectory =
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "SIR_ResizedImages");
            AppSettings.Default.Save();
        }
    }
}
