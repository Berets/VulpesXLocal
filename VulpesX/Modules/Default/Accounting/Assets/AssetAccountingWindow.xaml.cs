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
using VulpesX.Shared;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Accounting;
using VulpesX.ViewModels.Modules.Default.Accounting.Assets;

namespace VulpesX.Modules.Default.Accounting.Assets
{
    /// <summary>
    /// Interaction logic for AssetAccountingWindow.xaml
    /// </summary>
    public partial class AssetAccountingWindow : FluentDefaultWindow
    {
        private AssetAccountingWindowViewModel _dataContext;
        public AssetAccountingWindow()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<AssetAccountingWindowViewModel>();

            InitializeComponent();

            this.DataContext = _dataContext;
            cmbAccountingYear.ItemsSource = _dataContext.GetESERCIZIOs();
            cmbAccountingYear.SelectedItem = cmbAccountingYear.Items[0] as ESERCIZIO;

            _dataContext.Causals = _dataContext.GetCAUCONTs();
            _dataContext.AccountingYear = cmbAccountingYear.Items[0] as ESERCIZIO;
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            if (ConfirmHandler.Confirm("Confermate i parametri per eseguire la contabilizzazione ?"))
            {
                Mouse.OverrideCursor = Cursors.Wait;
                cmdCancel.IsEnabled = false;
                cmdSave.IsEnabled = false;

                if (_dataContext.AccountingYear != null)
                {
                    if (_dataContext.SelectedCausal != null)
                    {
                        if (rdpDateTo.SelectedValue.HasValue)
                        {
                            #region Retrieve parms and checks
                            // previous esercizio month
                            var prevMonth = (_dataContext.AccountingYear.eseini.HasValue && _dataContext.AccountingYear.eseini.Value > 1 ? _dataContext.AccountingYear.eseini.Value - 1 : 12);
                            #endregion

                            if ((_dataContext.AccountingYear.eseini == 1 && rdpDateTo.SelectedValue.Value.Year == _dataContext.AccountingYear.eseann) ||
                                (_dataContext.AccountingYear.eseini != 1 && rdpDateTo.SelectedValue.Value >= new DateTime(_dataContext.AccountingYear.eseann, (_dataContext.AccountingYear.eseini ?? 1), 1) && rdpDateTo.SelectedValue.Value <= new DateTime(_dataContext.AccountingYear.eseann + 1, prevMonth, DateTime.DaysInMonth(_dataContext.AccountingYear.eseann, prevMonth))))
                            {
                                if (_dataContext.Accounting(rdpDateTo.SelectedValue.Value))
                                {
                                    this.DialogResult = true;
                                }
                                else
                                {
                                    Mouse.OverrideCursor = null;
                                    cmdCancel.IsEnabled = true;
                                    cmdSave.IsEnabled = true;
                                }
                            }
                            else
                            {
                                Mouse.OverrideCursor = null;
                                ErrorHandler.Validation("La data di contabilizzazione deve essere inclusa nell'esercizio selezionato");
                                cmdCancel.IsEnabled = true;
                                cmdSave.IsEnabled = true;
                            }
                        }
                        else
                        {
                            Mouse.OverrideCursor = null;
                            ErrorHandler.Validation("La data fino alla quale elaborare è obbligatoria");
                            cmdCancel.IsEnabled = true;
                            cmdSave.IsEnabled = true;
                        }
                    }
                    else
                    {
                        Mouse.OverrideCursor = null;
                        ErrorHandler.Validation("La causale deve avere un valore valido");
                        cmdCancel.IsEnabled = true;
                        cmdSave.IsEnabled = true;
                    }
                }
                else
                {
                    Mouse.OverrideCursor = null;
                    ErrorHandler.Validation("L'esercizio deve avere un valore valido");
                    cmdCancel.IsEnabled = true;
                    cmdSave.IsEnabled = true;
                }
            }
        }

        private void cmbAccountingYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _dataContext.AccountingYear = cmbAccountingYear.SelectedItem as ESERCIZIO;
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
