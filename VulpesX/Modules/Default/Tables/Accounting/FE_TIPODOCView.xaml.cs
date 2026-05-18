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
    /// Interaction logic for FE_TIPODOCView.xaml
    /// </summary>
    public partial class FE_TIPODOCView : UserControl
    {
        private FE_TIPODOCViewModel _dataContext;
        private int _selectedPage = 0;

        public FE_TIPODOCView()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<FE_TIPODOCViewModel>();

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
            var item = (sender as Button)?.DataContext as FE_TIPODOC;

            if (item != null)
            {
                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<FE_TIPODOCWindowViewModel>();
                windowViewModel.Data = item;
                windowViewModel.IsInsert = false;

                var wFE_TIPODOC = new FE_TIPODOCWindow(windowViewModel);
                Mouse.OverrideCursor = null;
                wFE_TIPODOC.Owner = Window.GetWindow(this);
                if (wFE_TIPODOC.ShowDialog() == true)
                    LoadData();
            }
        }

        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)?.DataContext as FE_TIPODOC;
            MessageBox.Show($"{item?.FETDCod} ");
        }

        private void cmdInsert_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<FE_TIPODOCWindowViewModel>();
            windowViewModel.Data = new FE_TIPODOC { FETDCod = string.Empty };
            windowViewModel.IsInsert = true;

            var wFE_TIPODOC = new FE_TIPODOCWindow(windowViewModel);
            Mouse.OverrideCursor = null;
            wFE_TIPODOC.Owner = Window.GetWindow(this);
            wFE_TIPODOC.ShowDialog();
            LoadData();
        }
        #endregion

        #region UC standard functions
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs? e)
        {
            (RadGridViewCommands.SearchByText as RoutedUICommand)?.Execute(txtSearch.Text, GridView);
        }
        #endregion
    }
}
