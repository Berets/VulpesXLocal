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
    /// Interaction logic for CostMaterialsWindow.xaml
    /// </summary>
    public partial class CostMaterialsWindow : FluentDefaultWindow
    {
        private CostMaterialsWindowViewModel _dataContext;
        public CostMaterialsWindow(CostMaterialsWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            _dataContext.Products = _dataContext.GetTab_Articolos();

            if(!_dataContext.IsInsert)
            {
                _dataContext.OldProductID = _dataContext.Data.product_id;
                _dataContext.OldYear = _dataContext.Data.year;
                _dataContext.OldMonth = _dataContext.Data.month;
                _dataContext.SelectedDate = new DateTime(_dataContext.Data.year, _dataContext.Data.month, 1);
                _dataContext.SelectedProduct = _dataContext.Products?.Where(w => w.ID == _dataContext.Data.product_id).FirstOrDefault();
            }
        }


        #region Buttons
        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            if (_dataContext.SelectedProduct != null)
            {
                _dataContext.Data.product_id = _dataContext.SelectedProduct.ID;

                if (_dataContext.SelectedDate.HasValue)
                {
                    _dataContext.Data.year = _dataContext.SelectedDate.Value.Year;
                    _dataContext.Data.month = _dataContext.SelectedDate.Value.Month;
                }
                else
                {
                    _dataContext.Data.year = 0;
                    _dataContext.Data.month = 0;
                }

                var validated = _dataContext.Validate();
                if (string.IsNullOrWhiteSpace(validated))
                {
                    if(_dataContext.Save())
                    {
                        Mouse.OverrideCursor = null;
                        this.DialogResult = true;
                    }
                    else
                    {
                        Mouse.OverrideCursor = null;
                        ErrorHandler.Validation("Errore irreversibile durante l'aggiornamento");
                    }
                }
                else
                {
                    Mouse.OverrideCursor = null;
                    ErrorHandler.Validation(validated);
                }
            }
            else
            {
                Mouse.OverrideCursor = null;
                ErrorHandler.Validation("Selezionare un articolo");
            }
        }
        #endregion

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
