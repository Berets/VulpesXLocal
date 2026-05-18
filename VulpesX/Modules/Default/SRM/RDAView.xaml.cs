using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using VulpesX.DAL;
using VulpesX.Models;
using VulpesX.Models.Models;
using VulpesX.Modules.Default.Commons;
using VulpesX.Shared;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Commons;
using VulpesX.ViewModels.Modules.Default.SRM;

namespace VulpesX.Modules.Default.SRM
{
    /// <summary>
    /// Interaction logic for RDAView.xaml
    /// </summary>
    public partial class RDAView : UserControl
    {
        private RDAViewModel _dataContext;
        private int _selectedPage = 0;

        public RDAView()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<RDAViewModel>();

            InitializeComponent();

            cmbStatus.ItemsSource = CommonsService.RDAStatuses;
            cmbStatus.SelectedValue = "P";

            this.DataContext = _dataContext;

            this.PreviewKeyDown += (s, e) =>
            {
                if (e.Key == Key.F5)
                {
                    LoadData();
                }
            };

            GridView.DataLoaded += (s, e) =>
            {
                rdpGrid.MoveToPage(_selectedPage);
                txtSearch_TextChanged(txtSearch, null);
            };
            rdpGrid.PageIndexChanged += (s, e) =>
            {
                _selectedPage = e.NewPageIndex;
            };

            LoadData();
        }

        private async void LoadData()
        {
            string? selectedStatusID = cmbStatus.SelectedValue?.ToString();

            if (selectedStatusID != null)
            {
                await _dataContext.Load(selectedStatusID);
            }
        }

        #region Buttons
        private void cmdEdit_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var item = (sender as Button)?.DataContext as SRM_RDA;

