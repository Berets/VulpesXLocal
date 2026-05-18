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
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Tables.Accounting;

namespace VulpesX.Modules.Ufp.Tables.Accounting
{
    /// <summary>
    /// Interaction logic for TAB_ACC_TIPINCWindow.xaml
    /// </summary>
    public partial class TAB_ACC_TIPINCWindow : FluentDefaultWindow
    {
        private TAB_ACC_TIPINCWindowViewModel _dataContext;
        public TAB_ACC_TIPINCWindow(TAB_ACC_TIPINCWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;
            if (!_dataContext.IsInsert)
            {
                _dataContext.SelectedCausal = _dataContext.Causals?.Where(w => w.caucod == _dataContext.Data.icscau).FirstOrDefault();
                _dataContext.SelectedFE = _dataContext.FEs?.Where(w => w.FEPACOD == _dataContext.Data.icsfepacod).FirstOrDefault();
            }
        }

        #region Buttons
        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            var validated = _dataContext.Validate();

            if (string.IsNullOrWhiteSpace(validated))
            {

                if (_dataContext.Save())
                {
                    Mouse.OverrideCursor = null;
                    this.DialogResult = true;
                }

            }
            else
            { Mouse.OverrideCursor = null; ErrorHandler.Validation(validated); }
        }

        #endregion

        #region Autocompletes
        private void acCausal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedCausal != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedCausal.caucod))
            {
                _dataContext.Data.icscau = _dataContext.SelectedCausal.caucod;
            }
        }
        private void acFE_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedFE != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedFE.FEPACOD))
            {
                _dataContext.Data.icsfepacod = _dataContext.SelectedFE.FEPACOD;
            }
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
