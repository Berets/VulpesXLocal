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
    /// Interaction logic for ORDID00FWindow.xaml
    /// </summary>
    public partial class ORDID00FWindow : FluentDefaultWindow
    {
        private ORDID00FWindowViewModel _dataContext;

        private RadCallout coPrices = new RadCallout()
        {
            Background = new SolidColorBrush(Colors.Black),
            Padding = new Thickness(0),
            Width = 650,
            Height = 130,
            StrokeThickness = 1,
            BorderBrush = new SolidColorBrush(Colors.MediumVioletRed),
            UseLayoutRounding = true,
            ArrowAnchorPoint = new Point(-0.15, 0.5),
            ArrowBasePoint1 = new Point(1, 0),
            ArrowBasePoint2 = new Point(1, 1)
        };
        private RadCallout coQuantities = new RadCallout()
        {
            Background = new SolidColorBrush(Colors.Black),
            Padding = new Thickness(0),
            Width = 650,
            Height = 350,
            StrokeThickness = 1,
            BorderBrush = new SolidColorBrush(Colors.MediumVioletRed),
            UseLayoutRounding = true,
            ArrowAnchorPoint = new Point(1.15, 0.5),
            ArrowBasePoint1 = new Point(0.7, 0.3),
            ArrowBasePoint2 = new Point(0.7, 0.7)
        };

        public ORDID00FWindow(ORDID00FWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.Height = (System.Windows.SystemParameters.PrimaryScreenHeight - 200);
            this.Width = (System.Windows.SystemParameters.PrimaryScreenWidth);

            this.DataContext = dataContext;

            CalloutPopupService.CloseAll();

            _dataContext.LoadDetails();

            // get default agents
            var customerData = _dataContext.GetCLIAMMI(_dataContext.Head.OTCLIE ?? 0);
            if (!_dataContext.Head.DESTIN.HasValue || _dataContext.Head.DESTIN.Value == 0)
            {
                // defaults from customer
                _dataContext.Head.DefaultFirstAgent = new AGENTI() { agecod = customerData?.CLAGEN ?? string.Empty, agepvg = customerData?.CLAGENP, agepvgt = customerData?.CLAGENPT, agedes = string.Empty, ageflag = string.Empty };
                _dataContext.Head.DefaultSecondAgent = new AGENTI() { agecod = customerData?.CLAGEN2 ?? string.Empty, agepvg = customerData?.CLAGEN2P, agepvgt = customerData?.CLAGEN2PT, agedes = string.Empty, ageflag = string.Empty };
            }
            else
            {
                var recipient = _dataContext.GetDESTINATARI(_dataContext.Head.OTCLIE!.Value, _dataContext.Head.DESTIN.Value);

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

            _dataContext.Rows = _dataContext.Head.Rows ?? new ObservableCollection<ORDID00F>();
            _dataContext.HasMergedSigns = (UserContext.Instance!.ACCESS!.Roles?.canOrdersSignTech ?? false) && (UserContext.Instance!.ACCESS!.Roles?.canOrdersSignCommercial ?? false);

            Loaded += (sender, e) =>
            {
                this.Title = $"Dettagli dell'offerta {_dataContext.Head.PrintFullID} - del {_dataContext.Head.OTDAOR?.ToString("d")} - {_dataContext.Head.Customer?.FullDescriptionSearchable}";
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
                if ((_dataContext.Head.OTFICO.HasValue || _dataContext.Head.OTFITE.HasValue) && !_dataContext.HasMergedSigns)
                {
                    if (ConfirmHandler.Confirm("Questo ordine ha già delle firme apposte, proseguendo verranno annullate e bisognerà richiederle nuovamente, procedere ?"))
                    {
                        // clear signs
                        _dataContext.Head.OTFITE = null;
                        _dataContext.Head.OTFITEUSR = null;
                        _dataContext.Head.OTFICO = null;
                        _dataContext.Head.OTFICOUSR = null;
                        _dataContext.Head.OTINFI = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
                        _dataContext.Head.OTINFIUSR = _dataContext.UserID;
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
                        _dataContext.Head.OTINFI = now;
                        _dataContext.Head.OTINFIUSR = _dataContext.UserID;
                        _dataContext.Head.OTFITE = now;
                        _dataContext.Head.OTFITEUSR = _dataContext.UserID;
                        _dataContext.Head.OTFICO = now;
                        _dataContext.Head.OTFICOUSR = _dataContext.UserID;
                        _dataContext.Head.flgchi = "F";
                    }
                }
                if (proceed)
                {
                    _dataContext.Head.updatedUserID = _dataContext.UserID;
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
            var items = rgvRows.Items.Cast<ORDID00F>();

            var customerData = _dataContext.GetCLIENTI(_dataContext.Head.OTCLIE!.Value);

            var data = new ORDID00F
            {
                otsoci = _dataContext.Head.otsoci,
                ODRIGA = items.Any() ? items.Max(max => max.ODRIGA) + 1 : 1,
                OTANNO = _dataContext.Head.OTANNO,
                OTNUOR = _dataContext.Head.OTNUOR,
                ODDARI = _dataContext.Head.OTDARI,
                ODDACO = _dataContext.Head.OTDATC,
                ODDAPA = _dataContext.Head.OTDAPA,
                ODQTAV = 0,
                ODTQTA = "V",
                ODPREZ = 0,
                ODTPRE = "U",
                ODSHOW = false,
                AllAccounts = _dataContext.AllAccounts,
                AllSubccounts = _dataContext.AllSubccounts,
                GroupsList = _dataContext.GroupsList,
                UMsCache = _dataContext.UMsCache,
                Products = _dataContext.Products,
                RatesList = _dataContext.RatesList,
                AgentsList = _dataContext.AgentsList,
                CustomerID = _dataContext.Head.OTCLIE ?? 0,
                RecipientID = _dataContext.Head.DESTIN ?? 0,
                OrderDate = _dataContext.Head.OTDAOR!.Value,
                CanEdit = true,
                ODCODA = string.Empty
            };

            data.QuantityValueChanged += _dataContext.OnQuantityValueChanged;

            if (_dataContext.Head.DefaultFirstAgent != null)
                data.SelectedFirstAgent = data.AgentsList?.Where(w => w.agecod == _dataContext.Head.DefaultFirstAgent.agecod).FirstOrDefault();
            if (_dataContext.Head.DefaultSecondAgent != null)
                data.SelectedSecondAgent = data.AgentsList?.Where(w => w.agecod == _dataContext.Head.DefaultSecondAgent.agecod).FirstOrDefault();

            data.SelectedRate = _dataContext.RatesList?.Where(w => w.asscod == customerData?.classo && w.assali == customerData?.classa).FirstOrDefault();

            e.NewObject = data;
            var grid = e.OwnerGridViewItemsControl;
            grid.CurrentColumn = grid.Columns[1];
        }

        private void rgvRows_RowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            if (e.EditOperationType != Telerik.Windows.Controls.GridView.GridViewEditOperationType.None)
            {
                var item = e.Row.Item as ORDID00F;
                if (item != null)
                {
                    var validated = _dataContext.Validate(item);

                    if (!string.IsNullOrEmpty(validated))
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

        private void rgvRows_BeginningEdit(object sender, GridViewBeginningEditRoutedEventArgs e)
        {
            var model = e.Row.Item as ORDID00F;

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

        private void rgvRows_Deleting(object sender, GridViewDeletingEventArgs e)
        {
            var model = e.Items.First() as ORDID00F;

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
            var item = (sender as RadGlyph)?.DataContext as ORDID00F;

            if (item != null)
            {
                var clonedItem = new ORDID00F()
                {
                    otsoci = item.otsoci,
                    OTANNO = item.OTANNO,
                    OTNUOR = item.OTNUOR,
                    ODCODA = item.ODCODA,
                    ODQTAV = item.ODQTAV,
                    ODTQTA = item.ODTQTA,
                    ODPREZ = item.ODPREZ,
                    ODSCO1 = item.ODSCO1,
                    ODSCO2 = item.ODSCO2,
                    ODSCO3 = item.ODSCO3,
                    ODMAGG = item.ODMAGG,
                    ODTPRE = item.ODTPRE,
                    ODTSC1 = item.ODTSC1,
                    ODTSC2 = item.ODTSC2,
                    ODTSC3 = item.ODTSC3,
                    ODTMAG = item.ODTMAG,
                    ODALIV = item.ODALIV,
                    ODASSF = item.ODASSF,
                    ODDACO = item.ODDACO,
                    ODDAPA = item.ODDAPA,
                    ODRIFC = item.ODRIFC,
                    ODGRUP = item.ODGRUP,
                    ODCONT = item.ODCONT,
                    ODSCTO = item.ODSCTO,
                    odunit = item.odunit,
                    ODDARI = item.ODDARI,
                    ODNOTE = item.ODNOTE,
                    ODSHOW = item.ODSHOW,
                    ODSTA = item.ODSTA,
                    ODCOA1 = item.ODCOA1,
                    ODCOA2 = item.ODCOA2,
                    ODCOA1P = item.ODCOA1P,
                    ODCOA2P = item.ODCOA2P,
                    ODCOA1PT = item.ODCOA1PT,
                    ODCOA2PT = item.ODCOA2PT,
                    ODQTAEV = item.ODQTAEV,
                    IsHeadReadonly = item.IsHeadReadonly,
                    CanEdit = item.CanEdit,
                    CustomerID = _dataContext.Head.OTCLIE ?? 0,
                    RecipientID = _dataContext.Head.DESTIN ?? 0,
                    OrderDate = _dataContext.Head.OTDAOR!.Value
                };
                clonedItem.RatesList = item.RatesList;
                clonedItem.UMsCache = item.UMsCache;
                clonedItem.Products = item.Products;
                clonedItem.AllAccounts = item.AllAccounts;
                clonedItem.AllSubccounts = item.AllSubccounts;
                clonedItem.GroupsList = item.GroupsList;
                clonedItem.AccountsList = item.AccountsList;
                clonedItem.SubaccountsList = item.SubaccountsList;
                clonedItem.AgentsList = item.AgentsList;
                clonedItem.QuantityValueChanged += _dataContext.OnQuantityValueChanged;

                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ORDID00FMagnifierWindowViewModel>();
                windowViewModel.Title = "Dettagli della riga d'ordine";
                windowViewModel.Row = clonedItem;
                windowViewModel.IsReadonly = _dataContext.IsReadonly;

                var wMagnifier = new ORDID00FMagnifierWindow(windowViewModel);
                wMagnifier.Owner = Window.GetWindow(this);
                if (wMagnifier.ShowDialog() == true)
                {
                    // update row data
                    item.SelectedProduct = windowViewModel.Row.SelectedProduct;
                    item.ODQTAV = windowViewModel.Row.ODQTAV;
                    item.ODTQTA = windowViewModel.Row.ODTQTA;
                    item.ODPREZ = windowViewModel.Row.ODPREZ;
                    item.ODSCO1 = windowViewModel.Row.ODSCO1;
                    item.ODSCO2 = windowViewModel.Row.ODSCO2;
                    item.ODSCO3 = windowViewModel.Row.ODSCO3;
                    item.ODMAGG = windowViewModel.Row.ODMAGG;
                    item.ODTPRE = windowViewModel.Row.ODTPRE;
                    item.ODTSC1 = windowViewModel.Row.ODTSC1;
                    item.ODTSC2 = windowViewModel.Row.ODTSC2;
                    item.ODTSC3 = windowViewModel.Row.ODTSC3;
                    item.ODTMAG = windowViewModel.Row.ODTMAG;
                    item.SelectedRate = windowViewModel.Row.SelectedRate;
                    item.ODASSF = windowViewModel.Row.ODASSF;
                    item.ODDACO = windowViewModel.Row.ODDACO;
                    item.ODDAPA = windowViewModel.Row.ODDAPA;
                    item.ODRIFC = windowViewModel.Row.ODRIFC;
                    item.SelectedGroup = windowViewModel.Row.SelectedGroup;
                    item.SelectedAccount = windowViewModel.Row.SelectedAccount;
                    item.SelectedSubaccount = windowViewModel.Row.SelectedSubaccount;
                    item.odunit = windowViewModel.Row.odunit;
                    item.ODDARI = windowViewModel.Row.ODDARI;
                    item.ODNOTE = windowViewModel.Row.ODNOTE;
                    item.ODSHOW = windowViewModel.Row.ODSHOW;
                    item.ODSTA = windowViewModel.Row.ODSTA;
                    item.SelectedFirstAgent = windowViewModel.Row.SelectedFirstAgent;
                    item.SelectedSecondAgent = windowViewModel.Row.SelectedSecondAgent;
                    item.ODCOA1P = windowViewModel.Row.ODCOA1P;
                    item.ODCOA2P = windowViewModel.Row.ODCOA2P;
                    item.ODCOA1PT = windowViewModel.Row.ODCOA1PT;
                    item.ODCOA2PT = windowViewModel.Row.ODCOA2PT;
                    item.ODQTAEV = windowViewModel.Row.ODQTAEV;

                    _dataContext.ViewUpdate = true;
                }
            }
        }
        #endregion

    }
}
