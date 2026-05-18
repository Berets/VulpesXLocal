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
using Telerik.Windows.Documents.Spreadsheet.Model;
using VulpesX.Models;
using VulpesX.Models.Default.Partials;
using VulpesX.Models.Models.Reports.Accounting;
using VulpesX.Shared;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Accounting;

namespace VulpesX.Modules.Default.Accounting
{
    /// <summary>
    /// Interaction logic for PNRIGHEWindow.xaml
    /// </summary>
    public partial class PNRIGHEWindow : FluentDefaultWindow
    {
        private PNRIGHEWindowViewModel _dataContext;
        public PNRIGHEWindow(PNRIGHEWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.Height = (System.Windows.SystemParameters.PrimaryScreenHeight - 200);
            this.Width = (System.Windows.SystemParameters.PrimaryScreenWidth);
            this.DataContext = _dataContext;


            if (_dataContext.IsInsert)
            {
                _dataContext.Head.Rows = new ObservableCollection<PNRIGHE>();
                _dataContext.IVARows = new ObservableCollection<PNIVA>();
                _dataContext.Rows = new ObservableCollection<PNRIGHE>();

                var causalGroups = _dataContext.GetCAUCONT_GROUPS();
                decimal iva = 0;
                if (!string.IsNullOrWhiteSpace(_dataContext.HeadSelectedCausal?.cauiva) && _dataContext.HeadSelectedCausal.cauiva == "S")
                {
                    string? aliq = null;
                    string? asso = null;
                    int indetraibile = 0;

                    if (_dataContext.Head.N1FLCF == "C")
                    {
                        var customer = _dataContext.GetCLIENTI();

                        if (customer != null)
                        {
                            var rate = _dataContext.GetASSOGGETAMENTI(customer.classo, customer.classa);
                            aliq = customer.classa;
                            asso = customer.classo;
                            indetraibile = rate?.asspin ?? 0;
                        }
                    }
                    else
                    {
                        var supplier = _dataContext.GetFORNAMMI();

                        if (supplier != null)
                        {
                            var rate = _dataContext.GetASSOGGETAMENTI(supplier.foaass, supplier.foaali);
                            aliq = supplier.foaali;
                            asso = supplier.foaass;
                            indetraibile = rate?.asspin ?? 0;
                        }
                    }

                    decimal amount = 0;
                    decimal ivaindetraibile = 0;
                    if (!string.IsNullOrWhiteSpace(aliq) && !string.IsNullOrWhiteSpace(asso))
                    {

                        decimal aliqValue = 0;
                        decimal.TryParse(aliq, out aliqValue);
                        iva = Math.Round((_dataContext.Head.Amount / (100 + aliqValue)) * aliqValue, 2);
                        amount = _dataContext.Head.Amount - iva;
                        if (indetraibile > 0 && iva > 0)
                        {
                            ivaindetraibile = Math.Round((iva * indetraibile) / 100, 2);
                        }
                    }

                    var ivaBook = _dataContext.GetLIBRIIVA();

                    foreach (var cg in causalGroups ?? new ObservableCollection<CAUCONT_GROUPS>())
                    {
                        var newRow = new PNRIGHE
                        {
                            N1SOCI = _dataContext.CompanyID,
                            N1ANNO = _dataContext.Head.N1ANNO,
                            N1RIGA = _dataContext.Rows.Any() ? _dataContext.Rows.Max(max => max.N1RIGA) + 1 : 1,
                            N1DOCU = _dataContext.Head.N1docn,
                            N1DADO = _dataContext.Head.N1docd,
                            N1RIFE = _dataContext.Head.N1rifn,
                            N1DARI = _dataContext.Head.N1rifd,
                            N1SEGN = cg.grpseg,
                            N1TIDO = "E",
                            N1DIVI = _dataContext.Head.pnvdiv,
                            N1IMEU = causalGroups?.Count == 1 ? iva : 0,
                            N1CHIU = "A",
                            N1STBO = string.Empty,
                            N1STNO = string.Empty,
                            N1STMA = string.Empty,
                            N1RIGB = 0,
                            N1DABB = null,
                            N1DASM = null,
                            N1DACC = null,
                            N1tmpPNR = _dataContext.Head.N1TmpPN,
                            pngrup = cg.grpgrp,
                            pncont = cg.grpcto,
                            pnsott = cg.grpsct,
                            AccountCache = _dataContext.AccountCache,
                            SubaccountCache = _dataContext.SubaccountCache,
                            GroupsList = _dataContext.GroupsList,
                            CostCentersList = _dataContext.CostCentersList,
                            Testata = _dataContext.Head,
                        };
                        newRow.SubaccountChanged += _dataContext.OnSubaccountChanged;
                        _dataContext.OnSubaccountChanged(newRow, null);

                        _dataContext.Rows.Add(newRow);
                    }

                    _dataContext.IVARows.Add(new PNIVA
                    {
                        N4SOCI = _dataContext.CompanyID,
                        N4ANNO = _dataContext.Head.N1ANNO,
                        N4RIGA = 1,
                        N4DARE = _dataContext.Head.N1DARE,
                        N4DOCU = _dataContext.Head.N1docn,
                        N4DADO = _dataContext.Head.N1docd,
                        N4RIFE = _dataContext.Head.N1rifn,
                        N4DARI = _dataContext.Head.N1rifd,
                        N4CAUS = _dataContext.Head.pncaus,
                        N4LIBR = _dataContext.HeadSelectedCausal.cauliv,
                        N4SEGN = _dataContext.HeadSelectedCausal.causeg,
                        N4TCLF = _dataContext.Head.N1FLCF,
                        N4SOTT = _dataContext.Head.N1CLFO,
                        N4TIDO = "E",
                        n4assa = aliq,
                        N4ASSF = asso,
                        N4IMEU = amount,
                        N4IVEU = iva,
                        N4IIEU = ivaindetraibile,
                        N4INDP = indetraibile,
                        n4tmppn = "N",
                        N4DTSCPG = _dataContext.Head.N1DARE,
                        n4donu = !string.IsNullOrWhiteSpace(_dataContext.Head.N1docn) ? int.Parse(_dataContext.Head.N1docn) : null,
                        RatesList = _dataContext.RatesList,
                        IVABooksList = _dataContext.IVABooksList
                    });
                }

                if (!string.IsNullOrWhiteSpace(_dataContext.Head.N1FLCF))
                {
                    // add general row
                    string? groupID = null;
                    string? accountID = null;
                    string? subaccountID = null;
                    string? paymentID = null;

                    if (_dataContext.Head.N1FLCF == "C")
                    {
                        var cliammi = _dataContext.GetCLIAMMI();
                        groupID = cliammi?.clGRUP;
                        accountID = cliammi?.clcont;
                        paymentID = cliammi?.pclcod;
                    }
                    else
                    {
                        var fornammi = _dataContext.GetFORNAMMI();
                        groupID = fornammi?.foGRUP;
                        accountID = fornammi?.foCONT;
                        paymentID = fornammi?.pfocod;
                    }
                    subaccountID = _dataContext.GetPDCSOTTO(groupID, accountID)?.P3SOTC;
                    string? firstRowSign = null;
                    if (_dataContext.Rows.Count > 0)
                    {
                        firstRowSign = _dataContext.Rows.Where(w => w.N1RIGA == 1).FirstOrDefault()?.N1SEGN;
                    }
                    if (!string.IsNullOrWhiteSpace(firstRowSign))
                    {
                        if (firstRowSign == "D")
                            firstRowSign = "A";
                        else
                            firstRowSign = "D";
                    }
                    else
                    {
                        if (_dataContext.Head.N1FLCF == "F")
                            firstRowSign = "D";
                        else
                            firstRowSign = "A";
                    }
                    var newRow = new PNRIGHE
                    {
                        N1SOCI = _dataContext.CompanyID,
                        N1ANNO = _dataContext.Head.N1ANNO,
                        N1RIGA = _dataContext.Rows.Any() ? _dataContext.Rows.Max(max => max.N1RIGA) + 1 : 1,
                        N1DOCU = _dataContext.Head.N1docn,
                        N1DADO = _dataContext.Head.N1docd,
                        N1RIFE = _dataContext.Head.N1rifn,
                        N1DARI = _dataContext.Head.N1rifd,
                        N1SEGN = firstRowSign,
                        N1TIDO = "E",
                        n1clie = _dataContext.Head.N1CLFO,
                        N1CHIU = "A",
                        N1STBO = string.Empty,
                        N1STNO = string.Empty,
                        N1STMA = string.Empty,
                        N1RIGB = 0,
                        N1DABB = null,
                        N1DASM = null,
                        N1DACC = null,
                        n1paga = paymentID,
                        N1IMEU = _dataContext.Head.Amount,
                        pngrup = groupID,
                        pncont = accountID,
                        pnsott = subaccountID,
                        N1DIVI = _dataContext.Head.pnvdiv,
                        N1tmpPNR = _dataContext.Head.N1TmpPN,
                        AccountCache = _dataContext.AccountCache,
                        SubaccountCache = _dataContext.SubaccountCache,
                        GroupsList = _dataContext.GroupsList,
                        CostCentersList = _dataContext.CostCentersList,
                        Testata = _dataContext.Head,
                    };
                    newRow.SubaccountChanged += _dataContext.OnSubaccountChanged;
                    _dataContext.OnSubaccountChanged(newRow, null);

                    _dataContext.Rows.Add(newRow);
                }

                if (causalGroups != null && _dataContext.HeadSelectedCausal?.cauiva == "N")
                {
                    foreach (var cg in causalGroups)
                    {
                        var newRow = new PNRIGHE
                        {
                            N1SOCI = _dataContext.CompanyID,
                            N1ANNO = _dataContext.Head.N1ANNO,
                            N1RIGA = _dataContext.Rows.Any() ? _dataContext.Rows.Max(max => max.N1RIGA) + 1 : 1,
                            N1DOCU = _dataContext.Head.N1docn,
                            N1DADO = _dataContext.Head.N1docd,
                            N1RIFE = _dataContext.Head.N1rifn,
                            N1DARI = _dataContext.Head.N1rifd,
                            N1SEGN = cg.grpseg,
                            N1TIDO = "E",
                            N1DIVI = _dataContext.Head.pnvdiv,
                            N1IMEU = causalGroups.Count == 1 ? _dataContext.Head.Amount : 0,
                            N1CHIU = "A",
                            N1STBO = string.Empty,
                            N1STNO = string.Empty,
                            N1STMA = string.Empty,
                            N1RIGB = 0,
                            N1DABB = null,
                            N1DASM = null,
                            N1DACC = null,
                            N1tmpPNR = _dataContext.Head.N1TmpPN,
                            pngrup = cg.grpgrp,
                            pncont = cg.grpcto,
                            pnsott = cg.grpsct,
                            AccountCache = _dataContext.AccountCache,
                            SubaccountCache = _dataContext.SubaccountCache,
                            GroupsList = _dataContext.GroupsList,
                            CostCentersList = _dataContext.CostCentersList,
                            Testata = _dataContext.Head,
                        };
                        newRow.SubaccountChanged += _dataContext.OnSubaccountChanged;
                        _dataContext.OnSubaccountChanged(newRow, null);

                        _dataContext.Rows.Add(newRow);
                    }
                }

                var counterparts = _dataContext.GetSUPPLIER_GROUPS();
                if (counterparts != null)
                {
                    foreach (var cp in counterparts)
                    {
                        var newRow = new PNRIGHE
                        {
                            N1SOCI = _dataContext.CompanyID,
                            N1ANNO = _dataContext.Head.N1ANNO,
                            N1RIGA = _dataContext.Rows.Any() ? _dataContext.Rows.Max(max => max.N1RIGA) + 1 : 1,
                            N1DOCU = _dataContext.Head.N1docn,
                            N1DADO = _dataContext.Head.N1docd,
                            N1RIFE = _dataContext.Head.N1rifn,
                            N1DARI = _dataContext.Head.N1rifd,
                            N1SEGN = cp.cfsegn,
                            N1TIDO = "E",
                            N1DIVI = _dataContext.Head.pnvdiv,
                            N1IMEU = counterparts.Count == 1 ? _dataContext.Head.Amount - iva : 0,
                            N1CHIU = "A",
                            N1STBO = string.Empty,
                            N1STNO = string.Empty,
                            N1STMA = string.Empty,
                            N1RIGB = 0,
                            N1DABB = null,
                            N1DASM = null,
                            N1DACC = null,
                            N1tmpPNR = _dataContext.Head.N1TmpPN,
                            pngrup = cp.cfgrup,
                            pncont = cp.cfcont,
                            pnsott = cp.cfsott,
                            AccountCache = _dataContext.AccountCache,
                            SubaccountCache = _dataContext.SubaccountCache,
                            GroupsList = _dataContext.GroupsList,
                            CostCentersList = _dataContext.CostCentersList,
                            Testata = _dataContext.Head,
                        };
                        newRow.SubaccountChanged += _dataContext.OnSubaccountChanged;
                        _dataContext.OnSubaccountChanged(newRow, null);

                        _dataContext.Rows.Add(newRow);
                    }
                }
            }
            else
            {
                _dataContext.Rows = _dataContext.GetPNRIGHE() ?? new ObservableCollection<PNRIGHE>();
                _dataContext.IsReadonly = (_dataContext.Rows ?? new ObservableCollection<PNRIGHE>()).Any(any => any.N1STBO == "*");
                _dataContext.IVARows = _dataContext.GetPNIVA();
            }

            if (!_dataContext.IsInsert)
            {
                LoadExpires();
            }
            Loaded += (sender, e) =>
            {
                this.Title = $"Dettagli della registrazione di prima nota {_dataContext.Head.PrintFullID}";
                if (_dataContext.IsReadonly)
                    this.Title += " - [sola lettura]";
            };

            rgvRows.Columns["colCheck"].IsVisible = _dataContext.HeadSelectedCausal?.cauceco == "S";
        }

