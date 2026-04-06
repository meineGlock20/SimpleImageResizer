using System.Windows;

namespace SimpleImageResizer.Views
{
    /// <summary>
    /// Interaction logic for BatchWindow.xaml
    /// </summary>
    public partial class BatchWindow : Window
    {
        public BatchWindow(ViewModels.BatchWindowViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;

            // There is a CloseAction property on the VM that needs to be wired up so the window can be closed from the VM.
            viewModel.CloseAction ??= Close;
        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
