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
using System.Windows.Threading;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Tables.Store;

namespace VulpesX.Modules.Default.Tables.Store
{
    /// <summary>
    /// Interaction logic for STORE_CAUSALSWindow.xaml
    /// </summary>
    public partial class STORE_CAUSALSWindow : FluentDefaultWindow
    {
        private STORE_CAUSALSWindowViewModel _dataContext;
        public STORE_CAUSALSWindow(STORE_CAUSALSWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            _dataContext.LoadDetails();
        }

        #region Buttons
        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            var validated = _dataContext.Validate();

            if (string.IsNullOrWhiteSpace(validated))
            {
                if (_dataContext.Save())
                {
                    this.DialogResult = true;
                }
            }
            else
            {
                ErrorHandler.Validation(validated);
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
