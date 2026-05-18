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
using VulpesX.DAL;
using VulpesX.DAL.General;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.General;

namespace VulpesX.Modules.Default.General
{
    /// <summary>
    /// Interaction logic for ARTView.xaml
    /// </summary>
    public partial class ARTView : UserControl
    {
        private ARTViewModel _dataContext;
        private int _selectedPage = 0;

        public ARTView()
        {
            _dataContext = VulpesServiceProvider.Provider.GetRequiredService<ARTViewModel>();

            InitializeComponent();

            this.DataContext = _dataContext;
            this.PreviewKeyDown += (s, e) =>
            {
                if (e.Key == Key.F5)
                    LoadData();
            };


            rgvList.DataLoaded += (s, e) =>
            {
                rdpGrid.MoveToPage(_selectedPage);
                txtSearch_TextChanged(txtSearch, null);
            };
            rdpGrid.PageIndexChanged += (s, e) =>
            {
                _selectedPage = e.NewPageIndex;
            };

            LoadData();
            txtSearch.Focus();
        }

        private async void LoadData()
        {
            await _dataContext.Load();
        }

        private void cmdEdit_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var item = (sender as Button)?.DataContext as tab_articolo;

            if (item != null)
            {
                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ARTWindowViewModel>();
                windowViewModel.Data = item;
                windowViewModel.IsInsert = false;

                var wArticolo = new ARTWindow(windowViewModel);
                wArticolo.Owner = Window.GetWindow(this);
                if (wArticolo.ShowDialog() == true)
                    LoadData();
            }
        }

        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            //Mouse.OverrideCursor = Cursors.Wait;
            //var item = (sender as Button).DataContext as tab_articolo;
            //item = svc.Get(item.SocietaID, item.ID);
            //if (item != null)
            //{
            //    if (MessageBoxResult.Yes == Constants.ShowQuestion($"Confermate l'annullamento dell'articolo\n{item.FullDescriptionSearchable} ?\n\nVerrà effettuata una ricerca per verificare che non ci siano giacenze, offerte aperte, ordini aperti, DDT non evasi, fatture non contabilizzate.\n\nL'articolo non verrà più visualizzato, non potrà più essere utilizzato, ma il suo codice non potrà comunque essere riutilizzato per garantire lo storico."))
            //    {
            //        // temporary lock the product to freeze the state
            //        item.LogCanceled = DateTimeService.GetDatabaseServerDateTime().Value;
            //        var sources = new tab_articolo_produzione_sorgentiService().GetList(ctx.CurrentCompanyID, item.ID);
            //        var externs = tab_articolo_externService.GetList(ctx.CurrentCompanyID, item.ID);
            //        svc.Update(item, sources, externs);
            //        var result = svc.CheckIsCancellable(item.SocietaID, item.ID);
            //        if (string.IsNullOrWhiteSpace(result))
            //        {
            //            // ask for reason
            //            var wAskCancelReason = new wAskCancelReason(ctx);
            //            wAskCancelReason.Owner = Window.GetWindow(this);
            //            Mouse.OverrideCursor = null;
            //            if (wAskCancelReason.ShowDialog() == true && !string.IsNullOrWhiteSpace(wAskCancelReason.SelectedReason))
            //            {
            //                Mouse.OverrideCursor = Cursors.Wait;
            //                item = svc.GetCanceled(item.SocietaID, item.ID);
            //                item.LogCanceled = DateTimeService.GetDatabaseServerDateTime().Value;
            //                item.LogCanceledUserID = ctx.CurrentUserID;
            //                item.LogCanceledReason = wAskCancelReason.SelectedReason;
            //                svc.Update(item, sources, externs);
            //                Mouse.OverrideCursor = null;
            //                Constants.ShowInfo($"L'articolo\n{item.FullDescriptionSearchable}\n è stato annullato con successo.\n\nDa questo momento non verrà più mostrato e non potrà più essere utilizzato, ma il codice non potrà essere riutilizzato perchè necessario per lo storico.");
            //                LoadData();
            //            }
            //            else
            //            {
            //                // unlock product
            //                item = svc.GetCanceled(item.SocietaID, item.ID);
            //                item.LogCanceled = null;
            //                svc.Update(item, sources, externs);
            //                Mouse.OverrideCursor = null;
            //                Constants.ShowError("Impossibile annullare un articolo senza una motivazione valida");
            //            }
            //        }
            //        else
            //        {
            //            // unlock product
            //            item = svc.GetCanceled(item.SocietaID, item.ID);
            //            item.LogCanceled = null;
            //            svc.Update(item, sources, externs);
            //            Mouse.OverrideCursor = null;
            //            Constants.ShowError(result);
            //        }
            //    }
            //    Mouse.OverrideCursor = null;
            //}
        }

        private void cmdInsert_Click(object sender, RoutedEventArgs e)
        {
            var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ARTWindowViewModel>();
            windowViewModel.Data = new tab_articolo
            {
                SocietaID = _dataContext.CompanyID,
                ID = string.Empty,
                Descrizione = string.Empty,
                TipoID = string.Empty,
            };
            windowViewModel.IsInsert = true;

            var wArticolo = new ARTWindow(windowViewModel);
            wArticolo.Owner = Window.GetWindow(this);
            wArticolo.ShowDialog();
            LoadData();
        }

        private void cmdChange_Click(object sender, RoutedEventArgs e)
        {
            var wArticoloSostituzione = new ARTCompositionReplaceWindow(VulpesServiceProvider.Provider.GetRequiredService<ARTCompositionReplaceWindowViewModel>());
            wArticoloSostituzione.ShowDialog();

            LoadData();
        }

        #region Grid events
        private void rgvList_SelectionChanged(object sender, SelectionChangeEventArgs e)
        {
            _selectedPage = rdpGrid.PageIndex;
        }
        #endregion

        #region Context menu
        private void btnComposition_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            var selected = rgvList.SelectedItem as tab_articolo;

            if (selected != null)
            {
                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ARTCompositionWindowViewModel>();
                windowViewModel.Data = selected;

                var wArticoloComposizione = new ARTCompositionWindow(windowViewModel);
                wArticoloComposizione.Owner = Window.GetWindow(this);
                wArticoloComposizione.ShowDialog();

                LoadData();
            }
        }

        private void btnAttach_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            var selected = rgvList.SelectedItem as tab_articolo;
            if (selected != null)
            {
                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ARTAttachWindowViewModel>();
                windowViewModel.Data = selected;

                var wArticoloImmagine = new ARTAttachWindow(windowViewModel);
                wArticoloImmagine.Owner = Window.GetWindow(this);
                wArticoloImmagine.ShowDialog();
            }
        }

        private void btnImage_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            var selected = rgvList.SelectedItem as tab_articolo;
            if (selected != null)
            {
                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ARTImageWindowViewModel>();
                windowViewModel.Data = selected;

                var wArticoloImmagine = new ARTImageWindow(windowViewModel);
                wArticoloImmagine.Owner = Window.GetWindow(this);
                wArticoloImmagine.ShowDialog();
            }
        }
        #endregion

        #region Various events
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs? e)
        {
            (RadGridViewCommands.SearchByText as RoutedUICommand)?.Execute(txtSearch.Text, rgvList);
        }

        private void patComposizione_ToolTipOpening(object sender, RoutedEventArgs e)
        {
            var item = (sender as Path)?.DataContext as tab_articolo;

            if (item != null)
            {
                var composizione = _dataContext.GetComposizione(item.ID, item.UltimaRevisioneID!);
                if (composizione != null)
                {
                    (sender as System.Windows.Shapes.Path)!.DataContext = composizione;
                }
                else
                {
                    (sender as System.Windows.Shapes.Path)!.DataContext = _dataContext;
                }
            }
        }

        private void patAllegati_ToolTipOpening(object sender, RoutedEventArgs e)
        {
            var item = (sender as Path)?.DataContext as tab_articolo;

            if (item != null)
            {
                var allegati = _dataContext.GetAllegati(item.ID);

                if (allegati != null)
                {
                    (sender as System.Windows.Shapes.Path)!.DataContext = allegati;
                    (sender as System.Windows.Shapes.Path)!.Fill = allegati.Any() ? this.FindResource("VulpesXGreenBrush") as SolidColorBrush : this.FindResource("VulpesXFirstBrush") as SolidColorBrush;
                }
            }
            else
                (sender as System.Windows.Shapes.Path)!.DataContext = _dataContext;
        }

        private void patImmagini_ToolTipOpening(object sender, RoutedEventArgs e)
        {
            var item = (sender as Path)?.DataContext as tab_articolo;

            if (item != null)
            {
                var immagini = _dataContext.GetImmagini(item.ID);

                if (immagini != null)
                {
                    (sender as System.Windows.Shapes.Path)!.DataContext = immagini;
                    (sender as System.Windows.Shapes.Path)!.Fill = immagini.Any() ? this.FindResource("VulpesXGreenBrush") as SolidColorBrush : this.FindResource("VulpesXFirstBrush") as SolidColorBrush;
                }
            }
            else
                (sender as System.Windows.Shapes.Path)!.DataContext = _dataContext;
        }
        #endregion

        private void rgPrintProductLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            var item = (sender as RadGlyph)?.DataContext as tab_articolo;
            if (item != null)
            {
                var reportData = _dataContext.GetPrintProductLabel(item.ID);
                if (reportData != null && reportData.Articolo != null)
                    ReportingHandler.PrintPDF(UserContext.Instance!.Domain!, Constants.MODULE_SRM, Constants.REPORT_TYPE_PRODUCT_LABEL, _dataContext.CompanyID, reportData, $"Etichetta prodotto {reportData.Articolo.ID}", $"Etichettaprodotto {reportData.Articolo.ID}.pdf", true);
                else
                    ErrorHandler.Validation($"Impossibile recuperare l'articolo");
            }
        }

        private void rgDependencies_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as RadGlyph)?.DataContext as tab_articolo;

            if (item != null)
            {
                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ARTDependenciesWindowViewModel>();
                windowViewModel.ProductID = item.ID;
                windowViewModel.ProductDescription = item.Descrizione ?? string.Empty;

                var window = new ARTDependenciesWindow(windowViewModel);
                window.ShowDialog();
            }
        }
    }
}
