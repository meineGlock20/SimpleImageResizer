using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        ((ViewModels.MainWindowViewModel)DataContext).Window = this;
    }

    private void MenuItemExit_Click(object sender, RoutedEventArgs e)
    {
        Environment.Exit(0);
    }

    private void ButtonSelectImages_Click(object sender, RoutedEventArgs e)
    {
        SelectImages();
    }

    private void MenuToolsSelect_Click(object sender, RoutedEventArgs e)
    {
        SelectImages();

    }

    private void SelectImages()
    {
        OpenFileDialog openFileDialog = new()
        {
            Multiselect = true,
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
            Filter = Localize.MainWindow.SelectImagesSupportedTypes,
        };

        if (openFileDialog.ShowDialog() == true)
        {
            Cursor = Cursors.Wait;

            List<Models.Image>? images = new();
            foreach (string image in openFileDialog.FileNames)
            {
                Size size = Core.Imaging.Size.GetImageSize(image);
                images.Add(new Models.Image()
                {
                    FullPathToImage = image,
                    ImageHeight = size.Height,
                    ImageWidth = size.Width,
                    ImageBytes = new FileInfo(image).Length,
                    Thumbnail = Core.Imaging.Thumbnail.GenerateByWidth(128, image),
                });
            }

            foreach (var m in images)
            {
                ((ViewModels.MainWindowViewModel)DataContext).AddImagesToCollection(m);
            }

            Cursor = null;
        }
    }

    private void ListBoxImages_DragEnter(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop, autoConvert: false))
        {
            e.Effects = DragDropEffects.Link;
        }
    }

    private void ListBoxImages_Drop(object sender, DragEventArgs e)
    {
        foreach (string image in (string[])e.Data.GetData(DataFormats.FileDrop))
        {
            Cursor = Cursors.Wait;
            Size size = Core.Imaging.Size.GetImageSize(image);
            var i = new Models.Image()
            {
                FullPathToImage = image,
                ImageHeight = size.Height,
                ImageWidth = size.Width,
                ImageBytes = new FileInfo(image).Length,
                Thumbnail = Core.Imaging.Thumbnail.GenerateByWidth(128, image),
            };
            ((ViewModels.MainWindowViewModel)DataContext).AddImagesToCollection(i);
            Cursor = null;
        }
    }

    private void TextBoxNumericOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        Regex regex = NumericRegex();
        e.Handled = regex.IsMatch(e.Text);
    }

    [GeneratedRegex("[^0-9]+")]
    private static partial Regex NumericRegex();
}