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
    /// Interaction logic for ACC_ASSETS_TYPESView.xaml
    /// </summary>
    public partial class ACC_ASSETS_TYPESView : UserControl
    {
        private ACC_ASSETS_TYPESViewModel _dataContext;
        private int _selectedPage = 0;

        public ACC_ASSETS_TYPESView()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<ACC_ASSETS_TYPESViewModel>();

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
            var item = (sender as Button)!.DataContext as ACC_ASSETS_TYPES;

            if (item != null)
            {
                var affidabiltaWindowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ACC_ASSETS_TYPESWindowViewModel>();
                affidabiltaWindowViewModel.Data = item;
                affidabiltaWindowViewModel.IsInsert = false;

                var wAFFIDABILITA = new ACC_ASSETS_TYPESWindow(affidabiltaWindowViewModel);
                if (wAFFIDABILITA.ShowDialog() == true)
                    LoadData();
            }
        }

        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)!.DataContext as ACC_ASSETS_TYPES;
            MessageBox.Show(item!.JTICE);
        }

        private void cmdInsert_Click(object sender, RoutedEventArgs e)
        {
            var affidabiltaWindowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ACC_ASSETS_TYPESWindowViewModel>();
            affidabiltaWindowViewModel.Data = new ACC_ASSETS_TYPES { socij = _dataContext.CompanyID, JTICE = string.Empty, TCDES = string.Empty };
            affidabiltaWindowViewModel.IsInsert = true;

            var wAFFIDABILITA = new ACC_ASSETS_TYPESWindow(affidabiltaWindowViewModel);
            wAFFIDABILITA.ShowDialog();
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
