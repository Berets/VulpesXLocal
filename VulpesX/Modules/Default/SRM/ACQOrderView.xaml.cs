using DocumentFormat.OpenXml.Office2010.Word;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Mail;
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
using VulpesX.DAL;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Modules.Default.Commons;
using VulpesX.Shared;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Commons;
using VulpesX.ViewModels.Modules.Default.SRM;
using static VulpesX.Shared.Utilities.NotifierHelper;

namespace VulpesX.Modules.Default.SRM
{
    /// <summary>
    /// Interaction logic for ACQOrderView.xaml
    /// </summary>
    public partial class ACQOrderView : UserControl
    {
        private ACQOrderViewModel _dataContext;
        private int _selectedPage = 0;

        public ACQOrderView()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<ACQOrderViewModel>();

            InitializeComponent();

            this.DataContext = _dataContext;

            cmbStatus.ItemsSource = CommonsService.PurchaseOrderStatuses;
            cmbStatus.SelectedValue = "M";
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

            var item = (sender as Button)?.DataContext as acq_orders_heads;

            if (item != null)
            {
                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ACQOrderHeadWindowViewModel>();
                windowViewModel.Data = item;
                windowViewModel.IsInsert = false;

                var wACQ_ORDERS_HEADS = new ACQOrderHeadWindow(windowViewModel);
                wACQ_ORDERS_HEADS.Owner = Window.GetWindow(this);
                Mouse.OverrideCursor = null;
                if (wACQ_ORDERS_HEADS.ShowDialog() == true)
                    LoadData();
            }
        }

        private void cmdEditRows_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            var item = (sender as Button)?.DataContext as acq_orders_heads;

