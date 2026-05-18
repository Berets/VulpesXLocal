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
using VulpesX.Models;
using VulpesX.Shared;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Production;

namespace VulpesX.Modules.Default.Production
{
    /// <summary>
    /// Interaction logic for LotManagerView.xaml
    /// </summary>
    public partial class LotManagerView : UserControl
    {
        private LotManagerViewModel _dataContext;
        private int _selectedPage;

        public LotManagerView()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<LotManagerViewModel>();

            InitializeComponent();

            this.DataContext = _dataContext;

            this.PreviewKeyDown += (s, e) =>
            {
                if (e.Key == Key.F5)
                    LoadData();
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
            var item = (sender as Button)?.DataContext as pro_ordine_lotti;

            if (item != null)
            {
                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<LotManagerWindowViewModel>();
                windowViewModel.Data = item;
                windowViewModel.IsInsert = false;

                var wPRO_ORDINE_LOTTI = new LotManagerWindow(windowViewModel);
                wPRO_ORDINE_LOTTI.Owner = Window.GetWindow(this);
                Mouse.OverrideCursor = null;
                if (wPRO_ORDINE_LOTTI.ShowDialog() == true)
                    LoadData();
            }
        }

        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)?.DataContext as pro_ordine_lotti;
        }

        private void cmdInsert_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<LotManagerWindowViewModel>();
            windowViewModel.Data = new pro_ordine_lotti
            {
                addedUserID = _dataContext.UserID,
                Descrizione = string.Empty,
                ID = String.Empty,
                SocietaID = _dataContext.CompanyID,
                OrdineID = string.Empty
            };
            windowViewModel.IsInsert = true;

            var wPRO_ORDINE_LOTTI = new LotManagerWindow(windowViewModel);
            wPRO_ORDINE_LOTTI.Owner = Window.GetWindow(this);
            Mouse.OverrideCursor = null;
            if (wPRO_ORDINE_LOTTI.ShowDialog() == true)
                LoadData();
        }
        #endregion

        #region UC standard functions
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs? e)
        {
            (RadGridViewCommands.SearchByText as RoutedUICommand)?.Execute(txtSearch.Text, GridView);
        }
        #endregion

        #region Grid functions
        private void rgPrintLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            var item = (sender as RadGlyph)?.DataContext as pro_ordine_lotti;
            if (item != null)
            {
                item = _dataContext.GetFull(item.OrdineID, item.ID);

                if (item != null)
                {
                    var reportData = _dataContext.PrintLabel(item);
                    if (reportData != null)
                        ReportingHandler.PrintBookPDF(UserContext.Instance!.Domain!, Constants.MODULE_PRODUCTION, new string[] { Constants.REPORT_TYPE_FINAL_LOT_LABEL }, _dataContext.CompanyID, reportData, $"{item.ID}", $"{item.ID}.pdf", true, false);
                    else
                        ErrorHandler.Validation($"Impossibile trovare il lotto {item.ID}");
                }
            }
        }

        private void rgTracking_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var item = (sender as RadGlyph)?.DataContext as pro_ordine_lotti;
            if (item != null)
            {
                Mouse.OverrideCursor = Cursors.Wait;

                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<LotManagerTrackingWindowViewModel>();
                windowViewModel.Data = item;

                Mouse.OverrideCursor = null;
                var wShowData = new LotManagerTrackingWindow(windowViewModel);
                wShowData.Owner = Window.GetWindow(this);
                wShowData.ShowDialog();
            }
        }
        #endregion
    }
}
