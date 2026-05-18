using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata;
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
using VulpesX.Models.Default.Partials;
using VulpesX.Shared;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Production;

namespace VulpesX.Modules.Default.Production
{
    /// <summary>
    /// Interaction logic for ProductionOrderConfirmWindow.xaml
    /// </summary>
    public partial class ProductionOrderConfirmWindow : FluentDefaultWindow
    {
        private ProductionOrderConfirmWindowViewModel _dataContext;
        public ProductionOrderConfirmWindow(ProductionOrderConfirmWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            var availabilities = _dataContext.CheckAvailabilityByProduct(_dataContext.Order.ArticoloID ?? string.Empty, null);

            // check if already engaged items and load them
            var engages = _dataContext.GetList();
            if (engages != null && engages.Count > 0)
            {
                foreach (var ava in availabilities ?? new ObservableCollection<StockInfo>())
                {
                    ava.QuantityEngagedForOrder = engages.Where(w => w.lot == (ava.Lot != Constants.NO_LOT_ID ? ava.Lot : null)).Sum(sum => sum.quantity);
                }
            }

            if (_dataContext.Order.OrdineClienteAnno.HasValue && _dataContext.Order.OrdineClienteAnno.Value > 0)
            {
                var customerOrder = _dataContext.GetPrintFull();
                // remove other rows
                if (customerOrder != null)
                {
                    customerOrder.Rows = new ObservableCollection<ORDID00F>(customerOrder.Rows?.Where(w => w.ODRIGA == _dataContext.Order.OrdineClienteRiga).ToList() ?? new List<ORDID00F>());

                    _dataContext.AvailableStocks = new ObservableCollection<StockInfo>(availabilities?.Where(w => w.QuantityAvailable > 0 || (w.QuantityAvailable == 0 && w.QuantityEngagedForOrder > 0)).ToList() ?? new());
                    _dataContext.CustomerOrder = customerOrder;
                    _dataContext.OriginalQuantity = customerOrder.Rows?[0].ODQTAV;
                    _dataContext.OlderRevisionID = _dataContext.Order.RevisioneID;
                    _dataContext.QuantityOrdered = _dataContext.Order.Quantita ?? 0;
                    _dataContext.QuantityAvailable = availabilities?.Sum(sum => sum.QuantityAvailable) ?? 0;
                    _dataContext.QuantityToEngage = 0;
                    _dataContext.Revisions = _dataContext.GetRevisioniSimpleList();
                }
            }
            else
            {
                _dataContext.AvailableStocks = new ObservableCollection<StockInfo>(availabilities?.Where(w => w.QuantityAvailable > 0 || (w.QuantityAvailable == 0 && w.QuantityEngagedForOrder > 0)).ToList() ?? new());
                _dataContext.CustomerOrder = null;
                _dataContext.OriginalQuantity = _dataContext.Order.Quantita;
                _dataContext.OlderRevisionID = _dataContext.Order.RevisioneID;
                _dataContext.QuantityOrdered = _dataContext.Order.Quantita ?? 0;
                _dataContext.QuantityAvailable = availabilities?.Sum(sum => sum.QuantityAvailable) ?? 0;
                _dataContext.QuantityToEngage = 0;
                _dataContext.Revisions = _dataContext.GetRevisioniSimpleList();
            }

            RefreshQuantities();
            rnToProduce.Focus();
        }

        #region Buttons
        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            cmdCancel.IsEnabled = false;
            cmdSave.IsEnabled = false;

