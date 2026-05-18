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
using VulpesX.ViewModels.Modules.Default.Tables.Accounting;

namespace VulpesX.Modules.Default.Tables.Accounting
{
    /// <summary>
    /// Interaction logic for PAGCLIWindow.xaml
    /// </summary>
    public partial class PAGCLIWindow : FluentDefaultWindow
    {
        private PAGCLIWindowViewModel _dataContext;
        public PAGCLIWindow(PAGCLIWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            if (!_dataContext.IsInsert)
            {
                _dataContext.SelectedPaymentType = _dataContext.PaymentTypes?.Where(w => w.icscod == _dataContext.Data.pcltip).FirstOrDefault();
            }
            _dataContext.Lingue = _dataContext.GetLINGUA();
            _dataContext.SelectedLingua = _dataContext.Lingue?.FirstOrDefault();
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
            { Mouse.OverrideCursor = null; ErrorHandler.Validation(validated); }
        }
        #endregion

        #region Autocompletes
        private void acPaymentTpe_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedPaymentType != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedPaymentType.icscod))
            {
                _dataContext.Data.pcltip = _dataContext.SelectedPaymentType.icscod;
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

        private void cmbLingua_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var model = (this.DataContext as PAGCLIViewModel);

            if (e.RemovedItems.Count > 0)
            {
                _dataContext.InsertOrUpdateLanguage();
            }

            if (e.AddedItems.Count > 0)
            {
                var added = e.AddedItems[0] as LINGUA;

                if (added != null)
                    _dataContext.SelectedTraduzione = _dataContext.GetLINGUA(added.lincod) ?? new PAGCLI_LINGUA { pclcod = _dataContext.Data.pclcod, lincod = added.lincod };
            }
        }
    }
}
