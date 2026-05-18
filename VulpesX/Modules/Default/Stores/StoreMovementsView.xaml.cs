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
using Telerik.Windows.Controls.Filtering.Editors;
using VulpesX.DAL;
using VulpesX.Shared;
using VulpesX.Shared.Controls.Utilities;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Stores;
using static VulpesX.Shared.Utilities.TelerikGridService;

namespace VulpesX.Modules.Default.Stores
{
    /// <summary>
    /// Interaction logic for StoreMovementsView.xaml
    /// </summary>
    public partial class StoreMovementsView : UserControl
    {
        private StoreMovementsViewModel _dataContext;
        private int _selectedPage;
        private List<GenericIDDescription> _currentSort = new List<GenericIDDescription>();
        private List<FilterEntry> _currentWhere = new List<FilterEntry>();

        public StoreMovementsView()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<StoreMovementsViewModel>();

            InitializeComponent();


            dtpYear.Culture = new System.Globalization.CultureInfo("it-IT");
            dtpYear.Culture.DateTimeFormat.ShortDatePattern = "yyyy";

            this.DataContext = _dataContext;

            _dataContext.Year = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

            this.PreviewKeyDown += (s, e) =>
            {
                if (e.Key == Key.F5)
                    LoadData();
            };

            GridView.DataLoaded += (s, e) =>
            {
                rdpGrid.MoveToPage(_selectedPage);
                rdpGrid.ItemCount = _dataContext.TotalCount;
            };
            rdpGrid.PageIndexChanged += (s, e) =>
            {
                _selectedPage = e.NewPageIndex;
            };

            LoadData();
        }

        private async void LoadData()
        {
            if (_dataContext.Year != null)
            {
                await _dataContext.Load(_selectedPage, _currentSort, _currentWhere);
            }
        }

        #region Buttons
        private void cmdEdit_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var item = (sender as Button)?.DataContext as store_movements;

            if (item != null)
            {
                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<StoreMovementsWindowViewModel>();
                windowViewModel.Data = item;
                windowViewModel.IsInsert = false;

                var wSTORE_MOVEMENTS = new StoreMovementsWindow(windowViewModel);
                if (wSTORE_MOVEMENTS.ShowDialog() == true)
                    LoadData();
            }
        }

        private void cmdInsert_Click(object sender, RoutedEventArgs e)
        {
            var year = dtpYear.SelectedValue?.Year ?? 0;

            if (year > 0)
            {
                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<StoreMovementsWindowViewModel>();
                windowViewModel.Data = new store_movements()
                {
                    id = _dataContext.GetMovementNumerator(),
                    company_id = _dataContext.CompanyID,
                    date = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime(),
                    store_id = string.Empty,
                    product_id = string.Empty,
                };
                windowViewModel.IsInsert = true;
                var wSTORE_MOVEMENTS = new StoreMovementsWindow(windowViewModel);
                wSTORE_MOVEMENTS.ShowDialog();
                LoadData();
            }
            else
            {
                ErrorHandler.Validation("Selezionare un anno");
            }
        }

        private void cmdCheckExistance_Click(object sender, RoutedEventArgs e)
        {
            var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<StoreMovementsCheckWindowViewModel>();
            var wSTORE_CHECKEXISTANCE = new StoreMovementsCheckWindow(windowViewModel);
            wSTORE_CHECKEXISTANCE.ShowDialog();
        }
        #endregion

        #region Custom loading
        private void dtpYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadData();
        }
        private void rdpGrid_PageIndexChanged(object sender, PageIndexChangedEventArgs e)
        {
            if (e.OldPageIndex != -1)
            {
                _selectedPage = e.NewPageIndex;

                LoadData();
            }
        }
        private void GridView_Filtering(object sender, GridViewFilteringEventArgs e)
        {
            e.Cancel = true;
            TelerikControls.FilterManager(_currentWhere, e);
            LoadData();
            e.Cancel = false;
        }
        private void GridView_Sorting(object sender, GridViewSortingEventArgs e)
        {
            if (e.OldSortingState == SortingState.None)
            {
                e.NewSortingState = SortingState.Ascending;
            }
            else if (e.OldSortingState == SortingState.Ascending)
            {
                e.NewSortingState = SortingState.Descending;
            }
            else
            {
                e.NewSortingState = SortingState.None;
            }
            TelerikControls.SortManager(_currentSort, e);
            LoadData();
            e.Cancel = true;
        }
        private void txtSearch_LostFocus(object sender, RoutedEventArgs? e)
        {
            if (e != null)
                LoadData();
            else
                (RadGridViewCommands.SearchByText as RoutedUICommand)?.Execute(txtSearch.Text, GridView);
        }
        private void GridView_FieldFilterEditorCreated(object sender, Telerik.Windows.Controls.GridView.EditorCreatedEventArgs e)
        {
            //get the StringFilterEditor in your RadGridView 
            var stringFilterEditor = e.Editor as StringFilterEditor;
            if (stringFilterEditor != null)
            {
                stringFilterEditor.MatchCaseVisibility = Visibility.Collapsed;
            }
        }
        private void OnRadGridViewFilterOperatorsLoading(object sender, Telerik.Windows.Controls.GridView.FilterOperatorsLoadingEventArgs e)
        {
            TelerikControls.CleanFilters(e);
        }
        #endregion
    }
}