            if (CheckQuantityMultiple())
            {
                if (_dataContext.OriginalQuantity <= _dataContext.QuantityToProduce + _dataContext.QuantityToEngage + (_dataContext.AvailableStocks?.Sum(sum => sum.QuantityEngagedForOrder) ?? 0))
                {
                    if (_dataContext.QuantityToProduce > 0)
                    {
                        // recreate composizione for new quantity if different
                        var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
                        bool cleared = false;
                        bool recreated = false;
                        if (_dataContext.QuantityToProduce != _dataContext.QuantityOrdered || _dataContext.Order.RevisioneID != _dataContext.OlderRevisionID)
                        {
                            // if has customer order
                            if (_dataContext.CustomerOrder != null)
                            {
                                if (_dataContext.QuantityToEngage == _dataContext.QuantityOrdered)
                                {
                                    // close production for customer order 
                                    _dataContext.Order.Stato = "E";
                                    _dataContext.Update();

                                    cleared = true;
                                    // create new production order for difference

                                    if (_dataContext.CustomerOrder.Rows?[0] != null)
                                    {
                                        _dataContext.CustomerOrder.Rows[0].OriginalQuantity = _dataContext.CustomerOrder.Rows[0].ODQTAV;
                                        _dataContext.CustomerOrder.Rows[0].ODQTAV = _dataContext.QuantityToProduce;
                                        _dataContext.CustomerOrder.Rows[0].ODSTA = null;

                                        _dataContext.Recreate(false);
                                    }
                                }
                                else
                                {
                                    // set new quantity
                                    if (_dataContext.CustomerOrder.Rows?[0] != null)
                                    {
                                        _dataContext.CustomerOrder.Rows[0].OriginalQuantity = _dataContext.CustomerOrder.Rows[0].ODQTAV;
                                        _dataContext.CustomerOrder.Rows[0].ODQTAV = _dataContext.QuantityToProduce;
                                        _dataContext.CustomerOrder.Rows[0].ODSTA = null;
                                        // clear old order
                                        _dataContext.Recreate(true);
                                    }
                                }
                            }
                            else
                            {
                                // only update production order
                                _dataContext.Order.LogUpdatedUserID = _dataContext.UserID;
                                _dataContext.Order.Quantita = _dataContext.QuantityToProduce;

                                _dataContext.Update();

                                cleared = true;
                                recreated = true;
                            }
                            // if set, engaged final product 
                            if (_dataContext.QuantityToEngage > 0)
                            {
                                foreach (var lot in (_dataContext.AvailableStocks ?? new()).Where(w => w.QuantityToEngage > 0))
                                {
                                    _dataContext.Insertstore_stocks_engage(lot);
                                }
                            }
                        }
                        else
                        {
                            cleared = true;
                            recreated = true;
                        }
                        // recreated succesfully
                        if (cleared && recreated)
                        {
                            var engages = _dataContext.GetSimpleListByProductionOrder();
                            // check half worked availabilities
                            _dataContext.ComponentsList = _dataContext.GetMaterialsListByOrder();
                            _dataContext.HalfworkedList = _dataContext.GetHalfmadeListByOrder();
                            bool halfworkedConfirmed = false;
                            if (_dataContext.HalfworkedList != null && _dataContext.HalfworkedList.Count > 0)
                            {
                                // log
                                _dataContext.Insertpro_ordine_history(Constants.PRODUCTION_START_HALFWORK, _dataContext.Order.Stato ?? string.Empty);

                                // halfworked needed
                                foreach (var hw in _dataContext.HalfworkedList)
                                {
                                    hw.Availabilities = _dataContext.CheckAvailabilityByProduct(hw.ComponenteArticoloID ?? string.Empty, null);
                                    // check if already engaged items and load them
                                    if (engages != null && engages.Where(w => w.product_id == hw.ComponenteArticoloID).Count() > 0)
                                    {
                                        foreach (var ava in hw.Availabilities ?? new ObservableCollection<StockInfo>())
                                        {
                                            ava.QuantityEngagedForOrder = engages.Where(w => w.product_id == hw.ComponenteArticoloID && w.lot == (ava.Lot != Constants.NO_LOT_ID ? ava.Lot : null)).Sum(sum => sum.quantity);
                                        }
                                    }
                                    hw.QuantitaGiaImpegnata = engages?.Where(w => w.product_id == hw.ComponenteArticoloID).Sum(sum => sum.quantity) ?? 0;
                                    hw.ComponenteArticoloDescrizione = _dataContext.GetTab_Articolo(hw.ComponenteArticoloID ?? string.Empty)?.FullDescriptionSearchable;
                                    hw.QuantitaOriginale = hw.Quantita;
                                }
                                // show halfworked resume
                                var windowHalfViewModel = VulpesServiceProvider.Provider.GetRequiredService<ProductionOrderConfirmHalfWindowViewModel>();
                                windowHalfViewModel.Order = _dataContext.Order;
                                windowHalfViewModel.HalfworkedList = _dataContext.HalfworkedList;
                                windowHalfViewModel.MaterialList = _dataContext.ComponentsList ?? new ObservableCollection<pro_ordine_composizione>();

                                var wConfirmHalfworked = new ProductionOrderConfirmHalfWindow(windowHalfViewModel);
                                wConfirmHalfworked.Owner = Window.GetWindow(this);
                                halfworkedConfirmed = wConfirmHalfworked.ShowDialog() ?? false;
                            }
                            else
                            {
                                halfworkedConfirmed = true;
                            }

                            if (halfworkedConfirmed)
                            {
                                // check raws availabilities
                                foreach (var raw in (_dataContext.ComponentsList ?? new()).Where(w => w.ComponenteRevisioneID == null))
                                {
                                    raw.Availabilities = _dataContext.CheckAvailabilityByProduct(raw.ComponenteArticoloID ?? string.Empty, null);
                                    raw.ComponenteArticoloDescrizione = _dataContext.GetTab_Articolo(raw.ComponenteArticoloID ?? string.Empty)?.FullDescriptionSearchable;
                                    // set default existing quantities
                                    if (raw.Availabilities != null && raw.Availabilities.Count == 1)
                                    {
                                        raw.Availabilities[0].QuantityToEngage = raw.Availabilities[0].QuantityAvailable >= raw.Quantita ? raw.Quantita.Value : raw.Availabilities[0].QuantityAvailable;
                                    }
                                    raw.QuantitaOriginale = raw.Quantita;
                                    // check if already engaged items and load them
                                    if (engages != null && engages.Where(w => w.product_id == raw.ComponenteArticoloID).Count() > 0)
                                    {
                                        foreach (var ava in raw.Availabilities ?? new ObservableCollection<StockInfo>())
                                        {
                                            ava.QuantityEngagedForOrder = engages.Where(w => w.product_id == raw.ComponenteArticoloID && w.lot == (ava.Lot != Constants.NO_LOT_ID ? ava.Lot : null)).Sum(sum => sum.quantity);
                                        }
                                    }
                                    raw.QuantitaGiaImpegnata = engages?.Where(w => w.product_id == raw.ComponenteArticoloID).Sum(sum => sum.quantity) ?? 0;
                                }
                                // show raws 
                                // log
                                _dataContext.Insertpro_ordine_history(Constants.PRODUCTION_START_RAWS, _dataContext.Order.Stato ?? string.Empty);

                                var windowRawViewModel = VulpesServiceProvider.Provider.GetRequiredService<ProductionOrderConfirmRawWindowViewModel>();
                                windowRawViewModel.Order = _dataContext.Order;
                                windowRawViewModel.Components = _dataContext.ComponentsList;

                                var wRaws = new ProductionOrderConfirmRawWindow(windowRawViewModel);
                                wRaws.Owner = Window.GetWindow(this);
                                if (wRaws.ShowDialog() == true)
                                {
                                    // engage existing raws
                                    foreach (var raw in (_dataContext.ComponentsList ?? new ObservableCollection<pro_ordine_composizione>()).Where(w => w.ComponenteArticoloID != null && w.ComponenteRevisioneID == null && w.QuantitaImpegnata > 0))
                                    {
                                        foreach (var lot in (raw.Availabilities ?? new()).Where(w => w.QuantityToEngage > 0))
                                        {
                                            _dataContext.Insertstore_stocks_engage(raw, lot);
                                        }
                                    }

                                    // update pro_ordine_composizione with new quantities
                                    foreach (var raw in _dataContext.ComponentsList ?? new ObservableCollection<pro_ordine_composizione>())
                                    {
                                        _dataContext.Updatepro_ordine_composizione(raw);
                                    }
                                    // update pro_ordine_composizione with new quantities halfmade
                                    foreach (var hm in _dataContext.HalfworkedList ?? new ObservableCollection<pro_ordine_composizione>())
                                    {
                                        _dataContext.Updatepro_ordine_composizione(hm);
                                        _dataContext.Updatepro_ordine_composizioneHalf(hm.OrdineID, hm.ComponenteArticoloID ?? string.Empty, hm.ComponenteRevisioneID ?? string.Empty, hm.Quantita ?? 0);
                                    }

                                    // generate RDA
                                    bool isWaiting = false;
                                    foreach (var raw in (_dataContext.ComponentsList ?? new ObservableCollection<pro_ordine_composizione>()).Where(w => w.ComponenteArticoloID != null && w.ComponenteRevisioneID == null && w.Quantita > w.QuantitaImpegnata + w.QuantitaGiaImpegnata))
                                    {
                                        // compute quantity by product info
                                        _dataContext.InsertSRM_RDA(raw);
                                        // at least 1 RDA so prod order is waiting
                                        isWaiting = true;
                                    }
                                    // update order status 
                                    if (isWaiting)
                                    {
                                        // log
                                        _dataContext.Insertpro_ordine_history(Constants.PRODUCTION_LAUNCH_WAIT, _dataContext.Order.Stato ?? string.Empty);

                                        _dataContext.Order.Stato = "W";
                                    }
                                    else
                                    {
                                        // log
                                        _dataContext.Insertpro_ordine_history(Constants.PRODUCTION_LAUNCH_OK, _dataContext.Order.Stato ?? string.Empty);

                                        _dataContext.Order.Stato = "O";
                                    }

                                    _dataContext.Order.LogUpdatedUserID = _dataContext.UserID;
                                    _dataContext.Update();

                                    Mouse.OverrideCursor = null;
                                    this.DialogResult = true;
                                }
                                else
                                {
                                    Mouse.OverrideCursor = null;
                                    this.DialogResult = false;
                                }
                            }
                            else
                            {
                                Mouse.OverrideCursor = null;
                                this.DialogResult = false;
                            }
                        }
                        else
                        {
                            cmdCancel.IsEnabled = true;
                            cmdSave.IsEnabled = true;
                            Mouse.OverrideCursor = null;
                        }
                    }
                    else
                    {
                        if (ConfirmHandler.Confirm("In base alle scelte fatte per questo ordine non e' necessario procedere alla produzione.\n\nIl materiale verra' impegnato e l'ordine chiuso, e sara' quindi posssibile procedere all'emissione del DDT.\n\nContinuare ?"))
                        {
                            // engage product and close order
                            if (_dataContext.EngageAndClose())
                            {
                                // log
                                _dataContext.Insertpro_ordine_history(Constants.PRODUCTION_LAUNCH_NOPROD, _dataContext.Order.Stato  ?? string.Empty);

                                Mouse.OverrideCursor = null;
                                this.DialogResult = true;
                            }
                            else
                            {
                                cmdCancel.IsEnabled = true;
                                cmdSave.IsEnabled = true;
                                Mouse.OverrideCursor = null;
                            }
                        }
                        else
                        {
                            cmdCancel.IsEnabled = true;
                            cmdSave.IsEnabled = true;
                            Mouse.OverrideCursor = null;
                        }
                    }
                }
                else
                {
                    Mouse.OverrideCursor = null;
                    ErrorHandler.Validation("La quantità totale deve coprire almeno l'intera quantità ordinata");
                }
            }

