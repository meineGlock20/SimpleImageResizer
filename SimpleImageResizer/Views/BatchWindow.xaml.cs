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

namespace SimpleImageResizer.Views
{
    /// <summary>
    /// Interaction logic for BatchWindow.xaml
    /// </summary>
    public partial class BatchWindow : Window
    {
        public BatchWindow()
        {
            InitializeComponent();

            // There is an CloseAction property on the VM that needs to be wired up so the window can be closed from the VM.
            if (((ViewModels.BatchWindowViewModel)DataContext).CloseAction == null)
            {
                ((ViewModels.BatchWindowViewModel)DataContext).CloseAction = new Action(() => Close());
            }
        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}
