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
    /// Interaction logic for FE_PAGDOCWindow.xaml
    /// </summary>
    public partial class FE_PAGDOCWindow : FluentDefaultWindow
    {
        private FE_PAGDOCWindowViewModel _dataContext;
        public FE_PAGDOCWindow(FE_PAGDOCWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;
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

                Mouse.OverrideCursor = null;

                if (_dataContext.Save())
                    this.DialogResult = true;

            }
            else
            {
                Mouse.OverrideCursor = null;
                ErrorHandler.Validation(validated);
            }
        }
        #endregion

        #region Autocompletes
        private void acPaymentType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (_dataContext.SelectedPaymentType != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedPaymentType.inccod))
            {
                _dataContext.Data.FEPATIPP = _dataContext.SelectedPaymentType.inccod;
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
    }
}
