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
using VulpesX.DAL;
using VulpesX.Modules.Default.Commons;
using VulpesX.Shared;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Commons;
using VulpesX.ViewModels.Modules.Default.SRM;

namespace VulpesX.Modules.Default.SRM
{
    /// <summary>
    /// Interaction logic for SRM_LISFORView.xaml
    /// </summary>
    public partial class SRM_LISFORView : UserControl
    {
        private SRM_LISFORViewModel _dataContext;
        private int _selectedPage = 0;

        public SRM_LISFORView()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<SRM_LISFORViewModel>();

            InitializeComponent();

            this.DataContext = _dataContext;

            _dataContext.LoadDetails();

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
            var item = (sender as Button)?.DataContext as SRM_LISFOR;

            if (item != null)
            {
                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<SRM_LISFORWindowViewModel>();
                windowViewModel.Data = item;
                windowViewModel.IsInsert = false;

                var wSRM_LISFOR = new SRM_LISFORWindow(windowViewModel);
                wSRM_LISFOR.Owner = Window.GetWindow(this);
                Mouse.OverrideCursor = null;
                if (wSRM_LISFOR.ShowDialog() == true)
                    LoadData();
            }
        }

        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            var item = (sender as Button)?.DataContext as SRM_LISFOR;

            if (item != null)
            {
                if (ConfirmHandler.Confirm($"Confermate l'annullamento del listino\n{item.Product?.FullDescriptionSearchable} dal {item.fromDate.ToShortDateString()} al {item.toDate.ToShortDateString()} Intervallo: {item.fromQuantity.ToString("N6")}-{item.toQuantity.ToString("N6")} per il fornitore {item.Supplier?.FullDescriptionSearchable} ?"))
                {
                    // ask for reason
                    var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<CancelReasonWindowViewModel>();

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

            var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<SRM_LISFORWindowViewModel>();
            windowViewModel.Data = new SRM_LISFOR()
            {
                companyID = _dataContext.CompanyID,
                fromDate = today,
                toDate = today.AddMonths(1),
                fromQuantity = 1,
                supplierID = _dataContext.SelectedSupplier != null ? _dataContext.SelectedSupplier.abecod : 0,
                productID = string.Empty,
            };
            windowViewModel.IsInsert = true;

            var wSRM_LISFOR = new SRM_LISFORWindow(windowViewModel);
            wSRM_LISFOR.Owner = Window.GetWindow(this);
            Mouse.OverrideCursor = null;
            if (wSRM_LISFOR.ShowDialog() == true)
                LoadData();
        }

        private void cmdCopyFrom_Click(object sender, RoutedEventArgs e)
        {
            var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<SRM_LISFORCopySupplierWindowViewModel>();
            var wCopy = new SRM_LISFORCopySupplierWindow(windowViewModel);
            wCopy.Owner = Window.GetWindow(this);
            if (wCopy.ShowDialog() == true && windowViewModel.SelectedSourceSupplier != null && windowViewModel.SelectedTargetSupplier != null)
            {
                _dataContext.CopyFrom(windowViewModel.SelectedSourceSupplier.abecod, windowViewModel.SelectedTargetSupplier.abecod);
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

        #region Autocompletes
        private void acSupplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadData();
        }

        private void ac_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = ((RadAutoCompleteBox)sender).ChildrenOfType<TextBox>().First();
            Dispatcher.BeginInvoke(new Action(() => { textBox.SelectAll(); }));
        }

        private void ac_LostFocus(object sender, RoutedEventArgs e)
        {
            var ac = sender as RadAutoCompleteBox;
            if (ac != null)
            {
                if (ac.SelectedItem == null)
                {
                    ac.SearchText = null;
                }
            }
        }
        #endregion
    }
}
