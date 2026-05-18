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
using VulpesX.ViewModels.Modules.Default.Accounting;

namespace VulpesX.Modules.Default.Accounting
{
    /// <summary>
    /// Interaction logic for ACC_PLAFOND_PARMSWindow.xaml
    /// </summary>
    public partial class ACC_PLAFOND_PARMSWindow : FluentDefaultWindow
    {
        private ACC_PLAFOND_PARMSWindowViewModel _dataContext;
        public ACC_PLAFOND_PARMSWindow(ACC_PLAFOND_PARMSWindowViewModel dataContxt)
        {
            _dataContext = dataContxt;

            InitializeComponent();

            this.DataContext = _dataContext;

            _dataContext.LoadDetail();
        }

        #region Buttons
        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var validated = _dataContext.Validate() ;
            if (string.IsNullOrWhiteSpace(validated))
            {
                if(_dataContext.Save())
                {
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
        private void acGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.Data.SelectedGroup != null && !string.IsNullOrWhiteSpace(_dataContext.Data.SelectedGroup.P1GRUP))
            {
                _dataContext.Data.group_id = _dataContext.Data.SelectedGroup.P1GRUP;
            }
        }

        private void acAccount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.Data.SelectedAccount != null && !string.IsNullOrWhiteSpace(_dataContext.Data.SelectedAccount.P2CONT))
            {
                _dataContext.Data.account_id = _dataContext.Data.SelectedAccount.P2CONT;
            }
        }

        private void acSubaccount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext.Data.SelectedSubaccount != null && !string.IsNullOrWhiteSpace(_dataContext.Data.SelectedSubaccount.P3SOTC))
            {
                _dataContext.Data.subaccount_id = _dataContext.Data.SelectedSubaccount.P3SOTC;
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
