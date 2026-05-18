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
using VulpesX.Shared;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Tables.Accounting;

namespace VulpesX.Modules.Default.Tables.Accounting
{
    /// <summary>
    /// Interaction logic for PAGFORView.xaml
    /// </summary>
    public partial class PAGFORView : UserControl
    {
        private PAGFORViewModel _dataContext;
        private int _selectedPage = 0;

        public PAGFORView()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<PAGFORViewModel>();

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
            var item = (sender as Button)!.DataContext as PAGFOR;

            if (item != null)
            {
                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<PAGFORWindowViewModel>();
                windowViewModel.Data = item;
                windowViewModel.IsInsert = false;

                var wPAGFOR = new PAGFORWindow(windowViewModel);
                Mouse.OverrideCursor = null;
                if (wPAGFOR.ShowDialog() == true)
                    LoadData();
            }
        }
        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)!.DataContext as PAGFOR;
            if (item != null)
            {
                if (ConfirmHandler.Confirm($"Confermate l'eliminazione del pagamento fornitore\n{item.FullDescriptionSearchable}"))
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    var canDeleteMessage = _dataContext.CanDelete(item);
                    if (string.IsNullOrWhiteSpace(canDeleteMessage))
                    {
                        //// ask for reason
                        //var wAskCR = new wAskCancelReason(ctx);
                        //wAskCR.Owner = Window.GetWindow(this);
                        //Mouse.OverrideCursor = null;
                        //if (wAskCR.ShowDialog() == true && !string.IsNullOrWhiteSpace(wAskCR.SelectedReason))
                        //{
                        //    Mouse.OverrideCursor = Cursors.Wait;
                        //    item.canceled = DateTimeService.GetDatabaseServerDateTime().Value;
                        //    item.canceledUserID = ctx.CurrentUserID;
                        //    item.canceledNote = wAskCR.SelectedReason;
                        //    svc.Update(item);
                        //    Mouse.OverrideCursor = null;
                        //    LoadData();
                        //}
                    }
                    else
                    { Mouse.OverrideCursor = null; ErrorHandler.Validation(canDeleteMessage); }
                }
            }
        }
        private void cmdInsert_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<PAGFORWindowViewModel>();
            windowViewModel.Data = new PAGFOR { pfocod = string.Empty, pfodes = string.Empty, pfoppa = "0" };
            windowViewModel.IsInsert = true;

            var wPAGFOR = new PAGFORWindow(windowViewModel);
            Mouse.OverrideCursor = null;
            wPAGFOR.ShowDialog();
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
