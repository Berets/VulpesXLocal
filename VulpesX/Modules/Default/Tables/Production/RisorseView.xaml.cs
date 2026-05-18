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
using VulpesX.ViewModels.Modules.Default.Tables.Production;

namespace VulpesX.Modules.Default.Tables.Production
{
    /// <summary>
    /// Interaction logic for RisorseView.xaml
    /// </summary>
    public partial class RisorseView : UserControl
    {
        private RisorseViewModel _dataContext;
        private int _selectedPage = 0;
        public RisorseView()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<RisorseViewModel>();

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
            var item = (sender as Button)!.DataContext as tab_produzione_risorsa;

            if (item != null)
            {

                var banazienWindowViewModel = VulpesServiceProvider.Provider.GetRequiredService<RisorseWindowViewModel>();
                banazienWindowViewModel.Data = item;
                banazienWindowViewModel.IsInsert = false;
                banazienWindowViewModel.Costs = _dataContext.GetCosts(item.ID);
                banazienWindowViewModel.Sources = _dataContext.GetSources(item.ID);

                var wBANAZIEN = new RisorseWindow(banazienWindowViewModel);
                if (wBANAZIEN.ShowDialog() == true)
                    LoadData();
            }
        }

        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)!.DataContext as tab_produzione_risorsa;
            MessageBox.Show(item!.ID);
        }

        private void cmdInsert_Click(object sender, RoutedEventArgs e)
        {
            var banazienWindowViewModel = VulpesServiceProvider.Provider.GetRequiredService<RisorseWindowViewModel>();
            banazienWindowViewModel.Data = new tab_produzione_risorsa
            {
                ID = string.Empty,
                SocietaID = _dataContext.CompanyID,
                Descrizione = string.Empty,
            };
            banazienWindowViewModel.IsInsert = true;
            banazienWindowViewModel.Costs = new System.Collections.ObjectModel.ObservableCollection<tab_produzione_risorsa_costo>();
            banazienWindowViewModel.Sources = new System.Collections.ObjectModel.ObservableCollection<tab_produzione_risorsa_sorgenti>();
            var wBANAZIEN = new RisorseWindow(banazienWindowViewModel);
            wBANAZIEN.ShowDialog();
            LoadData();
        }

        private void btnReportBadge_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            var selected = GridView.SelectedItem as tab_produzione_risorsa;
            if (selected != null)
            {
                // TODO report
            }

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
