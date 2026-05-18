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
using VulpesX.Modules.Default.Tables.Assets;
using VulpesX.Shared;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Tables.CustomerRating;

namespace VulpesX.Modules.Default.Tables.CustomerRating
{
    /// <summary>
    /// Interaction logic for PointView.xaml
    /// </summary>
    public partial class PointView : UserControl
    {
        private PointViewModel _dataContext;
        private int _selectedPage = 0;
        public PointView()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<PointViewModel>();

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
            var item = (sender as Button)!.DataContext as cr_tab_points_financial;

            if (item != null)
            {

                var banazienWindowViewModel = VulpesServiceProvider.Provider.GetRequiredService<PointWindowViewModel>();
                banazienWindowViewModel.Data = item;
                banazienWindowViewModel.IsInsert = false;

                var wBANAZIEN = new PointWindow(banazienWindowViewModel);
                if (wBANAZIEN.ShowDialog() == true)
                    LoadData();
            }
        }

        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)!.DataContext as cr_tab_points_financial;

            if (item != null)
            {
                if (ConfirmHandler.Confirm($"Confermate l'eliminazione point {item.description} ?"))
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
            var banazienWindowViewModel = VulpesServiceProvider.Provider.GetRequiredService<PointWindowViewModel>();
            banazienWindowViewModel.Data = new cr_tab_points_financial { societaID = _dataContext.CompanyID, id = string.Empty };
            banazienWindowViewModel.IsInsert = true;

            var wBANAZIEN = new PointWindow(banazienWindowViewModel);
            wBANAZIEN.ShowDialog();
            LoadData();
        }

        private async void cmdGenerate_Click(object sender, RoutedEventArgs e)
        {
            if (ConfirmHandler.Confirm("Sei sicuro di voler generare i ratings di default? Quelli presenti verranno sovrascritti"))
            {
                await _dataContext.Generate();

                LoadData();
            }
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
