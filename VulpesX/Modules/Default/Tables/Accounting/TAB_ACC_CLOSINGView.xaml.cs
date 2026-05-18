using Microsoft.Extensions.DependencyInjection;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using VulpesX.Shared;
using VulpesX.ViewModels.Modules.Default.Tables.Accounting;

namespace VulpesX.Modules.Default.Tables.Accounting
{
    /// <summary>
    /// Interaction logic for TAB_ACC_CLOSINGView.xaml
    /// </summary>
    public partial class TAB_ACC_CLOSINGView : UserControl
    {
        private TAB_ACC_CLOSINGViewModel _dataContext;
        private int _selectedPage = 0;

        public TAB_ACC_CLOSINGView()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<TAB_ACC_CLOSINGViewModel>();

            InitializeComponent();

            this.DataContext = _dataContext;
            this.PreviewKeyDown += (s, e) =>
            {
                if (e.Key == Key.F5)
                {
                    LoadData();
                }
            };

            GridView.DataLoaded += (s, e) =>
            {
                rdpGrid.MoveToPage(_selectedPage);
                txtSearch_TextChanged(txtSearch, null);
            };
            rdpGrid.PageIndexChanged += (s, e) =>
            {
                _selectedPage = e.NewPageIndex;
            };

            LoadData();
        }

        private async void LoadData()
        {
            await _dataContext.Load();
        }

        #region Buttons
        private void cmdEdit_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            var item = (sender as Button)!.DataContext as TAB_ACC_CLOSING;

            if (item != null)
            {
                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<TAB_ACC_CLOSINGWindowViewModel>();
                windowViewModel.Data = item;
                windowViewModel.IsInsert = false;

                var wTAB_ACC_CLOSING = new TAB_ACC_CLOSINGWindow(windowViewModel);
                wTAB_ACC_CLOSING.Owner = Window.GetWindow(this);
                Mouse.OverrideCursor = null;
                if (wTAB_ACC_CLOSING.ShowDialog() == true)
                    LoadData();
            }
        }

        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)!.DataContext as TAB_ACC_CLOSING;
            MessageBox.Show($"{item!.cchcod} ");
        }

        private void cmdInsert_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<TAB_ACC_CLOSINGWindowViewModel>();
            windowViewModel.Data = new TAB_ACC_CLOSING { cchcod = string.Empty, cchdes = string.Empty };
            windowViewModel.IsInsert = true;

            var wTAB_ACC_CLOSING = new TAB_ACC_CLOSINGWindow(windowViewModel);
            wTAB_ACC_CLOSING.Owner = Window.GetWindow(this);
            Mouse.OverrideCursor = null;
            if (wTAB_ACC_CLOSING.ShowDialog() == true)
                LoadData();
        }
        #endregion

        #region UC standard functions
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs? e)
        {
            (RadGridViewCommands.SearchByText as RoutedUICommand)!.Execute(txtSearch.Text, GridView);
        }
        #endregion
    }
}
