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
    /// Interaction logic for RITENUTEView.xaml
    /// </summary>
    public partial class RITENUTEView : UserControl
    {
        private RITENUTEViewModel _dataContext;
        private int _selectedPage = 0;

        public RITENUTEView()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<RITENUTEViewModel>();

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
            var item = (sender as Button)!.DataContext as RITENUTE;

            if (item != null)
            {
                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<RITENUTEWindowViewModel>();
                windowViewModel.Data = item;
                windowViewModel.IsInsert = false;

                var wRITENUTE = new RITENUTEWindow(windowViewModel);
                Mouse.OverrideCursor = null;
                if (wRITENUTE.ShowDialog() == true)
                    LoadData();
            }
        }

        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)!.DataContext as RITENUTE;
            MessageBox.Show($"{item!.ritcod} ");
        }

        private void cmdInsert_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<RITENUTEWindowViewModel>();
            windowViewModel.Data = new RITENUTE
            {
                CompanyID = _dataContext.CompanyID,
                ritcod = string.Empty,
                ritdes = string.Empty,
                rtmese = 0,
                rtsez = "E",
                ritfl1 = "N",
                ritfl2 = "N",
                rtTipRed = "A",
            };
            windowViewModel.IsInsert = true;

            var wRITENUTE = new RITENUTEWindow(windowViewModel);
            Mouse.OverrideCursor = null;
            wRITENUTE.ShowDialog();
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
