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
using VulpesX.Models;
using VulpesX.Shared;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.CRM;

namespace VulpesX.Modules.Default.CRM
{
    /// <summary>
    /// Interaction logic for AskDDTSendDateWindow.xaml
    /// </summary>
    public partial class AskDDTSendDateWindow : FluentDefaultWindow
    {
        private AskDDTSendDateWindowViewModel _dataContext;
        public AskDDTSendDateWindow(AskDDTSendDateWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            _dataContext.LoadDetails();

            _dataContext.SelectedDelivery = _dataContext.Deliveries?.Where(w => w.concod == _dataContext.Data.BTCONS).FirstOrDefault();
            _dataContext.SelectedShipment = _dataContext.Shipments?.Where(w => w.specod == _dataContext.Data.BTSPED).FirstOrDefault();
            _dataContext.SelectedPacking = _dataContext.Packings?.Where(w => w.imbcod == _dataContext.Data.BTIMBA).FirstOrDefault();
            _dataContext.SelectedFirstCarrier = _dataContext.Carriers?.Where(w => w.vetcod == _dataContext.Data.BTCORR).FirstOrDefault();
            _dataContext.SelectedSecondCarrier = _dataContext.Carriers?.Where(w => w.vetcod == _dataContext.Data.BTCORR2).FirstOrDefault();

            var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

            _dataContext.Data.BTDATA = now;
            if (!_dataContext.Data.BTDASP.HasValue)
                _dataContext.Data.BTDASP = now;
        }

