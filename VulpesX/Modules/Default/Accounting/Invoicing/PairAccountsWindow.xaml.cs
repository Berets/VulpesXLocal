using Org.BouncyCastle.Bcpg.Sig;
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
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Accounting.Invoicing;

namespace VulpesX.Modules.Default.Accounting.Invoicing
{
    /// <summary>
    /// Interaction logic for PairAccountsWindow.xaml
    /// </summary>
    public partial class PairAccountsWindow : FluentDefaultWindow
    {
        private PairAccountsWindowViewModel _dataContext;
        public PairAccountsWindow(PairAccountsWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;
            this.Title = $"Raggruppa le righe di fattura per contropartita";

            var accountCache = _dataContext.GetPDCCONTIs();
            var subaccountCache = _dataContext.GetPDCSOTTOs();

            _dataContext.LoadDetails();

            foreach (var cp in _dataContext.Counterparts ?? new ObservableCollection<SUPPLIER_GROUPS>())
            {
                _dataContext.PNRows.Add(new PNRIGHE()
                {
                    N1SOCI = _dataContext.CompanyID,
                    AccountCache = accountCache,
                    SubaccountCache = subaccountCache,
                    N1RIGA = cp.cfprog,
                    pngrup = cp.cfgrup,
                    SelectedGroup = cp.SelectedGroup,
                    pncont = cp.cfcont,
                    SelectedAccount = cp.SelectedAccount,
                    pnsott = cp.cfsott,
                    SelectedSubaccount = cp.SelectedSubaccount,
                    N1IMEU = 0,
                    N1CCCC = _dataContext.HeadCostCenterID,
                    CostCentersList = _dataContext.CostCenters
                });
            }

            var natures = _dataContext.GetFE_IVADOCs();
            var rates = _dataContext.GetASSOGGETAMENTIs();

            foreach (var vat in _dataContext.PNIVARows)
            {
                vat.Natures = natures;
                // set first found
                vat.Rates = rates;
                var first = _dataContext.GetFirstAliquota(vat.Fattaliq ?? string.Empty);

                if (first == null && vat.Fattaliq?.Length == 1)
                    first = _dataContext.GetFirstAliquota($"0{vat.Fattaliq.Trim()}"); // try with 0 prefix

                vat.SelectedRate = vat.Rates?.Where(w => w.asscod == first?.asscod && w.assali == first?.assali).FirstOrDefault();
            }
            // clean qta field to use as pair id and set default cost center
            foreach (var eir in _dataContext.Rows)
            {
                eir.fattqta = null;
            }
        }

        #region Grid
        private void rgvPNRows_SelectionChanged(object sender, SelectionChangeEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var item = (e.AddedItems[0] as PNRIGHE);

                if (item != null)
                {
                    rgvRows.Unselect(_dataContext.Rows?.Where(w => w.fattqta != item.N1RIGA));
                    rgvRows.Select(_dataContext.Rows?.Where(w => w.fattqta == item.N1RIGA));
                }
            }
        }
        private void rgvRows_SelectionChanged(object sender, SelectionChangeEventArgs e)
        {
            var item = rgvPNRows.SelectedItem as PNRIGHE;

            if (item != null)
            {
                var pnrid = item.N1RIGA;
                foreach (var removed in e.RemovedItems.Cast<ACC_EINVOICE_ROWS>())
                {
                    var existing = _dataContext.Paired.Where(w => w.Item1 == pnrid && w.Item2 == removed.fattriga).FirstOrDefault();
                    if (existing != null)
                    {
                        _dataContext.Paired.Remove(existing);
                        removed.fattqta = null;
                        item.N1IMEU -= removed.fatttotriga;
                    }
                }
                foreach (var added in e.AddedItems.Cast<ACC_EINVOICE_ROWS>())
                {
                    var existingOthers = _dataContext.Paired.Where(w => w.Item1 != pnrid && w.Item2 == added.fattriga).FirstOrDefault();
                    if (existingOthers != null)
                    {
                        _dataContext.Paired.Remove(existingOthers);
                        _dataContext.PNRows.Where(w => w.N1RIGA == existingOthers.Item1).First().N1IMEU -= added.fatttotriga;
                    }
                    var existing = _dataContext.Paired.Where(w => w.Item1 == pnrid && w.Item2 == added.fattriga).FirstOrDefault();
                    if (existing == null)
                    {
                        added.fattqta = pnrid;
                        _dataContext.Paired.Add(new Tuple<int, int>(pnrid, added.fattriga));
                        item.N1IMEU += added.fatttotriga;
                    }
                }
            }
        }
        #endregion

