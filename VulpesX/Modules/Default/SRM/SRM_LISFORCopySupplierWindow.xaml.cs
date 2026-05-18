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
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.SRM;

namespace VulpesX.Modules.Default.SRM
{
    /// <summary>
    /// Interaction logic for SRM_LISFORCopySupplierWindow.xaml
    /// </summary>
    public partial class SRM_LISFORCopySupplierWindow : FluentDefaultWindow
    {
        private SRM_LISFORCopySupplierWindowViewModel _dataContext;
        public SRM_LISFORCopySupplierWindow(SRM_LISFORCopySupplierWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            _dataContext.LoadDetails();
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            if (_dataContext.SelectedSourceSupplier != null && _dataContext.SelectedTargetSupplier != null)
            {
                if (_dataContext.SelectedSourceSupplier.abecod != _dataContext.SelectedTargetSupplier.abecod)
                {
                    this.DialogResult = true;
                }
                else
                {
                    ErrorHandler.Validation($"E' necessario selezionare due fornitori differenti");
                    e.Handled = true;
                }
            }
            else
            {
                ErrorHandler.Validation($"E' necessario selezionare due fornitori");
                e.Handled = true;
            }
        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            _dataContext.SelectedSourceSupplier = null;
            _dataContext.SelectedTargetSupplier = null;
            this.DialogResult = false;
        }

        #region Autocompletes
        private void ac_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = ((RadAutoCompleteBox)sender).ChildrenOfType<TextBox>().First();
            Dispatcher.BeginInvoke(new Action(() => { textBox.SelectAll(); }));
        }

        private void ac_LostFocus(object sender, RoutedEventArgs e)
        {
            var ac = sender as RadAutoCompleteBox;
            if (ac != null)
            {
                if (ac.SelectedItem == null)
                {
                    ac.SearchText = null;
                }
            }
        }
        #endregion
    }
}
