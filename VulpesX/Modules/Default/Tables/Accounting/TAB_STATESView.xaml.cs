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
using VulpesX.ViewModels.Modules.Default.Tables.Accounting;

namespace VulpesX.Modules.Default.Tables.Accounting
{
    /// <summary>
    /// Interaction logic for TAB_STATESView.xaml
    /// </summary>
    public partial class TAB_STATESView : UserControl
    {
        private TAB_STATESViewModel _dataContext;
        private int _selectedPage = 0;

        public TAB_STATESView()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<TAB_STATESViewModel>();

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
            var item = (sender as Button)!.DataContext as TAB_STATES;

            if (item != null)
            {
                if (!string.IsNullOrWhiteSpace(item.cappro))
                {
                    var tab_statesWindowViewModel = VulpesServiceProvider.Provider.GetRequiredService<TAB_STATESWindowViewModel>();
                    tab_statesWindowViewModel.Data = item;
                    tab_statesWindowViewModel.IsInsert = false;

                    var wState = new TAB_STATESWindow(tab_statesWindowViewModel);
                    wState.ShowDialog();
                    LoadData();
                }
            }
        }

        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)!.DataContext as TAB_STATES;
            MessageBox.Show(item!.capdpr);
        }

        private void cmdInsert_Click(object sender, RoutedEventArgs e)
        {
            var tab_statesWindowViewModel = VulpesServiceProvider.Provider.GetRequiredService<TAB_STATESWindowViewModel>();
            tab_statesWindowViewModel.Data = new TAB_STATES { capdpr = string.Empty, cappro = string.Empty };
            tab_statesWindowViewModel.IsInsert = true;

            var wState = new TAB_STATESWindow(tab_statesWindowViewModel); wState.ShowDialog();
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
