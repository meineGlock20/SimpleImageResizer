global using System;
global using System.Collections.Generic;
global using System.Data;
global using System.Linq;
global using System.Threading.Tasks;
global using System.Windows;
global using System.IO;
using System.Windows.Media;
using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SimpleImageResizer.Properties;
using SimpleImageResizer.Services;
using SimpleImageResizer.ViewModels;
using SimpleImageResizer.Views;

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
    private IHost? host;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        ApplyCultureAndDefaults();

        var builder = Host.CreateApplicationBuilder();
        ConfigureServices(builder.Services);

        host = builder.Build();
        host.Start();

        MainWindow = host.Services.GetRequiredService<MainWindow>();
        MainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        if (host is not null)
        {
            host.StopAsync().GetAwaiter().GetResult();
            host.Dispose();
        }

        base.OnExit(e);
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IMessageService, MessageBoxService>();
        services.AddSingleton<IViewFactory, ViewFactory>();

        services.AddTransient<MainWindowViewModel>();
        services.AddTransient<BatchWindowViewModel>();

        services.AddTransient<MainWindow>();
        services.AddTransient<BatchWindow>();
    }

    private static void ApplyCultureAndDefaults()
    {
        CultureInfo.CurrentCulture = new CultureInfo(
            string.IsNullOrWhiteSpace(AppSettings.Default.CurrentCulture)
                ? "en-US"
                : AppSettings.Default.CurrentCulture);

        CultureInfo.CurrentUICulture = new CultureInfo(
            string.IsNullOrWhiteSpace(AppSettings.Default.CurrentUICulture)
                ? "en-US"
                : AppSettings.Default.CurrentUICulture);

        if (!string.IsNullOrWhiteSpace(AppSettings.Default.DestinationDirectory))
        {
            return;
        }

        AppSettings.Default.DestinationDirectory =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "SIR_ResizedImages");
        AppSettings.Default.Save();
    }
}
