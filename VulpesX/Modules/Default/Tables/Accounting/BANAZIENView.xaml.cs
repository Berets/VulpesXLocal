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
    /// Interaction logic for BANAZIENView.xaml
    /// </summary>
    public partial class BANAZIENView : UserControl
    {
        private BANAZIENViewModel _dataContext;
        private int _selectedPage = 0;

        public BANAZIENView()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<BANAZIENViewModel>();

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

                Telerik.Windows.Controls.GridViewColumn countryColumn = this.GridView.Columns["colAtt"];
                Telerik.Windows.Controls.GridView.IColumnFilterDescriptor countryFilter = countryColumn.ColumnFilterDescriptor;

                countryFilter.SuspendNotifications();
                countryFilter.DistinctFilter.AddDistinctValue(true);
                countryFilter.ResumeNotifications();
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
            var item = (sender as Button)!.DataContext as BANAZIEN;

            if (item != null)
            {
                if (item.abiabi > 0 && item.abicab > 0 && !string.IsNullOrWhiteSpace(item.abicon))
                {
                    var banazienWindowViewModel = VulpesServiceProvider.Provider.GetRequiredService<BANAZIENWindowViewModel>();
                    banazienWindowViewModel.Data = item;
                    banazienWindowViewModel.IsInsert = false;

                    var wBANAZIEN = new BANAZIENWindow(banazienWindowViewModel);
                    if (wBANAZIEN.ShowDialog() == true)
                        LoadData();
                }
            }
        }

        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)!.DataContext as BANAZIEN;
            MessageBox.Show(item!.abicon);
        }

        private void cmdInsert_Click(object sender, RoutedEventArgs e)
        {
            var banazienWindowViewModel = VulpesServiceProvider.Provider.GetRequiredService<BANAZIENWindowViewModel>();
            banazienWindowViewModel.Data = new BANAZIEN { abicon = string.Empty, abisoc = _dataContext.CompanyID };
            banazienWindowViewModel.IsInsert = true;

            var wBANAZIEN = new BANAZIENWindow(banazienWindowViewModel);
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
