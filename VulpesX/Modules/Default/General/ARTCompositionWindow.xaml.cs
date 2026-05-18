using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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
using VulpesX.Shared;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Controls.DragDrop;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Default.General;
using VulpesX.ViewModels.Modules.Default.Tables.Production;

namespace VulpesX.Modules.Default.General
{
    /// <summary>
    /// Interaction logic for ARTCompositionWindow.xaml
    /// </summary>
    public partial class ARTCompositionWindow : FluentDefaultWindow
    {
        private ARTCompositionWindowViewModel _dataContext;
        public ARTCompositionWindow(ARTCompositionWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            InitializeComponent();

            this.Height = (System.Windows.SystemParameters.PrimaryScreenHeight - 200);
            this.Width = (System.Windows.SystemParameters.PrimaryScreenWidth);

            this.DataContext = _dataContext;

            this.Loaded += async (s, e) =>
            {
                ArticleDropHandler.SyncDropAction = (data, destinationItems, dropIndex) =>
                {
                    var destinationCompositions = destinationItems as ObservableCollection<tab_articolo_composizione>;

                    if (destinationCompositions != null)
                    {
                        if (data is tab_produzione_reparto)
                        {
                            var ward = data as tab_produzione_reparto;

                            var composition = new tab_articolo_composizione
                            {
                                SocietaID = ward!.SocietaID,
                                RepartoID = ward!.ID,
                                ArticoloID = ward!.ID,
                                Descrizione = ward!.Descrizione,
                                RevisioneID = string.Empty,
                                Componenti = new ObservableCollection<tab_articolo_composizione>(),
                                Risorse = _dataContext.GetTab_Produzione_Risorsas(ward.ID),
                                Tempo = ward!.TempoDefault,
                                EComponente = true,
                            };

                            destinationCompositions.Insert(dropIndex, composition);
                        }

                        if (data is tab_articolo)
                        {
                            var article = data as tab_articolo;

                            var composition = _dataContext.GetComposition(article!.ID, article!.RevisioneID);

                            if (composition != null)
                                destinationCompositions.Insert(dropIndex, composition);
                        }

                        if (data is string)
                        {
                            var composition = new tab_articolo_composizione
                            {
                                SocietaID = _dataContext.CompanyID,
                                RepartoID = string.Empty,
                                ArticoloID = string.Empty,
                                Descrizione = string.Empty,
                                RevisioneID = string.Empty,
                                Componenti = new ObservableCollection<tab_articolo_composizione>(),
                                ESummary = (data.ToString() == "Summary"),
                                EMilestone = (data.ToString() == "Milestone"),
                                EComponente = true,
                            };

                            destinationCompositions.Insert(dropIndex, composition);
                        }
                    }
                    return true;
                };

                _dataContext.LoadDetails();
                await _dataContext.LoadRevisions();
            };
            this.Closed += (s, e) =>
            {
                ArticleDropHandler.SyncDropAction = null;
            };

            rgvRevisions.DataLoaded += async (s, e) =>
            {
                if (_dataContext.Revisioni != null && _dataContext.Revisioni.Any())
                {
                    rgvRevisions.SelectedItem = _dataContext.Revisioni.FirstOrDefault();
                }
                else
                {
                    await _dataContext.Load();
                    rtvDistinct.ItemsSource = _dataContext.Distinta;
                    rtvDistinct.ExpandAll();

                }
            };

            var now = VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime();

            rdtLimitLastDate.Culture = new System.Globalization.CultureInfo("it-IT");
            rdtLimitLastDate.Culture.DateTimeFormat.ShortDatePattern = "MMMM yyyy";
            rdtLimitLastDate.SelectedDate = now;
            rdtLimitDate.Culture = new System.Globalization.CultureInfo("it-IT");
            rdtLimitDate.Culture.DateTimeFormat.ShortDatePattern = "MMMM yyyy";
            rdtLimitDate.SelectedDate = now.AddMonths(-12);
        }

        private async void LoadData(string RevisioneID)
        {
            await _dataContext.Load(RevisioneID);

            rtvDistinct.ItemsSource = _dataContext.Distinta;
            rtvDistinct.ExpandAll();

            rtvMonetization.ItemsSource = _dataContext.Distinta;
            rtvMonetization.ExpandAll();

            // Costs
            decimal last = 0;
            decimal average = 0;

            var limitLastDate = rdtLimitLastDate.SelectedValue;
            var limitDate = rdtLimitDate.SelectedValue;

            if (limitDate.HasValue && limitLastDate.HasValue)
            {
                _ComputeCosts(_dataContext.Data.ID, _dataContext.Distinta ?? new ObservableCollection<tab_articolo_composizione>(), ref last, ref average, limitLastDate.Value, limitDate.Value);
                _RefreshPercentage(_dataContext.Data.ID, 0, _dataContext.Distinta ?? new ObservableCollection<tab_articolo_composizione>(), last, average);
            }

            _dataContext.LastProductCost = last;
            _dataContext.AverageProductCost = average;
        }


