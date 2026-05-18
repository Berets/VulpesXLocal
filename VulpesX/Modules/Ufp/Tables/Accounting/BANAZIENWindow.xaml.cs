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
using Telerik.Windows.Persistence.Core;
using VulpesX.DAL.Accounting;
using VulpesX.Shared;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Tables.Accounting;

namespace VulpesX.Modules.Ufp.Tables.Accounting
{
    /// <summary>
    /// Interaction logic for BANAZIENWindow.xaml
    /// </summary>
    public partial class BANAZIENWindow : FluentDefaultWindow
    {
        private BANAZIENWindowViewModel _dataContext;

        public BANAZIENWindow(BANAZIENWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            _dataContext.ABICABs = _dataContext.GetABICAB();
            _dataContext.Data.AllAccounts = _dataContext.GetPDCCONTI();
            _dataContext.Data.AllSubccounts = _dataContext.GetPDCSOTTO();
            _dataContext.Data.GroupsList = _dataContext.GetPDCGRUPPI();

            if (!_dataContext.IsInsert)
            {
                _dataContext.SelectedABICAB = _dataContext.ABICABs?.Where(w => w.ABI == _dataContext.Data.abiabi && w.CAB == _dataContext.Data.abicab).FirstOrDefault();
                _dataContext.SelectedISO = _dataContext.ISOs?.Where(w => w.isocod == _dataContext.Data.abiisocod).FirstOrDefault();
                _dataContext.SelectedFromEffectsCausal = _dataContext.Causals?.Where(w => w.caucod == _dataContext.Data.abicau3).FirstOrDefault();
                _dataContext.SelectedToEffectsCausal = _dataContext.Causals?.Where(w => w.caucod == _dataContext.Data.abicau1).FirstOrDefault();
                _dataContext.SelectedToAnticipationCausal = _dataContext.Causals?.Where(w => w.caucod == _dataContext.Data.abicau2).FirstOrDefault();
                _dataContext.SelectedFromAnticipationCausal = _dataContext.Causals?.Where(w => w.caucod == _dataContext.Data.abicau4).FirstOrDefault();
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

        #region Autocompletes
        private void acACs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedABICAB != null && _dataContext.SelectedABICAB.ABI > 0 && _dataContext.SelectedABICAB.CAB > 0)
            {
                _dataContext.Data.abiabi = _dataContext.SelectedABICAB.ABI;
                _dataContext.Data.abicab = _dataContext.SelectedABICAB.CAB;
            }
        }
        private void acISO_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedISO != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedISO.isocod))
            {
                _dataContext.Data.abiisocod = _dataContext.SelectedISO.isocod;
            }
            else
            {
                _dataContext.Data.abiisocod = null;

            }
        }
        private void acFromEffectsCausal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedFromEffectsCausal != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedFromEffectsCausal?.caucod))
            {
                _dataContext.Data.abicau3 = _dataContext.SelectedFromEffectsCausal.caucod;
            }
            else
            { _dataContext.Data.abicau3 = null; }
        }
        private void acToEffectsCausal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedToEffectsCausal != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedToEffectsCausal?.caucod))
            {
                _dataContext.Data.abicau1 = _dataContext.SelectedToEffectsCausal.caucod;
            }
            else
            { _dataContext.Data.abicau1 = null; }
        }
        private void acToAnticipationCausal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedToAnticipationCausal != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedFromAnticipationCausal?.caucod))
            {
                _dataContext.Data.abicau2 = _dataContext.SelectedToAnticipationCausal.caucod;
            }
            else
            { _dataContext.Data.abicau2 = null; }
        }
        private void acFromAnticipationCausal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedFromAnticipationCausal != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedFromAnticipationCausal?.caucod))
            {
                _dataContext.Data.abicau4 = _dataContext.SelectedFromAnticipationCausal.caucod;
            }
            else
            { _dataContext.Data.abicau4 = null; }
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
    }
}
