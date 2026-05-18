using Microsoft.Extensions.DependencyInjection;
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
using System.Windows.Threading;
using Telerik.Windows.Documents.Spreadsheet.Model;
using VulpesX.Shared;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Accounting.Invoicing;

namespace VulpesX.Modules.Default.Accounting.Invoicing
{
    /// <summary>
    /// Interaction logic for AskAccountingWindow.xaml
    /// </summary>
    public partial class AskAccountingWindow : FluentDefaultWindow
    {
        private AskAccountingWindowViewModel _dataContext;
        public AskAccountingWindow(AskAccountingWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            CultureInfo noSeparator = new CultureInfo("it-IT");
            noSeparator.NumberFormat.NumberGroupSeparator = "";
            cmbAccountingYear.ItemsSource = _dataContext.GetESERCIZIOs();
            cmbAccountingYear.SelectedItem = cmbAccountingYear.Items[0] as ESERCIZIO;
            
            if (!_dataContext.ShowCostCenter)
            {
                tbCostCenter.Visibility = Visibility.Collapsed;
                acCostCenter.Visibility = Visibility.Collapsed;
            }

            _dataContext.SelectedRegDate = _dataContext.Invoice.fattdata;
            _dataContext.SelectedProtDate = _dataContext.Invoice.fattdata;
            _dataContext.AccountingYear = cmbAccountingYear.Items[0] as ESERCIZIO;

            _dataContext.LoadDetails();
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            cmdCancel.IsEnabled = false;
            cmdSave.IsEnabled = false;

            if (_dataContext.AccountingYear != null && _dataContext.SelectedRegDate.HasValue && _dataContext.SelectedProtDate.HasValue)
            {
                int eseini = _dataContext.AccountingYear.eseini!.Value;
                if (eseini == 1 || (eseini != 1 && _dataContext.Invoice.fattdata >= new DateTime(_dataContext.AccountingYear.eseann, eseini, 1)))
                {
                    if (_dataContext.SelectedCausal != null)
                    {
                        if (_dataContext.SelectedCostCenter != null || !_dataContext.ShowCostCenter || (_dataContext.ShowCostCenter && _dataContext.SelectedCostCenter == null && ConfirmHandler.Confirm("Proseguire senza specificare un centro di costo ?")))
                        {
                            if (_dataContext.IsSupplier)
                            {
                                var lastProtocolInfo = _dataContext.GetLastProtocolUsed();

                                if (lastProtocolInfo != null)
                                {
                                    if (lastProtocolInfo.Item2 <= _dataContext.SelectedProtDate.Value)
                                    {
                                        // received
                                        var cps = _dataContext.GetSUPPLIER_GROUPSs();
                                        if (cps != null && cps.Count > 0)
                                        {
                                            // come on
                                            _dataContext.Invoice = _dataContext.GetFull(_dataContext.Invoice.id)!;
                                            // add CP rows
                                            var lastRow = (_dataContext.Invoice.Rows?.Max(max => max.fattriga) + 1) ?? 1;
                                            foreach (var cp in _dataContext.Invoice.CPs ?? new System.Collections.ObjectModel.ObservableCollection<ACC_EINVOICE_CP>())
                                            {
                                                if (_dataContext.Invoice.Rows == null)
                                                    _dataContext.Invoice.Rows = new System.Collections.ObjectModel.ObservableCollection<ACC_EINVOICE_ROWS>();

                                                _dataContext.Invoice.Rows.Add(new ACC_EINVOICE_ROWS()
                                                {
                                                    fattsoc = cp.fattsoc,
                                                    fattnum = cp.fattnum,
                                                    fattdata = cp.fattdata,
                                                    fattpiva = cp.fattpiva,
                                                    fattriga = lastRow++,
                                                    fatttotriga = cp.impcontricassa
                                                });
                                            }
                                            // add arrotondamenti rows
                                            // main arrotondamento
                                            if (_dataContext.Invoice.fattarrotondamento.HasValue && _dataContext.Invoice.fattarrotondamento.Value > 0)
                                            {
                                                if (_dataContext.Invoice.Rows == null)
                                                    _dataContext.Invoice.Rows = new System.Collections.ObjectModel.ObservableCollection<ACC_EINVOICE_ROWS>();

                                                _dataContext.Invoice.Rows.Add(new ACC_EINVOICE_ROWS()
                                                {
                                                    fattsoc = _dataContext.Invoice.Rows.First().fattsoc,
                                                    fattnum = _dataContext.Invoice.Rows.First().fattnum,
                                                    fattdata = _dataContext.Invoice.Rows.First().fattdata,
                                                    fattpiva = _dataContext.Invoice.Rows.First().fattpiva,
                                                    fattriga = lastRow++,
                                                    fatttotriga = _dataContext.Invoice.fattarrotondamento.Value
                                                });
                                            }
                                            // datiriepilogo arrotondamento
                                            if (_dataContext.Invoice.VATs != null && _dataContext.Invoice.VATs.Count > 0 &&
                                                _dataContext.Invoice.VATs.Any(any => any.fattarrotondamento.HasValue))
                                            {
                                                foreach (var vat in _dataContext.Invoice.VATs.Where(w => w.fattarrotondamento.HasValue && w.fattarrotondamento.Value > 0))
                                                {
                                                    if (_dataContext.Invoice.Rows == null)
                                                        _dataContext.Invoice.Rows = new System.Collections.ObjectModel.ObservableCollection<ACC_EINVOICE_ROWS>();

                                                    _dataContext.Invoice.Rows.Add(new ACC_EINVOICE_ROWS()
                                                    {
                                                        fattsoc = vat.fattsoc,
                                                        fattnum = vat.fattnum,
                                                        fattdata = vat.fattdata,
                                                        fattpiva = vat.fattpiva,
                                                        fattriga = lastRow++,
                                                        fatttotriga = vat.fattarrotondamento
                                                    });
                                                }
                                            }

                                            var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<PairAccountsWindowViewModel>();
                                            windowViewModel.SupplierDescription = _dataContext.Invoice.Supplier?.FullDescriptionNotSearchable;
                                            windowViewModel.InvoiceText = $"{_dataContext.Invoice.fattnum} del {_dataContext.Invoice.fattdata.ToString("dd/MM/yyyy")}";
                                            windowViewModel.Rows = _dataContext.Invoice.Rows ?? new System.Collections.ObjectModel.ObservableCollection<ACC_EINVOICE_ROWS>();
                                            windowViewModel.Counterparts = cps;
                                            windowViewModel.PNIVARows = _dataContext.Invoice.VATs ?? new System.Collections.ObjectModel.ObservableCollection<ACC_EINVOICE_VAT>();
                                            windowViewModel.HeadCostCenterID = _dataContext.SelectedCostCenter?.cecodc;

                                            var wPair = new PairAccountsWindow(windowViewModel);
                                            wPair.Owner = Window.GetWindow(this);
                                            if (wPair.ShowDialog() == true)
                                            {
                                                if (!_dataContext.AccountingReceivedInvoice(windowViewModel.PNRows))
                                                {
                                                    ErrorHandler.Validation($"Fattura {_dataContext.Invoice.fattnum} del {_dataContext.Invoice.fattdata.ToString("dd/MM/yyyy")} NON contabilizzata per un errore imprevisto");
                                                    Mouse.OverrideCursor = Cursors.Wait;
                                                }
                                                else
                                                {
                                                    InfoHandler.Show($"Contabilizzazione avvenuta correttamente");
                                                    this.DialogResult = true;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            ErrorHandler.Validation("Non ci sono contropartite per il fornitore selezionato");
                                        }
                                    }
                                    else
                                    {
                                        Mouse.OverrideCursor = null;
                                        ErrorHandler.Validation($"La data protocollo deve essere maggiore o uguale all'ultima data protocollo presente : {lastProtocolInfo.Item2.ToString("dd/MM/yyyy")}");
                                    }
                                }
                                else
                                {
                                    Mouse.OverrideCursor = null;
                                    ErrorHandler.Validation($"La data protocollo deve essere maggiore o uguale all'ultima data protocollo presente : {lastProtocolInfo?.Item2.ToString("dd/MM/yyyy")}");
                                }
                            }
                            else
                            {
                                // external sent
                                // come on
                                _dataContext.Invoice = _dataContext.GetFull(_dataContext.Invoice.id)!;
                                if (!_dataContext.AccountingSentExternalInvoice())
                                {
                                    ErrorHandler.Validation($"Fattura {_dataContext.Invoice.fattnum} del {_dataContext.Invoice.fattdata.ToString("dd/MM/yyyy")} NON contabilizzata per un errore imprevisto");
                                    Mouse.OverrideCursor = Cursors.Wait;
                                }
                                else
                                {
                                    InfoHandler.Show($"Contabilizzazione avvenuta correttamente");
                                    this.DialogResult = true;
                                }
                            }
                        }
                        Mouse.OverrideCursor = null;
                    }
                    else
                    {
                        Mouse.OverrideCursor = null;
                        ErrorHandler.Validation("La causale contabile è obbligatoria");
                    }
                }
                else
                {
                    Mouse.OverrideCursor = null;
                    ErrorHandler.Validation($"Fattura {_dataContext.Invoice.fattnum} del {_dataContext.Invoice.fattdata.ToString("dd/MM/yyyy")} NON contabilizzabile perchè in caso di sovrapposizione di esercizio l'anno IVA deve essere precedente all'anno contabile se la data fattura e' successiva o uguale al 1 Gennaio {(cmbAccountingYear.SelectedItem as ESERCIZIO)?.eseann}");
                }
            }
            else
            {
                Mouse.OverrideCursor = null;
                ErrorHandler.Validation("Le date di registrazione e protocollo devono avere un valore valido");
            }
            cmdCancel.IsEnabled = true;
            cmdSave.IsEnabled = true;
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
