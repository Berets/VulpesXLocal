using System;
using System.Collections.Generic;
using System.Globalization;
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
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Treasury;

namespace VulpesX.Modules.Default.Treasury
{
    /// <summary>
    /// Interaction logic for AskAccountingCommitmentWindow.xaml
    /// </summary>
    public partial class AskAccountingCommitmentWindow : FluentDefaultWindow
    {
        private AskAccountingCommitmentWindowViewModel _dataContext;
        public AskAccountingCommitmentWindow(AskAccountingCommitmentWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            _dataContext.LoadDetails();

            CultureInfo noSeparator = new CultureInfo("it-IT");
            noSeparator.NumberFormat.NumberGroupSeparator = "";
            cmbAccountingYear.ItemsSource = _dataContext.GetESERCIZIO();
            cmbAccountingYear.SelectedItem = cmbAccountingYear.Items[0] as ESERCIZIO;
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            cmdCancel.IsEnabled = false;
            cmdSave.IsEnabled = false;


            if (_dataContext.AccountingYear.HasValue && _dataContext.AccountingYear > 0)
            {
                if (rdpDate.SelectedValue.HasValue)
                {
                    if (_dataContext.SelectedCausal != null)
                    {
                        if (_dataContext.SelectedGroup != null && _dataContext.SelectedAccount != null &&
                            _dataContext.SelectedSubaccount != null)
                        {
                            // come on
                            if (_dataContext.Accounting())
                            {
                                Mouse.OverrideCursor = null;
                                this.DialogResult = true;
                            }
                            else
                            {
                                Mouse.OverrideCursor = null;
                                ErrorHandler.Validation("Errore durante la contabilizzazione");
                                cmdCancel.IsEnabled = true;
                                cmdSave.IsEnabled = true;
                            }
                        }
                        else
                        {
                            Mouse.OverrideCursor = null;
                            ErrorHandler.Validation("Gruppo, conto e sottoconto contabili sono obbligatori");
                            cmdCancel.IsEnabled = true;
                            cmdSave.IsEnabled = true;
                        }
                    }
                    else
                    {
                        Mouse.OverrideCursor = null;
                        ErrorHandler.Validation("La causale contabile e' obbligatoria");
                        cmdCancel.IsEnabled = true;
                        cmdSave.IsEnabled = true;
                    }
                }
                else
                {
                    Mouse.OverrideCursor = null;
                    ErrorHandler.Validation("La data di contabilizzazione deve avere un valore valido");
                    cmdCancel.IsEnabled = true;
                    cmdSave.IsEnabled = true;
                }
            }
            else
            {
                Mouse.OverrideCursor = null;
                ErrorHandler.Validation("L'anno di contabilizzazione deve avere un valore valido");
                cmdCancel.IsEnabled = true;
                cmdSave.IsEnabled = true;
            }

        }

        private void cmbAccountingYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _dataContext.AccountingYear = (cmbAccountingYear.SelectedItem as ESERCIZIO)?.eseann;
        }

        #region Autocompletes
        private void acGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedGroup != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedGroup.P1GRUP))
            {
                _dataContext.Accounts = _dataContext.GetPDCCONTIs(_dataContext.SelectedGroup.P1GRUP);
                _dataContext.SelectedAccount = null;
                _dataContext.SelectedSubaccount = null;
            }
        }

        private void acAccount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedGroup != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedGroup.P1GRUP) && _dataContext.SelectedAccount != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedAccount.P2CONT))
            {
                _dataContext.Subaccounts = _dataContext.GetPDCSOTTOs(_dataContext.SelectedGroup.P1GRUP, _dataContext.SelectedAccount.P2CONT);
                _dataContext.SelectedSubaccount = null;
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
