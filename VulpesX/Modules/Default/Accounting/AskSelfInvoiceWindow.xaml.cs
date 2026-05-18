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
using VulpesX.Models.Models.Accounting;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Accounting;

namespace VulpesX.Modules.Default.Accounting
{
    /// <summary>
    /// Interaction logic for AskSelfInvoiceWindow.xaml
    /// </summary>
    public partial class AskSelfInvoiceWindow : FluentDefaultWindow
    {
        private AskSelfInvoiceWindowViewModel _dataContext;
        public AskSelfInvoiceWindow(AskSelfInvoiceWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            CultureInfo noSeparator = new CultureInfo("it-IT");
            noSeparator.NumberFormat.NumberGroupSeparator = "";

            cmbAccountingYear.ItemsSource = _dataContext.GetESERCIZIO();
            cmbAccountingYear.SelectedItem = cmbAccountingYear.Items[0] as ESERCIZIO;

            _dataContext.AccountingYear = cmbAccountingYear.Items[0] as ESERCIZIO;

            this.Loaded += async (s,e)   =>
            {
                await _dataContext.LoadDetails();
            };
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            cmdCancel.IsEnabled = false;
            cmdSave.IsEnabled = false;

            if (_dataContext.AccountingYear != null)
            {
                if (_dataContext.SelectedDate.HasValue)
                {
                    if (_dataContext.Causal != null)
                    {
                        if (_dataContext.Customer != null)
                        {
                            if (_dataContext.Product != null)
                            {
                                Mouse.OverrideCursor = Cursors.Wait;
                                var result = _dataContext.Generate();
                                Mouse.OverrideCursor = null;
                                if (!string.IsNullOrWhiteSpace(result))
                                {
                                    InfoHandler.Show($"Generazione autofattura {result} terminata correttamente");
                                    this.DialogResult = true;
                                }
                                else
                                {
                                    ErrorHandler.Validation("Errore generico");
                                    cmdCancel.IsEnabled = true;
                                    cmdSave.IsEnabled = true;
                                }
                            }
                            else
                            {
                                ErrorHandler.Validation("L'articolo deve avere un valore valido");
                                cmdCancel.IsEnabled = true;
                                cmdSave.IsEnabled = true;
                            }
                        }
                        else
                        {
                            ErrorHandler.Validation("Il cliente deve avere un valore valido");
                            cmdCancel.IsEnabled = true;
                            cmdSave.IsEnabled = true;
                        }
                    }
                    else
                    {
                        ErrorHandler.Validation("La causale della fattura deve avere un valore valido");
                        cmdCancel.IsEnabled = true;
                        cmdSave.IsEnabled = true;
                    }
                }
                else
                {
                    ErrorHandler.Validation("La data della fattura provvisoria deve avere un valore valido");
                    cmdCancel.IsEnabled = true;
                    cmdSave.IsEnabled = true;
                }
            }
            else
            {
                ErrorHandler.Validation("L'anno contabile è obbligatorio");
                cmdCancel.IsEnabled = true;
                cmdSave.IsEnabled = true;
            }

        }

        private void cmbAccountingYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.DataContext != null)
            {
                _dataContext.AccountingYear = cmbAccountingYear.SelectedItem as ESERCIZIO;
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
        #endregion
    }
}
