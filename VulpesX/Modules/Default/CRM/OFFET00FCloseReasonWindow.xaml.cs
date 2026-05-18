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
using VulpesX.ViewModels.Modules.Default.CRM;

namespace VulpesX.Modules.Default.CRM
{
    /// <summary>
    /// Interaction logic for OFFET00FCloseReasonWindow.xaml
    /// </summary>
    public partial class OFFET00FCloseReasonWindow : FluentDefaultWindow
    {
        private OFFET00FCloseReasonWindowViewModel _dataContext;
        public OFFET00FCloseReasonWindow(OFFET00FCloseReasonWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            _dataContext.Causals = _dataContext.GetTAB_CRM_CAUOFFCLOs();
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            cmdCancel.IsEnabled = false;
            cmdSave.IsEnabled = false;

            if (_dataContext.SelectedCausal != null)
            {
                this.DialogResult = true;
            }
            else
            {
                Mouse.OverrideCursor = null;
                ErrorHandler.Validation("La causale è un dato obbligatorio");
                cmdCancel.IsEnabled = true;
                cmdSave.IsEnabled = true;
            }
        }

        private void txtReason_TextChanged(object sender, TextChangedEventArgs e)
        {
            _dataContext.CurrentSize = txtReason.Text.Trim().Length;
            e.Handled = true;
        }

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
    }
}
