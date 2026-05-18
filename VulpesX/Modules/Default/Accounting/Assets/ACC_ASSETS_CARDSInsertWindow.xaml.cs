using Microsoft.Extensions.DependencyInjection;
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
using VulpesX.DAL;
using VulpesX.Shared;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Accounting;
using VulpesX.ViewModels.Modules.Default.Accounting.Assets;

namespace VulpesX.Modules.Default.Accounting.Assets
{
    /// <summary>
    /// Interaction logic for ACC_ASSETS_CARDS_InsertWindow.xaml
    /// </summary>
    public partial class ACC_ASSETS_CARDSInsertWindow : FluentDefaultWindow
    {
        private ACC_ASSETS_CARDSInsertWindowViewModel _dataContext;
        public ACC_ASSETS_CARDSInsertWindow(ACC_ASSETS_CARDSInsertWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            rdpDate.Culture = new System.Globalization.CultureInfo("it-IT");
            rdpDate.Culture.DateTimeFormat.ShortDatePattern = "yyyy";
            rdpDate.SelectedValue = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

            _dataContext.Groups = _dataContext.GetPDCGRUPPIs();
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            cmdCancel.IsEnabled = false;
            cmdSave.IsEnabled = false;
            if (rdpDate.SelectedValue.HasValue)
            {
                if (!string.IsNullOrWhiteSpace(_dataContext.SelectedGroupID) && !string.IsNullOrWhiteSpace(_dataContext.SelectedAccountID) && !string.IsNullOrWhiteSpace(_dataContext.SelectedSubaccountID))
                {
                    _dataContext.SelectedYear = rdpDate.SelectedValue.Value.Year;
                    this.DialogResult = true;
                }
                else
                {
                    Mouse.OverrideCursor = null;
                    ErrorHandler.Validation("Gruppo, conto e sottoconto sono obbligatori");
                    cmdCancel.IsEnabled = true;
                    cmdSave.IsEnabled = true;
                }
            }
            else
            {
                Mouse.OverrideCursor = null;
                ErrorHandler.Validation("L'anno' deve avere un valore valido");
                cmdCancel.IsEnabled = true;
                cmdSave.IsEnabled = true;
            }
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
        private void acGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (acGroup.SelectedItem != null)
            {
                _dataContext.SelectedGroupID = (acGroup.SelectedItem as PDCGRUPPI)?.P1GRUP;

                if (!string.IsNullOrEmpty(_dataContext.SelectedGroupID))
                    _dataContext.Accounts = _dataContext.GetPDCCONTIs(_dataContext.SelectedGroupID);
            }
            else
            {
                _dataContext.SelectedGroupID = null;
                _dataContext.Accounts = null;
            }

            _dataContext.SelectedAccountID = null;
            acAccount.SelectedItem = null;
            _dataContext.SelectedSubaccountID = null;
            acSubaccount.SelectedItem = null;
            _dataContext.Subaccounts = null;
        }

        private void acAccount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (acAccount.SelectedItem != null)
            {
                _dataContext.SelectedAccountID = (acAccount.SelectedItem as PDCCONTI)?.P2CONT;

                if (!string.IsNullOrEmpty(_dataContext.SelectedGroupID) && !string.IsNullOrEmpty(_dataContext.SelectedAccountID))
                    _dataContext.Subaccounts = _dataContext.GetPDCSOTTOs(_dataContext.SelectedGroupID, _dataContext.SelectedAccountID);
            }
            else
            {
                _dataContext.SelectedAccountID = null;
                _dataContext.Subaccounts = null;
            }
            acSubaccount.SelectedItem = null;
            _dataContext.SelectedSubaccountID = null;
        }

        private void acSubaccount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (acSubaccount.SelectedItem != null)
            {
                _dataContext.SelectedSubaccountID = (acSubaccount.SelectedItem as PDCSOTTO)?.P3SOTC;
            }
            else
            {
                _dataContext.SelectedSubaccountID = null;
            }
        }
        #endregion
    }
}
