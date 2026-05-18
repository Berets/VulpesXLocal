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
    /// Interaction logic for SRM_LISFORCopySupplierWindow.xaml
    /// </summary>
    public partial class LISGENCopyProductWindow : FluentDefaultWindow
    {
        private LISGENCopyProductWindowViewModel _dataContext;
        public LISGENCopyProductWindow(LISGENCopyProductWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            _dataContext.LoadDetails();
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            if (_dataContext.SelectedSourceProduct != null && _dataContext.SelectedTargetProduct != null)
            {
                if (_dataContext.SelectedSourceProduct.ID != _dataContext.SelectedTargetProduct.ID)
                {
                    this.DialogResult = true;
                }
                else
                {
                    ErrorHandler.Validation($"E' necessario selezionare due prodotti differenti");
                    e.Handled = true;
                }
            }
            else
            {
                ErrorHandler.Validation($"E' necessario selezionare due prodotti");
                e.Handled = true;
            }
        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            _dataContext.SelectedSourceProduct = null;
            _dataContext.SelectedTargetProduct = null;
            this.DialogResult = false;
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
