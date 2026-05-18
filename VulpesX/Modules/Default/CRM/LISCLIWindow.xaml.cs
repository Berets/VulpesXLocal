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
using VulpesX.ViewModels.Modules.Default.CRM;

namespace VulpesX.Modules.Default.CRM
{
    /// <summary>
    /// Interaction logic for LISCLIWindow.xaml
    /// </summary>
    public partial class LISCLIWindow : FluentDefaultWindow
    {
        private LISCLIWindowViewModel _dataContext;
        public LISCLIWindow(LISCLIWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            _dataContext.LoadDetails();

            _dataContext.SelectedCustomer = _dataContext.Customers?.Where(w => w.abecod == _dataContext.Data.customerID).FirstOrDefault();

            if (_dataContext.Recipients != null && _dataContext.Recipients.Count > 0)
            {
                if (_dataContext.Data.recipientID.HasValue)
                    _dataContext.SelectedRecipient = _dataContext.Recipients.Where(w => w.ID == _dataContext.Data.recipientID?.ToString()).FirstOrDefault();
                else
                    _dataContext.SelectedRecipient = _dataContext.Recipients.Where(w => string.IsNullOrWhiteSpace(w.ID)).FirstOrDefault();
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
        private void acCustomer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedCustomer != null && _dataContext.SelectedCustomer.abecod > 0)
            {
                _dataContext.Data.customerID = _dataContext.SelectedCustomer.abecod;
                _dataContext.Recipients = _dataContext.GetDESTINATARIs(_dataContext.Data.customerID);

                if (!_dataContext.Data.recipientID.HasValue)
                    _dataContext.SelectedRecipient = _dataContext.Recipients?.Where(w => string.IsNullOrWhiteSpace(w.ID)).FirstOrDefault();
            }
        }

        private void acRecipient_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _dataContext.Data.recipientID = _dataContext.SelectedRecipient != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedRecipient.ID) ? int.Parse(_dataContext.SelectedRecipient.ID) : null;
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
