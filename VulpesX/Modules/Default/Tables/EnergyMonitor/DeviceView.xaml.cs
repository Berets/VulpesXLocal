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
using VulpesX.ViewModels.Modules.Default.Tables.EnergyMonitor;

namespace VulpesX.Modules.Default.Tables.EnergyMonitor
{
    /// <summary>
    /// Interaction logic for DeviceView.xaml
    /// </summary>
    public partial class DeviceView : UserControl
    {
        private DeviceViewModel _dataContext;
        private int _selectedPage = 0;
        public DeviceView()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<DeviceViewModel>();

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
            var item = (sender as Button)!.DataContext as EM_DEVICE;

            if (item != null)
            {

                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<DeviceWindowViewModel>();
                windowViewModel.Data = item;
                windowViewModel.Costs = _dataContext.GetEM_DEVICE_PERIODs(item.ID);
                windowViewModel.IsInsert = false;

                var wBANAZIEN = new DeviceWindow(windowViewModel);
                if (wBANAZIEN.ShowDialog() == true)
                    LoadData();
            }
        }

        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)!.DataContext as EM_DEVICE;
            MessageBox.Show(item!.ID);
        }

        private void cmdInsert_Click(object sender, RoutedEventArgs e)
        {
            var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<DeviceWindowViewModel>();
            windowViewModel.Data = new EM_DEVICE { ID = string.Empty, SocietaID = _dataContext.CompanyID, Descrizione = string.Empty };
            windowViewModel.IsInsert = true;
            windowViewModel.Costs = new System.Collections.ObjectModel.ObservableCollection<EM_DEVICE_PERIOD>();

            var wBANAZIEN = new DeviceWindow(windowViewModel);
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