        private void LoadExpires()
        {
            Parallel.ForEach(_dataContext.Rows, row =>
            {
                if (row.n1clie.HasValue && row.n1clie.Value > 0)
                {
                    ObservableCollection<SectionalItem> expiresList = new ObservableCollection<SectionalItem>();
                    if (row.SelectedSubaccount?.P3CLFO == "C")
                    {
                        var expires = _dataContext.GetPNCLIENTI(row.N1RIGA, row.n1clie.Value);
                        if (expires != null)
                        {
                            foreach (var exp in expires)
                            {
                                expiresList.Add(new SectionalItem()
                                {
                                    RowID = exp.N2RIGA,
                                    OriginalRowID = exp.n2rior,
                                    ExpireDate = exp.N2SCAD,
                                    Amount = exp.N2IMEU ?? 0,
                                    Sign = exp.N2SEGN,
                                    CurrencyAmount = exp.N2IMPO ?? 0,
                                    CurrencyDoc = exp.N2DIDO,
                                    CurrencyID = exp.N2DIVI,
                                    Note = exp.N2DESC,
                                    EntityType = "C",
                                    CurrencyChange = exp.N2CAMB,
                                    CurrencyValue = exp.N2VALU,
                                });
                            }
                        }
                    }
                    else
                    {
                        var expires = _dataContext.GetPNFORNITORI(row.N1RIGA, row.n1clie.Value);
                        if (expires != null)
                        {
                            foreach (var exp in expires)
                            {
                                expiresList.Add(new SectionalItem()
                                {
                                    RowID = exp.N3RIGA,
                                    OriginalRowID = exp.n3rior,
                                    ExpireDate = exp.N3SCAD,
                                    Amount = exp.N3IMEU ?? 0,
                                    Sign = exp.N3SEGN,
                                    CurrencyAmount = exp.N3IMPO ?? 0,
                                    CurrencyDoc = exp.N3DIDO,
                                    CurrencyID = exp.N3DIVI,
                                    Note = exp.N3DESC,
                                    EntityType = "F",
                                    LockedInfoText = exp.locked.HasValue ? $"Pagamento bloccato da {exp.lockedUserID} in data {exp.locked.Value.ToString("dd/MM/yyyy HH:mm:ss")} per {exp.lockedReason?.Trim()}" : null,
                                    CurrencyChange = exp.N3CAMB,
                                    CurrencyValue = exp.N3VALU,
                                });
                            }
                        }
                    }
                    row.ExpireRows = expiresList;
                }
                else
                {
                    row.ExpireRows = null;
                }
            });


            if(_dataContext.Rows.Where(o=>o.ExpireRows?.Where(oo=>(oo.CurrencyValue ?? 0) > 0).Any() ?? false).Any())
            {
                rgvExpire.Columns["colValValuta"].IsVisible = true;
                rgvExpire.Columns["colValCambio"].IsVisible = true;
                rgvExpire.Columns["colValValoreValuta"].IsVisible = true;
            }
            else
            {
                rgvExpire.Columns["colValValuta"].IsVisible = false;
                rgvExpire.Columns["colValCambio"].IsVisible = false;
                rgvExpire.Columns["colValValoreValuta"].IsVisible = false;
            }
        }

