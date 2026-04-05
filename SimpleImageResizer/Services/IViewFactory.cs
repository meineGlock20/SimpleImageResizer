using System.Windows;
using SimpleImageResizer.Views;

namespace SimpleImageResizer.Services;

public interface IViewFactory
{
    BatchWindow CreateBatchWindow(Window? owner);
}