        #region Private methods
        private void _RefreshPercentage(string ProductID, decimal Qty, ObservableCollection<tab_articolo_composizione> distinta, decimal LastValue, decimal AverageValue)
        {
            foreach (var item in distinta)
            {
                decimal qty = 0;
                // get costs only for raw materials
                if (!string.IsNullOrWhiteSpace(item.ComponenteArticoloID) && (item.Componenti == null || item.Componenti.Count == 0))
                {
                    if (item.ArticoloID == ProductID)
                    {
                        item.LastCostPercentage = LastValue > 0 ? item.LastValue / LastValue * 100 : 0;
                        item.AverageCostPercentage = AverageValue > 0 ? item.AverageValue / AverageValue * 100 : 0;
                    }
                    else
                    {
                        item.LastCostPercentage = LastValue > 0 ? (item.LastValue * Qty) / LastValue * 100 : 0;
                        item.AverageCostPercentage = AverageValue > 0 ? (item.AverageValue * Qty) / AverageValue * 100 : 0;
                    }
                }
                else
                {
                    if ((item.Componenti ?? new ObservableCollection<tab_articolo_composizione>()).All(all => all.EReparto))
                    {
                        item.LastCostPercentage = LastValue > 0 ? item.LastValue / LastValue * 100 : 0;
                        item.AverageCostPercentage = AverageValue > 0 ? item.AverageValue / AverageValue * 100 : 0;
                        qty = item.Quantita ?? 0;
                    }
                    else
                    {
                        qty = Qty;
                    }
                }

                if (item.Componenti != null && item.Componenti.Count > 0)
                {
                    _RefreshPercentage(ProductID, qty, item.Componenti, LastValue, AverageValue);
                }
            }
        }

        private void _ComputeCosts(string ProductID, ObservableCollection<tab_articolo_composizione> distinta, ref decimal LastCost, ref decimal AverageCost, DateTime LimitLastDate, DateTime LimitDate)
        {
            foreach (var item in distinta)
            {
                // get costs only for raw materials
                if (!string.IsNullOrWhiteSpace(item.ComponenteArticoloID) && (item.Componenti == null || item.Componenti.Count == 0))
                {
                    var costsTuple = _dataContext.GetCosts(item.ComponenteArticoloID, LimitLastDate, LimitDate);

                    var lastCosts = costsTuple.Item1;
                    var lastAverage = costsTuple.Item2;
                    if (lastCosts != null)
                    {
                        item.LastCost = lastCosts.last_cost;
                        item.AverageCost = lastAverage;
                    }
                }
                else
                {
                    //check if halfmade or raw
                    if ((item.Componenti ?? new ObservableCollection<tab_articolo_composizione>()).All(all => all.EReparto))
                    {
                        // halfmade
                        decimal hmLast = 0;
                        decimal hmAverage = 0;

                        _ComputeCosts(item.ComponenteArticoloID!, item.Componenti ?? new ObservableCollection<tab_articolo_composizione>(), ref hmLast, ref hmAverage, LimitLastDate, LimitDate);

                        item.LastCost = hmLast;
                        item.AverageCost = hmAverage;
                    }
                    if (item.Componenti != null && item.Componenti.Count > 0)
                    {
                        _ComputeCosts(ProductID, item.Componenti, ref LastCost, ref AverageCost, LimitLastDate, LimitDate);
                    }
                }
                if (item.ArticoloID == ProductID)
                {
                    LastCost += item.LastValue;
                    AverageCost += item.AverageValue;
                }
            }
        }

        private void ClearSelectedDistinct(tab_articolo_composizione Articolo)
        {
            foreach (var dst in Articolo.Componenti)
            {
                dst.ESelezionato = false;
                ClearSelectedDistinct(dst);
            }
        }

        private void SearchDistinct(tab_articolo_composizione Articolo, string SearchText)
        {
            foreach (var dst in Articolo.Componenti.Where(o => !string.IsNullOrEmpty(o.Descrizione)))
            {
                if (dst.Descrizione!.ToLower().Contains(rwtSearch.Text.ToLower()))
                {
                    var padre = dst.Padre;

                    while (padre != null)
                    {
                        padre.EEspanso = true;
                        padre = padre.Padre;
                    }

                    dst.ESelezionato = true;
                }

                SearchDistinct(dst, rwtSearch.Text);
            }
        }

        private void ExportExcelComponenti(tab_articolo_composizione Articolo, ref ObservableCollection<tab_articolo_composizione> Componenti)
        {
            foreach (var cmp in Articolo.Componenti)
            {
                Componenti.Add(cmp);

                ExportExcelComponenti(cmp, ref Componenti);
            }
        }
        #endregion

