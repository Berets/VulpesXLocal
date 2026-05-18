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
using VulpesX.ViewModels.Modules.Default.SRM;

namespace VulpesX.Modules.Default.SRM
{
    /// <summary>
    /// Interaction logic for GoodsReceiptReceiveWindow.xaml
    /// </summary>
    public partial class GoodsReceiptReceiveWindow : FluentDefaultWindow
    {
        private GoodsReceiptReceiveWindowViewModel _dataContext;
        public GoodsReceiptReceiveWindow(GoodsReceiptReceiveWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            _dataContext.LoadDetails();

            Loaded += (sender, e) =>
            {
                this.Title = $"Dettagli entrata merce fornitori n.{_dataContext.Data.id}";
            };
        }

        #region Buttons
        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            CreateLot();

            var validated = _dataContext.Validate();
            decimal assignedQuantity = -1;

            // jobs
            if (_dataContext.OrderRowData.Jobs != null && _dataContext.OrderRowData.Jobs.Count > 0)
            {
                assignedQuantity = _dataContext.OrderRowData.Jobs.Sum(sum => sum.QuantityAssigned);
            }
            // customer orders
            if (_dataContext.OrderRowData.CustomerOrders != null && _dataContext.OrderRowData.CustomerOrders.Count > 0)
            {
                assignedQuantity = _dataContext.OrderRowData.CustomerOrders.Sum(sum => sum.QuantityAssigned);
            }
            if (string.IsNullOrWhiteSpace(validated))
            {
                if (_dataContext.OrderRowData.Product != null)
                {
                    if (_dataContext.OrderRowData.quantity.HasValue)
                    {
                        if (!_dataContext.OrderRowData.Product.QuantitaDefault.HasValue ||
                            (_dataContext.OrderRowData.Product.QuantitaDefault.HasValue && _dataContext.OrderRowData.unit_id == _dataContext.OrderRowData.Product.UnitaIDAlt && (_dataContext.Data.quantity % 1 == 0 ? (_dataContext.Data.quantity * (_dataContext.OrderRowData.Product.QuantitaDefault ?? 1)) % (_dataContext.OrderRowData.Product.QuantitaDefault ?? 1) == 0 : (_dataContext.Data.quantity % (_dataContext.OrderRowData.Product.QuantitaDefault ?? 1) == 0))) ||
                            (_dataContext.OrderRowData.Product.QuantitaDefault.HasValue && _dataContext.OrderRowData.unit_id != _dataContext.OrderRowData.Product.UnitaIDAlt && (_dataContext.Data.quantity % (_dataContext.OrderRowData.Product.QuantitaDefault ?? 1) == 0)))
                        {
                            if (_dataContext.Data.quantity!.Value + (_dataContext.OrderRowData.quantity_received ?? 0) <= _dataContext.OrderRowData.quantity ||
                                (_dataContext.Data.quantity!.Value + (_dataContext.OrderRowData.quantity_received ?? 0) > _dataContext.OrderRowData.quantity && ConfirmHandler.Confirm("La quantità digitata, sommata a quella eventualemnte già ricevuta, è superiore alla quantità ordinata, procedere comunque all'entrata merce ?")))
                            {
                                if (assignedQuantity == -1 || assignedQuantity > 0 || (assignedQuantity == 0 && ConfirmHandler.Confirm("Non è stata assegnata nessuna quantità agli ordini di produzione in attesa, se non viene assegnata ora l'impegno di materiale e lo sblocco dell'ordine di produzione andranno gestiti a mano.\n\nContinuare lo stesso ?")))
                                {
                                    if (_dataContext.OrderRowData.Jobs == null || _dataContext.OrderRowData.Jobs.Count == 0 || _dataContext.OrderRowData.Jobs.Sum(sum => sum.QuantityAssigned) <= _dataContext.Data.quantity)
                                    {
                                        if (_dataContext.OrderRowData.CustomerOrders == null || _dataContext.OrderRowData.CustomerOrders.Count == 0 || _dataContext.OrderRowData.CustomerOrders.Sum(sum => sum.QuantityAssigned) <= _dataContext.Data.quantity)
                                        {
                                            var confirmClose = (_dataContext.OrderRowData.quantity > (_dataContext.OrderRowData.quantity_received ?? 0) + _dataContext.Data.quantity ? ConfirmHandler.Confirm("Questa entrata merce deve chiudere la relativa riga d'ordine di acquisto ?") : true);
                                            if (_dataContext.Insert(confirmClose))
                                            {
                                                // check if need to unlock production order
                                                if (_dataContext.OrderRowData.Jobs != null && _dataContext.OrderRowData.Jobs.Count > 0)
                                                {
                                                    _dataContext.UnlockOrdersProductions();
                                                }
                                                // check if need to unlock customer order row
                                                if (_dataContext.OrderRowData.CustomerOrders != null && _dataContext.OrderRowData.CustomerOrders.Count > 0)
                                                {
                                                    _dataContext.UnlockOrdersCustomers();
                                                }
                                                InfoHandler.Show($"Entrata merce n.{_dataContext.Data.id} inserita correttamente");

                                                if (_dataContext.LotLabelPrint && !string.IsNullOrEmpty(_dataContext.Data.store_id) && !string.IsNullOrEmpty(_dataContext.Data.product_id) && !string.IsNullOrEmpty(_dataContext.Data.lot))
                                                {
                                                    var reportData = _dataContext.PrintLotLabel();
                                                    if (reportData != null)
                                                        ReportingHandler.PrintPDF(UserContext.Instance!.Domain!, Constants.MODULE_SRM, Constants.REPORT_TYPE_LOT_LABEL, _dataContext.CompanyID, reportData, $"LOTL n.{_dataContext.Data.lot}", $"LOTL n.{_dataContext.Data.lot}.pdf", true);
                                                    else
                                                        ErrorHandler.Validation($"Impossibile recuperare il lotto");
                                                }

                                                Mouse.OverrideCursor = null;
                                                this.DialogResult = true;
                                            }
                                            Mouse.OverrideCursor = null;
                                        }
                                        else
                                        { Mouse.OverrideCursor = null; ErrorHandler.Validation("E' stata assegnata agli ordini cliente una quantità superiore a quella ricevuta"); }
                                    }
                                    else
                                    { Mouse.OverrideCursor = null; ErrorHandler.Validation("E' stata assegnata agli ordini di produzione una quantità superiore a quella ricevuta"); }
                                }
                                else
                                { Mouse.OverrideCursor = null; }
                            }
                            else
                            { Mouse.OverrideCursor = null; }
                        }
                        else
                        {
                            Mouse.OverrideCursor = null;
                            ErrorHandler.Validation($"La quantità digitata ({_dataContext.Data.quantity!.Value.ToString("N6")}) non è valida in quanto non è un multiplo della quantità per confezione presente ({(_dataContext.OrderRowData.Product.QuantitaDefault ?? 1).ToString("N6")})");
                        }
                    }
                    else
                    {
                        Mouse.OverrideCursor = null;
                        ErrorHandler.Validation($"Selezionare la quantità");
                    }
                }
                else
                {
                    Mouse.OverrideCursor = null;
                    ErrorHandler.Validation($"Selezionare il prodotto");
                }
            }
            else
            { Mouse.OverrideCursor = null; ErrorHandler.Validation(validated); }
        }
        #endregion

