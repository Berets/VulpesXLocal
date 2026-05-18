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
using VulpesX.Models.Ufp;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Ufp.Tables.Accounting;

namespace VulpesX.Modules.Ufp.Tables.Accounting
{
    /// <summary>
    /// Interaction logic for COMTIPREGWindow.xaml
    /// </summary>
    public partial class COMTIPREGWindow : FluentDefaultWindow
    {
        private COMTIPREGWindowViewModel _dataContext;
        public COMTIPREGWindow(COMTIPREGWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            _dataContext.AccountCache = _dataContext.GetPDCCONTI();
            _dataContext.SubaccountCache = _dataContext.GetPDCSOTTO();
            _dataContext.GroupsList = _dataContext.GetPDCGRUPPI();
            _dataContext.CausalsList = _dataContext.GetCAUCONT();

            if (!_dataContext.IsInsert)
            {
                _dataContext.Details = _dataContext.GetCOMTIPREGLEVEL1() ?? new System.Collections.ObjectModel.ObservableCollection<COMTIPREGLEVEL1>();

                _dataContext.SelectedCausal = _dataContext.CausalsList?.Where(w => w.caucod == _dataContext.Data.causcon).FirstOrDefault();
            }
            else
            {
                _dataContext.Details = new System.Collections.ObjectModel.ObservableCollection<COMTIPREGLEVEL1>();
            }

            foreach (var item in _dataContext.Details ?? new System.Collections.ObjectModel.ObservableCollection<COMTIPREGLEVEL1>())
            {
                item.AccountCache = _dataContext.AccountCache;
                item.SubaccountCache = _dataContext.SubaccountCache;
                item.GroupsList = _dataContext.GroupsList;
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
                DialogResult = _dataContext.Save();
            }
            else
            {
                ErrorHandler.Show(validated);
            }
        }

        #region Autocomplete
        private void acCausal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedCausal != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedCausal.caucod))
            {
                _dataContext.Data.causcon = _dataContext.SelectedCausal.caucod;
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
        private void rgvDetails_AddingNewDataItem(object sender, GridViewAddingNewEventArgs e)
        {
            var data = new COMTIPREGLEVEL1 { Cocodso = _dataContext.CompanyID, causcon = _dataContext.Data.causcon, cauprco = _dataContext.Data.cauprco };
            data.IsInsert = true;
            data.Conriga = (short?)((_dataContext.Details ?? new System.Collections.ObjectModel.ObservableCollection<COMTIPREGLEVEL1>()).Any() ? _dataContext.Details!.Max(max => max.Conriga) + 1 : 1);
            data.Cosegno = "D";
            data.AccountCache = _dataContext.AccountCache;
            data.SubaccountCache = _dataContext.SubaccountCache;
            data.GroupsList = _dataContext.GroupsList;

            e.NewObject = data;

            var grid = e.OwnerGridViewItemsControl;
            grid.CurrentColumn = grid.Columns[0];

        }

        private void rgvDetails_RowEditEnded(object sender, GridViewRowEditEndedEventArgs e)
        {
            rgvDetails.ScrollIntoView(e.Row.Item, rgvDetails.Columns[0]);
        }

        private void rgvDetails_RowValidating(object sender, GridViewRowValidatingEventArgs e)
        {
            var item = e.Row.Item as COMTIPREGLEVEL1;

            if (item != null)
            {
                var validated = _dataContext.ValidateRow(item);

                if (validated != null)
                {
                    ErrorHandler.Show(validated);
                }

                e.IsValid = string.IsNullOrWhiteSpace(validated);
            }
        }

        private void rgvDetails_RowValidated(object sender, GridViewRowValidatedEventArgs e)
        {
            var data = e.Row.Item as COMTIPREGLEVEL1;

            if (data != null)
            {
                data.Cogrup = data.SelectedGroup?.P1GRUP;
                data.Cocont = data.SelectedAccount?.P2CONT;
                data.CoSotc = data.SelectedSubaccount?.P3SOTC;
            }
        }
    }
}
