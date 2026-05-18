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
using VulpesX.ViewModels.Modules.Default.Tables.Accounting;

namespace VulpesX.Modules.Default.Tables.Accounting
{
    /// <summary>
    /// Interaction logic for FE_IVADOCView.xaml
    /// </summary>
    public partial class FE_IVADOCView : UserControl
    {
        private FE_IVADOCViewModel _dataContext;
        private int _selectedPage = 0;

        public FE_IVADOCView()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<FE_IVADOCViewModel>();

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
            var item = (sender as Button)?.DataContext as FE_IVADOC;

            if (item != null)
            {
                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<FE_IVADOCWindowViewModel>();
                windowViewModel.Data = item;
                windowViewModel.IsInsert = false;

                var wFE_IVADOC = new FE_IVADOCWindow(windowViewModel);
                wFE_IVADOC.Owner = Window.GetWindow(this);
                Mouse.OverrideCursor = null;
                if (wFE_IVADOC.ShowDialog() == true)
                    LoadData();
            }
        }

        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)?.DataContext as FE_IVADOC;

            if (item != null)
            {
                if (ConfirmHandler.Confirm($"Confermate l'annullamento del tipo natura IVA\n {item.FullDescriptionSearchable} ?"))
                {
                    // ask for reason
                    var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<CancelReasonWindowViewModel>();
                    var wAskCR = new CancelReasonWindow(windowViewModel);
                    wAskCR.Owner = Window.GetWindow(this);
                    if (wAskCR.ShowDialog() == true && !string.IsNullOrWhiteSpace(windowViewModel.SelectedReason))
                    {
                        Mouse.OverrideCursor = Cursors.Wait;

                        _dataContext.Delete(item, windowViewModel.SelectedReason);

                        Mouse.OverrideCursor = null;

                        InfoHandler.Show($"Tipo natura IVA annullata con successo.");
                        LoadData();
                    }
                }
                Mouse.OverrideCursor = null;
            }
        }

        private void cmdInsert_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<FE_IVADOCWindowViewModel>();
            windowViewModel.Data = new FE_IVADOC { FETICod = string.Empty, FETIDes = string.Empty };
            windowViewModel.IsInsert = true;

            var wFE_IVADOC = new FE_IVADOCWindow(windowViewModel);
            wFE_IVADOC.Owner = Window.GetWindow(this);
            Mouse.OverrideCursor = null;
            if (wFE_IVADOC.ShowDialog() == true)
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
