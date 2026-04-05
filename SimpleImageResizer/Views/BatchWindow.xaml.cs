using System;
using System.Windows;

namespace SimpleImageResizer.Views
{
    public partial class BatchWindow : Window
    {
        public BatchWindow()
        {
            InitializeComponent();

            // There is a CloseAction property on the VM that needs to be wired up so the window can be closed from the VM.
            var vm = (ViewModels.BatchWindowViewModel)DataContext;
            vm.CloseAction ??= Close;
        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
