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
using VulpesX.ViewModels.Modules.Default.Tables.CRM;

namespace VulpesX.Modules.Default.Tables.CRM
{
    /// <summary>
    /// Interaction logic for TAB_CRM_CAUOFFWindow.xaml
    /// </summary>
    public partial class TAB_CRM_CAUOFFWindow : FluentDefaultWindow
    {
        private TAB_CRM_CAUOFFWindowViewModel _dataContext;
        public TAB_CRM_CAUOFFWindow(TAB_CRM_CAUOFFWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            if (!_dataContext.IsInsert)
            {
                _dataContext.SelectedOrderCausal = _dataContext.OrderCausals?.Where(w => w.cauacq == _dataContext.Data.offord).FirstOrDefault();
                _dataContext.SelectedText = _dataContext.Texts?.Where(w => w.TTxcod == _dataContext.Data.offte1).FirstOrDefault();
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
            {
                Mouse.OverrideCursor = null;
                ErrorHandler.Validation(validated);
            }
        }
        #endregion

        #region Autocompletes
        private void acOrderCausal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedOrderCausal != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedOrderCausal.cauacq))
            {
                _dataContext.Data.offord = _dataContext.SelectedOrderCausal.cauacq;
            }
        }
        private void acText_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedText != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedText.TTxcod))
            {
                _dataContext.Data.offtxt = TAB_GEN_TEXTS.OFFERS;
                _dataContext.Data.offte1 = _dataContext.SelectedText.TTxcod;
            }
            else
            {
                _dataContext.Data.offtxt = null;
                _dataContext.Data.offte1 = null;
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