            if (item != null)
            {
                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<RDAWindowViewModel>();
                windowViewModel.Data = item;
                windowViewModel.IsInsert = false;

                var wSRM_RDA = new RDAWindow(windowViewModel);
                wSRM_RDA.Owner = Window.GetWindow(this);
                Mouse.OverrideCursor = null;
                if (wSRM_RDA.ShowDialog() == true)
                    LoadData();
            }
        }
        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            var item = (sender as Button)?.DataContext as SRM_RDA;

            if (item != null)
            {
                if (item.CanDelete)
                {
                    if (ConfirmHandler.Confirm($"Confermate di voler eliminare la RDA n. {item.id} ?\nNon sara' possibile recuperare le informazioni eliminate, procedere ?"))
                    {
                        // check if linked order
                        ORDIT00F? order = null;
                        ORDID00F? orderRow = null;
                        ObservableCollection<store_stocks_engage>? engages = null;

                        if (item.order_year.HasValue && item.order_number.HasValue && item.order_row.HasValue)
                        {
                            order = _dataContext.GetORDIT00F(item.companyID, item.order_year.Value, item.order_number.Value);
                            orderRow = _dataContext.GetORDID00F(item.companyID, item.order_year.Value, item.order_number.Value, item.order_row.Value);
                            engages = _dataContext.GetStore_Stocks_Engages(item.companyID, item.order_year.Value, item.order_number.Value, item.order_row.Value);
                        }

                        if (order == null || (order != null && string.IsNullOrWhiteSpace(orderRow?.ODSTATO)))
                        {
                            Mouse.OverrideCursor = null;
                            if (order == null || (order != null && orderRow != null && ConfirmHandler.Confirm($"La RDA che si sta annullando è legata all'ordine cliente {order.PrintFullID} riga {orderRow.ODRIGA}{(engages != null && engages.Count > 0 ? $" ed ha già degli impegni parziali di materiale ({engages.Count})" : null)} : proseguendo con l'operazione la riga dell'ordine verrà riaperta ed eventuali impegni di materiale verranno annullati, continuare ?")))
                            {
                                // ask for reason
                                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<CancelReasonWindowViewModel>();
                                var wAskCR = new CancelReasonWindow(windowViewModel);

                                wAskCR.Owner = Window.GetWindow(this);
                                if (wAskCR.ShowDialog() == true && !string.IsNullOrWhiteSpace(windowViewModel.SelectedReason))
                                {
                                    Mouse.OverrideCursor = Cursors.Wait;
                                    // clear and reopen
                                    bool orderCleared = true;
                                    if (order != null && orderRow != null)
                                    {
                                        orderCleared = _dataContext.UnlockCustomerOrder(order, orderRow, engages ?? new ObservableCollection<store_stocks_engage>());
                                    }
                                    if (orderCleared)
                                    {
                                        item.canceled = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
                                        item.canceledUserID = _dataContext.UserID;
                                        item.canceledNote = windowViewModel.SelectedReason;

                                        _dataContext.Update(item);

                                        LoadData();
                                    }
                                    else
                                    {
                                        Mouse.OverrideCursor = null;
                                        ErrorHandler.Validation("Errore durante lo sblocco dell'ordine cliente");
                                    }
                                }
                            }
                        }
                        else
                        {
                            Mouse.OverrideCursor = null;
                            ErrorHandler.Validation("Impossibile cancellare una RDA collegata ad una posizione di un ordine cliente che è già stata trasformata in DDT o fattura");
                        }
                    }
                    Mouse.OverrideCursor = null;
                }
                else
                {
                    Mouse.OverrideCursor = null;
                    ErrorHandler.Validation("Impossibile eliminare un ordine chiuso, trasformato in ordini di produzione o gia' annullato");
                }
            }
        }

        private void cmdInsert_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

            var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<RDAWindowViewModel>();
            windowViewModel.Data = new SRM_RDA()
            {
                companyID = _dataContext.CompanyID,
                id = _dataContext.GetNumerator(Constants.RDA),
                requested_delivery = now,
                addedUserID = _dataContext.UserID,
                product_id = string.Empty,
            };
            windowViewModel.IsInsert = true;

            var wSRM_RDA = new RDAWindow(windowViewModel);
            wSRM_RDA.Owner = Window.GetWindow(this);
            Mouse.OverrideCursor = null;
            if (wSRM_RDA.ShowDialog() == true)
                LoadData();
        }

        private void cmdTransform_Click(object sender, RoutedEventArgs e)
        {
            if (GridView.SelectedItems != null && GridView.SelectedItems.Count > 0)
            {
                if (ConfirmHandler.Confirm("Confermate la trasformazione in ordine delle RDA selezionate ?"))
                {
                    Mouse.OverrideCursor = Cursors.Wait;

                    int sequence = 1;

                    var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

                    foreach (var sup in GridView.SelectedItems.Cast<SRM_RDA>().GroupBy(g => g.supplier_id))
                    {
                        var newOrderID = _dataContext.GetNumerator(Constants.BUY_ORDERS);

                        FORNAMMI? supplierInfo = null;
                        if (sup.Key.HasValue && sup.Key.Value > 0)
                            supplierInfo = _dataContext.GetFORNAMMI(sup.Key.Value);

                        // head
                        var newOrderHead = new acq_orders_heads()
                        {
                            company_id = _dataContext.CompanyID,
                            addedUserID = _dataContext.UserID,
                            id = newOrderID,
                            order_date = now,
                            Rows = new ObservableCollection<acq_orders_rows>(),
                            payment_id = supplierInfo != null && !string.IsNullOrWhiteSpace(supplierInfo.pfocod) ? supplierInfo.pfocod : null,
                            supplier_id = sup.Key ?? 0,
                            shipping_id = supplierInfo != null && !string.IsNullOrWhiteSpace(supplierInfo.specod) ? supplierInfo.specod : null,
                            delivery_id = supplierInfo != null && !string.IsNullOrWhiteSpace(supplierInfo.concod) ? supplierInfo.concod : null,
                            commercial_signed = (UserContext.Instance!.ACCESS!.Roles?.canPOSignCommercial ?? false) ? now : null,
                            commercial_signer = (UserContext.Instance!.ACCESS!.Roles?.canPOSignCommercial ?? false) ? _dataContext.UserID : null,
                            management_signed = (UserContext.Instance!.ACCESS!.Roles?.canPOSignManagement ?? false) ? now : null,
                            management_signer = (UserContext.Instance!.ACCESS!.Roles?.canPOSignManagement ?? false) ? _dataContext.UserID : null
                        };

                        foreach (var item in GridView.SelectedItems.Cast<SRM_RDA>().Where(w => w.supplier_id == sup.Key).GroupBy(g => g.product_id).Select(s => new
                        {
                            pid = s.Key,
                            count = s.Count(),
                            qty = s.Sum(sum => sum.quantity),
                            ori = s.Sum(sum => sum.original_needed)
                        }))
                        {
                            var rdas = GridView.SelectedItems.Cast<SRM_RDA>().Where(w => w.supplier_id == sup.Key && w.product_id == item.pid).OrderBy(o => o.requested_delivery).ToList();
                            var product = _dataContext.GetTab_Articolo(item.pid);
                            decimal price = 0;

                            GenericPriceInfo? lastPL = null;
                            // check if price list entry
                            if (product != null && sup.Key.HasValue && sup.Key.Value > 0)
                            {
                                lastPL = _dataContext.GetGenericPriceInfo(item.pid, sup.Key.Value, now, product.UnitaID ?? string.Empty);
                                if (lastPL != null)
                                {
                                    price = lastPL.Price;
                                }
                            }

                            var newRow = new acq_orders_rows()
                            {
                                company_id = _dataContext.CompanyID,
                                id = newOrderID,
                                sequence = sequence++,
                                product_id = item.pid,
                                quantity = item.qty,
                                price = price,
                                price_type = "U",
                                discount = lastPL?.Discount1,
                                discount_type = lastPL?.DiscountType1,
                                delivery_requested = rdas.First().requested_delivery,
                                store_id = _dataContext.GetDefaultStoreRaw(),
                                tax_code = !string.IsNullOrWhiteSpace(product?.asscod) ? product.asscod : "A",
                                tax_rate = !string.IsNullOrWhiteSpace(product?.assali) ? product.assali : "22",
                                RDAs = new ObservableCollection<acq_orders_rows_rdas>(),
                                Jobs = new ObservableCollection<acq_orders_rows_jobs>(),
                                CustomerOrders = new ObservableCollection<acq_orders_rows_customer_orders>()
                            };

                            // RDAs
                            foreach (var ra in rdas)
                            {
                                newRow.RDAs.Add(new acq_orders_rows_rdas()
                                {
                                    company_id = _dataContext.CompanyID,
                                    order_id = newRow.id,
                                    order_row_id = newRow.sequence,
                                    rda_id = ra.id
                                });
                            }

                            // jobs
                            foreach (var jo in rdas.Where(w => w.product_id == item.pid).GroupBy(g => g.production_order_id).Select(s => new { prid = s.Key, qty = s.Sum(sum => sum.quantity), ori = s.Sum(sum => sum.original_needed) }).ToList())
                            {
                                if (!string.IsNullOrWhiteSpace(jo.prid))
                                {
                                    newRow.Jobs.Add(new acq_orders_rows_jobs()
                                    {
                                        company_id = _dataContext.CompanyID,
                                        order_id = newRow.id,
                                        order_row_id = newRow.sequence,
                                        job_id = jo.prid,
                                        quantity_received = 0,
                                        quantity_needed = jo.qty,
                                        quantity_original = jo.ori
                                    });
                                }
                            }

                            // customer orders
                            foreach (var co in rdas.Where(w => w.product_id == item.pid).GroupBy(g => new { g.order_year, g.order_number, g.order_row }).Select(s => new { cuord = s.Key, qty = s.Sum(sum => sum.quantity), ori = s.Sum(sum => sum.original_needed) }).ToList())
                            {
                                if (co.cuord.order_year.HasValue && co.cuord.order_number.HasValue && co.cuord.order_row.HasValue)
                                {
                                    newRow.CustomerOrders.Add(new acq_orders_rows_customer_orders()
                                    {
                                        company_id = _dataContext.CompanyID,
                                        order_id = newRow.id,
                                        order_row_id = newRow.sequence,
                                        customer_order_year = co.cuord.order_year.Value,
                                        customer_order_number = co.cuord.order_number.Value,
                                        customer_order_row = co.cuord.order_row.Value,
                                        quantity_received = 0,
                                        quantity_needed = co.qty,
                                        quantity_original = co.ori
                                    });
                                }
                            }
                            newOrderHead.Rows.Add(newRow);
                        }
                        if (_dataContext.Insert_acq_orders_heads(newOrderHead))
                        {
                            // update RDA
                            foreach (var item in GridView.SelectedItems.Cast<SRM_RDA>().Where(w => w.supplier_id == sup.Key))
                            {
                                item.transformed = now;
                                item.transform_user = _dataContext.UserID;
                                _dataContext.Update(item);
                            }
                        }
                    }
                    Mouse.OverrideCursor = null;

                    InfoHandler.Show("Ordini di acquisto generati correttamente");
                }

                Mouse.OverrideCursor = null;

                LoadData();
            }
        }

        private void rgApprove_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (UserContext.Instance!.ACCESS!.Roles?.canApproveRDA ?? false)
            {
                var row = (sender as RadGlyph)?.DataContext as SRM_RDA;
                if (row != null && row.CanApprove)
                {
                    if (row != null)
                    {
                        if (ConfirmHandler.Confirm($"Confermate l'approvazione per la RDA {row.id} ?"))
                        {
                            row.approval_date = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
                            row.approval_user = _dataContext.UserID;

                            _dataContext.Update(row);

                            LoadData();
                        }
                    }
                }
            }
            else
            {
                ErrorHandler.Validation("Non si possiede l'abilitazione necessaria per l'approvazione delle RDA");
            }
        }
        #endregion

        #region UC standard functions
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs? e)
        {
            (RadGridViewCommands.SearchByText as RoutedUICommand)?.Execute(txtSearch.Text, GridView);
        }

        private void GridView_SelectionChanged(object sender, SelectionChangeEventArgs e)
        {
            if (UserContext.Instance!.ACCESS!.Roles?.canTransformRDA ?? false)
            {
                bool canTransform = true;
                foreach (var item in GridView.SelectedItems)
                {
                    var rda = item as SRM_RDA;

                    if (rda != null)
                    {
                        if (rda.canceled != null || rda.approval_date == null || rda.transformed != null)
                        {
                            canTransform = false;
                            break;
                        }
                    }
                    else
                    {
                        canTransform = false;
                        break;
                    }
                }

                if (canTransform)
                {
                    cmdTransform.IsEnabled = true;
                }
                else
                {
                    cmdTransform.IsEnabled = false;
                    cmdTransform.ToolTip = "Avete selezionato delle RDA gia' trasformate, annullate o non approvate";
                }
            }
            else
            {
                cmdTransform.IsEnabled = false;
                cmdTransform.ToolTip = "Non si possiede l'abilitazione per trasformare le RDA in ordini di acquisto";
            }
        }

        private void cmbStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadData();
        }
        #endregion
    }
}
