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
using VulpesX.ViewModels.Modules.Default.Tables.Assets;

namespace VulpesX.Modules.Default.Tables.Assets
{
    /// <summary>
    /// Interaction logic for TAB_AST_LOCATIONSView.xaml
    /// </summary>
    public partial class TAB_AST_LOCATIONSView : UserControl
    {
        private TAB_AST_LOCATIONSViewModel _dataContext;
        private int _selectedPage = 0;
        public TAB_AST_LOCATIONSView()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<TAB_AST_LOCATIONSViewModel>();

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
            var item = (sender as Button)!.DataContext as TAB_AST_LOCATIONS;

            if (item != null)
            {

                var banazienWindowViewModel = VulpesServiceProvider.Provider.GetRequiredService<TAB_AST_LOCATIONSWindowViewModel>();
                banazienWindowViewModel.Data = item;
                banazienWindowViewModel.IsInsert = false;

                var wBANAZIEN = new TAB_AST_LOCATIONSWindow(banazienWindowViewModel);
                if (wBANAZIEN.ShowDialog() == true)
                    LoadData();
            }
        }

        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)!.DataContext as TAB_AST_LOCATIONS;
            MessageBox.Show(item!.id);
        }

        private void cmdInsert_Click(object sender, RoutedEventArgs e)
        {
            var banazienWindowViewModel = VulpesServiceProvider.Provider.GetRequiredService<TAB_AST_LOCATIONSWindowViewModel>();
            banazienWindowViewModel.Data = new TAB_AST_LOCATIONS { company_id = _dataContext.CompanyID, id = string.Empty, description = string.Empty };
            banazienWindowViewModel.IsInsert = true;

            var wBANAZIEN = new TAB_AST_LOCATIONSWindow(banazienWindowViewModel);
            wBANAZIEN.ShowDialog();
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
