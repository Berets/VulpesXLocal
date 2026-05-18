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
using VulpesX.ViewModels.Modules.Default.Tables.CRM;

namespace VulpesX.Modules.Default.Tables.CRM
{
    /// <summary>
    /// Interaction logic for CAUSBOLLWindow.xaml
    /// </summary>
    public partial class CAUSBOLLWindow : FluentDefaultWindow
    {
        private CAUSBOLLWindowViewModel _dataContext;
        public CAUSBOLLWindow(CAUSBOLLWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            if (!_dataContext.IsInsert)
            {
                _dataContext.SelectedInvoiceCausal = _dataContext.InvoiceCausals?.Where(w => w.fatcod == _dataContext.Data.bolfac).FirstOrDefault();
                _dataContext.SelectedStoreCausal = _dataContext.StoreCausals?.Where(w => w.company_id == _dataContext.CompanyID && w.id == _dataContext.Data.BOLCAU).FirstOrDefault();
                _dataContext.SelectedNumerator = _dataContext.Numerators?.Where(w => w.PERCOD == _dataContext.Data.bolnum).FirstOrDefault();
            }
        }

        #region Buttons
        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            var validated = _dataContext.Validate();

            if (string.IsNullOrWhiteSpace(validated))
            {
                if (_dataContext.Save())
                {
                    Mouse.OverrideCursor = null;
                    this.DialogResult = true;
                }
            }
            else
            {
                Mouse.OverrideCursor = null;
                ErrorHandler.Validation(validated);
            }
        }
        #endregion

        #region Autocompletes
        private void acInvoiceCausal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedInvoiceCausal != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedInvoiceCausal.fatcod))
            {
                _dataContext.Data.bolfac = _dataContext.SelectedInvoiceCausal.fatcod;
            }
            else
            {
                _dataContext.Data.bolfac = null;
            }
        }
        private void acStoreCausal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedStoreCausal != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedStoreCausal.id))
            {
                _dataContext.Data.BOLCAU = _dataContext.SelectedStoreCausal.id;
            }
            else
            {
                _dataContext.Data.BOLCAU = null;
            }
        }
        private void acNumerator_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedNumerator != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedNumerator.PERCOD))
            {
                _dataContext.Data.bolnum = _dataContext.SelectedNumerator.PERCOD;
            }
            else
            {
                _dataContext.Data.bolnum = null;
            }
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

        private void ac_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = ((RadAutoCompleteBox)sender).ChildrenOfType<TextBox>().First();
            Dispatcher.BeginInvoke(new Action(() => { textBox.SelectAll(); }));
        }

        #endregion

    }
}
