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
using VulpesX.ViewModels.Modules.Default.Tables.General;

namespace VulpesX.Modules.Default.Tables.General
{
    /// <summary>
    /// Interaction logic for TAB_GEN_ACTIVITY_TYPESView.xaml
    /// </summary>
    public partial class TAB_GEN_ACTIVITY_TYPESView : UserControl
    {
        private TAB_GEN_ACTIVITY_TYPESViewModel _dataContext;
        private int _selectedPage = 0;

        public TAB_GEN_ACTIVITY_TYPESView()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<TAB_GEN_ACTIVITY_TYPESViewModel>();

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
            var item = (sender as Button)!.DataContext as TAB_GEN_ACTIVITY_TYPES;

            if (item != null)
            {
                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<TAB_GEN_ACTIVITY_TYPESWindowViewModel>();
                windowViewModel.Data = item;
                windowViewModel.IsInsert = false;

                var wTAB_GEN_ACTIVITY_TYPES = new TAB_GEN_ACTIVITY_TYPESWindow(windowViewModel);
                wTAB_GEN_ACTIVITY_TYPES.Owner = Window.GetWindow(this);
                if (wTAB_GEN_ACTIVITY_TYPES.ShowDialog() == true)
                    LoadData();
            }
        }

        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)!.DataContext as TAB_GEN_ACTIVITY_TYPES;
            MessageBox.Show($"{item!.company_id} {item!.id} ");
        }

        private void cmdInsert_Click(object sender, RoutedEventArgs e)
        {
            var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<TAB_GEN_ACTIVITY_TYPESWindowViewModel>();
            windowViewModel.Data = new TAB_GEN_ACTIVITY_TYPES { company_id = _dataContext.CompanyID, id = string.Empty, description = string.Empty };
            windowViewModel.IsInsert = true;

            var wTAB_GEN_ACTIVITY_TYPES = new TAB_GEN_ACTIVITY_TYPESWindow(windowViewModel);
            wTAB_GEN_ACTIVITY_TYPES.Owner = Window.GetWindow(this);
            if (wTAB_GEN_ACTIVITY_TYPES.ShowDialog() == true)
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
