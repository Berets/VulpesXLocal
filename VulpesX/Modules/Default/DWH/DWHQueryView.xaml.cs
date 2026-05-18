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
using VulpesX.Models.Default;
using VulpesX.Shared;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.DWH;

namespace VulpesX.Modules.Default.DWH
{
    /// <summary>
    /// Interaction logic for DWHQueryView.xaml
    /// </summary>
    public partial class DWHQueryView : UserControl
    {
        private DWHQueryViewModel _dataContext;
        private int _selectedPage = 0;

        public DWHQueryView()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<DWHQueryViewModel>();

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

        #region UC events
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs? e)
        {
            (RadGridViewCommands.SearchByText as RoutedUICommand)?.Execute(txtSearch.Text, GridView);
        }
        #endregion

        #region Buttons
        private void cmdEdit_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var item = (sender as Button)?.DataContext as DWH_Query;

            if (item != null)
            {
                item = _dataContext.GetQuery(item.ID);

                if (item != null)
                {
                    var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<DWHQueryWindowViewModel>();
                    windowViewModel.Data = item;
                    windowViewModel.IsInsert = false;

                    var wQuery = new DWHQueryWindow(windowViewModel);
                    wQuery.Owner = Window.GetWindow(this);
                    if (wQuery.ShowDialog() == true)
                        LoadData();
                }
            }
        }

        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)?.DataContext as DWH_Query;

            if (item != null)
            {
                if (ConfirmHandler.Confirm($"Confermate l'eliminazione della query {item.Titolo} ?"))
                {
                    if (_dataContext.Delete(item))
                        LoadData();
                    else
                        MessageBox.Show("Errore imprevisto durante l'eliminazione", Constants.APP_NAME, MessageBoxButton.OK, MessageBoxImage.Hand);
                }
            }
        }

        private void cmdInsert_Click(object sender, RoutedEventArgs e)
        {
            var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<DWHQueryWindowViewModel>();
            windowViewModel.Data = new DWH_Query { SocietaID = _dataContext.CompanyID };
            windowViewModel.IsInsert = true;

            var wQuery = new DWHQueryWindow(windowViewModel);
            wQuery.Owner = Window.GetWindow(this);
            wQuery.ShowDialog();
            LoadData();
        }
        #endregion
    }
}
