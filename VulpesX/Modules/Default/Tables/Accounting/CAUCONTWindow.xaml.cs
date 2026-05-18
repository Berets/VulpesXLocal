using System;
using System.Collections.Generic;
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
using VulpesX.Models;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Tables.Accounting;

namespace VulpesX.Modules.Default.Tables.Accounting
{
    /// <summary>
    /// Interaction logic for CAUCONTWindow.xaml
    /// </summary>
    public partial class CAUCONTWindow : FluentDefaultWindow, IWindowFactory
    {
        private CAUCONTWindowViewModel _dataContext;
        public CAUCONTWindow(CAUCONTWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            _dataContext.AccountCache = _dataContext.GetPDCCONTI();
            _dataContext.SubaccountCache = _dataContext.GetPDCSOTTO();
            _dataContext.GroupsList = _dataContext.GetPDCGRUPPI();
            _dataContext.CausalsList = _dataContext.GetCAUCONT();
            _dataContext.CostCenters = _dataContext.GetTCECO00F();

            if (!_dataContext.IsInsert)
            {
                _dataContext.CounterpartsRows = _dataContext.GetCAUCONT_GROUPS();

                _dataContext.SelectedRate = _dataContext.Rates?.Where(w => w.asscod == _dataContext.Data.cauass && w.assali == _dataContext.Data.cauali).FirstOrDefault();
                _dataContext.SelectedIVABook = _dataContext.IVABooks?.Where(w => w.livcod == _dataContext.Data.cauliv).FirstOrDefault();
                _dataContext.SelectedCostCenter = _dataContext.CostCenters?.Where(w => w.cecodc == _dataContext.Data.cauceco).FirstOrDefault();
            }

            foreach (var item in _dataContext.CounterpartsRows ?? new System.Collections.ObjectModel.ObservableCollection<CAUCONT_GROUPS>())
            {
                item.AccountCache = _dataContext.AccountCache;
                item.SubaccountCache = _dataContext.SubaccountCache;
                item.GroupsList = _dataContext.GroupsList;
                item.CausalsList = _dataContext.CausalsList;
            }
        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            var validated = _dataContext.Validate();

            if (string.IsNullOrWhiteSpace(validated))
            {
                if (_dataContext.IsInsert)
                    _dataContext.Data.addedUserID = UserContext.Instance.UserName;
                else
                    _dataContext.Data.updatedUserID = UserContext.Instance.UserName;

                DialogResult = _dataContext.Save();
            }
            else
            {
                ErrorHandler.Show(validated);
            }
        }

        #region Autocompletes
        private void acRate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedRate != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedRate.asscod))
            {
                _dataContext.Data.cauass = _dataContext.SelectedRate.asscod;
                _dataContext.Data.cauali = _dataContext.SelectedRate.assali;
            }
        }
        private void acIVABook_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedIVABook != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedIVABook.livcod))
            {
                _dataContext.Data.cauliv = _dataContext.SelectedIVABook.livcod;
            }
            else
            {
                _dataContext.Data.cauliv = null;
            }
        }
        private void acCostCenter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedCostCenter != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedCostCenter.cecodc))
            {
                _dataContext.Data.cauceco = _dataContext.SelectedCostCenter.cecodc;
            }
            else
            {
                _dataContext.Data.cauceco = null;
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

        #region Counterparts grid
        private void rgvCounterpartsRows_AddingNewDataItem(object sender, Telerik.Windows.Controls.GridView.GridViewAddingNewEventArgs e)
        {
            var data = new CAUCONT_GROUPS { caucod = _dataContext.Data.caucod, grpsoc = _dataContext.CompanyID };
            data.IsInsert = true;
            data.prog = (_dataContext.CounterpartsRows ?? new System.Collections.ObjectModel.ObservableCollection<CAUCONT_GROUPS>()).Any() ? _dataContext.CounterpartsRows!.Max(max => max.prog) + 1 : 1;
            data.grpseg = "D";
            data.AccountCache = _dataContext.AccountCache;
            data.SubaccountCache = _dataContext.SubaccountCache;
            data.GroupsList = _dataContext.GroupsList;
            data.CausalsList = _dataContext.CausalsList;

            e.NewObject = data;

            var grid = e.OwnerGridViewItemsControl;
            grid.CurrentColumn = grid.Columns[0];
        }

        private void rgvCounterpartsRows_RowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            var item = e.Row.Item as CAUCONT_GROUPS;

            if (item != null)
            {
                var validated = _dataContext.ValidateCAUCONT_GROUPS(item, item.IsInsert);

                if (validated != null)
                {
                    ErrorHandler.Show(validated);
                }

                e.IsValid = string.IsNullOrWhiteSpace(validated);
            }
        }

        private void rgvCounterpartsRows_RowValidated(object sender, GridViewRowValidatedEventArgs e)
        {
            var data = e.Row.Item as CAUCONT_GROUPS;

            if (data != null)
            {
                data.grpgrp = data.SelectedGroup?.P1GRUP;
                data.grpcto = data.SelectedAccount?.P2CONT;
                data.grpsct = data.SelectedSubaccount?.P3SOTC;
            }
        }

        private void rgvCounterpartsRows_RowEditEnded(object sender, GridViewRowEditEndedEventArgs e)
        {
            rgvCounterpartsRows.ScrollIntoView(e.Row.Item, rgvCounterpartsRows.Columns[0]);
        }
        #endregion

    }
}
