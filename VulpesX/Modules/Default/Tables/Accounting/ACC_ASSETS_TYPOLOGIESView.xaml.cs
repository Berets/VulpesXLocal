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
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Tables.Accounting;

namespace VulpesX.Modules.Default.Tables.Accounting
{
    /// <summary>
    /// Interaction logic for ACC_ASSETS_TYPOLOGIESView.xaml
    /// </summary>
    public partial class ACC_ASSETS_TYPOLOGIESView : UserControl
    {
        private ACC_ASSETS_TYPOLOGIESViewModel _dataContext;
        private int _selectedPage = 0;

        public ACC_ASSETS_TYPOLOGIESView()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<ACC_ASSETS_TYPOLOGIESViewModel>();

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
            else
            {
                _dataContext.ItemsData = null;
            }
        }

        #region Buttons
        private void cmdEdit_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var item = (sender as Button)!.DataContext as ACC_ASSETS_TYPOLOGIES;

            if (item != null)
            {
                var affidabiltaWindowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ACC_ASSETS_TYPOLOGIESWindowViewModel>();
                affidabiltaWindowViewModel.Data = item;
                affidabiltaWindowViewModel.IsInsert = false;

                var wAFFIDABILITA = new ACC_ASSETS_TYPOLOGIESWindow(affidabiltaWindowViewModel);
                if (wAFFIDABILITA.ShowDialog() == true)
                    LoadData();
            }
        }

        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)!.DataContext as ACC_ASSETS_TYPOLOGIES;

            if (item != null)
            {
                if (ConfirmHandler.Confirm($"Confermate l'eliminazione della query {item.Group?.P1DES1} ?"))
                {
                    _dataContext.Delete(item);

                    LoadData();
                    LoadDataDetails();
                }
            }
        }

        private void cmdInsert_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = GridView.SelectedItem as PDCSOTTO;

            var affidabiltaWindowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ACC_ASSETS_TYPOLOGIESWindowViewModel>();
            affidabiltaWindowViewModel.Data = new ACC_ASSETS_TYPOLOGIES
            {
                tsoci = _dataContext.CompanyID,
                tgrupp = selectedItem?.Group?.P1GRUP ?? string.Empty,
                gconto = selectedItem?.Account?.P2CONT ?? string.Empty,
                tsotco = selectedItem?.P3SOTC ?? string.Empty,
                AllAccounts = new System.Collections.ObjectModel.ObservableCollection<PDCCONTI>(_dataContext.GetPDCCONTIs() ?? new List<PDCCONTI>()),
                AllSubccounts = new System.Collections.ObjectModel.ObservableCollection<PDCSOTTO>(_dataContext.GetPDCSOTTOs() ?? new List<PDCSOTTO>())
            };
            affidabiltaWindowViewModel.IsInsert = true;

            var wAFFIDABILITA = new ACC_ASSETS_TYPOLOGIESWindow(affidabiltaWindowViewModel);
            wAFFIDABILITA.ShowDialog();
            LoadData();
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
