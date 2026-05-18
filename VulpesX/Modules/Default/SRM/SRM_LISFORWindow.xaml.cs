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
using VulpesX.ViewModels.Modules.Default.SRM;

namespace VulpesX.Modules.Default.SRM
{
    /// <summary>
    /// Interaction logic for SRM_LISFORWindow.xaml
    /// </summary>
    public partial class SRM_LISFORWindow : FluentDefaultWindow
    {
        private SRM_LISFORWindowViewModel _dataContext;
        public SRM_LISFORWindow(SRM_LISFORWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            _dataContext.LoadDetails();
            _dataContext.SelectedSupplier = _dataContext.Suppliers?.Where(w => w.abecod == _dataContext.Data.supplierID).FirstOrDefault();
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
        private void acSupplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedSupplier != null && _dataContext.SelectedSupplier.abecod > 0)
            {
                _dataContext.Data.supplierID = _dataContext.SelectedSupplier.abecod;
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