        #region Buttons
        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            if (ConfirmHandler.Confirm("Confermate la creazione della registrazione contabile ?"))
            {
                if (_dataContext.PNIVARows.All(all => all.SelectedRate != null))
                {
                    var completed = _dataContext.Rows.All(all => all.fattqta.HasValue);
                    if (completed || (!completed && ConfirmHandler.Confirm("Non tutte le righe sono state assegnate, proseguire comunque ?")))
                    {
                        Mouse.OverrideCursor = Cursors.Wait;
                        this.DialogResult = true;
                    }
                }
                else
                {
                    ErrorHandler.Validation("Impossibile proseguire senza aver selezionato tutti gli abbinamenti per le aliquote IVA");
                }
            }
            else
            {
                Mouse.OverrideCursor = null;
                this.DialogResult = false;
            }
        }
        #endregion

        private void rgvPNRows_RowValidated(object sender, GridViewRowValidatedEventArgs e)
        {
            var data = e.Row.Item as PNRIGHE;

            if (data != null)
            {
                data.N1CCCC = data.SelectedCostCenter?.cecodc;
                data.pngrup = data.SelectedGroup?.P1GRUP;
                data.pncont = data.SelectedAccount?.P2CONT;
                data.pnsott = data.SelectedSubaccount?.P3SOTC;
            }
        }

        private void rgvPNIVARows_AddingNewDataItem(object sender, GridViewAddingNewEventArgs e)
        {
            var nextRowID = rgvPNIVARows.Items.Cast<ACC_EINVOICE_VAT>().Max(max => max.fattprog) + 1;
            var natures = _dataContext.GetFE_IVADOCs();
            var rates = _dataContext.GetASSOGGETAMENTIs();

            var data = new ACC_EINVOICE_VAT()
            {
                fattsoc = _dataContext.CompanyID,
                fattprog = nextRowID,
                fattnum = "",
                fattpiva = "",
            };

            data.Natures = natures;
            data.Rates = rates;

            var first = _dataContext.GetFirstAliquota(data.Fattaliq ?? string.Empty);
            data.SelectedRate = data.Rates?.Where(w => w.asscod == first?.asscod && w.assali == first?.assali).FirstOrDefault();
            e.NewObject = data;
        }

        private void rgvPNIVARows_RowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            if (e.EditOperationType == Telerik.Windows.Controls.GridView.GridViewEditOperationType.Edit)
            {
                var item = e.Row.Item as ACC_EINVOICE_VAT;

                if (item != null)
                    e.IsValid = item.SelectedRate != null;
            }
        }

        private void rgvPNIVARows_RowValidated(object sender, GridViewRowValidatedEventArgs e)
        {
            var data = e.Row.Item as ACC_EINVOICE_VAT;

            if (data != null)
            {
                int rateValue = 0;

                if (data.SelectedRate != null)
                {
                    if (int.TryParse(data.SelectedRate.assali, out rateValue))
                    {
                        data.fattimpostadett = Math.Round(((data.fattimpodett ?? 0) * rateValue / 100), 2, MidpointRounding.AwayFromZero);
                        data.Fattaliq = data.SelectedRate.assali;
                    }
                    else
                    {
                        data.fattimpostadett = 0;
                    }
                }
                else
                {
                    data.fattimpostadett = 0;
                }
            }
        }

        private void rgvPNRows_AddingNewDataItem(object sender, Telerik.Windows.Controls.GridView.GridViewAddingNewEventArgs e)
        {
            var nextRowID = rgvPNRows.Items.Cast<PNRIGHE>().Max(max => max.N1RIGA) + 1;

            var data = new PNRIGHE()
            {
                N1SOCI = _dataContext.CompanyID,
                N1RIGA = nextRowID,
                N1IMEU = 0,
                AccountCache = _dataContext.GetPDCCONTIs(),
                SubaccountCache = _dataContext.GetPDCSOTTOs(),
                GroupsList = _dataContext.GetPDCGRUPPIs(),
                N1CCCC = _dataContext.HeadCostCenterID,
                CostCentersList = _dataContext.CostCenters
            };
            e.NewObject = data;
        }

        private void rgvPNRows_RowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            if (e.EditOperationType == Telerik.Windows.Controls.GridView.GridViewEditOperationType.Edit || e.EditOperationType == Telerik.Windows.Controls.GridView.GridViewEditOperationType.Insert)
            {
                var item = e.Row.Item as PNRIGHE;

                if (item != null)
                    e.IsValid = item.SelectedGroup != null && item.SelectedAccount != null && item.SelectedSubaccount != null;
            }
        }

        private void rgvPNRows_Deleted(object sender, GridViewDeletedEventArgs e)
        {
            var deleted = e.Items.Cast<PNRIGHE>().First();

            foreach (var row in rgvRows.Items.Cast<ACC_EINVOICE_ROWS>().Where(w => w.fattqta == deleted.N1RIGA))
            {
                var remove = _dataContext.Paired.Where(w => w.Item1 == deleted.N1RIGA).FirstOrDefault();

                if (remove != null)
                {
                    _dataContext.Paired.Remove(remove);
                    row.fattqta = null;
                }
            }
        }

        #region IVA context menu
        private void cmGridReceived_Opening(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            if (rgvPNIVARows.SelectedItem != null)
            {
                var item = rgvPNIVARows.SelectedItem as ACC_EINVOICE_VAT;
                rmiAddPNRow.IsEnabled = item != null && item.SelectedRate != null && item.fattimpostadett.HasValue &&
                    item.fattimpostadett.Value > 0 && item.SelectedNature != null && item.SelectedNature.FETICod == "N6.7" &&
                    !item.AlreadyCreatedPNRow;
            }
            else
            {
                rmiAddPNRow.IsEnabled = false;
            }
        }

        private void rmiAddPNRow_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            if (rgvPNIVARows.SelectedItem != null)
            {
                var item = rgvPNIVARows.SelectedItem as ACC_EINVOICE_VAT;

                if (item != null)
                {
                    item.AlreadyCreatedPNRow = true;

                    var nextRowID = rgvPNRows.Items.Cast<PNRIGHE>().Max(max => max.N1RIGA) + 1;

                    _dataContext.PNRows.Add(new PNRIGHE()
                    {
                        N1SOCI = _dataContext.CompanyID,
                        N1RIGA = nextRowID,
                        N1IMEU = item.fattimpostadett,
                        AccountCache = _dataContext.GetPDCCONTIs(),
                        SubaccountCache = _dataContext.GetPDCSOTTOs(),
                        GroupsList = _dataContext.GetPDCGRUPPIs(),
                        N1CCCC = _dataContext.HeadCostCenterID,
                        CostCentersList = _dataContext.CostCenters,
                        NotPair = true
                    });
                }
            }
        }
        #endregion

        private void rgvPNRows_SelectionChanging(object sender, SelectionChangingEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var row = e.AddedItems[0] as PNRIGHE;

                if (row != null)
                    e.Cancel = row.NotPair;
            }
        }


    }
}
