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
using System.Windows.Shapes;
using VulpesX.DAL;
using VulpesX.Models;
using VulpesX.Shared;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Stores;

namespace VulpesX.Modules.Default.Stores
{
    /// <summary>
    /// Interaction logic for StoreStocksReportWindow.xaml
    /// </summary>
    public partial class StoreStocksReportWindow : FluentDefaultWindow
    {
        private StoreStocksReportWindowViewModel _dataContext;
        public StoreStocksReportWindow()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<StoreStocksReportWindowViewModel>();

            InitializeComponent();

            this.DataContext = _dataContext;

            _dataContext.LoadDetails();
            _dataContext.ShowZeroStocks = true;

            _dataContext.SelectedStore = _dataContext.Stores?.Where(w => w.id == null).FirstOrDefault();
            _dataContext.SelectedProduct = _dataContext.Products?.Where(w => w.ID == null).FirstOrDefault();
            _dataContext.SelectedProductType = _dataContext.ProductTypes?.Where(w => w.ID == null).FirstOrDefault();
        }

        private async void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

            var reportData = await _dataContext.GetStocksListReport();

            if (reportData != null)
            {
                reportData.ReportTitle = $"SITUAZIONE GIACENZE AL {now.ToString("dd/MM/yyyy")}";
                reportData.PrintedText = $"Stampato il {now.ToString("dd/MM/yyyy HH:mm:ss")}";

                Mouse.OverrideCursor = null;
                var result = ReportingHandler.PrintPDF(UserContext.Instance.Domain!, Constants.MODULE_STORE, Constants.REPORT_TYPE_STORE_STOCKS, _dataContext.CompanyID, reportData, $"Giacenze al {now.ToString("dd_MM_yyyy HH_mm_ss")}", $"Giacenze_{now.ToString("dd_MM_yyyy_HH_mm_ss")}.pdf", true);
            }
        }

        #region Autocompletes
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

        private void togShowMovements_Checked(object sender, RoutedEventArgs e)
        {
            InfoHandler.Show("Attenzione, la stampa dei movimenti di magazzino, in caso di tutti i magazzini e tutti i prodotti potrebbe richiedere molto tempo e generare una stampa di dimensioni estremamente impegnative");
        }

        private void togShowEngages_Checked(object sender, RoutedEventArgs e)
        {
            InfoHandler.Show("Attenzione, la stampa degli impegni di magazzino, in caso di tutti i magazzini e tutti i prodotti potrebbe richiedere molto tempo e generare una stampa di dimensioni estremamente impegnative");
        }
    }
}