        #region Buttons
        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            string? validated = _dataContext.ValidateModel();
            if (validated == null)
            {
                if (_dataContext.Save())
                    this.DialogResult = true;
            }
            else
            {
                ErrorHandler.Show(validated);
            }
        }
        #endregion

        #region Rows grid
        private void rgvRows_AddingNewDataItem(object sender, Telerik.Windows.Controls.GridView.GridViewAddingNewEventArgs e)
        {
            var items = rgvRows.Items.Cast<PNRIGHE>();

            var data = new PNRIGHE
            {

                N1SOCI = _dataContext.Head.N1SOCI,
                N1RIGA = items.Any() ? items.Max(max => max.N1RIGA) + 1 : 1,
                N1ANNO = _dataContext.Head.N1ANNO,
                N1RIFE = _dataContext.Head.N1rifn,
                N1DARI = _dataContext.Head.N1rifd,
                N1DOCU = _dataContext.Head.N1docn,
                N1DADO = _dataContext.Head.N1docd,
                N1IMEU = 0,
                N1CHIU = "A",
                N1TIDO = "E",
                N1STBO = string.Empty,
                N1STNO = string.Empty,
                N1STMA = string.Empty,
                N1RIGB = 0,
                N1DIVI = _dataContext.Head.pnvdiv,
                N1tmpPNR = _dataContext.Head.N1TmpPN,
                AccountCache = _dataContext.AccountCache,
                SubaccountCache = _dataContext.SubaccountCache,
                GroupsList = _dataContext.GroupsList,
                CostCentersList = _dataContext.CostCentersList,
                Testata = _dataContext.Head,
            };
            data.SubaccountChanged += _dataContext.OnSubaccountChanged;
            e.NewObject = data;
            var grid = e.OwnerGridViewItemsControl;
            grid.CurrentColumn = grid.Columns[1];
        }