            cmdCancel.IsEnabled = true;
            cmdSave.IsEnabled = true;
            Mouse.OverrideCursor = null;
        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
        #endregion

        #region Private methods
        private void RefreshQuantities()
        {

            _dataContext.QuantityToEngage = _dataContext.AvailableStocks?.Sum(sum => sum.QuantityToEngage) ?? 0;
            _dataContext.QuantityToProduce = (_dataContext.OriginalQuantity - _dataContext.QuantityToEngage - (_dataContext.AvailableStocks?.Sum(sum => sum.QuantityEngagedForOrder) ?? 0) ?? 0);
            if (_dataContext.QuantityToProduce < 0)
                _dataContext.QuantityToProduce = 0;
            if (decimal.IsNegative(_dataContext.QuantityToProduce))
                _dataContext.QuantityToProduce = decimal.Negate(_dataContext.QuantityToProduce);
        }

        private void rnuToEngage_ValueChanged(object sender, RadRangeBaseValueChangedEventArgs e)
        {
            RefreshQuantities();
        }

        private bool CheckQuantityMultiple()
        {
            var product = _dataContext.GetTab_Articolo(_dataContext.Order.ArticoloID ?? string.Empty);

            if (product == null)
            {
                ErrorHandler.Validation($"Articolo non trovato - {_dataContext.Order.ArticoloID ?? string.Empty}");
                return false;
            }

            if (_dataContext.QuantityToProduce % (product.QuantitaDefault ?? 1) != 0)
            {
                ErrorHandler.Validation($"La quantità digitata ({_dataContext.QuantityToProduce.ToString("N6")}) non è valida in quanto non è un multiplo della quantità per confezione presente ({(product.QuantitaDefault ?? 1).ToString("N6")})");
                return false;
            }
            return true;
        }
        #endregion

