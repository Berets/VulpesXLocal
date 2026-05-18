using DocumentFormat.OpenXml.Spreadsheet;
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
    /// Interaction logic for FATTD00FWindow.xaml
    /// </summary>
    public partial class FATTD00FWindow : FluentDefaultWindow
    {
        private FATTD00FWindowViewModel _dataContext;
        public FATTD00FWindow(FATTD00FWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.Height = (System.Windows.SystemParameters.PrimaryScreenHeight - 200);
            this.Width = (System.Windows.SystemParameters.PrimaryScreenWidth);

            this.DataContext = _dataContext;

            _dataContext.LoadDetails();

            var customerData = _dataContext.GetCLIAMMI(_dataContext.Head.FTCODC ?? 0);

            if (!_dataContext.Head.FTCODD.HasValue || _dataContext.Head.FTCODD.Value == 0)
            {
                // defaults from customer
                _dataContext.Head.DefaultFirstAgent = new AGENTI() { agecod = customerData?.CLAGEN ?? string.Empty, agepvg = customerData?.CLAGENP, agepvgt = customerData?.CLAGENPT, agedes = string.Empty, ageflag = string.Empty };
                _dataContext.Head.DefaultSecondAgent = new AGENTI() { agecod = customerData?.CLAGEN2 ?? string.Empty, agepvg = customerData?.CLAGEN2P, agepvgt = customerData?.CLAGEN2PT, agedes = string.Empty, ageflag = string.Empty };
            }
            else
            {
                var recipient =_dataContext.GetDESTINATARI(_dataContext.Head.FTCODC!.Value, _dataContext.Head.FTCODD.Value);

                _dataContext.Head.DefaultFirstAgent = !string.IsNullOrWhiteSpace(recipient?.decoag1) ?
                    new AGENTI() { agecod = recipient?.decoag1 ?? string.Empty, agepvg = recipient?.deage1p, agepvgt = recipient?.deage1pt, agedes = string.Empty, ageflag = string.Empty } :
                    new AGENTI() { agecod = customerData?.CLAGEN ?? string.Empty, agepvg = customerData?.CLAGENP, agepvgt = customerData?.CLAGENPT, agedes = string.Empty, ageflag = string.Empty };
                _dataContext.Head.DefaultSecondAgent = !string.IsNullOrWhiteSpace(recipient?.decoag2) ?
                    new AGENTI() { agecod = recipient?.decoag2 ?? string.Empty, agepvg = recipient?.deage2p, agepvgt = recipient?.deage2pt, agedes = string.Empty, ageflag = string.Empty } :
                    new AGENTI() { agecod = customerData?.CLAGEN2 ?? string.Empty, agepvg = customerData?.CLAGEN2P, agepvgt = customerData?.CLAGEN2PT, agedes = string.Empty, ageflag = string.Empty };
            }
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

            _dataContext.Rows = _dataContext.GetList() ?? new ObservableCollection<FATTD00F>();

            Loaded += (sender, e) =>
            {
                this.Title = $"Dettagli della fattura {_dataContext.Head.PrintFullID}";
                if (_dataContext.IsReadonly)
                    Title += " - [sola lettura]";
                MoveFocus(new TraversalRequest(System.Windows.Input.FocusNavigationDirection.First));
            };
        }

        #region Buttons
        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            string? validated = _dataContext.ValidateModel();
            if (string.IsNullOrEmpty(validated))
            {
                if (_dataContext.UpdateAll())
                    this.DialogResult = true;
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
            var items = rgvRows.Items.Cast<FATTD00F>();

            var customerData = _dataContext.GetCLIENTI(_dataContext.Head.FTCODC!.Value);

            var data = new FATTD00F
            {
               ftsoci = _dataContext.Head.ftsoci,
               FDRIGA = items.Any() ? items.Where(w => w.FDRIGA != 999999).Max(max => max.FDRIGA) + 1 : 1,
               FTANNO = _dataContext.Head.FTANNO,
               FTNUOR = _dataContext.Head.FTNUOR,
               FDTQTA = "V",
               FDTPRE = "U",
               FDSHOW = false,
               FDQTAV = 0,
               FDPREZ = 0,
               FDCECO = _dataContext.Head.FTCECO,
               AccountCache = _dataContext.AccountCache,
               SubaccountCache = _dataContext.SubaccountCache,
               GroupsList = _dataContext.GroupsList,
               UMsCache = _dataContext.UMsCache,
               Products = _dataContext.Products,
               RatesList = _dataContext.RatesList,
               NaturaList = _dataContext.NaturaList,
               Agents = _dataContext.Agents,
               CostCentersList = _dataContext.CostCentersList,
               CustomerID = _dataContext.Head.FTCODC ?? 0,
               RecipientID = _dataContext.Head.FTCODD ?? 0,
               InvoiceDate = _dataContext.Head.FTDAOR!.Value,
               SelectedRate = _dataContext.RatesList?.Where(w => w.asscod == customerData?.classo && w.assali == customerData?.classa).FirstOrDefault(),
            };
            data.QuantityValueChanged += _dataContext.OnQuantityValueChanged;

            e.NewObject = data;

            var grid = e.OwnerGridViewItemsControl;
            grid.CurrentColumn = grid.Columns[1];
        }

        private void rgvRows_RowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            // TODO Check edit trigger
            if (e.EditOperationType != Telerik.Windows.Controls.GridView.GridViewEditOperationType.None)
            {
                var item = e.Row.Item as FATTD00F;

                if (item != null)
                {
                    // set row for stamp
                    if (item.FDTQTA == "B")
                    {
                        item.FDRIGA = 999999;
                    }
                    var validated = _dataContext.Validate(item);
                    if (validated != null)
                    {
                        Dispatcher.BeginInvoke(() => { ErrorHandler.Validation(validated); });
                    }
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
            var item = (sender as RadGlyph)?.DataContext as FATTD00F;
            if (item != null)
            {
                var clonedItem = new FATTD00F()
                {
                    ftsoci = item.ftsoci,
                    FTANNO = item.FTANNO,
                    FTNUOR = item.FTNUOR,
                    FDRIGA = item.FDTQTA == "B" ? 999999 : item.FDRIGA,
                    FDCODA = item.FDCODA,
                    FDQTAV = item.FDQTAV,
                    FDTQTA = item.FDTQTA,
                    FDPREZ = item.FDPREZ,
                    FDSCO1 = item.FDSCO1,
                    FDSCO2 = item.FDSCO2,
                    FDSCO3 = item.FDSCO3,
                    FDMAGG = item.FDMAGG,
                    FDTPRE = item.FDTPRE,
                    FDTSC1 = item.FDTSC1,
                    FDTSC2 = item.FDTSC2,
                    FDTSC3 = item.FDTSC3,
                    FDTMAG = item.FDTMAG,
                    FDALIV = item.FDALIV,
                    FDASSF = item.FDASSF,
                    FDDACO = item.FDDACO,
                    FDRIFC = item.FDRIFC,
                    FDGRUP = item.FDGRUP,
                    FDCONT = item.FDCONT,
                    FDSCTO = item.FDSCTO,
                    FDUNIM = item.FDUNIM,
                    fdtiva = item.fdtiva,
                    FDNOTE = item.FDNOTE,
                    FDSHOW = item.FDSHOW,
                    FDSTAMP = item.FDSTAMP,
                    FDCOAG1 = item.FDCOAG1,
                    FDCOAG2 = item.FDCOAG2,
                    FDPROV = item.FDPROV,
                    fdpro2 = item.fdpro2,
                    FDCOAG1PT = item.FDCOAG1PT,
                    FDCOAG2PT = item.FDCOAG2PT,
                    FDCECO = item.FDCECO,
                    CustomerID = _dataContext.Head.FTCODC ?? 0,
                    RecipientID = _dataContext.Head.FTCODD ?? 0,
                    InvoiceDate = _dataContext.Head.FTDAOR!.Value
                };
                clonedItem.NaturaList = item.NaturaList;
                clonedItem.AccountCache = item.AccountCache;
                clonedItem.SubaccountCache = item.SubaccountCache;
                clonedItem.RatesList = item.RatesList;
                clonedItem.UMsCache = item.UMsCache;
                clonedItem.Products = item.Products;
                clonedItem.GroupsList = item.GroupsList;
                clonedItem.AccountsList = item.AccountsList;
                clonedItem.SubaccountsList = item.SubaccountsList;
                clonedItem.Agents = item.Agents;
                clonedItem.CostCentersList = item.CostCentersList;
                clonedItem.QuantityValueChanged += _dataContext.OnQuantityValueChanged;

                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<FATTD00FMagnifierWindowViewModel>();
                windowViewModel.Title = "Dettagli della riga di fattura";
                windowViewModel.Row = clonedItem;
                windowViewModel.IsReadonly = _dataContext.IsReadonly;

                var wMagnifier = new FATTD00FMagnifierWindow(windowViewModel);
                wMagnifier.Owner = Window.GetWindow(this);
                if (wMagnifier.ShowDialog() == true)
                {
                    // update row data
                    item.SelectedProduct = windowViewModel.Row.SelectedProduct;
                    item.FDQTAV = windowViewModel.Row.FDQTAV;
                    item.FDTQTA = windowViewModel.Row.FDTQTA;
                    item.FDPREZ = windowViewModel.Row.FDPREZ;
                    item.FDSCO1 = windowViewModel.Row.FDSCO1;
                    item.FDSCO2 = windowViewModel.Row.FDSCO2;
                    item.FDSCO3 = windowViewModel.Row.FDSCO3;
                    item.FDMAGG = windowViewModel.Row.FDMAGG;
                    item.FDTPRE = windowViewModel.Row.FDTPRE;
                    item.FDTSC1 = windowViewModel.Row.FDTSC1;
                    item.FDTSC2 = windowViewModel.Row.FDTSC2;
                    item.FDTSC3 = windowViewModel.Row.FDTSC3;
                    item.FDTMAG = windowViewModel.Row.FDTMAG;
                    item.SelectedRate = windowViewModel.Row.SelectedRate;
                    item.FDASSF = windowViewModel.Row.FDASSF;
                    item.FDDACO = windowViewModel.Row.FDDACO;
                    item.FDRIFC = windowViewModel.Row.FDRIFC;
                    item.SelectedGroup = windowViewModel.Row.SelectedGroup;
                    item.SelectedAccount = windowViewModel.Row.SelectedAccount;
                    item.SelectedSubaccount = windowViewModel.Row.SelectedSubaccount;
                    item.FDUNIM = windowViewModel.Row.FDUNIM;
                    item.FDNOTE = windowViewModel.Row.FDNOTE;
                    item.FDSHOW = windowViewModel.Row.FDSHOW;
                    item.FirstAgent = windowViewModel.Row.FirstAgent;
                    item.SecondAgent = windowViewModel.Row.SecondAgent;
                    item.SelectedCostCenter = windowViewModel.Row.SelectedCostCenter;
                    item.FDPROV = windowViewModel.Row.FDPROV;
                    item.fdpro2 =windowViewModel.Row.fdpro2;
                    item.FDCOAG1PT = windowViewModel.Row.FDCOAG1PT;
                    item.FDCOAG2PT = windowViewModel.Row.FDCOAG2PT;
                    item.FDSTAMP = windowViewModel.Row.FDSTAMP;

                    _dataContext.ViewUpdate = true;
                }
            }
        }
        #endregion
    }
}
