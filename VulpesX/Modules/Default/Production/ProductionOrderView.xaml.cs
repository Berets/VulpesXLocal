using DocumentFormat.OpenXml.Drawing.Charts;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using VulpesX.DAL;
using VulpesX.Models;
using VulpesX.Shared;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Production;

namespace VulpesX.Modules.Default.Production
{
    /// <summary>
    /// Interaction logic for ProductionOrderView.xaml
    /// </summary>
    public partial class ProductionOrderView : UserControl
    {
        private ProductionOrderViewModel _dataContext;
        private int _selectedPage = 0;

        public ProductionOrderView()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<ProductionOrderViewModel>();

            InitializeComponent();

            this.DataContext = _dataContext;

            dtpYear.Culture = new System.Globalization.CultureInfo("it-IT");
            dtpYear.Culture.DateTimeFormat.ShortDatePattern = "yyyy";
            dtpYear.SelectedValue = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

            cmbStatus.ItemsSource = CommonsService.ProductOrderTypes;
            cmbStatus.SelectedValue = "A";

            this.PreviewKeyDown += (s, e) =>
            {
                if (e.Key == Key.F5)
                    LoadData();
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
            int? selectedYear = dtpYear.SelectedDate?.Year;

            if (selectedStatusID != null && selectedYear.HasValue)
            {
                await _dataContext.Load(selectedYear.Value ,selectedStatusID);
            }
        }

        #region Buttons and Context
        private void cmdEdit_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var item = (sender as Button)?.DataContext as pro_ordine;

