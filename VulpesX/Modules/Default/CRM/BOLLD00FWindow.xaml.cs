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
using VulpesX.Shared;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.CRM;

namespace VulpesX.Modules.Default.CRM
{
    /// <summary>
    /// Interaction logic for BOLLD00FWindow.xaml
    /// </summary>
    public partial class BOLLD00FWindow : FluentDefaultWindow
    {
        private BOLLD00FWindowViewModel _dataContext;

        private BOLLT00F_history? currentVersion;
        public BOLLD00FWindow(BOLLD00FWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.Height = (System.Windows.SystemParameters.PrimaryScreenHeight - 200);
            this.Width = (System.Windows.SystemParameters.PrimaryScreenWidth);

            this.DataContext = _dataContext;

            CalloutPopupService.CloseAll();

            _dataContext.LoadDetails();

            // replace commissions and type with defaults
            foreach (var age in (_dataContext.Agents ?? new ObservableCollection<AGENTI>()).Where(o => o.agecod == _dataContext.Head.DefaultFirstAgent?.agecod || o.agecod == _dataContext.Head.DefaultSecondAgent?.agecod))
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

            var customization = _dataContext.GetAZIENDA();


            _dataContext.IsReadonly = _dataContext.Head.BTSTATO == "F" || _dataContext.Head.canceled.HasValue || (_dataContext.Head.BTNUBD > 0 && !(customization?.azddtdefic ?? true));
            _dataContext.Rows = _dataContext.Head.Rows ?? new ObservableCollection<BOLLD00F>();

            Loaded += (sender, e) =>
            {
                this.Title = $"Dettagli del DDT {_dataContext.Head.PrintFullID}";
                if (_dataContext.IsReadonly)
                    Title += " - [sola lettura]";

                #region Save current version
                currentVersion = new BOLLT00F_history()
                {
                    bolsoc = _dataContext.Head.bolsoc,
                    BTANNO = _dataContext.Head.BTANNO,
                    BTBOLL = _dataContext.Head.BTBOLL,
                    BTNUBD = _dataContext.Head.BTNUBD,
                    BTDATP = _dataContext.Head.BTDATP,
                    BTDATA = _dataContext.Head.BTDATA,
                    BTCAUS = _dataContext.Head.BTCAUS,
                    BTCODC = _dataContext.Head.BTCODC,
                    BTCODD = _dataContext.Head.BTCODD,
                    BTPAGA = _dataContext.Head.BTPAGA,
                    BTCONS = _dataContext.Head.BTCONS,
                    BTSPED = _dataContext.Head.BTSPED,
                    BTCORR = _dataContext.Head.BTCORR,
                    BTCORR2 = _dataContext.Head.BTCORR2,
                    BTDE25 = _dataContext.Head.BTDE25,
                    BTPESO = _dataContext.Head.BTPESO,
                    BTPES2 = _dataContext.Head.BTPES2,
                    BTDASP = _dataContext.Head.BTDASP,
                    BTCOLL = _dataContext.Head.BTCOLL,
                    BTAREA = _dataContext.Head.BTAREA,
                    BTDEBE = _dataContext.Head.BTDEBE,
                    BTIMBA = _dataContext.Head.BTIMBA,
                    abiabi = _dataContext.Head.abiabi,
                    abicab = _dataContext.Head.abicab,
                    BTLING = _dataContext.Head.BTLING,
                    BTNOTET = _dataContext.Head.BTNOTET,
                    BTNOTEP = _dataContext.Head.BTNOTEP,
                    BTSHOWT = _dataContext.Head.BTSHOWT,
                    BTSHOWP = _dataContext.Head.BTSHOWP,
                    added = _dataContext.Head.added,
                    addedUserID = _dataContext.Head.addedUserID,
                    updated = _dataContext.Head.updated,
                    updatedUserID = _dataContext.Head.updatedUserID,
                    BTBCON = _dataContext.Head.BTBCON,
                    BTCONZ = _dataContext.Head.BTCONZ,
                    BTSTATO = _dataContext.Head.BTSTATO,
                    BTDAFA = _dataContext.Head.BTDAFA,
                    BTFILI = _dataContext.Head.BTFILI,
                    BTSCCL = _dataContext.Head.BTSCCL,
                    BTZONA = _dataContext.Head.BTZONA,
                    BTSETM = _dataContext.Head.BTSETM,
                    BTREGI = _dataContext.Head.BTREGI,
                    BTRIVE = _dataContext.Head.BTRIVE
                };
                currentVersion.Rows = new List<BOLLD00F_history>();
                foreach (var row in _dataContext.Head.Rows ?? new ObservableCollection<BOLLD00F>())
                {
                    var newRow = new BOLLD00F_history()
                    {
                        bolsoc = row.bolsoc,
                        BTANNO = row.BTANNO,
                        BTBOLL = row.BTBOLL,
                        BORIGB = row.BORIGB,
                        BOANNO = row.BOANNO,
                        BONUOR = row.BONUOR,
                        BORIGA = row.BORIGA,
                        BOMAGA = row.BOMAGA,
                        BOCODA = row.BOCODA,
                        BOQTAV = row.BOQTAV,
                        BOTQTA = row.BOTQTA,
                        BOUNIM = row.BOUNIM,
                        BODACO = row.BODACO,
                        BOSERI = row.BOSERI,
                        BORIFC = row.BORIFC,
                        boprez = row.boprez,
                        botpre = row.botpre,
                        boaliq = row.boaliq,
                        boasso = row.boasso,
                        bogrup = row.bogrup,
                        bocont = row.bocont,
                        bosotc = row.bosotc,
                        bosco1 = row.bosco1,
                        bosco2 = row.bosco2,
                        bosco3 = row.bosco3,
                        bomagg = row.bomagg,
                        botsc1 = row.botsc1,
                        botsc2 = row.botsc2,
                        botsc3 = row.botsc3,
                        botmag = row.botmag,
                        BONOTE = row.BONOTE,
                        BOSHOW = row.BOSHOW,
                        BOCOA1 = row.BOCOA1,
                        BOCOA2 = row.BOCOA2,
                        BOCOA1P = row.BOCOA1P,
                        BOCOA2P = row.BOCOA2P,
                        BOCOA1PT = row.BOCOA1PT,
                        BOCOA2PT = row.BOCOA2PT
                    };
                    newRow.Engages = new List<BOLLD00F1_history>();
                    foreach (var eng in row.EngagesRows ?? new ObservableCollection<BOLLD00F1>())
                    {
                        newRow.Engages.Add(new BOLLD00F1_history()
                        {
                            bolsoc = eng.bolsoc,
                            BTANNO = eng.BTANNO,
                            BTBOLL = eng.BTBOLL,
                            BORIGB = eng.BORIGB,
                            boposc = eng.boposc,
                            bolott = eng.bolott,
                            boqtlo = eng.boqtlo,
                            store_id = eng.store_id,
                            product_id = eng.product_id
                        });
                    }
                    currentVersion.Rows.Add(newRow);
                }
                #endregion
                MoveFocus(new TraversalRequest(System.Windows.Input.FocusNavigationDirection.First));
            };
        }

