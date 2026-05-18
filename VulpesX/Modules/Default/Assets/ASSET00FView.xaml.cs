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
using VulpesX.ViewModels.Modules.Default.Assets;
using VulpesX.ViewModels.Modules.Default.Commons;

namespace VulpesX.Modules.Default.Assets
{
    /// <summary>
    /// Interaction logic for ASSET00FView.xaml
    /// </summary>
    public partial class ASSET00FView : UserControl
    {
        private ASSET00FViewModel _dataContext;
        private int _selectedPage = 0;

        public ASSET00FView()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<ASSET00FViewModel>();

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
            var item = (sender as Button)?.DataContext as ASSET00F;

            if (item != null)
            {
                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ASSET00FWindowViewModel>();
                windowViewModel.Data = item;
                windowViewModel.IsInsert = false;

                var wASSET00F = new ASSET00FWindow(windowViewModel);
                wASSET00F.Owner = Window.GetWindow(this);
                Mouse.OverrideCursor = null;
                if (wASSET00F.ShowDialog() == true)
                    LoadData();
            }
        }

        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var item = (sender as Button)?.DataContext as ASSET00F;

            if (item != null)
            {
                if (ConfirmHandler.Confirm($"Confermate di voler annullare l'asset selezionato [{item.id} {item.description}] ?\nNon sara' possibile recuperare le informazioni eliminate, procedere ?"))
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
                        LoadData();
                    }
                }
                Mouse.OverrideCursor = null;
            }
        }

        private void cmdInsert_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ASSET00FWindowViewModel>();
            windowViewModel.Data = new ASSET00F
            {
                company_id = _dataContext.CompanyID,
                description = string.Empty,
                id = string.Empty
            };
            windowViewModel.IsInsert = false;

            var wASSET00F = new ASSET00FWindow(windowViewModel);
            Mouse.OverrideCursor = null;
            if (wASSET00F.ShowDialog() == true)
                LoadData();
        }
        #endregion

        #region UC standard functions
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs? e)
        {
            (RadGridViewCommands.SearchByText as RoutedUICommand)?.Execute(txtSearch.Text, GridView);
        }
        #endregion

        #region Grid icons
        private void rgDetails_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var item = (sender as RadGlyph)?.DataContext as ASSET00F;

            if (item != null)
            {
                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ASSED00FWindowViewModel>();
                windowViewModel.Head = item;

                var wASSED00F = new ASSED00FWindow(windowViewModel);
                wASSED00F.Owner = Window.GetWindow(this);
                Mouse.OverrideCursor = null;
                if (wASSED00F.ShowDialog() == true)
                    LoadData();
            }
        }

        private void rgContacts_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var row = (sender as RadGlyph)?.DataContext as ASSET00F;
            if (row != null)
            {
                row.Contacts = _dataContext.GetASSETCO00Fs(row.id);

                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ASSETCO00FWindowViewModel>();
                windowViewModel.Head = row;

                var wASSETCO00F = new ASSETCO00FWindow(windowViewModel);
                wASSETCO00F.Owner = Window.GetWindow(this);
                Mouse.OverrideCursor = null;
                if (wASSETCO00F.ShowDialog() == true)
                    LoadData();
            }
        }
        private void rgAssetAttachmentsIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var row = (sender as RadGlyph)?.DataContext as ASSET00F;
            if (row != null)
            {
                row.Attachments = _dataContext.GetASSETAL00Fs(row.id);
                    
                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ASSETAL00FWindowViewModel>();
                windowViewModel.Head = row;

                var wASSETAL00F = new ASSETAL00FWindow(windowViewModel);
                wASSETAL00F.Owner = Window.GetWindow(this);
                Mouse.OverrideCursor = null;
                if (wASSETAL00F.ShowDialog() == true)
                    LoadData();
            }
        }
        #endregion
    }
}
