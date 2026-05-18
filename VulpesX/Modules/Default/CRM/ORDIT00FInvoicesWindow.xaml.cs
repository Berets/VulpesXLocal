using DocumentFormat.OpenXml.Presentation;
using Microsoft.Extensions.DependencyInjection;
using Org.BouncyCastle.Asn1.Cmp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using VulpesX.ViewModels.Modules.Default.CRM;
using VulpesX.ViewModels.Modules.Default.General;
using VulpesX.WindowsFactory.Default.General;

namespace VulpesX.Modules.Default.CRM
{
    /// <summary>
    /// Interaction logic for ORDIT00FInvoicesWindow.xaml
    /// </summary>
    public partial class ORDIT00FInvoicesWindow : FluentDefaultWindow
    {
        private ORDIT00FInvoicesWindowViewModel _dataContext;
        private int _selectedPage;

        public ORDIT00FInvoicesWindow(ORDIT00FInvoicesWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.Height = (System.Windows.SystemParameters.PrimaryScreenHeight - 200);
            this.Width = (System.Windows.SystemParameters.PrimaryScreenWidth);

            this.DataContext = _dataContext;

            _dataContext.Items = _dataContext.Head.Invoices;

            GridView.DataLoaded += (s, e) =>
            {
                rdpGrid.MoveToPage(_selectedPage);
            };
            rdpGrid.PageIndexChanged += (s, e) =>
            {
                _selectedPage = e.NewPageIndex;
            };

            LoadData();
        }

        private async void LoadData()
        {
            await _dataContext.Load();
        }

        #region Buttons
        private void cmdEdit_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var item = (sender as Button)?.DataContext as FATTT00F;

            if (item != null)
            {
                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<FATTT00FWindowViewModel>();
                windowViewModel.Data = item;
                windowViewModel.IsInsert = false;

                var wFATTT00F = new FATTT00FWindow(windowViewModel);
                Mouse.OverrideCursor = null;
                wFATTT00F.Owner = Window.GetWindow(this);
                if (wFATTT00F.ShowDialog() == true)
                    LoadData();
            }
        }

        private void cmdEditRows_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var item = (sender as Button)?.DataContext as FATTT00F;

            if (item != null)
            {
                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<FATTD00FWindowViewModel>();
                windowViewModel.Head = item;

                var wFATTD00F = new FATTD00FWindow(windowViewModel);
                wFATTD00F.Owner = Window.GetWindow(this);
                Mouse.OverrideCursor = null;
                if (wFATTD00F.ShowDialog() == true)
                    LoadData();
            }
        }

        private void rgSelfInvoiceIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            var row = (sender as RadGlyph)?.DataContext as FATTT00F;
            if (row != null && row.SelfInvoice != null)
            {
                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<FATTAUTWindowViewModel>();
                windowViewModel.Data = row.SelfInvoice;
                windowViewModel.IsInsert = row.SelfInvoice == null;

                var wFATTAUT = new FATTAUTWindow(windowViewModel);
                wFATTAUT.Owner = Window.GetWindow(this);
                Mouse.OverrideCursor = null;
                if (wFATTAUT.ShowDialog() == true)
                    LoadData();
            }
        }

        private void rgInvoiceAttachmentsIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var row = (sender as RadGlyph)?.DataContext as FATTT00F;
            if (row != null)
            {
                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<FATTAL00FWindowViewModel>();
                windowViewModel.Head = row;

                var wFATTAL00F = new FATTAL00FWindow(windowViewModel);
                wFATTAL00F.Owner = Window.GetWindow(this);
                Mouse.OverrideCursor = null;
                if (wFATTAL00F.ShowDialog() == true)
                    LoadData();
            }
        }

        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)?.DataContext as FATTT00F;

            if (item != null)
            {
                if (item.FTNUFD <= 0)
                {
                    if (ConfirmHandler.Confirm($"Confermate di voler eliminare la fattura provvisoria n. {item.FTANNO}/{item.FTNUOR} ?\nNon sara' possibile recuperare le informazioni eliminate, procedere ?"))
                    {
                        if (_dataContext.Delete(item))
                        {
                            LoadData();
                        }
                    }
                }
                else
                {
                    ErrorHandler.Validation("Impossibile eliminare una fattura stampata in definitivo");
                }
            }
        }

        private void cmdEditCustomer_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            var selected = (sender as Button)?.DataContext as FATTT00F;

            if (selected != null && selected.FTCODC.HasValue && selected.FTCODC.Value > 0)
            {
                var item = _dataContext.GetABE(selected.FTCODC.Value);

                if (item != null)
                {
                    if (item.abecod > 0)
                    {


                        var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ABEWindowViewModel>();

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
                        if (item.abecfe == "C" || item.abecfe == "E")
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

                        windowViewModel.Data = item;
                        windowViewModel.CustomerNotes = _dataContext.GetNOTECLI1s(item.abecod);
                        windowViewModel.SupplierNotes = _dataContext.GetNOTEFORs(item.abecod);
                        windowViewModel.SupplierData = supplierData;
                        windowViewModel.CustomerData = customerData;
                        windowViewModel.CustomerCommercialData = customerCommercialData;
                        windowViewModel.SupplierCommercialData = supplierCommercialData;
                        windowViewModel.SupplierReferences = _dataContext.GetRFFTB00Fs(item.abecod);
                        windowViewModel.CustomerReferences = _dataContext.GetANDEFREs(item.abecod);
                        windowViewModel.CounterpartsRows = _dataContext.GetSUPPLIER_GROUPs(item.abecod);
                        windowViewModel.CounterpartsRowsCustomer = _dataContext.GetCUSTOMER_GROUPs(item.abecod);
                        windowViewModel.IsInsert = false;

                        var wABE = VulpesServiceProvider.Provider.GetRequiredService<IABEWindowFactory>().Create(windowViewModel);
                        Mouse.OverrideCursor = null;
                        wABE.Owner = Window.GetWindow(this);
                        if (wABE.ShowDialog() == true)
                            LoadData();
                    }
                }
            }
        }
        #endregion
    }
}
