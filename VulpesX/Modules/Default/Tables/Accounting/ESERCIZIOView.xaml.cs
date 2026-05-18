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
    /// Interaction logic for ESERCIZIOView.xaml
    /// </summary>
    public partial class ESERCIZIOView : UserControl
    {
        private ESERCIZIOViewModel _dataContext;
        private int _selectedPage = 0;

        public ESERCIZIOView()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<ESERCIZIOViewModel>();

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
            var item = (sender as Button)!.DataContext as ESERCIZIO;

            if (item != null)
            {
                if (!string.IsNullOrWhiteSpace(item.esesoc) && item.eseann > 0)
                {
                    var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ESERCIZIOWindowViewModel>();
                    windowViewModel.Data = item;
                    windowViewModel.IsInsert = false;

                    var wESERCIZIO = new ESERCIZIOWindow(windowViewModel);
                    if (wESERCIZIO.ShowDialog() == true)
                        LoadData();
                }
            }
        }

        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)!.DataContext as ESERCIZIO;
            MessageBox.Show(item!.eseann.ToString());
        }

        private void cmdInsert_Click(object sender, RoutedEventArgs e)
        {
            var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ESERCIZIOWindowViewModel>();
            windowViewModel.Data = new ESERCIZIO { esesoc = _dataContext.CompanyID };
            windowViewModel.IsInsert = true;

            var wESERCIZIO = new ESERCIZIOWindow(windowViewModel);
            if (wESERCIZIO.ShowDialog() == true)
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
