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
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Tables.Accounting;

namespace VulpesX.Modules.Default.Tables.Accounting
{
    /// <summary>
    /// Interaction logic for TCECO00FView.xaml
    /// </summary>
    public partial class TCECO00FView : UserControl
    {
        private TCECO00FViewModel _dataContext;
        private int _selectedPage = 0;

        public TCECO00FView()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<TCECO00FViewModel>();

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
            var item = (sender as Button)!.DataContext as TCECO00F;

            if (item != null)
            {
                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<TCECO00FWindowViewModel>();
                windowViewModel.Data = item;
                windowViewModel.IsInsert = false;

                var wTCECO00F = new TCECO00FWindow(windowViewModel);
                wTCECO00F.Owner = Window.GetWindow(this);
                if (wTCECO00F.ShowDialog() == true)
                    LoadData();
            }
        }

        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)!.DataContext as TCECO00F;

            if (item != null)
            {
                if (ConfirmHandler.Confirm($"Confermate l'eliminazione definitiva del centro di costo\n[{item.FullDescriptionSearchable}] ?"))
                {
                    var checkMessage = _dataContext.CanDelete(item.cecodc);
                    if (string.IsNullOrWhiteSpace(checkMessage))
                    {
                        _dataContext.Delete(item);
                        LoadData();
                    }
                    else
                    {
                        ErrorHandler.Show(checkMessage);
                    }
                }
            }
        }

        private void cmdInsert_Click(object sender, RoutedEventArgs e)
        {
            var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<TCECO00FWindowViewModel>();
            windowViewModel.Data = new TCECO00F { cecodc = string.Empty };
            windowViewModel.IsInsert = true;

            var wTCECO00F = new TCECO00FWindow(windowViewModel);
            wTCECO00F.Owner = Window.GetWindow(this);
            if (wTCECO00F.ShowDialog() == true)
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