        #region Buttons
        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (_dataContext.Distinta != null && _dataContext.Distinta.Any())
            {
                var validation = _dataContext.Validation();
                if (string.IsNullOrEmpty(validation))
                {
                    if (_dataContext.Save() > 0)
                    {
                        await _dataContext.LoadRevisions();

                        if (_dataContext.Revisioni != null && _dataContext.Revisioni.Any())
                            rgvRevisions.SelectedItem = _dataContext.Revisioni.FirstOrDefault();

                        InfoHandler.Show("Composizione salvata correttamente");
                    }
                    else
                    {
                        ErrorHandler.Validation("Attenzione qualcosa è andato storto, contattare GxItalia");
                    }
                }
                else
                {
                    ErrorHandler.Validation(validation);
                }
            }
            else
            {
                ErrorHandler.Validation("Impossibile salvare una distinta vuota");
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            var revision = rgvRevisions.SelectedItem as tab_articolo;

            if (revision != null)
                LoadData(revision.RevisioneID!);
        }

        private void btnExpandAll_Click(object sender, RoutedEventArgs e)
        {
            rtvDistinct.ExpandAll();
        }

        private void btnCollapseAll_Click(object sender, RoutedEventArgs e)
        {
            rtvDistinct.CollapseAll();
        }

        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
            //    Microsoft.Win32.SaveFileDialog sfdExcel = new Microsoft.Win32.SaveFileDialog();
            //    sfdExcel.Filter = "Excel |*.xlsx";
            //    sfdExcel.ShowDialog();

            //    if (!string.IsNullOrEmpty(sfdExcel.FileName))
            //    {
            //        using (var workbook = new XLWorkbook())
            //        {
            //            var rowID = 1;

            //            var worksheetName = model.Data.ID;
            //            var worksheet = workbook.Worksheets.Add((worksheetName.Length > 31) ? worksheetName.Substring(0, 30) : worksheetName);

            //            worksheet.Cell(rowID, 1).Value = "ArticoloID";
            //            worksheet.Cell(rowID, 2).Value = "ComposizioneID";
            //            worksheet.Cell(rowID, 3).Value = "ComposizioneIDPadre";
            //            worksheet.Cell(rowID, 4).Value = "RepartoID";
            //            worksheet.Cell(rowID, 5).Value = "RisorsaID";
            //            worksheet.Cell(rowID, 6).Value = "ComponenteArticoloID";
            //            worksheet.Cell(rowID, 7).Value = "ESummary";
            //            worksheet.Cell(rowID, 8).Value = "EMilestone";
            //            worksheet.Cell(rowID, 9).Value = "Descrizone Summary/Milestone";
            //            worksheet.Cell(rowID, 10).Value = "Quantità";
            //            worksheet.Cell(rowID, 11).Value = "Tempo (mm)";

            //            var componenti = new ObservableCollection<tab_articolo_composizione>();
            //            ExportExcelComponenti(distinta.FirstOrDefault(), ref componenti);

            //            foreach (var cmp in componenti)
            //            {
            //                ++rowID;
            //                worksheet.Cell(rowID, 1).Value = cmp.ArticoloID;
            //                worksheet.Cell(rowID, 2).Value = cmp.ComposizioneID;
            //                worksheet.Cell(rowID, 3).Value = cmp.ComposizioneIDPadre ?? 0;
            //                worksheet.Cell(rowID, 4).Value = cmp.RepartoID;
            //                worksheet.Cell(rowID, 5).Value = cmp.RisorsaID;
            //                worksheet.Cell(rowID, 6).Value = cmp.ComponenteArticoloID;
            //                worksheet.Cell(rowID, 7).Value = cmp.ESummary;
            //                worksheet.Cell(rowID, 8).Value = cmp.EMilestone;
            //                worksheet.Cell(rowID, 9).Value = (cmp.EMilestone || cmp.ESummary) ? cmp.Descrizione : string.Empty;
            //                worksheet.Cell(rowID, 10).Value = cmp.Quantita ?? 0;
            //                worksheet.Cell(rowID, 11).Value = new TimeSpan(cmp.Tempo ?? 0).TotalMinutes;
            //            }

            //            worksheet.Columns(1, 200).AdjustToContents();
            //            workbook.SaveAs(sfdExcel.FileName);

            //            if (System.IO.File.Exists(sfdExcel.FileName))
            //                Process.Start(@"cmd.exe ", @"/c" + sfdExcel.FileName);
            //        }
            //    }
            //}
            //catch (Exception)
            //{
            //}
        }

        private void cmdInsertArticle_Click(object sender, RoutedEventArgs e)
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
            wArticolo.ShowDialog();

