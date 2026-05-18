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
using VulpesX.Models;
using VulpesX.Shared;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Tables.Production;

namespace VulpesX.Modules.Default.Tables.Production
{
    /// <summary>
    /// Interaction logic for OperatoriView.xaml
    /// </summary>
    public partial class OperatoriView : UserControl
    {
        private OperatoriViewModel _dataContext;
        private int _selectedPage = 0;
        public OperatoriView()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<OperatoriViewModel>();

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
            var item = (sender as Button)!.DataContext as tab_produzione_operatore;

            if (item != null)
            {

                var banazienWindowViewModel = VulpesServiceProvider.Provider.GetRequiredService<OperatoriWindowViewModel>();
                banazienWindowViewModel.Data = item;
                banazienWindowViewModel.IsInsert = false;
                banazienWindowViewModel.Costs = _dataContext.GetCosts(item.ID);

                var wBANAZIEN = new OperatoriWindow(banazienWindowViewModel);
                if (wBANAZIEN.ShowDialog() == true)
                    LoadData();
            }
        }

        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)!.DataContext as tab_produzione_operatore;

            if (item != null)
            {
                if (ConfirmHandler.Confirm($"Confermate l'eliminazione operatore {item.Descrizione} ?"))
                {
                    if (_dataContext.Delete(item))
                        LoadData();
                    else
                        ErrorHandler.Validation("Errore imprevisto durante l'eliminazione");
                }
            }
        }

        private void cmdInsert_Click(object sender, RoutedEventArgs e)
        {
            var banazienWindowViewModel = VulpesServiceProvider.Provider.GetRequiredService<OperatoriWindowViewModel>();
            banazienWindowViewModel.Data = new tab_produzione_operatore
            {
                ID = string.Empty,
                SocietaID = _dataContext.CompanyID,
                Descrizione = string.Empty,
                OperatoreRisorse = new System.Collections.ObjectModel.ObservableCollection<tab_produzione_risorsa>(),
            };
            banazienWindowViewModel.IsInsert = true;
            banazienWindowViewModel.Costs = new System.Collections.ObjectModel.ObservableCollection<tab_produzione_operatore_costo>();

            var wBANAZIEN = new OperatoriWindow(banazienWindowViewModel);
            wBANAZIEN.ShowDialog();
            LoadData();
        }

        private void rgPrint_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            var item = (sender as RadGlyph)?.DataContext as tab_produzione_operatore;

            if (item != null)
            {
                var reportData = _dataContext.GetPrintBadge(item);

                if (reportData != null)
                    ReportingHandler.PrintPDF(UserContext.Instance.Domain!, Constants.MODULE_PRODUCTION, Constants.REPORT_TYPE_PRODUCTION_BADGE_OPERATORE, _dataContext.CompanyID, reportData, $"BADGEOPERATORE n.{item.ID}", $"BADGEOPERATORE [{item.SocietaID}] {item.Badge}.pdf", true);
                else
                    ErrorHandler.Validation($"Impossibile recuperare il BADGE per l'operatore {item.ID}/{item.Descrizione}");
            }
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
