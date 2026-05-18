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
    /// Interaction logic for ACC_ASSETS_TYPOLOGIESWindow.xaml
    /// </summary>
    public partial class ACC_ASSETS_TYPOLOGIESWindow : FluentDefaultWindow
    {
        private ACC_ASSETS_TYPOLOGIESWindowViewModel _dataContext;
        public ACC_ASSETS_TYPOLOGIESWindow(ACC_ASSETS_TYPOLOGIESWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.DataContext = _dataContext;

            _dataContext.LoadDetails();

            _dataContext.Data.OrdinaryGroups = _dataContext.Data.Groups;
            _dataContext.Data.AnticipatedGroups = _dataContext.Data.Groups;
            _dataContext.Data.FundGroups = _dataContext.Data.Groups;
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            var validated = _dataContext.Validate();

            if (string.IsNullOrWhiteSpace(validated))
            {
                DialogResult = _dataContext.Save();
            }
            else
            {
                ErrorHandler.Show(validated);
            }
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