        #region Form events
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (this.DialogResult == false)
            {
                Mouse.OverrideCursor = Cursors.Wait;

                // log
                _dataContext.Insertpro_ordine_history(Constants.PRODUCTION_CANCELED_LAUNCH, _dataContext.Order.Stato ?? string.Empty);
                // restore original quantity and recreate
                if (_dataContext.CustomerOrder != null && (_dataContext.CustomerOrder.Rows?.Any() ?? false))
                {
                    // set new quantity
                    _dataContext.CustomerOrder.Rows[0].ODQTAV = _dataContext.OriginalQuantity ?? 0;
                    // clear old order
                    _dataContext.Delete(_dataContext.Order);

                    if (_dataContext.Order.OrdineClienteAnno.HasValue && _dataContext.Order.OrdineClienteID.HasValue && _dataContext.Order.OrdineClienteRiga.HasValue)
                    {
                        _dataContext.UpdateORDID00F(_dataContext.Order);
                    }
                    // refresh customer order
                    _dataContext.CustomerOrder = _dataContext.GetPrintFull();

                    if (_dataContext.CustomerOrder != null)
                    {
                        // remove other rows
                        _dataContext.CustomerOrder.Rows = new ObservableCollection<ORDID00F>((_dataContext.CustomerOrder.Rows ?? new()).Where(w => w.ODRIGA == _dataContext.Order.OrdineClienteRiga).ToList());

                        // recreate with original quantity and revision
                        _dataContext.GenerateProductionOrder();
                    }
                }
                else
                {
                    // restore only prod_ordine
                    _dataContext.Order.LogUpdatedUserID = _dataContext.UserID;
                    _dataContext.Order.Quantita = _dataContext.OriginalQuantity;
                    _dataContext.Update();
                }
                Mouse.OverrideCursor = null;
                if (ConfirmHandler.Confirm("Lancio di produzione annullato, scegliere SI per eliminare eventuali impegni di materiale su questo ordine oppure NO per mantenerli"))
                {
                    // clear order's engages
                    Mouse.OverrideCursor = Cursors.Wait;
                    // log
                    _dataContext.Insertpro_ordine_history(Constants.PRODUCTION_CLEAR_ENGAGES, _dataContext.Order.Stato ?? string.Empty);

                    _dataContext.FreeByProductionOrder();
                }
            }
        }
        #endregion
    }
}
