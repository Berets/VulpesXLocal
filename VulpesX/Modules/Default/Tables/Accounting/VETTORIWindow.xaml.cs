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
    /// Interaction logic for VETTORIWindow.xaml
    /// </summary>
    public partial class VETTORIWindow : FluentDefaultWindow
    {
        private VETTORIWindowViewModel _dataContext;
        public VETTORIWindow(VETTORIWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            if (!_dataContext.IsInsert)
            {
                _dataContext.SelectedCity = _dataContext.Cities?.Where(w => w.comdes == _dataContext.Data.vetloc?.Trim()).FirstOrDefault();
                _dataContext.SelectedState = _dataContext.States?.Where(w => w.cappro == _dataContext.Data.vetpro?.Trim()).FirstOrDefault();
                _dataContext.SelectedSupplier = _dataContext.Suppliers?.Where(w => w.abecod == _dataContext.Data.vetfor).FirstOrDefault();
            }
        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            var validated = _dataContext.Validate();

            if (string.IsNullOrWhiteSpace(validated))
            {
                this.DialogResult = _dataContext.Save();
            }
            else
            {
                ErrorHandler.Show(validated);
            }
        }

        #region Autocompletes
        private void acCity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedCity != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedCity.comdes))
            {
                _dataContext.Data.vetloc = _dataContext.SelectedCity.comdes;
            }
        }
        private void acState_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedState != null && !string.IsNullOrWhiteSpace(_dataContext.SelectedState.cappro))
            {
                _dataContext.Data.vetpro = _dataContext.SelectedState.cappro;
            }
        }
        private void acSupplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedSupplier != null)
            {
                _dataContext.Data.vetfor = _dataContext.SelectedSupplier.abecod;
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