        #region Buttons
        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            rgvRows.CommitEdit();

            string? validated = _dataContext.ValidateModel();

            if (string.IsNullOrEmpty(validated))
            {
                if (_dataContext.Save(currentVersion))
                {
                    Mouse.OverrideCursor = null;
                    this.DialogResult = true;
                }
            }
            else
            {
                Mouse.OverrideCursor = null;
                ErrorHandler.Validation(validated);
            }
        }
        #endregion

        #region Rows grid
        private void rgvRows_AddingNewDataItem(object sender, Telerik.Windows.Controls.GridView.GridViewAddingNewEventArgs e)
        {
            var items = rgvRows.Items.Cast<BOLLD00F>();

            var customerData = _dataContext.GetCLIENTI(_dataContext.Head.BTCODC!.Value);

            var data = new BOLLD00F
            {
                bolsoc = _dataContext.Head.bolsoc,
                BORIGB = items.Any() ? items.Max(max => max.BORIGB) + 1 : 1,
                BTANNO = _dataContext.Head.BTANNO,
                BTBOLL = _dataContext.Head.BTBOLL,
                BOQTAV = 0,
                boprez = 0,
                BOTQTA = "V",
                botpre = "U",
                BOSHOW = false,
                EngagesRows = new ObservableCollection<BOLLD00F1>(),
                AllAccounts = _dataContext.AllAccounts,
                AllSubccounts = _dataContext.AllSubccounts,
                Groups = _dataContext.Groups,
                UMsCache = _dataContext.UMsCache,
                Products = _dataContext.Products,
                Rates = _dataContext.Rates,
                Stores = _dataContext.Stores,
                Agents = _dataContext.Agents,
                CustomerID = _dataContext.Head.BTCODC ?? 0,
                RecipientID = _dataContext.Head.BTCODD ?? 0,
                DDTDate = _dataContext.Head.BTDATP!.Value,
            };

            data.QuantityValueChanged += _dataContext.OnQuantityValueChanged;

            if (_dataContext.Head.DefaultFirstAgent != null)
                data.FirstAgent = data.Agents?.Where(w => w.agecod == _dataContext.Head.DefaultFirstAgent.agecod).FirstOrDefault();
            if (_dataContext.Head.DefaultSecondAgent != null)
                data.SecondAgent = data.Agents?.Where(w => w.agecod == _dataContext.Head.DefaultSecondAgent.agecod).FirstOrDefault();
            if (_dataContext.Head.BTFLCF == "C")
                data.Rate = _dataContext.Rates?.Where(w => w.asscod == customerData?.classo && w.assali == customerData?.classa).FirstOrDefault();

            e.NewObject = data;
            var grid = e.OwnerGridViewItemsControl;
            grid.CurrentColumn = grid.Columns[1];
        }

