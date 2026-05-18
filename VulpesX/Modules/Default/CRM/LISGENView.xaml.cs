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
using VulpesX.DAL;
using VulpesX.Modules.Default.Commons;
using VulpesX.Shared;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Commons;
using VulpesX.ViewModels.Modules.Default.CRM;

namespace VulpesX.Modules.Default.CRM
{
    /// <summary>
    /// Interaction logic for LISGENView.xaml
    /// </summary>
    public partial class LISGENView : UserControl
    {
        private LISGENViewModel _dataContext;
        private int _selectedPage;

        public LISGENView()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<LISGENViewModel>();

            InitializeComponent();

            this.DataContext = _dataContext;

            cmbStatus.ItemsSource = CommonsService.PriceListFilters;
            cmbStatus.SelectedValue = "V";

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
            string? selectedStatusID = cmbStatus.SelectedValue?.ToString();

            if (selectedStatusID != null)
            {
                await _dataContext.Load(selectedStatusID);
            }
        }

        #region Buttons
        private void cmdEdit_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var item = (sender as Button)?.DataContext as CRM_LISGEN;

            if (item != null)
            {
                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<LISGENWindowViewModel>();
                windowViewModel.Data = item;
                windowViewModel.IsInsert = false;

                var wCRM_LISGEN = new LISGENWindow(windowViewModel);
                wCRM_LISGEN.Owner = Window.GetWindow(this);
                Mouse.OverrideCursor = null;
                if (wCRM_LISGEN.ShowDialog() == true)
                    LoadData();
            }
        }

        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var item = (sender as Button)?.DataContext as CRM_LISGEN;

            if (item != null)
            {
                if (ConfirmHandler.Confirm($"Confermate l'annullamento del listino\n{item.Product?.FullDescriptionSearchable} dal {item.fromDate.ToShortDateString()} al {item.toDate.ToShortDateString()} Intervallo: {item.fromQuantity.ToString("N6")}-{item.toQuantity.ToString("N6")} ?"))
                {
                    var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<CancelReasonWindowViewModel>();
                    // ask for reason
                    var wAskCR = new CancelReasonWindow(windowViewModel);
                    wAskCR.Owner = Window.GetWindow(this);
                    Mouse.OverrideCursor = null;
                    if (wAskCR.ShowDialog() == true && !string.IsNullOrWhiteSpace(windowViewModel.SelectedReason))
                    {
                        Mouse.OverrideCursor = Cursors.Wait;
                        item.canceled = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();
                        item.canceledUserID = _dataContext.UserID;
                        item.canceledNote = windowViewModel.SelectedReason;

                        _dataContext.Update(item);
                        Mouse.OverrideCursor = null;

                        InfoHandler.Show($"Listino annullato con successo.");
                        LoadData();
                    }
                }
                Mouse.OverrideCursor = null;
            }
        }

        private void cmdInsert_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var today = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

            var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<LISGENWindowViewModel>();
            windowViewModel.Data = new CRM_LISGEN()
            {
                companyID = _dataContext.CompanyID,
                fromDate = today,
                toDate = today.AddMonths(1),
                fromQuantity = 1,
                productID = string.Empty,
            };
            windowViewModel.IsInsert = true;

            var wCRM_LISGEN = new LISGENWindow(windowViewModel);
            wCRM_LISGEN.Owner = Window.GetWindow(this);
            Mouse.OverrideCursor = null;
            if (wCRM_LISGEN.ShowDialog() == true)
                LoadData();
        }

        private void cmdCopyFrom_Click(object sender, RoutedEventArgs e)
        {
            var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<LISGENCopyProductWindowViewModel>();

            var wCopy = new LISGENCopyProductWindow(windowViewModel);
            wCopy.Owner = Window.GetWindow(this);
            if (wCopy.ShowDialog() == true && windowViewModel.SelectedSourceProduct != null && windowViewModel.SelectedTargetProduct != null)
            {
                _dataContext.CopyFrom(windowViewModel.SelectedSourceProduct.ID, windowViewModel.SelectedTargetProduct.ID);
                LoadData();
            }
        }
        #endregion

        #region UC standard functions
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs? e)
        {
            (RadGridViewCommands.SearchByText as RoutedUICommand)?.Execute(txtSearch.Text, GridView);
        }

        private void cmbStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadData();
        }
        #endregion

    }
}
