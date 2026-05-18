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

namespace VulpesX.Modules.Default.Tables.Accounting
{
    /// <summary>
    /// Interaction logic for RITENUTEWindow.xaml
    /// </summary>
    public partial class RITENUTEWindow : FluentDefaultWindow
    {
        private RITENUTEWindowViewModel _dataContext;
        public RITENUTEWindow(RITENUTEWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            _dataContext.LoadData();

            if (!_dataContext.IsInsert)
            {
                _dataContext.SelectedCausal = _dataContext.Causals?.Where(w => w.caucod == _dataContext.Data.ritpag).FirstOrDefault();
                _dataContext.SelectedAssessmentCausal = _dataContext.Causals?.Where(w => w.caucod == _dataContext.Data.ritca1).FirstOrDefault();
                _dataContext.SelectedPaymentCausal = _dataContext.Causals?.Where(w => w.caucod == _dataContext.Data.ritca2).FirstOrDefault();
            }
        }

        #region Buttons
        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

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
                _dataContext.Data.ritpag = _dataContext.SelectedCausal.caucod;
            }
        }
        private void acAssessmentCausal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedAssessmentCausal != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedAssessmentCausal.caucod))
            {
                _dataContext.Data.ritca1 = _dataContext.SelectedAssessmentCausal.caucod;
            }
        }
        private void acPaymentCausal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedPaymentCausal != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedPaymentCausal.caucod))
            {
                _dataContext.Data.ritca2 = _dataContext.SelectedPaymentCausal.caucod;
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

        private void ac_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = ((RadAutoCompleteBox)sender).ChildrenOfType<TextBox>().First();
            Dispatcher.BeginInvoke(new Action(() => { textBox.SelectAll(); }));
        }

        #endregion
    }
}