            _dataContext.LoadDetails();
        }

        private void cmdInsertWard_Click(object sender, RoutedEventArgs e)
        {
            //var wReparti = new wReparti(ctx, new RepartoViewModel(ctx.CurrentCompanyID) { Data = new tab_produzione_reparto { SocietaID = ctx.CurrentCompanyID, RepartoRisorse = new ObservableCollection<tab_produzione_risorsa>() }, IsInsert = true });
            //wReparti.ShowDialog();

            //model.Reparti = new tab_produzione_repartoService().GetList(model.Data.SocietaID);
        }
        #endregion

        #region ContextMenu
        private async void btnDuplicateRevision_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            var selected = rgvRevisions.SelectedItem as tab_articolo;
            if (selected != null)
            {
                _dataContext.DuplicateRevision(selected);
                await _dataContext.LoadRevisions();
            }
        }

        private async void btnDeleteRevision_Click(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            var selected = rgvRevisions.SelectedItem as tab_articolo;
            if (selected != null)
            {
                _dataContext.DeleteRevision(selected);

                await _dataContext.LoadRevisions();
            }
        }
        #endregion

        #region Trash D&D
        private void grdTrash_DragOver(object sender, DragEventArgs e)
        {
            var dropDetails = Telerik.Windows.DragDrop.DragDropPayloadManager.GetDataFromObject(e.Data, "DropDetails") as DropIndicationDetails;

            if (dropDetails != null)
            {
                dropDetails.CurrentDraggedOverItem = new { Descrizione = "Cestino" };
                dropDetails.CurrentDropPosition = Telerik.Windows.Controls.DropPosition.Inside;

                var Path = grdTrash.Child as System.Windows.Shapes.Path;

                if (Path != null)
                {
                    Path.Fill = this.FindResource("VulpesXRedBrush") as SolidColorBrush;
                }
            }
        }

        private void grdTrash_DragLeave(object sender, DragEventArgs e)
        {
            var Path = grdTrash.Child as System.Windows.Shapes.Path;

            if (Path != null)
            {
                Path.Fill = this.FindResource("VulpesXSecondBrush") as SolidColorBrush;
            }
        }

        private void grdTrash_Drop(object sender, DragEventArgs e)
        {
            var Path = grdTrash.Child as System.Windows.Shapes.Path;

            if (Path != null)
            {
                Path.Fill = this.FindResource("VulpesXSecondBrush") as SolidColorBrush;
            }
        }
        #endregion

        #region Various events
        private void rgvRevisions_SelectionChanged(object sender, SelectionChangeEventArgs e)
        {
            var revision = rgvRevisions.SelectedItem as tab_articolo;

            if (revision != null)
                LoadData(revision.RevisioneID!);
        }

        private void rwtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            var distincts = rtvDistinct.ItemsSource.Cast<tab_articolo_composizione>();

            foreach (var dst in distincts)
            {
                dst.ESelezionato = false;
                ClearSelectedDistinct(dst);
            }

            if (!string.IsNullOrEmpty(rwtSearch.Text))
            {
                foreach (var dst in distincts.Where(o => !string.IsNullOrEmpty(o.Descrizione)))
                {
                    if (dst.Descrizione!.ToLower().Contains(rwtSearch.Text.ToLower()))
                    {
                        var padre = dst.Padre;

                        while (padre != null)
                        {
                            padre.EEspanso = true;
                            padre = padre.Padre;
                        }

                        dst.ESelezionato = true;
                    }

                    SearchDistinct(dst, rwtSearch.Text);
                }
            }
        }

        private void patComposizione_ToolTipOpening(object sender, RoutedEventArgs e)
        {
            var item = (sender as System.Windows.Shapes.Path)?.DataContext;

            if (item is tab_articolo)
            {
                var article = item as tab_articolo;

                if (article != null)
                    (sender as System.Windows.Shapes.Path)!.DataContext = _dataContext.GetCompositions(article.ID, article.RevisioneID!);
            }
            else
                (sender as System.Windows.Shapes.Path)!.DataContext = item;
        }

        private void patDipendenze_ToolTipOpening(object sender, RoutedEventArgs e)
        {
            var item = (sender as System.Windows.Shapes.Path)?.DataContext;

            if (item is tab_articolo)
            {
                var article = item as tab_articolo;

                if (article != null)
                    (sender as System.Windows.Shapes.Path)!.DataContext = _dataContext.GetDipendenze(article.ID, article.RevisioneID);
            }
            else
                (sender as System.Windows.Shapes.Path)!.DataContext = item;
        }
        #endregion

        private void rtvMonetization_PreviewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;
        }

        private void rdtLimitDate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var revision = rgvRevisions.SelectedItem as tab_articolo;

            if (revision != null)
                LoadData(revision.RevisioneID!);
        }

        private void rdtLimitLastDate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var revision = rgvRevisions.SelectedItem as tab_articolo;

            if (revision != null)
                LoadData(revision.RevisioneID!);
        }
    }
}