        private void rgvRows_RowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            // TODO Check edit trigger
            if (e.EditOperationType != Telerik.Windows.Controls.GridView.GridViewEditOperationType.None)
            {
                var item = e.Row.Item as PNRIGHE;

                if (item != null)
                {
                    var validated = _dataContext.ValidatePNRIGHE(item);
                    if (validated != null)
                    {
                        Dispatcher.BeginInvoke(() => { ErrorHandler.Show(validated); });
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
            var data = e.Row.Item as PNRIGHE;

            if (data != null)
            {
                data.pngrup = data.SelectedGroup?.P1GRUP;
                data.pncont = data.SelectedAccount?.P2CONT;
                data.pnsott = data.SelectedSubaccount?.P3SOTC;
                data.n1clie = data.SelectedEntity?.abecod;
                data.n1paga = data.SelectedPayment?.ID;
                data.N1CCCC = data.SelectedCostCenter?.cecodc;
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
        #endregion

        #region IVA grid
        private void rgvIVARows_AddingNewDataItem(object sender, Telerik.Windows.Controls.GridView.GridViewAddingNewEventArgs e)
        {
            var items = rgvIVARows.Items.Cast<PNIVA>();
            var data = new PNIVA
            {
                N4SOCI = _dataContext.Head.N1SOCI,
                N4RIGA = items.Any() ? items.Max(max => max.N4RIGA) + 1 : 1,
                N4ANNO = _dataContext.Head.N1ANNO,
                N4RIFE = _dataContext.Head.N1rifn,
                N4DARI = _dataContext.Head.N1rifd,
                N4DOCU = _dataContext.Head.N1docn,
                N4DADO = _dataContext.Head.N1docd,
                N4DARE = _dataContext.Head.N1DARE,
                N4CAUS = _dataContext.Head.pncaus,
                N4IMEU = 0,
                N4INDP = 0,
                n4tmppn = "N",
                N4TIDO = "E",
                N4SOTT = _dataContext.Head.N1CLFO,
                N4TCLF = _dataContext.Head.N1FLCF,
                N4DTSCPG = _dataContext.Head.N1DARE,
                n4donu = !string.IsNullOrWhiteSpace(_dataContext.Head.N1docn) ? int.Parse(_dataContext.Head.N1docn) : null,
                IVABooksList = _dataContext.IVABooksList,
                SelectedIVABook = _dataContext.IVABooksList?.Where(w => w.livcod == _dataContext.HeadSelectedCausal?.cauliv).FirstOrDefault(),
                RatesList = _dataContext.RatesList,
                N4SEGN = "+",
            };
            e.NewObject = data;
            var grid = e.OwnerGridViewItemsControl;
            grid.CurrentColumn = grid.Columns[1];
        }
      
        
        private void rgvIVARows_RowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            if (e.EditOperationType == Telerik.Windows.Controls.GridView.GridViewEditOperationType.Edit)
            {
                var item = e.Row.Item as PNIVA;

                if (item != null)
                {
                    string? validate = _dataContext.ValidatePNIVA(item);
                    if (string.IsNullOrWhiteSpace(validate))
                    {
                        if ((item.N4IVEU ?? 0) != item.ComputeIVAAmount())
                        {
                            if (ConfirmHandler.Confirm("L'importo dell'IVA non coincide con il calcolo automatico, forzare comunque il valore ?"))
                                e.IsValid = true;
                            else
                                e.IsValid = false;
                        }
                        else
                        {
                            e.IsValid = true;
                        }
                    }
                    else
                    {
                        ErrorHandler.Show(validate);
                        e.IsValid = false;
                    }
                }
            }
        }
       
        private void rgvIVARows_RowValidated(object sender, GridViewRowValidatedEventArgs e)
        {
            var data = e.Row.Item as PNIVA;

            if (data != null)
            {
                data.N4LIBR = data.SelectedIVABook?.livcod;
                data.n4assa = data.SelectedRate?.assali;
                data.N4ASSF = data.SelectedRate?.asscod;
            }
        }
      
        private void rgvIVARows_RowEditEnded(object sender, GridViewRowEditEndedEventArgs e)
        {
            rgvIVARows.ScrollIntoView(e.Row.Item, rgvIVARows.Columns[0]);
        }
      
        private void rgvIVARows_BeginningEdit(object sender, GridViewBeginningEditRoutedEventArgs e)
        {
            var model = e.Row.Item as PNIVA;

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
        }
      
        private void rgvIVARows_Deleting(object sender, GridViewDeletingEventArgs e)
        {
            if (e.Items != null && e.Items.Count() > 0)
            {
                var model = e.Items.First() as PNIVA;

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
            }
        }
        #endregion

        #region Expires grid
        private void rgvExpire_AddingNewDataItem(object sender, Telerik.Windows.Controls.GridView.GridViewAddingNewEventArgs e)
        {
            var selected = rgvRows.SelectedItem as PNRIGHE;
            if (selected != null && selected.n1clie.HasValue)
            {
                if (selected.ExpireRows == null)
                    selected.ExpireRows = new ObservableCollection<SectionalItem>();
                var items = rgvExpire.Items.Cast<SectionalItem>();
                var data = new SectionalItem()
                {
                    RowID = items.Any() ? items.Max(max => max.RowID) + 1 : 1,
                    OriginalRowID = selected.N1RIGA
                };
                e.NewObject = data;
                var grid = e.OwnerGridViewItemsControl;
                grid.CurrentColumn = grid.Columns[1];
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void rgvExpire_RowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            var item = e.Row.Item as SectionalItem;
            e.IsValid = true;
        }

        private void rgvExpire_RowValidated(object sender, GridViewRowValidatedEventArgs e)
        {
            var data = e.Row.Item as SectionalItem;
        }

        private void rgvExpire_RowEditEnded(object sender, GridViewRowEditEndedEventArgs e)
        {
            rgvExpire.ScrollIntoView(e.Row.Item, rgvExpire.Columns[0]);
        }

        #endregion

        #region Autocompletes
        private void acRates_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (rgvIVARows.SelectedItem != null)
            {
                var model = rgvIVARows.SelectedItem as PNIVA;

                if (model != null)
                {
                    model.N4IVEU = model.ComputeIVAAmount();
                    model.N4IIEU = model.ComputeIVANonDeduct();
                }
            }
        }
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

        private void rnudAmount_ValueChanged(object sender, RadRangeBaseValueChangedEventArgs e)
        {
            if (rgvIVARows.SelectedItem != null)
            {
                var model = rgvIVARows.SelectedItem as PNIVA;
                if (model != null)
                {
                    model.N4IVEU = model.ComputeIVAAmount();
                    model.N4IIEU = model.ComputeIVANonDeduct();
                }
            }
        }

        private void rgSelectEC_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as RadGlyph)!.DataContext as PNRIGHE;
            if (item != null)
            {
                if (item.SelectECVisibility)
                {
                    var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<SelectECWindowViewModel>();
                    windowViewModel.EntityType = item.SelectedAccount?.p2flcf ?? string.Empty;
                    windowViewModel.EntityID = item.SelectedEntity?.abecod ?? 0;

                    var wSelect = new SelectECWindow(windowViewModel);
                    wSelect.Owner = Window.GetWindow(this);

                    if (wSelect.ShowDialog() == true && wSelect.Tag != null)
                    {
                        var selectedItems = wSelect.Tag as List<MastrinoECReportItem>;

                        if (selectedItems != null && selectedItems.Count > 0)
                        {
                            bool first = true;
                            foreach (var selected in selectedItems)
                            {
                                if (first)
                                {
                                    item.N1DOCU = selected.DocumentID;
                                    item.N1DADO = selected.DocumentDate;
                                    item.N1RIFE = selected.ReferenceID;
                                    item.N1DARI = selected.ReferenceDate;
                                    item.N1SEGN = selected.Dare > 0 ? "A" : "D";
                                    item.N1IMEU = selected.Dare > 0 ? selected.Dare : selected.Avere;
                                    item.n1scad = selected.ExpireDate;
                                    first = false;
                                }
                                else
                                {
                                    var newRow = new PNRIGHE
                                    {
                                        N1SOCI = _dataContext.Head.N1SOCI,
                                        N1RIGA = _dataContext.Rows.Any() ? _dataContext.Rows.Max(max => max.N1RIGA) + 1 : 1,
                                        N1ANNO = _dataContext.Head.N1ANNO,
                                        N1CHIU = "A",
                                        N1TIDO = "E",
                                        N1STBO = string.Empty,
                                        N1STNO = string.Empty,
                                        N1STMA = string.Empty,
                                        pngrup = item.pngrup,
                                        pncont = item.pncont,
                                        pnsott = item.pnsott,
                                        n1clie = item.n1clie,
                                        N1RIGB = 0,
                                        N1DIVI = _dataContext.Head.pnvdiv,
                                        N1tmpPNR = _dataContext.Head.N1TmpPN,
                                        AccountCache = _dataContext.AccountCache,
                                        SubaccountCache = _dataContext.SubaccountCache,
                                        GroupsList = _dataContext.GroupsList,
                                        CostCentersList = _dataContext.CostCentersList,
                                        Testata = _dataContext.Head,
                                        N1DOCU = selected.DocumentID,
                                        N1DADO = selected.DocumentDate,
                                        N1RIFE = selected.ReferenceID,
                                        N1DARI = selected.ReferenceDate,
                                        N1SEGN = selected.Dare > 0 ? "A" : "D",
                                        N1IMEU = selected.Dare > 0 ? selected.Dare : selected.Avere,
                                        n1scad = selected.ExpireDate,
                                        n1paga = selected.PaymentID,
                                    };
                                    newRow.SubaccountChanged += _dataContext.OnSubaccountChanged;
                                    _dataContext.OnSubaccountChanged(newRow, null);
                                    _dataContext.Rows.Add(newRow);
                                }
                            }
                        }
                    }
                }
                _dataContext.ViewUpdate = true;
            }
        }

        private void rgViewMastrino_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as RadGlyph)!.DataContext as PNRIGHE;

