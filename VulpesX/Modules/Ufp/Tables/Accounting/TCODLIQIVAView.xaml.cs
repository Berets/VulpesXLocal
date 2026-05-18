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

namespace VulpesX.Modules.Ufp.Tables.Accounting
{
    /// <summary>
    /// Interaction logic for TCODLIQIVAView.xaml
    /// </summary>
    public partial class TCODLIQIVAView : UserControl
    {
        private TCODLIQIVAViewModel _dataContext;
        private int _selectedPage = 0;

        public TCODLIQIVAView()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<TCODLIQIVAViewModel>();

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
            Mouse.OverrideCursor = Cursors.Wait;
            var item = (sender as Button)!.DataContext as TCODLIQIVA;

            if (item != null)
            {
                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<TCODLIQIVAWindowViewModel>();
                windowViewModel.Data = item;
                windowViewModel.IsInsert = false;

                var wTCODLIQIVA = new TCODLIQIVAWindow(windowViewModel);
                wTCODLIQIVA.Owner = Window.GetWindow(this);
                Mouse.OverrideCursor = null;
                if (wTCODLIQIVA.ShowDialog() == true)
                    LoadData();
            }
        }

        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)!.DataContext as TCODLIQIVA;
            MessageBox.Show($"{item!.CVISoc} {item!.CVICod} ");
        }

        private void cmdInsert_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<TCODLIQIVAWindowViewModel>();
            windowViewModel.Data = new TCODLIQIVA { CVICod = string.Empty, CVIDes = string.Empty, CVISoc = _dataContext.CompanyID, CVITipo = string.Empty };
            windowViewModel.IsInsert = true;

            var wTCODLIQIVA = new TCODLIQIVAWindow(windowViewModel);
            wTCODLIQIVA.Owner = Window.GetWindow(this);
            Mouse.OverrideCursor = null;
            if (wTCODLIQIVA.ShowDialog() == true)
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
