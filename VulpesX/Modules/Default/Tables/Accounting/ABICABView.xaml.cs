using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for ABICABView.xaml
    /// </summary>
    public partial class ABICABView : UserControl
    {
        private ABICABViewModel _dataContext;

        public ABICABView()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<ABICABViewModel>();
            _dataContext.PageSize = 20;
            _dataContext.PageRequested = 0;

            InitializeComponent();

            this.DataContext = _dataContext;
            this.PreviewKeyDown += (s, e) =>
            {
                if (e.Key == Key.F5)
                {
                    LoadData();
                }
            };

            LoadData();
        }

        private async void LoadData()
        {
            await _dataContext.Load();
        }

        #region Filters
        private void txtSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            LoadData();
        }
        private void rmniABI_ValueChanged(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            LoadData();
        }

        private void rmniCAB_ValueChanged(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            LoadData();
        }
        #endregion

        private void cmdEdit_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var item = (sender as Button)!.DataContext as ABICAB;

            if (item != null)
            {
                if (item.abiabi > 0 && item.abicab > 0)
                {
                    var abicabWindowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ABICABWindowViewModel>();
                    abicabWindowViewModel.Data = item;
                    abicabWindowViewModel.IsInsert = false;

                    var wABICAB = new ABICABWindow(abicabWindowViewModel);
                    if (wABICAB.ShowDialog() == true)
                        LoadData();
                }
            }
        }

        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            //var item = (sender as Button).DataContext as ABICAB;
            //MessageBox.Show(item.abiage);
        }

        private void cmdInsert_Click(object sender, RoutedEventArgs e)
        {
            var abicabWindowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ABICABWindowViewModel>();
            abicabWindowViewModel.Data = new ABICAB { abiban = string.Empty };
            abicabWindowViewModel.IsInsert = true;

            var wABICAB = new ABICABWindow(abicabWindowViewModel);
            if (wABICAB.ShowDialog() == true)
                LoadData();
        }

        private void rdpGrid_PageIndexChanged(object sender, PageIndexChangedEventArgs e)
        {
            if (e.OldPageIndex != -1)
                LoadData();
        }
    }
}
