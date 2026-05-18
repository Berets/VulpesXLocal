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
    /// Interaction logic for ACC_ASSETS_RATESView.xaml
    /// </summary>
    public partial class ACC_ASSETS_RATESView : UserControl
    {
        private ACC_ASSETS_RATESViewModel _dataContext;
        private int _selectedPage = 0;

        public ACC_ASSETS_RATESView()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<ACC_ASSETS_RATESViewModel>();

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
            GridView.SelectionChanged += (s, e) =>
            {
                LoadDataDetails();
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

        private async void LoadDataDetails()
        {
            var selectedItem = GridView.SelectedItem as PDCSOTTO;
            if (selectedItem != null)
            {
                await _dataContext.LoadDetails(selectedItem);
            }
        }

        #region Buttons
        private void cmdEdit_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var item = (sender as Button)!.DataContext as ACC_ASSETS_RATES;

            if (item != null)
            {
                var affidabiltaWindowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ACC_ASSETS_RATESWindowViewModel>();
                affidabiltaWindowViewModel.Data = item;
                affidabiltaWindowViewModel.IsInsert = false;

                var wAFFIDABILITA = new ACC_ASSETS_RATESWindow(affidabiltaWindowViewModel);
                if (wAFFIDABILITA.ShowDialog() == true)
                    LoadData();
            }
        }

        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)!.DataContext as ACC_ASSETS_RATES;
            MessageBox.Show(item!.ammsoc);
        }

        private void cmdInsert_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = GridView.SelectedItem as PDCSOTTO;
            if (selectedItem != null && selectedItem.Group != null && selectedItem.Account != null)
            {
                var affidabiltaWindowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ACC_ASSETS_RATESWindowViewModel>();
                affidabiltaWindowViewModel.Data = new ACC_ASSETS_RATES
                {
                    ammsoc = _dataContext.CompanyID,
                    tgrupp = selectedItem.Group.P1GRUP,
                    gconto = selectedItem.Account.P2CONT,
                    tsotco = selectedItem.P3SOTC,
                    janno = DateTime.Now.Year
                };
                affidabiltaWindowViewModel.IsInsert = true;

                var wAFFIDABILITA = new ACC_ASSETS_RATESWindow(affidabiltaWindowViewModel);
                wAFFIDABILITA.ShowDialog();
                LoadData();
            }
        }

        private void cmdDuplicate_Click(object sender, RoutedEventArgs e)
        {
            //var wDuplicateYear = new wDuplicateYear(ctx);
            //wDuplicateYear.Owner = Window.GetWindow(this);
            //if (wDuplicateYear.ShowDialog() == true)
            //    LoadData();
        }

        #endregion

        #region UC standard functions
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs? e)
        {
            (RadGridViewCommands.SearchByText as RoutedUICommand)!.Execute(txtSearch.Text, GridView);
        }
        private void txtSearchData_TextChanged(object sender, TextChangedEventArgs? e)
        {
            (RadGridViewCommands.SearchByText as RoutedUICommand)!.Execute(txtSearchData.Text, GridViewData);
        }
        #endregion
    }
}
