using Itenso.TimePeriod;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using VulpesX.Models.Models.Accounting.Assets;
using VulpesX.Shared;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Accounting.Assets;

namespace VulpesX.Modules.Default.Accounting.Assets
{
    /// <summary>
    /// Interaction logic for ACC_ASSETS_CARDSView.xaml
    /// </summary>
    public partial class ACC_ASSETS_CARDSView : UserControl
    {
        private ACC_ASSETS_CARDSViewModel _dataContext;
        public ACC_ASSETS_CARDSView()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<ACC_ASSETS_CARDSViewModel>();

            InitializeComponent();

            this.DataContext = _dataContext;
            this.PreviewKeyDown += async (s, e) =>
            {
                if (e.Key == Key.F5)
                {
                    await LoadData();
                }
            };
            this.Loaded += async (s, e) =>
            {
                await LoadData();
            };

            _dataContext.Years = _dataContext.GetDistinctYear();
        }

        private async Task LoadData()
        {
            await _dataContext.Load();
        }

        #region Buttons
        private async void cmdEdit_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var item = (sender as Button)?.DataContext as ACC_ASSETS_CARDS;

            if (item != null)
            {
                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ACC_ASSETS_CARDSWindowViewModel>();
                windowViewModel.Data = item;
                windowViewModel.IsInsert = false;

                var wACC_ASSETS_CARDS = new ACC_ASSETS_CARDSWindow(windowViewModel);
                wACC_ASSETS_CARDS.Owner = Window.GetWindow(this);
                Mouse.OverrideCursor = null;
                if (wACC_ASSETS_CARDS.ShowDialog() == true)
                    await LoadData();
            }
        }

        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)?.DataContext as ACC_ASSETS_CARDS;

            MessageBox.Show($"Impossibile eliminare");
        }

        private async void cmdInsert_Click(object sender, RoutedEventArgs e)
        {
            var insertWindowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ACC_ASSETS_CARDSInsertWindowViewModel>();

            var wAsk = new ACC_ASSETS_CARDSInsertWindow(insertWindowViewModel);
            wAsk.Owner = Window.GetWindow(this);
            if (wAsk.ShowDialog() == true )
            {
                // get progressives
                var typ = _dataContext.GetACC_ASSETS_TYPOLOGIES(insertWindowViewModel.SelectedGroupID!, insertWindowViewModel.SelectedAccountID!, insertWindowViewModel.SelectedSubaccountID!);

                if (typ != null)
                {
                    var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ACC_ASSETS_CARDSWindowViewModel>();
                    windowViewModel.Data = new ACC_ASSETS_CARDS()
                    {
                        besoci = _dataContext.CompanyID,
                        beann4 = insertWindowViewModel.SelectedYear,
                        begrup = insertWindowViewModel.SelectedGroupID!,
                        becont = insertWindowViewModel.SelectedAccountID!,
                        besotc = insertWindowViewModel.SelectedSubaccountID!,
                        benin2 = (typ.tnupro ?? 0),
                        beinv = 0,
                        beinc = "+",
                        becat = typ.jcateg
                    };
                    windowViewModel.IsInsert = true;

                    var wACC_ASSETS_CARDS = new ACC_ASSETS_CARDSWindow(windowViewModel);
                    wACC_ASSETS_CARDS.Owner = Window.GetWindow(this);
                    Mouse.OverrideCursor = null;
                    if (wACC_ASSETS_CARDS.ShowDialog() == true)
                        await LoadData();
                }
                else
                {
                    Mouse.OverrideCursor = null;
                    ErrorHandler.Validation("Impossibile trovare la tipologia del cespite");
                }
            }
        }
        #endregion

        #region UC standard functions
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs? e)
        {
            (RadGridViewCommands.SearchByText as RoutedUICommand)?.Execute(txtSearch.Text, GridView);
        }

        private async void rcbYears_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           await LoadData();
        }
        #endregion

        private void rgHistory_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            var item = (sender as RadGlyph)?.DataContext as ACC_ASSETS_CARDS;

            if (item != null)
            {
                var fiscal = _dataContext.GetACC_ASSETS_DEP_HISTORYs(item);
                var civilistic = _dataContext.GetACC_ASSETS_DEP_CIV_HISTORYs(item);
                #region Merge
                var history = new List<AssetHistoryItem>();

                foreach (var fis in fiscal ?? new ObservableCollection<ACC_ASSETS_DEP_HISTORY>())
                {
                    history.Add(new AssetHistoryItem() { Year = fis.bhanco, Fiscal = fis });
                }
                foreach (var civ in civilistic ?? new ObservableCollection<ACC_ASSETS_DEP_CIV_HISTORY>())
                {
                    if (!history.Any(w => w.Year == civ.bcanco))
                        history.Add(new AssetHistoryItem() { Year = civ.bcanco, Civilistic = civ });
                    else
                        history.Where(w => w.Year == civ.bcanco).First().Civilistic = civ;
                }
                #endregion

                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ACC_ASSETS_CARDSHistoryWindowViewModel>();
                windowViewModel.Title = $"Storico calcoli cespite {item.begrup}|{item.becont}|{item.besotc}";
                windowViewModel.Items = new ObservableCollection<AssetHistoryItem>(history);

                var wShow = new ACC_ASSETS_CARDSHistoryWindow(windowViewModel);
                wShow.Owner = Window.GetWindow(this);
                wShow.ShowDialog();
            }
            Mouse.OverrideCursor = null;
        }
    }
}
