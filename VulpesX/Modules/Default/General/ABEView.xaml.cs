using Microsoft.Extensions.DependencyInjection;
using SharpDX;
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
using Telerik.Windows.Persistence.Core;
using VulpesX.Models;
using VulpesX.Modules.Default.Commons;
using VulpesX.Shared;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Commons;
using VulpesX.ViewModels.Modules.Default.General;

namespace VulpesX.Modules.Default.General
{
    public partial class ABEView : UserControl
    {
        private ABEViewModel _dataContext;
        private int _selectedPage = 0;

        public ABEView()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<ABEViewModel>();

            InitializeComponent();

            this.DataContext = _dataContext;
            this.PreviewKeyDown += (s, e) =>
            {
                if (e.Key == Key.F5)
                    LoadData();
            };

            gvcActivities.IsVisible = UserContext.Instance.ACCESS!.Roles!.canCRMActivities;

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
            txtSearch.Focus();
        }

        private async void LoadData()
        {
            await _dataContext.Load();
        }

        private void cmdEdit_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var item = (sender as Button)!.DataContext as ABE;

            if (item != null)
            {
                if (item.abecod > 0)
                {
                    FORNAMMI? supplierData = null;
                    FORNITORI? supplierCommercialData = null;
                    var groupsList = _dataContext.GetGruppi();
                    var accountCache = _dataContext.GetConti();
                    var subaccountCache = _dataContext.GetSotto();

                    if (item.abecfe == "F" || item.abecfe == "E")
                    {
                        supplierData = _dataContext.GetFORNAMMI(item.abecod);
                        if (supplierData != null)
                        {
                            supplierData.AccountCache = accountCache;
                            supplierData.SubaccountCache = subaccountCache;
                            supplierData.GroupsList = groupsList;
                        }
                        else
                        {
                            supplierData = new FORNAMMI()
                            {
                                AccountCache = accountCache,
                                SubaccountCache = subaccountCache,
                                GroupsList = groupsList,
                                Foraso = _dataContext.CompanyID,
                                Foraco = item.abecod
                            };
                        }
                        supplierCommercialData = _dataContext.GetFORNITORI(item.abecod);
                        if (supplierCommercialData == null)
                            supplierCommercialData = new FORNITORI() { FOCLIF = item.abecod };
                    }
                    CLIAMMI? customerData = null;
                    CLIENTI? customerCommercialData = null;
                    if (item.abecfe == "C" || item.abecfe == "E" || item.abecfe == "P")
                    {
                        customerData = _dataContext.GetCLIAMMI(item.abecod);
                        if (customerData != null)
                        {
                            customerData.AccountCache = accountCache;
                            customerData.SubaccountCache = subaccountCache;
                            customerData.GroupsList = groupsList;
                        }
                        else
                        {
                            customerData = new CLIAMMI()
                            {
                                AccountCache = accountCache,
                                SubaccountCache = subaccountCache,
                                GroupsList = groupsList,
                                Cliasoc = _dataContext.CompanyID,
                                Cliacod = item.abecod
                            };
                        }
                        customerCommercialData = _dataContext.GetCLIENTI(item.abecod);
                        if (customerCommercialData == null)
                            customerCommercialData = new CLIENTI() { CLIENT = item.abecod };
                    }

                    var abeWindowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ABEWindowViewModel>();
                    var data = _dataContext.GetData(item.abecod);

                    if (data != null)
                    {
                        abeWindowViewModel.Data = data;
                        abeWindowViewModel.CustomerNotes = _dataContext.GetNOTECLI(item.abecod);
                        abeWindowViewModel.SupplierNotes = _dataContext.GetNOTEFOR(item.abecod);
                        abeWindowViewModel.SupplierData = supplierData;
                        abeWindowViewModel.CustomerData = customerData;
                        abeWindowViewModel.CustomerCommercialData = customerCommercialData;
                        abeWindowViewModel.SupplierCommercialData = supplierCommercialData;
                        abeWindowViewModel.SupplierReferences = _dataContext.GetRFFTB00F(item.abecod);
                        abeWindowViewModel.CustomerReferences = _dataContext.GetANDEFRES(item.abecod);
                        abeWindowViewModel.CounterpartsRows = _dataContext.GetSUPPLIER_GROUPS(item.abecod);
                        abeWindowViewModel.CounterpartsRowsCustomer = _dataContext.GetCUSTOMER_GROUPS(item.abecod);
                        abeWindowViewModel.IsInsert = false;

                        var wABE = new ABEWindow(abeWindowViewModel);
                        Mouse.OverrideCursor = null;
                        wABE.Owner = Window.GetWindow(this);
                        if (wABE.ShowDialog() == true)
                            LoadData();
                    }
                }
            }
        }

        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)?.DataContext as ABE;

            if (item != null)
            {
                if (ConfirmHandler.Confirm($"Confermate l'eliminazione del codice anagrafico\n [{item.abecod.ToString("N0")} - {item.abers1?.Trim()}] ?"))
                {
                    if (ConfirmHandler.Confirm($"Per eliminare definitivamente il codice e cancellarne i dati (operazione irreversibile!) rispondere [SI], per annullare il codice e mantenerne i dati rispondere [NO] (in questo caso non sarà possibile riutilizzare il codice)"))
                    {
                        _dataContext.Delete(item, null, null);
                    }
                    else
                    {
                        var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<CancelReasonWindowViewModel>();
                        windowViewModel.MinSize = 15;
                        var wAsk = new CancelReasonWindow(windowViewModel);
                        wAsk.Owner = Window.GetWindow(this);

                        if (wAsk.ShowDialog() == true && !string.IsNullOrWhiteSpace(windowViewModel.SelectedReason))
                        {
                            _dataContext.Delete(item, windowViewModel.SelectedReason.Trim(), _dataContext.UserID);
                        }
                    }
                    LoadData();
                }
            }
        }

        private void cmdInsert_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var groupsList = _dataContext.GetGruppi();
            var accountCache = _dataContext.GetConti();
            var subaccountCache = _dataContext.GetSotto();

            var abeWindowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ABEWindowViewModel>();
            abeWindowViewModel.Data = new ABE
            {
                abeind = string.Empty,
                isocod = "IT"
            };
            abeWindowViewModel.Recipients = new ObservableCollection<DESTINATARI>();
            abeWindowViewModel.SupplierRecipients = new ObservableCollection<DESFOR>();
            abeWindowViewModel.CustomerNotes = new ObservableCollection<NOTECLI1>();
            abeWindowViewModel.SupplierNotes = new ObservableCollection<NOTEFOR>();
            abeWindowViewModel.SupplierData = new FORNAMMI()
            {
                AccountCache = accountCache,
                SubaccountCache = subaccountCache,
                GroupsList = groupsList,
                Foraso = _dataContext.CompanyID
            };
            abeWindowViewModel.CustomerData = new CLIAMMI()
            {
                AccountCache = accountCache,
                SubaccountCache = subaccountCache,
                GroupsList = groupsList,
                Cliasoc = _dataContext.CompanyID
            };
            abeWindowViewModel.SupplierCommercialData = new FORNITORI()
            { };
            abeWindowViewModel.CustomerCommercialData = new CLIENTI()
            { };
            abeWindowViewModel.SupplierReferences = new ObservableCollection<RFFTB00F>();
            abeWindowViewModel.CustomerReferences = new ObservableCollection<ANDEFRES>();
            abeWindowViewModel.CounterpartsRows = new ObservableCollection<SUPPLIER_GROUPS>();
            abeWindowViewModel.CounterpartsRowsCustomer = new ObservableCollection<CUSTOMER_GROUPS>();
            abeWindowViewModel.IsInsert = true;

            var wABE = new ABEWindow(abeWindowViewModel);
            Mouse.OverrideCursor = null;
            wABE.Owner = Window.GetWindow(this);
            if (wABE.ShowDialog() == true)
                LoadData();
        }

        private void cmGrid_Opening(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            rmiCreditScore.IsEnabled = GridView.SelectedItem != null;
            rmiReport.IsEnabled = GridView.SelectedItem != null;
            rmiBalance.IsEnabled = GridView.SelectedItem != null;
        }

        private void rmiCreditScore_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            //Mouse.OverrideCursor = Cursors.Wait;
            //var selected = GridView.SelectedItem as ABE;
            //if (!string.IsNullOrWhiteSpace(selected.abepiv))
            //{
            //    wCreditScore wCS = new wCreditScore(ITFClient.SUBSCRIBERID, selected.abepiv);
            //    wCS.Owner = Window.GetWindow(this);
            //    Mouse.OverrideCursor = null;
            //    wCS.ShowDialog();
            //}
            //else
            //{
            //    Constants.ShowError("La partita IVA non e' inserita o non e' valida");
            //}
        }

        private void rmiReport_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            //var selected = GridView.SelectedItem as ABE;
            //if (!string.IsNullOrWhiteSpace(selected.abepiv))
            //{
            //    wWait wReport = new wWait(ITFClient.SUBSCRIBERID, selected.abepiv);
            //    wReport.Owner = Window.GetWindow(this);
            //    wReport.ShowDialog();
            //}
            //else
            //{
            //    Constants.ShowError("La partita IVA non e' inserita o non e' valida");
            //}
        }

        private void rmiBalance_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            //var selected = GridView.SelectedItem as ABE;
            //if (!string.IsNullOrWhiteSpace(selected.abepiv))
            //{
            //    wBalance wBal = new wBalance(ITFClient.SUBSCRIBERID, selected.abepiv);
            //    wBal.Owner = Window.GetWindow(this);
            //    wBal.ShowDialog();
            //}
            //else
            //{
            //    Constants.ShowError("La partita IVA non e' inserita o non e' valida");
            //}
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs? e)
        {
            (RadGridViewCommands.SearchByText as RoutedUICommand)?.Execute(txtSearch.Text, GridView);
        }

        private void rgActivities_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as RadGlyph)!.DataContext as ABE;
            if (item != null)
            {
                //var wActivities = new wEntityActivities(ctx, new EntityActivitiesViewModel() { SelectedCustomerID = item.abecod });
                //wActivities.Owner = Window.GetWindow(this);
                //wActivities.ShowDialog();
                LoadData();
            }
        }

        private void GridView_LoadingRowDetails(object sender, GridViewRowDetailsEventArgs e)
        {
            var rowData = e.Row.Item as ABE;

            var userControlContainer = e.DetailsElement.FindName("UserControlContainer") as ContentControl;

            if (userControlContainer != null)
            {
                //userControlContainer.Content = new ucRECAP(ctx, rowData.abecod);
            }
        }
    }
}
