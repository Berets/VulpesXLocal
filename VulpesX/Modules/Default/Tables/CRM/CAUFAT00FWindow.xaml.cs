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
    /// Interaction logic for CAUFAT00FWindow.xaml
    /// </summary>
    public partial class CAUFAT00FWindow : FluentDefaultWindow
    {
        private CAUFAT00FWindowViewModel _dataContext;
        public CAUFAT00FWindow(CAUFAT00FWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            if (!_dataContext.IsInsert)
            {
                _dataContext.SelectedCausal = _dataContext.Causals?.Where(w => w.caucod == _dataContext.Data.fatcon).FirstOrDefault();
                _dataContext.SelectedSelfInvoiceCausal = _dataContext.Causals?.Where(w => w.caucod == _dataContext.Data.fatcaut).FirstOrDefault();
                _dataContext.SelectedNumerator = _dataContext.Numerators?.Where(w => w.PERCOD == _dataContext.Data.fatnmr).FirstOrDefault();
                _dataContext.SelectedFEDocType = _dataContext.FEDocTypes?.Where(w => w.FETDCod == _dataContext.Data.fattido).FirstOrDefault();
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
        private void acCausal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedCausal != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedCausal.caucod))
            {
                _dataContext.Data.fatcon = _dataContext.SelectedCausal.caucod;
            }
            else
            { _dataContext.Data.fatcon = null; }
        }
        private void acSelfInvoiceCausal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedSelfInvoiceCausal != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedSelfInvoiceCausal.caucod))
            {
                _dataContext.Data.fatcaut = _dataContext.SelectedSelfInvoiceCausal.caucod;
            }
            else
            { _dataContext.Data.fatcaut = null; }
        }
        private void acNumerator_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedNumerator != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedNumerator.PERCOD))
            {
                _dataContext.Data.fatnmr = _dataContext.SelectedNumerator.PERCOD;
            }
            else
            {
                _dataContext.Data.fatnmr = null;
            }
        }
        private void acFEDocType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedFEDocType != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedFEDocType.FETDCod))
            {
                _dataContext.Data.fattido = _dataContext.SelectedFEDocType.FETDCod;
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
