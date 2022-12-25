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
/// Interaction logic for MainWindow.xaml
/// </summary>
/// <remarks>
/// We can put UI related things there that does not break the MVVM pattern.
/// </remarks>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        Core.MyApplication.DpiScale = VisualTreeHelper.GetDpi(this).PixelsPerDip;

        InitializeComponent();
    }

    private void MenuItemExit_Click(object sender, RoutedEventArgs e)
    {
        Environment.Exit(0);
    }
}