using Microsoft.Extensions.DependencyInjection;
using Org.BouncyCastle.Asn1.Cmp;
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
using VulpesX.Models.Models.SRM;
using VulpesX.Shared;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.SRM;

namespace VulpesX.Modules.Default.SRM
{
    /// <summary>
    /// Interaction logic for CostMaterialsView.xaml
    /// </summary>
    public partial class CostMaterialsView : UserControl
    {
        private CostMaterialsViewModel _dataContext;

        private int _selectedPage = 0;

        public CostMaterialsView()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<CostMaterialsViewModel>();

            InitializeComponent();

            this.DataContext = _dataContext;
            this.PreviewKeyDown += (s, e) =>
            {
                if (e.Key == Key.F5)
                {
                    LoadData();
                }
            };

            GridViewMaster.DataLoaded += (s, e) =>
            {
                rdpGridMaster.MoveToPage(_selectedPage);
                txtSearch_TextChanged(txtSearch, null);
            };
            GridViewMaster.SelectionChanged += (s, e) =>
            {
                var item = GridViewMaster.SelectedItem as tab_articolo_costiListViewModel;
                if (item != null && !string.IsNullOrEmpty(item.ID))
                {
                    LoadProductCosts(item.ID);
                }
            };
            rdpGridMaster.PageIndexChanged += (s, e) =>
            {
                _selectedPage = e.NewPageIndex;
            };

            LoadData();
        }

        private async void LoadData()
        {
            await _dataContext.Load();
        }

        private async void LoadProductCosts(string ProductID)
        {
            await _dataContext.LoadProductCosts(ProductID);
        }

        #region Buttons
        private void cmdInsert_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<CostMaterialsWindowViewModel>();
            windowViewModel.Data = new tab_articolo_costi()
            {
                cid = _dataContext.CompanyID,
                addedUserID = _dataContext.UserID,
                product_id = string.Empty,
            };
            windowViewModel.IsInsert = true;

            var wTAB_ARTICOLO_COSTI = new CostMaterialsWindow(windowViewModel);
            wTAB_ARTICOLO_COSTI.Owner = Window.GetWindow(this);
            Mouse.OverrideCursor = null;
            if (wTAB_ARTICOLO_COSTI.ShowDialog() == true)
                LoadData();
        }

        private void cmdEdit_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var item = (sender as Button)?.DataContext as tab_articolo_costi;

            if (item != null)
            {
                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<CostMaterialsWindowViewModel>();
                windowViewModel.Data = item;
                windowViewModel.IsInsert = false;

                var wTAB_ARTICOLO_COSTIEdit = new CostMaterialsWindow(windowViewModel);
                wTAB_ARTICOLO_COSTIEdit.Owner = Window.GetWindow(this);
                Mouse.OverrideCursor = null;
                if (wTAB_ARTICOLO_COSTIEdit.ShowDialog() == true)
                    LoadData();
            }
        }

        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            var item = (sender as Button)?.DataContext as tab_articolo_costi;

            if (item != null)
            {
                if (ConfirmHandler.Confirm($"Confermate di voler eliminare il costo selezionato ?\nNon sara' possibile recuperare le informazioni eliminate, procedere ?"))
                {
                    _dataContext.Delete(item);
                    LoadData();
                }
                Mouse.OverrideCursor = null;
            }
        }
        #endregion

        #region UC standard functions
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs? e)
        {
            (RadGridViewCommands.SearchByText as RoutedUICommand)?.Execute(txtSearch.Text, GridViewMaster);
        }
        #endregion
    }
}
