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
    /// Interaction logic for FATTAUTWindow.xaml
    /// </summary>
    public partial class FATTAUTWindow : FluentDefaultWindow
    {
        private FATTAUTWindowViewModel _dataContext;
        public FATTAUTWindow(FATTAUTWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            _dataContext.LoadDetails();

            if (!_dataContext.IsInsert)
            {
                _dataContext.SelectedSupplier = _dataContext.Suppliers?.Where(w => w.abecod == _dataContext.Data.FTAUCOF).FirstOrDefault();
            }

            Title = $"Dettagli autofattura {_dataContext.Data.FTAUAN}/{_dataContext.Data.FTAUNUM}";
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
            {
                Mouse.OverrideCursor = null;
                ErrorHandler.Validation(validated);
            }
        }
        #endregion

        #region Autocompletes
        private void acSupplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.SelectedSupplier != null && _dataContext.SelectedSupplier.abecod > 0)
            {
                _dataContext.Data.FTAUCOF = _dataContext.SelectedSupplier.abecod;
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
