using System;
using System.Windows;
using SimpleImageResizer.Views;

namespace SimpleImageResizer.Services;

public sealed class ViewFactory(IServiceProvider serviceProvider) : IViewFactory
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public BatchWindow CreateBatchWindow(Window? owner)
    {
        var window = (BatchWindow)_serviceProvider.GetService(typeof(BatchWindow))!;
        window.Owner = owner;
        return window;
    }
}
