using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using VulpesX.DAL;
using VulpesX.Models.Models.Accounting;
using VulpesX.Shared;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.Accounting.IVA;
using VulpesX.ViewModels.Modules.Default.Tables.Accounting;

namespace VulpesX.Modules.Default.Accounting.IVA
{
    /// <summary>
    /// Interaction logic for TCOMLIQIVAView.xaml
    /// </summary>
    public partial class TCOMLIQIVAView : UserControl
    {
        private TCOMLIQIVAViewModel _dataContext;
        private int _selectedPage = 0;

        public TCOMLIQIVAView()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<TCOMLIQIVAViewModel>();

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

            dtpYear.Culture = new System.Globalization.CultureInfo("it-IT");
            dtpYear.Culture.DateTimeFormat.ShortDatePattern = "yyyy";
            dtpYear.SelectedValue = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

            LoadData();
        }

        private async void LoadData()
        {
            if (dtpYear.SelectedValue.HasValue)
                await _dataContext.Load(dtpYear.SelectedValue.Value.Year);
        }

        #region Buttons
        private void cmdEdit_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var item = (sender as Button)?.DataContext as TCOMLIQIVA;

            if (item != null)
            {
                var items = _dataContext.GetListDetails(item.CLIAnnLiq, item.CLIPerLiq);

                if ((items ?? new System.Collections.ObjectModel.ObservableCollection<TCOMLIQIVA>()).Any())
                {
                    var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<TCOMLIQIVAWindowViewModel>();
                    windowViewModel.Rows = items;
                    windowViewModel.Data = items!.First();
                    windowViewModel.IsInsert = false;

                    var wTCOMLIQIVA = new TCOMLIQIVAWindow(windowViewModel);
                    wTCOMLIQIVA.Owner = Window.GetWindow(this);
                    Mouse.OverrideCursor = null;
                    if (wTCOMLIQIVA.ShowDialog() == true)
                        LoadData();
                }
            }
        }

        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Button)?.DataContext as TCOMLIQIVA;

            if (item != null)
            {
                if (ConfirmHandler.Confirm($"Confermate di voler eliminare l'elaborazione LIPE {item.CLIAnnLiq}/{item.CLIPerLiq}\nNon sara' possibile recuperare le informazioni eliminate, procedere ?"))
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    _dataContext.Delete(item);
                    LoadData();
                }
                Mouse.OverrideCursor = null;
            }
        }

        private void cmdInsert_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<TCOMLIQIVALipeWindowViewModel>();

            var wLaunch = new TCOMLIQIVALipeWindow(windowViewModel);
            wLaunch.Owner = Window.GetWindow(this);
            Mouse.OverrideCursor = null;
            if (wLaunch.ShowDialog() == true)
                LoadData();
        }
        #endregion

        #region UC standard functions
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs? e)
        {
            (RadGridViewCommands.SearchByText as RoutedUICommand)?.Execute(txtSearch.Text, GridView);
        }

        private void dtpYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadData();
        }
        #endregion

        private void rgGenerate_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            var item = (sender as RadGlyph)?.DataContext as TCOMLIQIVA;

            if (item != null)
            {
                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<TCOMLIQIVALipeXMLWindowViewModel>();
                windowViewModel.Year = item.CLIAnnLiq;
                windowViewModel.Month = item.CLIPerLiq;
                windowViewModel.LIPEType = item.CLITipLiq;

                var wXML = new TCOMLIQIVALipeXMLWindow(windowViewModel);
                wXML.Owner = Window.GetWindow(this);
                Mouse.OverrideCursor = null;
                if (wXML.ShowDialog() == true)
                {
                    if (ConfirmHandler.Confirm($"Il file XML e' stato creato correttamente nel seguente percorso:\n\n{Path.GetTempPath()}\n\nVolete aprire la cartella dove e' avvenuto il salvataggio ?"))
                    {
                        var proc = new ProcessStartInfo(Path.GetTempPath());
                        proc.UseShellExecute = true;
                        Process.Start(proc);
                    }
                }
            }
        }
    }
}
