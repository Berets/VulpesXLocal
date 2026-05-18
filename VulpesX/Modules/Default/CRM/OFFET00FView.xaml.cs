using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using VulpesX.Modules.Default.Commons;
using VulpesX.Shared;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Commons;
using VulpesX.ViewModels.Modules.Default.CRM;

namespace VulpesX.Modules.Default.CRM
{
    /// <summary>
    /// Interaction logic for OFFET00FView.xaml
    /// </summary>
    public partial class OFFET00FView : UserControl
    {
        private OFFET00FViewModel _dataContext;
        private int _selectedPage;
        public OFFET00FView()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<OFFET00FViewModel>();

            InitializeComponent();

            this.DataContext = _dataContext;

            dtpYear.Culture = new System.Globalization.CultureInfo("it-IT");
            dtpYear.Culture.DateTimeFormat.ShortDatePattern = "yyyy";
            dtpYear.SelectedValue = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

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
            int? year = dtpYear.SelectedValue?.Year;

            if (selectedStatusID != null && year.HasValue)
            {
                await _dataContext.Load(year!.Value, selectedStatusID);
            }
        }

        #region Buttons
        private void cmdEdit_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            var item = (sender as Button)?.DataContext as OFFET00F;

            if (item != null)
            {
                item = _dataContext.GetFull(item.OFTANNO, item.OFTNUOR);

                if (item != null)
                {
                    var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<OFFET00FWindowViewModel>();
                    windowViewModel.Data = item;
                    windowViewModel.IsInsert = false;

                    var wOFFET00F = new OFFET00FWindow(windowViewModel);
                    Mouse.OverrideCursor = null;
                    wOFFET00F.Owner = Window.GetWindow(this);
                    if (wOFFET00F.ShowDialog() == true)
                        LoadData();
                }
            }
        }

        private void cmdEditRows_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            var item = (sender as Button)?.DataContext as OFFET00F;

            if (item != null)
            {
                item = _dataContext.GetFull(item.OFTANNO, item.OFTNUOR);

                if (item != null)
                {
                    var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<OFFED00FWindowViewModel>();
                    windowViewModel.Head = item;

                    var wOFFED00F = new OFFED00FWindow(windowViewModel);
                    wOFFED00F.Owner = Window.GetWindow(this);
                    Mouse.OverrideCursor = null;
                    if (wOFFED00F.ShowDialog() == true)
                        LoadData();
                }
            }
        }

        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var item = (sender as Button)?.DataContext as OFFET00F;

            if (item != null)
            {
                item = _dataContext.GetFull(item.OFTANNO, item.OFTNUOR);

                if (item != null)
                {
                    if (item.CanDelete)
                    {
                        if (ConfirmHandler.Confirm($"Confermate di voler eliminare l'offerta provvisoria n. {item.PrintFullID} ?\nNon sara' possibile recuperare le informazioni eliminate, procedere ?"))
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
                                item.oflgchi = "X";

                                _dataContext.Update(item);
                                LoadData();
                            }
                        }
                        Mouse.OverrideCursor = null;
                    }
                    else
                    {
                        Mouse.OverrideCursor = null;
                        ErrorHandler.Validation("Impossibile eliminare un'offerta chiusa, con delle righe parzialmente trasformate in ordine e/o chiuse o gia' annullata");
                    }
                }
            }
        }

        private void cmdInsert_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            int year = dtpYear.SelectedValue?.Year ?? 0;

            var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<OFFET00FWindowViewModel>();
            windowViewModel.Data = new OFFET00F()
            {
                oftsoci = _dataContext.CompanyID,
                OFTANNO = year,
                oflgchi = "A",
                OFTDAOR = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime(),
                oftdivi = "EUR",
                OFTCIDI = "UIC",
                OFTLING = "IT",
                addedUserID = _dataContext.UserID,
                OFTOGG = string.Empty,
            };
            windowViewModel.IsInsert = true;

            var wOFFET00F = new OFFET00FWindow(windowViewModel);
            Mouse.OverrideCursor = null;
            wOFFET00F.Owner = Window.GetWindow(this);
            if (wOFFET00F.ShowDialog() == true)
            {
                var head = _dataContext.GetFull(windowViewModel.Data.OFTANNO, windowViewModel.Data.OFTNUOR);

                if (head != null)
                {
                    var windowDetailViewModel = VulpesServiceProvider.Provider.GetRequiredService<OFFED00FWindowViewModel>();
                    windowDetailViewModel.Head = head;

                    var wOFFED00F = new OFFED00FWindow(windowDetailViewModel);
                    wOFFED00F.Owner = Window.GetWindow(this);
                    wOFFED00F.ShowDialog();
                    LoadData();
                }
            }
        }
        #endregion

        #region UC standard functions
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs? e)
        {
            (RadGridViewCommands.SearchByText as RoutedUICommand)?.Execute(txtSearch.Text, GridView);
        }

        private void dtpYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadData();
        }

        private void cmbStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadData();
        }
        #endregion

        #region Grid icons events
        private void rgOfferAttachmentsIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            var row = (sender as RadGlyph)?.DataContext as OFFET00F;

            if (row != null)
            {
                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<OFFETAL00FWindowViewModel>();
                windowViewModel.Head = row;

                var wOFFETAL00F = new OFFETAL00FWindow(windowViewModel);
                wOFFETAL00F.Owner = Window.GetWindow(this);
                Mouse.OverrideCursor = null;
                if (wOFFETAL00F.ShowDialog() == true)
                    LoadData();
            }
        }

        private void rgSendEmail_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as RadGlyph)?.DataContext as OFFET00F;
            if (item != null)
            {
                if (item.CanSend)
                {
                    Mouse.OverrideCursor = Cursors.Wait;

                    item = _dataContext.GetPrintFull(item.OFTANNO, item.OFTNUOR);

                    if (item != null)
                    {
                        var reportData = _dataContext.PrintOffer(item);

                        if (reportData != null && reportData.Offer != null)
                        {
                            string? pdf = null;
                            if (ConfirmHandler.Confirm("Stampare anche la cover dell'offerta ?"))
                            {
                                pdf = ReportingHandler.PrintBookPDF(UserContext.Instance!.Domain!, Constants.MODULE_CRM, new string[] { Constants.REPORT_TYPE_OFFER_COVER, Constants.REPORT_TYPE_OFFER }, _dataContext.CompanyID, reportData, $"Offerta n.{item.PrintFullID}", item.PrintFilename, false, !item.CanPrint);
                            }
                            else
                            {
                                pdf = ReportingHandler.PrintPDF(UserContext.Instance!.Domain!, Constants.MODULE_CRM, Constants.REPORT_TYPE_OFFER, _dataContext.CompanyID, reportData, $"Offerta n.{item.PrintFullID}", item.PrintFilename, false)?.FullPath;
                            }

                            if (!string.IsNullOrWhiteSpace(pdf))
                            {
                                var atts = new ObservableCollection<string>() { pdf };
                                if (item.Attachments != null && item.Attachments.Count > 0)
                                {
                                    foreach (var att in item.Attachments)
                                    {
                                        string path = $"{Path.GetTempPath()}{att.OFTANAME}";

                                        var data = StorageHelper.Download(StorageHelper.VULPESX_DATA_CONTAINER, $"{_dataContext.CompanyUID}/{StorageHelper.OFFERS_ATTACHMENTS_FOLDER}{att.OFTAUID}");

                                        if (data != null)
                                        {
                                            File.WriteAllBytes(path, data);
                                            atts.Add(path);
                                        }
                                    }
                                }

                                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<SendMailWindowViewModel>();
                                windowViewModel.SendClass = NotifierHelper.SendClasses.CRM_Offers;
                                windowViewModel.Attachments = atts;
                                windowViewModel.CustomerID = item.OFTCOCL ?? 0;
                                windowViewModel.To = null;
                                windowViewModel.DocumentYear = item.OFTANNO;
                                windowViewModel.DocumentNumber = item.OFTNUOR;
                                windowViewModel.OriginalFilename = $"{Path.GetFileNameWithoutExtension(item.PrintFilename)}.zip";
                                windowViewModel.Language = item.Language;

                                var wSendEmail = new SendMailWindow(windowViewModel);
                                wSendEmail.Owner = Window.GetWindow(this);
                                if (wSendEmail.ShowDialog() == true)
                                {
                                    LoadData();
                                }
                            }
                        }
                        else
                        {
                            ErrorHandler.Validation($"Impossibile trovare l'offerta {item.PrintFullID}");
                        }
                    }
                }
                else
                {
                    ErrorHandler.Validation("Impossibile inviare un'offerta non completamente firmata");
                }
            }
        }

        private void rgPrint_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as RadGlyph)?.DataContext as OFFET00F;

            Mouse.OverrideCursor = Cursors.Wait;
            if (item != null)
            {

                item = _dataContext.GetPrintFull(item.OFTANNO, item.OFTNUOR);

                if (item != null)
                {
                    var reportData = _dataContext.PrintOffer(item);

                    if (reportData != null && reportData.Offer != null)
                    {
                        if (ConfirmHandler.Confirm("Stampare anche la cover dell'offerta ?"))
                        {
                            reportData.Offer.PrintObject = true;
                            ReportingHandler.PrintBookPDF(UserContext.Instance!.Domain!, Constants.MODULE_CRM, new string[] { Constants.REPORT_TYPE_OFFER_COVER, Constants.REPORT_TYPE_OFFER }, _dataContext.CompanyID, reportData, $"Offerta n.{item.PrintFullID}", item.PrintFilename, true, !item.CanPrint);
                        }
                        else
                        {
                            reportData.Offer.PrintObject = false;
                            ReportingHandler.PrintPDF(UserContext.Instance!.Domain!, Constants.MODULE_CRM, Constants.REPORT_TYPE_OFFER, _dataContext.CompanyID, reportData, $"Offerta n.{item.PrintFullID}", item.PrintFilename, true);
                        }
                    }
                    else
                    {
                        ErrorHandler.Validation($"Impossibile trovare l'offerta {item.PrintFullID}");
                    }
                }
                else
                {
                    ErrorHandler.Validation($"Impossibile trovare l'offerta ");
                }
            }
        }

        private void rgSign_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as RadGlyph)?.DataContext as OFFET00F;

            if (item != null)
            {
                item = _dataContext.GetFull(item.OFTANNO, item.OFTNUOR);

                if (item != null)
                {
                    if (!item.OFTINFI.HasValue && string.IsNullOrWhiteSpace(item.OFTINFIUSR) &&
                    !item.OFTFITE.HasValue && string.IsNullOrWhiteSpace(item.OFTFITEUSR) &&
                    !item.OFTFICO.HasValue && string.IsNullOrWhiteSpace(item.OFTFICOUSR))
                    {
                        // send to sign
                        if ((UserContext.Instance!.ACCESS!.Roles?.canOffersSignTech ?? false) && !(UserContext.Instance!.ACCESS!.Roles?.canOffersSignCommercial ?? false))
                        {
                            // send to sign and technical sign
                            if (ConfirmHandler.Confirm($"Inviando l'offerta {item.PrintFullID} alla firma tecnica non sara' piu' possibile modificarla. Inoltre il tuo utente apporra' automaticamente anche la firma tecnica, proseguire ?"))
                            {
                                var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
                                item.OFTINFI = now;
                                item.OFTINFIUSR = _dataContext.UserID;
                                item.OFTFITE = now;
                                item.OFTFITEUSR = _dataContext.UserID;
                                item.oflgchi = "T";

                                _dataContext.Update(item);
                            }
                        }
                        else
                        {
                            // only send to sign
                            if (ConfirmHandler.Confirm($"Inviando l'offerta {item.PrintFullID} alla firma tecnica non sara' piu' possibile modificarla, proseguire ?"))
                            {
                                item.OFTINFI = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
                                item.OFTINFIUSR = _dataContext.UserID;
                                item.oflgchi = "I";

                                _dataContext.Update(item);
                            }
                        }
                    }
                    else
                    {
                        if (item.OFTINFI.HasValue && !string.IsNullOrWhiteSpace(item.OFTINFIUSR) &&
                            !item.OFTFITE.HasValue && string.IsNullOrWhiteSpace(item.OFTFITEUSR) &&
                            !item.OFTFICO.HasValue && string.IsNullOrWhiteSpace(item.OFTFICOUSR))
                        {
                            // technical sign
                            if ((UserContext.Instance!.ACCESS!.Roles?.canOffersSignTech ?? false))
                            {
                                if (ConfirmHandler.Confirm($"Apporre la firma tecnica sull'offerta {item.PrintFullID} ?"))
                                {
                                    item.OFTFITE = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
                                    item.OFTFITEUSR = _dataContext.UserID;
                                    item.oflgchi = "T";

                                    _dataContext.Update(item);
                                }
                            }
                            else
                            {
                                ErrorHandler.Validation("Il tuo utente non ha l'autorizzazione necessaria per apporre la firma tecnica");
                            }
                        }
                        else
                        {
                            if (item.OFTINFI.HasValue && !string.IsNullOrWhiteSpace(item.OFTINFIUSR) &&
                                item.OFTFITE.HasValue && !string.IsNullOrWhiteSpace(item.OFTFITEUSR) &&
                                !item.OFTFICO.HasValue && string.IsNullOrWhiteSpace(item.OFTFICOUSR))
                            {
                                // commercial sign
                                if ((UserContext.Instance!.ACCESS!.Roles?.canOffersSignCommercial ?? false))
                                {
                                    if (ConfirmHandler.Confirm($"Apporre la firma commerciale sull'offerta {item.PrintFullID} ?"))
                                    {
                                        item.OFTFICO = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
                                        item.OFTFICOUSR = _dataContext.UserID;
                                        item.oflgchi = "F";

                                        _dataContext.Update(item);
                                    }
                                }
                                else
                                {
                                    ErrorHandler.Validation("Il tuo utente non ha l'autorizzazione necessaria per apporre la firma commerciale");
                                }
                            }
                        }
                    }
                }
            }
            LoadData();
        }
        #endregion

        #region Context menu
        private void cmGrid_Opening(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            if (UserContext.Instance!.ACCESS!.Roles?.canTransformOffers ?? false)
            {
                var selected = GridView.SelectedItems.Cast<OFFET00F>().ToList() ?? new List<OFFET00F>();

                if (selected.Any(any => any.Customer?.abecfe == "P"))
                {
                    rmiTransform.IsEnabled = false;
                    rmiTransform.ToolTip = "Non e' possibile trasformare in ordine un'offerta a prospect, occorre prima promuoverlo a client";
                }
                else
                {
                    bool canTransform = true;

                    if (GridView.SelectedItems == null || GridView.SelectedItems.Count < 1)
                    {
                        canTransform = false;
                    }
                    else
                    {
                        if (selected.GroupBy(g => new { g.OFTCOCL, g.OFTDEST }).Count() > 1)
                            canTransform = false;
                    }
                    if (canTransform)
                    {
                        foreach (var item in selected)
                        {
                            if (item.canceled != null || !item.OFTINFI.HasValue || !item.OFTFICO.HasValue || !item.OFTFITE.HasValue || item.oflgchi != "F")
                            {
                                canTransform = false;
                                break;
                            }
                        }
                    }

                    if (canTransform)
                    {
                        rmiTransform.IsEnabled = true;
                    }
                    else
                    {
                        rmiTransform.IsEnabled = false;
                        rmiTransform.ToolTip = "Avete selezionato delle offerte annullate, non firmate o per clienti/destinazioni differenti";
                    }
                }
            }
            else
            {
                rmiTransform.IsEnabled = false;
                rmiTransform.ToolTip = "Non si possiede l'abilitazione per trasformare le offerte in ordini clienti";
            }

            if (GridView.SelectedItems == null || GridView.SelectedItems.Count < 1)
            {
                rmiClose.IsEnabled = false;
            }
            else
            {
                rmiClose.IsEnabled = GridView.SelectedItems.Cast<OFFET00F>().All(all => all.oflgchi == "F");
            }
        }

        private void rmiClose_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            if (GridView.SelectedItems != null && GridView.SelectedItems.Count > 0)
            {
                if (ConfirmHandler.Confirm("Procedere alla chiusura dalle offerte selezionate ?"))
                {
                    // ask causal
                    var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<OFFET00FCloseReasonWindowViewModel>();

                    var wAskCausal = new OFFET00FCloseReasonWindow(windowViewModel);
                    wAskCausal.Owner = Window.GetWindow(this);
                    if (wAskCausal.ShowDialog() == true)
                    {
                        Mouse.OverrideCursor = Cursors.Wait;

                        // close
                        if (_dataContext.CloseOrders(GridView.SelectedItems.Cast<OFFET00F>().ToList(), windowViewModel.SelectedCausal!.id, (!string.IsNullOrWhiteSpace(windowViewModel.SelectedNote) ? windowViewModel.SelectedNote : string.Empty)))
                        {
                            Mouse.OverrideCursor = null;
                            InfoHandler.Show("Offerte chiuse correttamente");
                            LoadData();
                        }
                    }
                    Mouse.OverrideCursor = null;
                }
            }
        }

        private void rmiTransform_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            if (GridView.SelectedItems != null && GridView.SelectedItems.Count > 0)
            {
                if (ConfirmHandler.Confirm("Procedere alla composizione dell'ordine cliente partendo dalle offerte selezionate ?"))
                {
                    var selectedRows = new List<OFFED00F>();
                    var selectedAttachments = new List<OFFETAL00F>();
                    var offers = new List<OFFET00F>();
                    foreach (var item in GridView.SelectedItems.Cast<OFFET00F>())
                    {
                        var full = _dataContext.GetFull(item.OFTANNO, item.OFTNUOR);
                        if (full != null)
                        {
                            offers.Add(full);
                            selectedRows.AddRange(full.Rows?.Where(w => !w.transformed.HasValue).ToList() ?? new());
                            selectedAttachments.AddRange(full.Attachments ?? new());
                        }
                    }
                    if (offers.Count > 0)
                    {
                        var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<OFFED00FSelectWindowViewModel>();
                        windowViewModel.OffersHeads = offers;
                        windowViewModel.AvailableRows = new ObservableCollection<OFFED00F>(selectedRows.OrderBy(o => o.OFTNUOR).ThenBy(o => o.OFDRIGA));
                        windowViewModel.AvailableAttachments = new ObservableCollection<OFFETAL00F>(selectedAttachments.OrderBy(o => o.OFTANUOR).ThenBy(o => o.OFTANAME));

                        var wSelection = new OFFED00FSelectWindow(windowViewModel);
                        wSelection.Owner = Window.GetWindow(this);
                        wSelection.ShowDialog();
                        LoadData();
                    }
                }
            }

        }
        #endregion
    }
}
