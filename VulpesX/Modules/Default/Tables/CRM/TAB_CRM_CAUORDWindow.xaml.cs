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
    /// Interaction logic for TAB_CRM_CAUORDWindow.xaml
    /// </summary>
    public partial class TAB_CRM_CAUORDWindow : FluentDefaultWindow
    {
        private TAB_CRM_CAUORDWindowViewModel _dataContext;
        public TAB_CRM_CAUORDWindow(TAB_CRM_CAUORDWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            _dataContext.Data.AccountCache = _dataContext.GetPDCCONTIs();
            _dataContext.Data.SubaccountCache = _dataContext.GetPDCSOTTOs();
            _dataContext.Data.GroupsList = _dataContext.GetPDCGRUPPIs();

            if (!_dataContext.IsInsert)
            {
                _dataContext.SelectedCausalDDT = _dataContext.Causals?.Where(w => w.bolcod == _dataContext.Data.caubol).FirstOrDefault();
                _dataContext.SelectedCausalInvoice = _dataContext.InvoiceCausals?.Where(w => w.fatcod == _dataContext.Data.caufat).FirstOrDefault();
                _dataContext.SelectedText = _dataContext.Texts?.Where(w => w.TTxcod == _dataContext.Data.cauacqtxc).FirstOrDefault();
            }
        }

        #region Buttons
        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

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
        private void acCausalDDT_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (_dataContext.SelectedCausalDDT != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedCausalDDT.bolcod))
            {
                _dataContext.Data.caubol = _dataContext.SelectedCausalDDT.bolcod;
            }
        }
        private void acCausalInvoice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (_dataContext.SelectedCausalInvoice != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedCausalInvoice.fatcod))
            {
                _dataContext.Data.caufat = _dataContext.SelectedCausalInvoice.fatcod;
            }
        }
        private void acText_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedText != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedText.TTxcod))
            {
                _dataContext.Data.cauacqtxt = TAB_GEN_TEXTS.ORDERS;
                _dataContext.Data.cauacqtxc = _dataContext.SelectedText.TTxcod;
            }
            else
            {
                _dataContext.Data.cauacqtxt = null;
                _dataContext.Data.cauacqtxc = null;
            }
        }
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