            if (item != null && item.SelectedGroup != null && item.SelectedAccount != null && item.SelectedSubaccount != null)
            {
                var selected = _dataContext.GetPDCANNI(item.SelectedGroup.P1GRUP, item.SelectedAccount.P2CONT, item.SelectedSubaccount.P3SOTC, item.N1ANNO);

                if (selected != null)
                {
                    var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<MastrinoWindowViewModel>();
                    windowViewModel.Year = item.N1ANNO;
                    windowViewModel.SelectedMastrino = selected;

                    var wMastrino = new MastrinoWindow(windowViewModel);
                    wMastrino.Owner = Window.GetWindow(this);
                    wMastrino.ShowDialog();
                }
            }
        }

        #region Context menu
        private void cmGrid_Opening(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            if (rgvRows.SelectedItem != null)
            {
                rmiAsset.IsEnabled = _dataContext.Roles != null && _dataContext.Roles.canAccountingAssets;
            }
            else
            {
                rmiAsset.IsEnabled = false;
            }
        }

        private void rmiAsset_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            if (rgvRows.SelectedItem != null)
            {
                var item = rgvRows.SelectedItem as PNRIGHE;

                if (item != null)
                {
                    if (!string.IsNullOrWhiteSpace(item.pngrup) && !string.IsNullOrWhiteSpace(item.pncont) && !string.IsNullOrWhiteSpace(item.pnsott))
                    {
                        if (ConfirmHandler.Confirm($"Aggiungere a cespite il bene {item.SelectedSubaccount?.FullDescriptionSearchable} ?"))
                        {
                            Mouse.OverrideCursor = Cursors.Wait;

                        }
                    }
                    else
                    {
                        ErrorHandler.Show("Impossibile aggiungere a cespite, mancano i dati del piano dei conti");
                    }
                }
            }
        }
        #endregion
    }
}