        private void rgvRows_RowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            if (e.EditOperationType != Telerik.Windows.Controls.GridView.GridViewEditOperationType.None)
            {
                var item = e.Row.Item as BOLLD00F;

                if (item != null)
                {
                    var validated = _dataContext.Validate(item);
                    if (!string.IsNullOrEmpty(validated))
                    {
                        Dispatcher.BeginInvoke(() => { ErrorHandler.Validation(validated); });
                    }
                    e.IsValid = string.IsNullOrWhiteSpace(validated);
                }
            }
            else
            {
                e.IsValid = true;
            }
        }

        private void rgvRows_RowValidated(object sender, GridViewRowValidatedEventArgs e)
        {
            ComputeEngageProgression();
            _dataContext.ViewUpdate = true;
            rgvRows.ScrollIntoView(e.Row.Item, rgvRows.Columns[0]);
            e.Handled = true;
        }

        private void rgvRows_Deleted(object sender, GridViewDeletedEventArgs e)
        {
            _dataContext.ViewUpdate = true;
        }

        private void rgvRows_SelectionChanged(object sender, SelectionChangeEventArgs e)
        {
            ComputeEngageProgression();
        }

        private void ComputeEngageProgression()
        {
            if (rgvRows.SelectedItem != null)
            {
                _dataContext.NeededQuantity = (rgvRows.SelectedItem as BOLLD00F)?.QuantityValue ?? 0;

                if ((rgvRows.SelectedItem as BOLLD00F)?.EngagesRows != null)
                    _dataContext.EngagedQuantity = (rgvRows.SelectedItem as BOLLD00F)?.EngagesRows?.Sum(sum => sum.boqtlo) ?? 0;
                else
                    _dataContext.EngagedQuantity = 0;
            }
            else
            {
                _dataContext.NeededQuantity = 0;
                _dataContext.EngagedQuantity = 0;
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

        #region Engages grid
        private void rgvRowsEngages_RowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            if (e.EditOperationType != Telerik.Windows.Controls.GridView.GridViewEditOperationType.None)
            {
                var item = e.Row.Item as BOLLD00F1;

                if (item != null)
                {
                    if (item.Lot != null)
                    {
                        // compute all rows for same lot quantities already selected
                        decimal sameLotAlreadyUsed = 0;
                        foreach (var row in _dataContext.Rows ?? new())
                        {
                            if (row.EngagesRows != null)
                                sameLotAlreadyUsed += row.EngagesRows.Where(w => w.bolott == item.Lot.lot && w.product_id == item.Lot.product_id && w.store_id == item.Lot.store_id && w.BORIGB != item.BORIGB).Sum(sum => sum.boqtlo);
                        }

                        var selected = (rgvRows.SelectedItem as BOLLD00F);

                        if (selected != null)
                        {
                            var validated = _dataContext.Validate(item, (selected.EngagesRows != null ? selected.EngagesRows.Where(w => w.boposc != item.boposc).Sum(sum => sum.boqtlo) : 0), selected.QuantityValue, sameLotAlreadyUsed, selected.BOUNIM ?? string.Empty);
                            if (validated != null)
                            {
                                Dispatcher.BeginInvoke(() =>
                                {
                                    ErrorHandler.Validation(validated);
                                });
                            }
                            e.IsValid = string.IsNullOrWhiteSpace(validated);
                        }
                        else
                        {
                            ErrorHandler.Validation("Riga non valida");
                            e.IsValid = false;
                        }
                    }
                    else
                    {
                        ErrorHandler.Validation("Selezionare un lotto valido");
                        e.IsValid = false;
                    }
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

        private void rgvRowsEngages_RowValidated(object sender, GridViewRowValidatedEventArgs e)
        {
            var data = e.Row.Item as BOLLD00F1;

            if (data != null && data.Lot != null)
            {
                data.bolott = data.Lot?.lot!;
                data.store_id = data.Lot?.store_id!;
                data.product_id = data.Lot?.product_id!;

                ComputeEngageProgression();
            }

            e.Handled = true;
        }

        private void rgvRowsEngages_AddingNewDataItem(object sender, GridViewAddingNewEventArgs e)
        {
            if (rgvRows.SelectedItem != null)
            {
                var items = rgvRowsEngages.Items.Cast<BOLLD00F1>();

                var selected = rgvRows.SelectedItem as BOLLD00F;

                if (selected != null)
                {
                    var data = new BOLLD00F1
                    {
                        bolsoc = _dataContext.Head.bolsoc,
                        boposc = items.Any() ? items.Max(max => max.boposc) + 1 : 1,
                        BTANNO = _dataContext.Head.BTANNO,
                        BTBOLL = _dataContext.Head.BTBOLL,
                        BORIGB = selected.BORIGB,
                        boqtlo = 0,
                        bolott = string.Empty,
                        product_id = string.Empty,
                        store_id = string.Empty,
                    };

                    if (!string.IsNullOrEmpty(selected.BOCODA))
                        data.Lots = _dataContext.GetListByProduct(selected.BOCODA);

                    e.NewObject = data;
                    var grid = e.OwnerGridViewItemsControl;
                    grid.CurrentColumn = grid.Columns[0];
                }
            }
            else
            {
                e.Cancel = true;
            }
        }
        #endregion

        #region Magnifier
        private void rgMagnifier_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as RadGlyph)?.DataContext as BOLLD00F;
            if (item != null)
            {
                var clonedItem = new BOLLD00F()
                {
                    bolsoc = item.bolsoc,
                    BTANNO = item.BTANNO,
                    BTBOLL = item.BTBOLL,
                    BOCODA = item.BOCODA,
                    BOQTAV = item.BOQTAV,
                    BOTQTA = item.BOTQTA,
                    boprez = item.boprez,
                    bosco1 = item.bosco1,
                    bosco2 = item.bosco2,
                    bosco3 = item.bosco3,
                    bomagg = item.bomagg,
                    botpre = item.botpre,
                    botsc1 = item.botsc1,
                    botsc2 = item.botsc2,
                    botsc3 = item.botsc3,
                    botmag = item.botmag,
                    boaliq = item.boaliq,
                    boasso = item.boasso,
                    BODACO = item.BODACO,
                    BORIFC = item.BORIFC,
                    bogrup = item.bogrup,
                    bocont = item.bocont,
                    bosotc = item.bosotc,
                    BOUNIM = item.BOUNIM,
                    BONOTE = item.BONOTE,
                    BOSHOW = item.BOSHOW,
                    BOCOA1 = item.BOCOA1,
                    BOCOA2 = item.BOCOA2,
                    BOCOA1P = item.BOCOA1P,
                    BOCOA2P = item.BOCOA2P,
                    BOCOA1PT = item.BOCOA1PT,
                    BOCOA2PT = item.BOCOA2PT,
                    CustomerID = _dataContext.Head.BTCODC ?? 0,
                    RecipientID = _dataContext.Head.BTCODD ?? 0,
                    DDTDate = _dataContext.Head.BTDATP!.Value
                };
                clonedItem.Rates = item.Rates;
                clonedItem.UMsCache = item.UMsCache;
                clonedItem.Products = item.Products;
                clonedItem.AllAccounts = item.AllAccounts;
                clonedItem.AllSubccounts = item.AllSubccounts;
                clonedItem.Groups = item.Groups;
                clonedItem.Accounts = item.Accounts;
                clonedItem.Subaccounts = item.Subaccounts;
                clonedItem.Agents = item.Agents;
                clonedItem.QuantityValueChanged += _dataContext.OnQuantityValueChanged;

                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<BOLLD00FMagnifierWindowViewModel>();
                windowViewModel.Title = "Dettagli della riga DDT";
                windowViewModel.Row = clonedItem;
                windowViewModel.IsReadonly = _dataContext.IsReadonly;

                var wMagnifier = new BOLLD00FMagnifierWindow(windowViewModel);
                wMagnifier.Owner = Window.GetWindow(this);
                if (wMagnifier.ShowDialog() == true)
                {
                    // update row data
                    item.Product = windowViewModel.Row.Product;
                    item.BOQTAV = windowViewModel.Row.BOQTAV;
                    item.BOTQTA = windowViewModel.Row.BOTQTA;
                    item.boprez = windowViewModel.Row.boprez;
                    item.bosco1 = windowViewModel.Row.bosco1;
                    item.bosco2 = windowViewModel.Row.bosco2;
                    item.bosco3 = windowViewModel.Row.bosco3;
                    item.bomagg = windowViewModel.Row.bomagg;
                    item.botpre = windowViewModel.Row.botpre;
                    item.botsc1 = windowViewModel.Row.botsc1;
                    item.botsc2 = windowViewModel.Row.botsc2;
                    item.botsc3 = windowViewModel.Row.botsc3;
                    item.botmag = windowViewModel.Row.botmag;
                    item.Rate = windowViewModel.Row.Rate;
                    item.boasso = windowViewModel.Row.boasso;
                    item.BODACO = windowViewModel.Row.BODACO;
                    item.BORIFC = windowViewModel.Row.BORIFC;
                    item.Group = windowViewModel.Row.Group;
                    item.Account = windowViewModel.Row.Account;
                    item.Subaccount = windowViewModel.Row.Subaccount;
                    item.BOUNIM = windowViewModel.Row.BOUNIM;
                    item.BONOTE = windowViewModel.Row.BONOTE;
                    item.BOSHOW = windowViewModel.Row.BOSHOW;
                    item.FirstAgent = windowViewModel.Row.FirstAgent;
                    item.SecondAgent = windowViewModel.Row.SecondAgent;
                    item.BOCOA1P = windowViewModel.Row.BOCOA1P;
                    item.BOCOA2P = windowViewModel.Row.BOCOA2P;
                    item.BOCOA1PT = windowViewModel.Row.BOCOA1PT;
                    item.BOCOA2PT = windowViewModel.Row.BOCOA2PT;

                    _dataContext.ViewUpdate = true;
                }
            }
        }
        #endregion
    }
}