        #region Buttons
        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            if (ConfirmHandler.Confirm($"Confermate la stampa definitiva del DDT n.{_dataContext.Data.PrintFullID} ?"))
            {
                if (_dataContext.Data.BTDASP.HasValue || (!_dataContext.Data.BTDASP.HasValue && ConfirmHandler.Confirm("Non è stata specificata una data ed ora di spedizione, procedere comunque ?\n\nData ed ora di spedizione andranno apposte a mano sul documento stampato o modificate successivamente se abilitata la modifica dei DDT definitivi")))
                {
                    // check is valid
                    var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

                    var causal = _dataContext.GetCAUSBOLL(_dataContext.Data.BTCAUS ?? string.Empty);

                    if (causal != null && !string.IsNullOrWhiteSpace(causal.bolnum))
                    {
                        // refresh info
                        _dataContext.Data.BTIMBA = _dataContext.SelectedPacking?.imbcod;
                        _dataContext.Data.BTSPED = _dataContext.SelectedShipment?.specod;
                        _dataContext.Data.BTCONS = _dataContext.SelectedDelivery?.concod;
                        _dataContext.Data.BTCORR = _dataContext.SelectedFirstCarrier?.vetcod;
                        _dataContext.Data.BTCORR2 = _dataContext.SelectedSecondCarrier?.vetcod;

                        var validateMessage = _dataContext.Validate();
                        if (string.IsNullOrWhiteSpace(validateMessage))
                        {
                            if (_dataContext.Data.BTPESO.HasValue && _dataContext.Data.BTPESO.Value > 0 &&
                                _dataContext.Data.BTPES2.HasValue && _dataContext.Data.BTPES2.Value > 0 &&
                                _dataContext.Data.BTPESO.Value >= _dataContext.Data.BTPES2.Value)
                            {
                                // print defi
                                var numerator = _dataContext.GetNumerator(now.Year, new GenericIDDescription() { ID = causal.bolnum, Description = causal.boldes });

                                if (numerator != null && numerator > 0)
                                {
                                    if (causal.bolmagBool)
                                    {
                                        // check again synced enagegs
                                        var quantities = _dataContext.GetRowsTotalQuantity();

                                        if (quantities != null && quantities.Item2 == quantities.Item3)
                                        {
                                            if (quantities.Item1 == quantities.Item2 && quantities.Item1 > 0 && quantities.Item2 > 0)
                                            {
                                                Mouse.OverrideCursor = Cursors.Wait;
                                                // set definitive info
                                                _dataContext.Data.BTNUBD = numerator!.Value;
                                                _dataContext.Data.updatedUserID = _dataContext.UserID;
                                                _dataContext.Data.BTSTATO = causal.bolfatBool ? "R" : "F";
                                                if (_dataContext.UpdateDefinitiveDDTAndUnloadEngages())
                                                {
                                                    var item = _dataContext.GetPrintFull(_dataContext.Data.BTANNO, _dataContext.Data.BTBOLL);

                                                    if (item != null)
                                                    {
                                                        var reportData = _dataContext.PrintDDT(item);
                                                        if (reportData != null)
                                                        {
                                                            ReportingHandler.PrintPDF(UserContext.Instance!.Domain!, Constants.MODULE_SHIPPING, Constants.REPORT_TYPE_DDT, _dataContext.CompanyID, reportData, $"DDT n.{item.PrintFullID}", item.PrintFilename, true);
                                                        }
                                                        else
                                                        {
                                                            Mouse.OverrideCursor = null;
                                                            ErrorHandler.Validation($"Impossibile trovare il DDT {item.BTANNO}/{item.BTBOLL}");
                                                        }
                                                    }

                                                    Mouse.OverrideCursor = null;
                                                    this.DialogResult = true;
                                                }
                                                Mouse.OverrideCursor = null;
                                            }
                                            else
                                            {
                                                Mouse.OverrideCursor = null;
                                                ErrorHandler.Validation($"Il DDT selezionato non può essere stampato perchè incompleto\nVerificare di aver impegnato tutto il materiale necessario");
                                            }
                                        }
                                        else
                                        {
                                            Mouse.OverrideCursor = null;
                                            ErrorHandler.Validation($"Il DDT selezionato non può essere stampato perchè gli impegni reali [{quantities?.Item3.ToString("N6")}] non corrispondono agli impegni registrati per il documento [{quantities?.Item2.ToString("N6")}]\n\nProbabilmente sono stati modificati gli impegni nella gestione del magazzino");
                                        }
                                    }
                                    else
                                    {
                                        Mouse.OverrideCursor = Cursors.Wait;
                                        // set definitive info
                                        _dataContext.Data.BTDATA = now;
                                        _dataContext.Data.BTNUBD = numerator!.Value;
                                        _dataContext.Data.updatedUserID = _dataContext.UserID;
                                        _dataContext.Data.BTSTATO = causal.bolfatBool ? "R" : "F";
                                        if (_dataContext.UpdateDefinitiveDDTAndUnloadEngages())
                                        {
                                            var item = _dataContext.GetPrintFull(_dataContext.Data.BTANNO, _dataContext.Data.BTBOLL);
                                            if (item != null)
                                            {
                                                var reportData = _dataContext.PrintDDT(item);
                                                if (reportData != null)
                                                {
                                                    ReportingHandler.PrintPDF(UserContext.Instance!.Domain!, Constants.MODULE_SHIPPING, Constants.REPORT_TYPE_DDT, _dataContext.CompanyID, reportData, $"DDT n.{item.PrintFullID}", item.PrintFilename, true);
                                                }
                                                else
                                                {
                                                    Mouse.OverrideCursor = null;
                                                    ErrorHandler.Validation($"Impossibile trovare il DDT {item.BTANNO}/{item.BTBOLL}");
                                                }
                                            }
                                            Mouse.OverrideCursor = null;
                                            this.DialogResult = true;
                                        }
                                        Mouse.OverrideCursor = null;
                                    }
                                }
                                else
                                {
                                    Mouse.OverrideCursor = null;
                                    ErrorHandler.Validation($"Impossibile recuperare un numeratore definitivo");
                                }
                            }
                            else
                            {
                                Mouse.OverrideCursor = null;
                                ErrorHandler.Validation("I pesi sono obbligatori, devono essere maggiori di 0 e il peso lordo non può essere inferiore a quello netto");
                            }
                        }
                        else
                        {
                            Mouse.OverrideCursor = null;
                            ErrorHandler.Validation(validateMessage);
                        }
                    }
                    else
                    {
                        Mouse.OverrideCursor = null;
                        ErrorHandler.Validation($"La causale impostata\n{causal?.FullDescriptionSearchable}\nnon ha impostato nessun numeratore necessario per assegnare il numero definitivo");
                    }
                }
            }
            Mouse.OverrideCursor = null;
            e.Handled = true;
        }
        #endregion

        #region Autocompletes
        private void acDelivery_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedDelivery != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedDelivery.concod))
            {
                _dataContext.Data.BTCONS = _dataContext.SelectedDelivery.concod;
            }
        }

        private void acShipment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedShipment != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedShipment.specod))
            {
                _dataContext.Data.BTSPED = _dataContext.SelectedShipment.specod;
            }
        }

        private void acPacking_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedPacking != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedPacking.imbcod))
            {
                _dataContext.Data.BTIMBA = _dataContext.SelectedPacking.imbcod;
                if (string.IsNullOrWhiteSpace(_dataContext.Data.BTDEBE))
                    _dataContext.Data.BTDEBE = _dataContext.SelectedPacking.imbdes.Trim();
            }
        }

        private void acFirstCarrier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedFirstCarrier != null && _dataContext.SelectedFirstCarrier.vetcod > 0)
            {
                _dataContext.Data.BTCORR = _dataContext.SelectedFirstCarrier.vetcod;
            }
        }

        private void acSecondCarrier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedSecondCarrier != null && _dataContext.SelectedSecondCarrier.vetcod > 0)
            {
                _dataContext.Data.BTCORR2 = _dataContext.SelectedSecondCarrier.vetcod;
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