        #region Jobs grid
        private void rgvJobs_RowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            var data = e.Row.Item as acq_orders_rows_jobs;

            if (data != null)
            {
                e.IsValid = data.quantity_needed >= data.quantity_received + data.QuantityAssigned;
                if (e.IsValid == false)
                {
                    e.ValidationResults.Add(new GridViewCellValidationResult()
                    {
                        ErrorMessage = "La quantità assegnata più quella già ricevuta superano la quantità necessaria.",
                        PropertyName = "QuantityAssigned"
                    });
                }
            }
        }
        #endregion

        #region Customer orders grid
        private void rgvCustomerOrders_RowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            var data = e.Row.Item as acq_orders_rows_customer_orders;

            if (data != null)
            {
                e.IsValid = data.quantity_needed >= data.quantity_received + data.QuantityAssigned;
                if (e.IsValid == false)
                {
                    e.ValidationResults.Add(new GridViewCellValidationResult()
                    {
                        ErrorMessage = "La quantità assegnata più quella già ricevuta superano la quantità necessaria.",
                        PropertyName = "QuantityAssigned"
                    });
                }
            }
        }
        #endregion

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

        private void CreateLot()
        {
            if (_dataContext.ShowLot && !string.IsNullOrEmpty(_dataContext.Data.product_id)) 
            {
                var product = _dataContext.GetTab_Articolo(_dataContext.Data.product_id);

                if (product != null)
                {
                    _dataContext.Data.lot = _dataContext.GetLot(product.ExpireDays ?? 0);
                }
            }
        }
        private void txtLot_GotFocus(object sender, RoutedEventArgs e)
        {
            CreateLot();
        }
    }
}