            if (item != null)
            {
                if (!string.IsNullOrWhiteSpace(item.ID))
                {
                    if (item.Stato == "A")
                    {
                        var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ProductionOrderWindowViewModel>();
                        windowViewModel.Data = item;
                        windowViewModel.IsInsert = false;

                        var wOrder = new ProductionOrderWindow(windowViewModel);
                        wOrder.Owner = Window.GetWindow(this);
                        if (wOrder.ShowDialog() == true)
                            LoadData();
                    }
                    else
                    {
                        ErrorHandler.Validation("Impossibile modificare un ordine di produzione già lanciato");
                    }
                }
            }
        }


        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)?.DataContext as pro_ordine;

            if (item != null)
            {
                if (item.Stato == "A" || item.Stato == "O")
                {
                    if (ConfirmHandler.Confirm($"Confermate l'eliminazione dell'ordine {item.ID} ? Verranno eliminate anche eventuali rilevazioni e tutti gli impegni di materiale!"))
                    {
                        if (_dataContext.Delete(item))
                        {
                            if (item.OrdineClienteAnno.HasValue && item.OrdineClienteID.HasValue && item.OrdineClienteRiga.HasValue)
                            {
                                _dataContext.UpdateORDID00F(item);
                            }
                            LoadData();
                        }
                        else
                        {
                            ErrorHandler.Validation("Errore imprevisto durante l'eliminazione");
                        }
                    }
                }
                else
                {
                    ErrorHandler.Validation("E' possibile eliminare solo gli ordini non ancora iniziati");
                }
            }
        }

        private void cmdInsert_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            var date = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

            var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ProductionOrderWindowViewModel>();
            windowViewModel.Data = new pro_ordine()
            {
                SocietaID = _dataContext.CompanyID,
                ID = _dataContext.GetNumerator(),
                DataOrdine = date,
                DataConsegna = date.AddDays(7),
                LogAddedUserID = _dataContext.UserID,
                Stato = "A"
            };
            windowViewModel.IsInsert = true;

            var wOrder = new ProductionOrderWindow(windowViewModel);

            Mouse.OverrideCursor = null;

            wOrder.Owner = Window.GetWindow(this);
            wOrder.ShowDialog();

            LoadData();
        }

        private void cmGrid_Opening(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            rmiStatus.Items.Clear();

            if (GridView.SelectedItem != null)
            {
                var item = GridView.SelectedItem as pro_ordine;
                if (item != null)
                {
                    rmiLaunch.IsEnabled = item.Stato == "A" || item.Stato == "R";

                    foreach (var st in CommonsService.ProductOrderTypes.Where(o => o.ID != "*"))
                    {
                        var ctxStatus = new RadMenuItem { Header = st.Description, Tag = st };
                        ctxStatus.Click += ctxStatus_Click;

                        rmiStatus.Items.Add(ctxStatus);
                    }
                }
            }
        }

        private void ctxStatus_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            if (ConfirmHandler.Confirm("Confermate la forzatura dello stato sugli ordini selezionati ?"))
            {
                Mouse.OverrideCursor = Cursors.Wait;

                var status = (sender as RadMenuItem)?.Tag as GenericIDDescription;

                var orders = GridView.SelectedItems.Cast<pro_ordine>().ToList();

                if (status != null && orders.Any())
                {
                    foreach (var order in orders)
                    {
                        var previousState = order.Stato;

                        order.Stato = status.ID;

                        _dataContext.Insertpro_ordine_history(order, Constants.PRODUCTION_MANUAL_CHANGE_STATUS, previousState ?? string.Empty);
                        _dataContext.UpdateStatus(order);
                    }
                    Mouse.OverrideCursor = null;
                    LoadData();
                }
            }
        }

        private void rmiLaunch_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            var item = GridView.SelectedItem as pro_ordine;
            if (item != null)
            {
                item = _dataContext.Get(item.ID);

                if (item != null)
                {
                    if (ConfirmHandler.Confirm($"Confermate il lancio in produzione dell'ordine {item.ID} [{item.ArticoloID} - {item.ArticoloDescrizione}] ?"))
                    {
                        // check needed defaults
                        // default stores
                        if (_dataContext.StoreDefaultCheck())
                        {
                            // default causals
                            if (_dataContext.CausalDefaultCheck())
                            {
                                if (item.Stato == "A")
                                {
                                    // log
                                    _dataContext.Insertpro_ordine_history(item, Constants.PRODUCTION_START_LAUNCH, "A");

                                    var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ProductionOrderConfirmWindowViewModel>();
                                    windowViewModel.Order = item;
                                    
                                    var wConfirmLaunch = new ProductionOrderConfirmWindow(windowViewModel);
                                    wConfirmLaunch.Owner = Window.GetWindow(this);
                                    wConfirmLaunch.ShowDialog();
                                }
                                else if (item.Stato == "R")
                                {
                                    item.LogUpdatedUserID = _dataContext.UserID;
                                    item.Stato = "O";

                                    _dataContext.Update(item);
                                    // log
                                    _dataContext.Insertpro_ordine_history(item, Constants.PRODUCTION_LAUNCH, "R");
                                }
                            }
                        }
                        Mouse.OverrideCursor = null;
                        LoadData();
                    }
                }
            }
        }

        private void rgPrint_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            var item = (sender as RadGlyph)?.DataContext as pro_ordine;

            if (item != null)
            {
                var reportData = _dataContext.PrintProductionOrder(item);
                if (reportData != null)
                    ReportingHandler.PrintPDF(UserContext.Instance!.Domain!, Constants.MODULE_PRODUCTION, Constants.REPORT_TYPE_PRODUCTION_ODP, _dataContext.CompanyID, reportData, $"ODP n.{item.ID}", $"ODP [{item.SocietaID}] {item.ID}.pdf", true);
                else
                    ErrorHandler.Validation($"Impossibile recuperare l'ordine di produzione per l'ordine {item.ID}");
            }

            Mouse.OverrideCursor = null;
        }

        private void rgUnlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            var item = (sender as RadGlyph)?.DataContext as pro_ordine;

            if (item != null && (item.Stato == "L" || item.Stato == "W"))
            {
                if (ConfirmHandler.Confirm($"Confermate lo sblocco dell'ordine {item.ID} ?\n\nSarà possibile rilanciarlo mantenendo eventuali impegni già presenti.\nL'attività viene registrata"))
                {
                    var previousState = item.Stato;

                    item.Stato = "A";
                    item.LogUpdatedUserID = _dataContext.UserID;

                    _dataContext.Update(item);

                    _dataContext.Insertpro_ordine_history(item, Constants.PRODUCTION_UNLOCK, previousState);

                    LoadData();
                }
            }
            Mouse.OverrideCursor = null;
        }

        private void rgView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as RadGlyph)?.DataContext as pro_ordine;

            if (item != null)
            {
                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ProductionOrderLookWindowViewModel>();
                windowViewModel.Data = item;

                var wLook = new ProductionOrderLookWindow(windowViewModel);
                wLook.Owner = Window.GetWindow(this);
                wLook.ShowDialog();
            }
        }

        private void cmdExportXLS_Click(object sender, RoutedEventArgs e)
        {
            //rdpGrid.PageSize = rdpGrid.PageCount * 20;
            //new TelerikGrid().ExportXlsx(GridView);
            //rdpGrid.PageSize = 20;
        }
        #endregion

        #region UC standard functions
        private void cmbStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadData();
        }

        private void dtpYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadData();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs? e)
        {
            (RadGridViewCommands.SearchByText as RoutedUICommand)?.Execute(txtSearch.Text, GridView);
        }
        #endregion

    }
}
