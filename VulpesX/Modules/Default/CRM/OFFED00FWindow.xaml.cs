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
    /// Interaction logic for OFFED00FWindow.xaml
    /// </summary>
    public partial class OFFED00FWindow : FluentDefaultWindow
    {
        private OFFED00FWindowViewModel _dataContext;

        public OFFED00FWindow(OFFED00FWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.Height = (System.Windows.SystemParameters.PrimaryScreenHeight - 200);
            this.Width = (System.Windows.SystemParameters.PrimaryScreenWidth);

            this.DataContext = _dataContext;

            _dataContext.LoadDetails();

            CalloutPopupService.CloseAll();

            _dataContext.HasMergedSigns = (UserContext.Instance!.ACCESS!.Roles?.canOffersSignTech ?? false) && (UserContext.Instance!.ACCESS!.Roles?.canOffersSignCommercial ?? false);
            _dataContext.Rows = _dataContext.Head.Rows ?? new ObservableCollection<OFFED00F>();

            // get default agents
            var customerData = _dataContext.GetCLIAMMI(_dataContext.Head.OFTCOCL ?? 0);
            if (!_dataContext.Head.OFTDEST.HasValue || _dataContext.Head.OFTDEST.Value == 0)
            {
                // defaults from customer
                _dataContext.Head.DefaultFirstAgent = new AGENTI() { agecod = customerData?.CLAGEN ?? string.Empty, agepvg = customerData?.CLAGENP, agepvgt = customerData?.CLAGENPT, agedes = string.Empty, ageflag = string.Empty, };
                _dataContext.Head.DefaultSecondAgent = new AGENTI() { agecod = customerData?.CLAGEN2 ?? string.Empty, agepvg = customerData?.CLAGEN2P, agepvgt = customerData?.CLAGEN2PT, agedes = string.Empty, ageflag = string.Empty };
            }
            else
            {
                var recipient = _dataContext.GetDESTINATARI(_dataContext.Head.OFTCOCL!.Value, _dataContext.Head.OFTDEST.Value);
                _dataContext.Head.DefaultFirstAgent = !string.IsNullOrWhiteSpace(recipient?.decoag1) ?
                    new AGENTI() { agecod = recipient?.decoag1 ?? string.Empty, agepvg = recipient?.deage1p, agepvgt = recipient?.deage1pt, agedes = string.Empty, ageflag = string.Empty } :
                    new AGENTI() { agecod = customerData?.CLAGEN ?? string.Empty, agepvg = customerData?.CLAGENP, agepvgt = customerData?.CLAGENPT, agedes = string.Empty, ageflag = string.Empty };
                _dataContext.Head.DefaultSecondAgent = !string.IsNullOrWhiteSpace(recipient?.decoag2) ?
                    new AGENTI() { agecod = recipient?.decoag2 ?? string.Empty, agepvg = recipient?.deage2p, agepvgt = recipient?.deage2pt, agedes = string.Empty, ageflag = string.Empty } :
                    new AGENTI() { agecod = customerData?.CLAGEN2 ?? string.Empty, agepvg = customerData?.CLAGEN2P, agepvgt = customerData?.CLAGEN2PT, agedes = string.Empty, ageflag = string.Empty };
            }

            // replace commissions and type with defaults
            foreach (var age in (_dataContext.AgentsList ?? new ObservableCollection<AGENTI>()).Where(o => o.agecod == _dataContext.Head.DefaultFirstAgent?.agecod || o.agecod == _dataContext.Head.DefaultSecondAgent?.agecod))
            {
                if (age.agecod == _dataContext.Head.DefaultFirstAgent?.agecod)
                {
                    age.agepvg = _dataContext.Head.DefaultFirstAgent?.agepvg;
                    age.agepvgt = _dataContext.Head.DefaultFirstAgent?.agepvgt;
                }
                if (age.agecod == _dataContext.Head.DefaultSecondAgent?.agecod)
                {
                    age.agepvg = _dataContext.Head.DefaultSecondAgent?.agepvg;
                    age.agepvgt = _dataContext.Head.DefaultSecondAgent?.agepvgt;
                }
            }


            //if (HeadModel.IsInsert)
            //    HeadModel.Data.Customer = HeadModel.SelectedCustomer;

            Loaded += (sender, e) =>
            {
                this.Title = $"Dettagli dell'offerta {_dataContext.Head.PrintFullID} - del {_dataContext.Head.OFTDAOR?.ToString("d")} - {_dataContext.Head.Customer?.FullDescriptionSearchable}";
                if (_dataContext.IsReadonly)
                    Title += " - [sola lettura]";
                MoveFocus(new TraversalRequest(System.Windows.Input.FocusNavigationDirection.First));
            };
        }


        #region Buttons
        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            rgvRows.CommitEdit();

            string? validated = _dataContext.ValidateModel();

            if (string.IsNullOrEmpty(validated))
            {
                // check if already signed
                bool proceed = true;

                if ((_dataContext.Head.OFTFICO.HasValue || _dataContext.Head.OFTFITE.HasValue) && !_dataContext.HasMergedSigns)
                {
                    if (ConfirmHandler.Confirm("Questa offerta ha già delle firme apposte, proseguendo verranno annullate e bisognerà richiederle nuovamente, procedere ?"))
                    {
                        // clear signs
                        _dataContext.Head.OFTFITE = null;
                        _dataContext.Head.OFTFITEUSR = null;
                        _dataContext.Head.OFTFICO = null;
                        _dataContext.Head.OFTFICOUSR = null;
                        _dataContext.Head.OFTINFI = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
                        _dataContext.Head.OFTINFIUSR = _dataContext.UserID;
                    }
                    else
                    {
                        proceed = false;
                    }
                }
                else
                {
                    if (_dataContext.HasMergedSigns)
                    {
                        // all signs merged
                        var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
                        _dataContext.Head.OFTINFI = now;
                        _dataContext.Head.OFTINFIUSR = _dataContext.UserID;
                        _dataContext.Head.OFTFITE = now;
                        _dataContext.Head.OFTFITEUSR = _dataContext.UserID;
                        _dataContext.Head.OFTFICO = now;
                        _dataContext.Head.OFTFICOUSR = _dataContext.UserID;
                        _dataContext.Head.oflgchi = "F";
                    }
                }

                if (proceed)
                {
                    if (_dataContext.UpdateAll())
                        this.DialogResult = true;
                }
            }
            else
            {
                ErrorHandler.Validation(validated);
            }
        }
        #endregion

        #region Rows grid
        private void rgvRows_AddingNewDataItem(object sender, Telerik.Windows.Controls.GridView.GridViewAddingNewEventArgs e)
        {
            var items = rgvRows.Items.Cast<OFFED00F>();
            var customerData = _dataContext.GetCLIENTI(_dataContext.Head.OFTCOCL ?? 0);

            var data = new OFFED00F
            {
                oftsoci = _dataContext.Head.oftsoci,
                OFDRIGA = items.Any() ? items.Max(max => max.OFDRIGA) + 1 : 1,
                OFTANNO = _dataContext.Head.OFTANNO,
                OFTNUOR = _dataContext.Head.OFTNUOR,
                OFDDARI = _dataContext.Head.OFTDARI,
                OFDDACO = _dataContext.Head.OFTDACO,
                OFDQTAV = 0,
                OFDTQTA = "V",
                OFDPREZ = 0,
                OFDTPRE = "U",
                OFDSHOW = false,
                AllAccounts = _dataContext.AllAccounts,
                AllSubccounts = _dataContext.AllSubccounts,
                GroupsList = _dataContext.GroupsList,
                UMsCache = _dataContext.UMsCache,
                Products = _dataContext.Products,
                RatesList = _dataContext.RatesList,
                AgentsList = _dataContext.AgentsList,
                CustomerID = _dataContext.Head.OFTCOCL ?? 0,
                RecipientID = _dataContext.Head.OFTDEST ?? 0,
                OfferDate = _dataContext.Head.OFTDAOR!.Value,
                OFDCODA = string.Empty,
            };

            data.QuantityValueChanged += _dataContext.OnQuantityValueChanged;

            if (_dataContext.Head.DefaultFirstAgent != null)
                data.SelectedFirstAgent = data.AgentsList?.Where(w => w.agecod == _dataContext.Head.DefaultFirstAgent.agecod).FirstOrDefault();
            if (_dataContext.Head.DefaultSecondAgent != null)
                data.SelectedSecondAgent = data.AgentsList?.Where(w => w.agecod == _dataContext.Head.DefaultSecondAgent.agecod).FirstOrDefault();
            if (customerData != null)
                data.SelectedRate = _dataContext.RatesList?.Where(w => w.asscod == customerData.classo && w.assali == customerData.classa).FirstOrDefault();

            e.NewObject = data;
            var grid = e.OwnerGridViewItemsControl;
            grid.CurrentColumn = grid.Columns[1];
        }

        private void rgvRows_RowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            if (e.EditOperationType != Telerik.Windows.Controls.GridView.GridViewEditOperationType.None)
            {
                var item = e.Row.Item as OFFED00F;

                if (item != null)
                {
                    var validated = _dataContext.Validate(item);
                    if (validated != null)
                        ErrorHandler.Validation(validated);
                    e.IsValid = string.IsNullOrWhiteSpace(validated);
                }
                else
                {
                    e.IsValid = false;
                }
            }
            else
            {
                e.IsValid = true;
            }
        }

        private void rgvRows_RowEditEnded(object sender, GridViewRowEditEndedEventArgs e)
        {
            _dataContext.ViewUpdate = true;
            rgvRows.ScrollIntoView(e.Row.Item, rgvRows.Columns[0]);
        }

        private void rgvRows_Deleted(object sender, GridViewDeletedEventArgs e)
        {
            _dataContext.ViewUpdate = true;
        }

        private void rgvRows_RowLoaded(object sender, RowLoadedEventArgs e)
        {
            if (e.Row.DataContext is OFFED00F)
            {
                var item = e.Row.DataContext as OFFED00F;

                if (item != null)
                {
                    if (item.IsReadOnlyRow)
                        e.Row.ToolTip = $"Questa riga e' gia' stata trasformata in ordine cliente ({item.ofdanf}/{item.ofdnuf}/{item.ofdrif}) il {item.transformed?.ToString("dd/MM/yyyy HH:mm:ss")} da {item.transform_user} e non e' piu' modificabile.";
                }
            }
        }

        private void rgvRows_BeginningEdit(object sender, GridViewBeginningEditRoutedEventArgs e)
        {
            var model = e.Row.Item as OFFED00F;

            if (model != null)
            {
                if (model.CanEdit)
                {
                    e.Handled = true;
                }
                else
                {
                    e.Cancel = true;
                }
            }
            else
            {
                e.Cancel = true;
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

        #region Magnifier
        private void rgMagnifier_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as RadGlyph)?.DataContext as OFFED00F;

            if (item != null && !item.IsReadOnlyRow)
            {
                var clonedItem = new OFFED00F()
                {
                    oftsoci = item.oftsoci,
                    OFTANNO = item.OFTANNO,
                    OFTNUOR = item.OFTNUOR,
                    OFDCODA = item.OFDCODA,
                    OFDQTAV = item.OFDQTAV,
                    OFDTQTA = item.OFDTQTA,
                    OFDPREZ = item.OFDPREZ,
                    OFDSCO1 = item.OFDSCO1,
                    OFDSCO2 = item.OFDSCO2,
                    OFDSCO3 = item.OFDSCO3,
                    OFDMAGG = item.OFDMAGG,
                    OFDTPRE = item.OFDTPRE,
                    OFDTSC1 = item.OFDTSC1,
                    OFDTSC2 = item.OFDTSC2,
                    OFDTSC3 = item.OFDTSC3,
                    OFDTMAG = item.OFDTMAG,
                    OFDALIV = item.OFDALIV,
                    OFDASSF = item.OFDASSF,
                    OFDDACO = item.OFDDACO,
                    OFDRIFC = item.OFDRIFC,
                    OFDGRUP = item.OFDGRUP,
                    OFDCONT = item.OFDCONT,
                    OFDSCTO = item.OFDSCTO,
                    ofdunim = item.ofdunim,
                    OFDDARI = item.OFDDARI,
                    OFDNOTE = item.OFDNOTE,
                    OFDSHOW = item.OFDSHOW,
                    OFDSTA = item.OFDSTA,
                    OFDCOA1 = item.OFDCOA1,
                    OFDCOA2 = item.OFDCOA2,
                    OFDCOA1P = item.OFDCOA1P,
                    OFDCOA2P = item.OFDCOA2P,
                    OFDCOA1PT = item.OFDCOA1PT,
                    OFDCOA2PT = item.OFDCOA2PT,
                    OFDQTAEV = item.OFDQTAEV,
                    IsHeadReadonly = item.IsHeadReadonly,
                    CustomerID = _dataContext.Head.OFTCOCL ?? 0,
                    RecipientID = _dataContext.Head.OFTDEST ?? 0,
                    OfferDate = _dataContext.Head.OFTDAOR!.Value
                };
                clonedItem.UMsCache = item.UMsCache;
                clonedItem.RatesList = item.RatesList;
                clonedItem.Products = item.Products;
                clonedItem.AllAccounts = item.AllAccounts;
                clonedItem.AllSubccounts = item.AllSubccounts;
                clonedItem.GroupsList = item.GroupsList;
                clonedItem.AccountsList = item.AccountsList;
                clonedItem.SubaccountsList = item.SubaccountsList;
                clonedItem.AgentsList = item.AgentsList;
                clonedItem.QuantityValueChanged += _dataContext.OnQuantityValueChanged;

                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<OFFED00FMagnifierWindowViewModel>();
                windowViewModel.Title = "Dettagli della riga d'offerta";
                windowViewModel.IsReadonly = _dataContext.IsReadonly;
                windowViewModel.Row = clonedItem;

                var wMagnifier = new OFFED00FMagnifierWindow(windowViewModel);
                wMagnifier.Owner = Window.GetWindow(this);
                if (wMagnifier.ShowDialog() == true)
                {
                    // update row data
                    item.SelectedProduct = windowViewModel.Row.SelectedProduct;
                    item.OFDQTAV = windowViewModel.Row.OFDQTAV;
                    item.OFDTQTA = windowViewModel.Row.OFDTQTA;
                    item.OFDPREZ = windowViewModel.Row.OFDPREZ;
                    item.OFDSCO1 = windowViewModel.Row.OFDSCO1;
                    item.OFDSCO2 = windowViewModel.Row.OFDSCO2;
                    item.OFDSCO3 = windowViewModel.Row.OFDSCO3;
                    item.OFDMAGG = windowViewModel.Row.OFDMAGG;
                    item.OFDTPRE = windowViewModel.Row.OFDTPRE;
                    item.OFDTSC1 = windowViewModel.Row.OFDTSC1;
                    item.OFDTSC2 = windowViewModel.Row.OFDTSC2;
                    item.OFDTSC3 = windowViewModel.Row.OFDTSC3;
                    item.OFDTMAG = windowViewModel.Row.OFDTMAG;
                    item.SelectedRate = windowViewModel.Row.SelectedRate;
                    item.OFDASSF = windowViewModel.Row.OFDASSF;
                    item.OFDDACO = windowViewModel.Row.OFDDACO;
                    item.OFDRIFC = windowViewModel.Row.OFDRIFC;
                    item.SelectedGroup = windowViewModel.Row.SelectedGroup;
                    item.SelectedAccount = windowViewModel.Row.SelectedAccount;
                    item.SelectedSubaccount = windowViewModel.Row.SelectedSubaccount;
                    item.ofdunim = windowViewModel.Row.ofdunim;
                    item.OFDDARI = windowViewModel.Row.OFDDARI;
                    item.OFDNOTE = windowViewModel.Row.OFDNOTE;
                    item.OFDSHOW = windowViewModel.Row.OFDSHOW;
                    item.OFDSTA = windowViewModel.Row.OFDSTA;
                    item.SelectedFirstAgent = windowViewModel.Row.SelectedFirstAgent;
                    item.SelectedSecondAgent = windowViewModel.Row.SelectedSecondAgent;
                    item.OFDCOA1P = windowViewModel.Row.OFDCOA1P;
                    item.OFDCOA2P = windowViewModel.Row.OFDCOA2P;
                    item.OFDCOA1PT = windowViewModel.Row.OFDCOA1PT;
                    item.OFDCOA2PT = windowViewModel.Row.OFDCOA2PT;
                    item.OFDQTAEV = windowViewModel.Row.OFDQTAEV;

                    _dataContext.ViewUpdate = true;
                }
            }
        }
        #endregion

    }
}