            if (item != null)
            {
                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ACQOrderDetailWindowViewModel>();
                windowViewModel.Head = item;

                var wacq_order_rows = new ACQOrderDetailWindow(windowViewModel);
                wacq_order_rows.Owner = Window.GetWindow(this);
                Mouse.OverrideCursor = null;
                if (wacq_order_rows.ShowDialog() == true)
                    LoadData();
            }
        }

        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            var item = (sender as Button)?.DataContext as acq_orders_heads;

            if (item != null)
            {
                if (item.CanDelete)
                {
                    if (ConfirmHandler.Confirm($"Confermate di voler eliminare l'ordine di acquisto n. {item.id} ?\nNon sara' possibile recuperare le informazioni eliminate, procedere ?"))
                    {
                        // ask for reason
                        var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<CancelReasonWindowViewModel>();
                        var wAskCR = new CancelReasonWindow(windowViewModel);
                        wAskCR.Owner = Window.GetWindow(this);
                        Mouse.OverrideCursor = null;
                        if (wAskCR.ShowDialog() == true && !string.IsNullOrWhiteSpace(windowViewModel.SelectedReason))
                        {
                            Mouse.OverrideCursor = Cursors.Wait;
                            item.canceled = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
                            item.canceledUserID = _dataContext.UserID;
                            item.canceledNote = windowViewModel.SelectedReason;

                            _dataContext.Update(item);
                            LoadData();
                        }
                    }
                    Mouse.OverrideCursor = null;
                }
                else
                {
                    Mouse.OverrideCursor = null;
                    ErrorHandler.Validation("Impossibile eliminare un'ordine di acquisto chiuso, inviato o gia' annullato");
                }
            }
        }

        private void cmdInsert_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

            var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ACQOrderHeadWindowViewModel>();
            windowViewModel.Data = new acq_orders_heads()
            {
                company_id = _dataContext.CompanyID,
                order_date = now.Date,
                addedUserID = _dataContext.UserID,

            };
            windowViewModel.IsInsert = true;

            var wACQ_ORDERS_HEADS = new ACQOrderHeadWindow(windowViewModel);
            wACQ_ORDERS_HEADS.Owner = Window.GetWindow(this);
            Mouse.OverrideCursor = null;
            if (wACQ_ORDERS_HEADS.ShowDialog() == true)
            {
                var head = _dataContext.GetHead(windowViewModel.Data.id);

                if (head != null)
                {
                    var windowDetailViewModel = VulpesServiceProvider.Provider.GetRequiredService<ACQOrderDetailWindowViewModel>();
                    windowDetailViewModel.Head = head;

                    var wacq_order_rows = new ACQOrderDetailWindow(windowDetailViewModel);
                    wacq_order_rows.Owner = Window.GetWindow(this);
                    wacq_order_rows.ShowDialog();
                }

                LoadData();
            }
        }
        #endregion

        #region UC standard functions
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs? e)
        {
            (RadGridViewCommands.SearchByText as RoutedUICommand)?.Execute(txtSearch.Text, GridView);
        }

        private void cmbStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadData();
        }
        #endregion

        #region Grid icons
        private void rgSendEmail_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as RadGlyph)?.DataContext as acq_orders_heads;

            if (item != null)
            {
                if (item.CanSend)
                {
                    Mouse.OverrideCursor = Cursors.Wait;

                    var validated = _dataContext.Validate(item);
                    if (string.IsNullOrWhiteSpace(validated))
                    {
                        var confirmSend = ConfirmHandler.Confirm("Inviare effettivamente l'ordine tramite email ?\nRispondendo [No] l'ordine verrà segnato come inviato senza inviare una email.");
                        if (confirmSend)
                        {
                            item = _dataContext.GetPrintFull(item.id);
                            if (item != null)
                            {
                                var reportData = _dataContext.PrintPurchaseOrder(item);

                                if (reportData != null)
                                {
                                    var pdf = ReportingHandler.PrintBookPDF(UserContext.Instance!.Domain!, Constants.MODULE_SRM, new string[] { Constants.REPORT_TYPE_BUY_ORDER }, _dataContext.CompanyID, reportData, $"Ordine di acquisto n.{item.id}", item.PrintFilename, false, !item.CanPrint);
                                    if (!string.IsNullOrWhiteSpace(pdf))
                                    {
                                        var atts = new ObservableCollection<string>() { pdf };
                                        if (item.Attachments != null && item.Attachments.Count > 0)
                                        {
                                            foreach (var att in item.Attachments)
                                            {
                                                string path = $"{Path.GetTempPath()}{att.document_name}";
                                                var bytes = StorageHelper.Download(StorageHelper.VULPESX_DATA_CONTAINER, $"{_dataContext.CompanyUID}/{StorageHelper.BUY_ATTACHMENTS_FOLDER}{att.document_id}");
                                                if (bytes != null)
                                                    File.WriteAllBytes(path, bytes);
                                                atts.Add(path);
                                            }
                                        }

                                        var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<SendMailWindowViewModel>();
                                        windowViewModel.SendClass = NotifierHelper.SendClasses.SRM_Purchase_Orders;
                                        windowViewModel.Attachments = atts;
                                        windowViewModel.SupplierID = item.supplier_id;
                                        windowViewModel.DocumentID = item.id;
                                        windowViewModel.OriginalFilename = $"{Path.GetFileNameWithoutExtension(item.PrintFilename)}.zip";
                                        windowViewModel.Language = item.Language;

                                        var wSendEmail = new SendMailWindow(windowViewModel);
                                        wSendEmail.Owner = Window.GetWindow(this);
                                        if (wSendEmail.ShowDialog() == true)
                                        {
                                            // update order
                                            item.send_user = _dataContext.UserID;
                                            item.sent = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

                                            _dataContext.Update(item);
                                        }
                                    }
                                }
                                else
                                {
                                    ErrorHandler.Validation($"Impossibile trovare l'ordine di acquisto n.{item.id}");
                                }
                            }
                            else
                            {
                                ErrorHandler.Validation($"Impossibile trovare l'ordine di acquisto");
                            }
                        }
                        else if (!confirmSend)
                        {
                            // flag as sent only
                            item.send_user = _dataContext.UserID;
                            item.sent = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

                            _dataContext.Update(item);
                        }
                    }
                    else
                    {
                        ErrorHandler.Validation(validated);
                    }

                    Mouse.OverrideCursor = null;
                    LoadData();
                }
                else
                { ErrorHandler.Validation("Impossibile inviare un'ordine di acquisto non completamente firmato"); }
            }
            else
            { ErrorHandler.Validation("Impossibile trovare un'ordine di acquisto"); }
        }

        private void rgPrint_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as RadGlyph)?.DataContext as acq_orders_heads;

            Mouse.OverrideCursor = Cursors.Wait;

            if (item != null)
            {
                item = _dataContext.GetPrintFull(item.id);
                if (item != null)
                {
                    var reportData = _dataContext.PrintPurchaseOrder(item);

                    if (reportData != null)
                        ReportingHandler.PrintBookPDF(UserContext.Instance!.Domain!, Constants.MODULE_SRM, new string[] { Constants.REPORT_TYPE_BUY_ORDER }, _dataContext.CompanyID, reportData, $"Ordine di acquisto n.{item.id}", item.PrintFilename, true, !item.CanPrint);
                    else
                        ErrorHandler.Validation($"Impossibile trovare l'ordine di acquisto n.{item.id}");
                }
                else
                {
                    ErrorHandler.Validation($"Impossibile trovare l'ordine di acquisto");
                }
            }
        }

        private void rgSign_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as RadGlyph)?.DataContext as acq_orders_heads;

            if (item != null)
            {
                item = _dataContext.GetFull(item.id);

                if (item != null)
                {
                    if (item.supplier_id > 0 && !string.IsNullOrWhiteSpace(item.payment_id) && item.Rows != null && item.Rows.Count > 0 && item.TotalAmount > 0)
                    {
                        if (!item.commercial_signed.HasValue && string.IsNullOrWhiteSpace(item.commercial_signer) &&
                            !item.management_signed.HasValue && string.IsNullOrWhiteSpace(item.management_signer))
                        {
                            // commercial sign
                            if (UserContext.Instance!.ACCESS!.Roles?.canPOSignCommercial ?? false)
                            {
                                if (ConfirmHandler.Confirm($"Apporre la firma commerciale sull'ordine di acquisto n.{item.id} ?"))
                                {
                                    item.commercial_signed = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
                                    item.commercial_signer = _dataContext.UserID;
                                    _dataContext.Update(item);
                                }
                            }
                            else
                            {
                                ErrorHandler.Validation("Il tuo utente non ha l'autorizzazione necessaria per apporre la firma commerciale");
                            }
                        }
                        else
                        {
                            if (item.commercial_signed.HasValue && !string.IsNullOrWhiteSpace(item.commercial_signer) &&
                                !item.management_signed.HasValue && string.IsNullOrWhiteSpace(item.management_signer))
                            {
                                // management sign
                                if (UserContext.Instance!.ACCESS!.Roles?.canPOSignManagement ?? false)
                                {
                                    if (ConfirmHandler.Confirm($"Apporre la firma direzionale sull'ordine di acquisto n.{item.id} ?"))
                                    {
                                        item.management_signed = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
                                        item.management_signer = _dataContext.UserID;
                                        _dataContext.Update(item);
                                    }
                                }
                                else
                                {
                                    ErrorHandler.Validation("Il tuo utente non ha l'autorizzazione necessaria per apporre la firma direzionale");
                                }
                            }
                        }
                    }
                    else
                    {
                        ErrorHandler.Validation("Impossibile firmare un ordine non completo: verificare fornitore, pagamento e dettagli");
                    }
                }
                LoadData();
            }
        }
        #endregion

        private void cmdClose_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var item = (sender as Button)?.DataContext as acq_orders_heads;

            if (item != null)
            {
                if (item.CanClose)
                {
                    if (ConfirmHandler.Confirm($"Confermate di voler chiudere l'ordine di acquisto n. {item.id} ?\nNon sara' possibile riaprire, procedere ?"))
                    {
                        Mouse.OverrideCursor = Cursors.Wait;
                        item.closed = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
                        item.canceledNote = "CHIUSURA FORZATA";

                        _dataContext.Update(item);
                        Mouse.OverrideCursor = null;

                        LoadData();
                    }
                }
                else
                {
                    Mouse.OverrideCursor = null;
                    ErrorHandler.Validation("Impossibile eliminare un'ordine di acquisto chiuso, inviato o gia' annullato");
                }
            }
        }
    }
}
