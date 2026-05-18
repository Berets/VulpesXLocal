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
using Telerik.Windows.Diagrams.Core;
using VulpesX.Models.Models.Production;
using VulpesX.Modules.Ufp.General;
using VulpesX.Modules.Ufp.Production;
using VulpesX.Shared;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Ufp.CRM.AF;
using VulpesX.ViewModels.Modules.Ufp.Production;

namespace VulpesX.Modules.Ufp.CRM.AF
{
    /// <summary>
    /// Interaction logic for ANAFAT_ROW_CycleView.xaml
    /// </summary>
    public partial class ANAFAT_ROW_CycleView : UserControl
    {
        private ANAFAT_ROWWindowViewModel? _dataContext;
        private bool _grdCycleUserChange = false;

        public ANAFAT_ROW_CycleView()
        {
            InitializeComponent();

            this.DataContextChanged += (s, e) =>
            {
                var newDC = e.NewValue as ANAFAT_ROWWindowViewModel;

                if (newDC != null)
                {
                    _dataContext = newDC;

                    this.DataContext = _dataContext;

                    _dataContext.ArtSelectCycleViewModel.SearchCompleted += async (count, isUserSearch) =>
                    {
                        _dataContext.ArticlesCycle = new System.Collections.ObjectModel.ObservableCollection<tab_articolo>((_dataContext.ArtSelectCycleViewModel.Items ?? new()).Where(o => o.HaTempiMedi > 0 || o.HaTempiMediCNC > 0));

                        if (isUserSearch)
                        {
                            _dataContext.ArticlesCycle.ForEach(a => a.IsSelected = true);

                            if (count <= 20 && _dataContext.ArticlesCycle.Where(o => o.IsSelected).Any())
                            {
                                await _dataContext.LoadCycles(_dataContext.ArticlesCycle.Where(o => o.IsSelected).ToList());

                                btnCycleSelectCNC_Click(null, null);
                            }
                            else
                            {
                                _dataContext.Cycles = null;
                            }
                        }
                        else
                        {
                            var selectedArticles = _dataContext.Item.afcicid?.Split(',');

                            if (selectedArticles != null)
                            {
                                foreach (var sel in selectedArticles)
                                {
                                    if (!string.IsNullOrEmpty(sel))
                                    {
                                        _dataContext.ArticlesCycle.Where(o => o.ID.TrimEnd() == sel).ForEach(a => a.IsSelected = true);
                                    }
                                }
                            }

                            await _dataContext.LoadCycles(_dataContext.ArticlesCycle.Where(o => o.IsSelected).ToList());

                            var selectedCycle = _dataContext.Item.afcicseq?.Split(',');

                            if (selectedCycle != null)
                            {
                                foreach (var sel in selectedCycle)
                                {
                                    if (!string.IsNullOrEmpty(sel))
                                    {
                                        var repartoID = sel.Split('-')[0];
                                        var faseID = sel.Split('-')[1];

                                        _dataContext.Cycles?.Where(o => o.RepartoID == repartoID && o.FaseID == faseID).ForEach(a => a.IsSelected = true);
                                    }
                                }
                            }
                        }
                    };

                    grdArtSelect.Children.Add(new ARTSelectView(_dataContext.ArtSelectCycleViewModel));
                }
            };
        }

        private void grdCycle_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _grdCycleUserChange = true;
        }

        private void grdCycleSelect_Checked(object sender, RoutedEventArgs e)
        {
            if (!_grdCycleUserChange || _dataContext == null)
                return;

            _dataContext.Item.afcicmin = grdCycle.Items.Cast<ProductionModel.ProductionUfpModel>().Where(o => o.IsSelected).Sum(s => s.TempoProduzione);
            _dataContext.CalculateCost();

            _grdCycleUserChange = false;
        }

        private void grdCycleSelect_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!_grdCycleUserChange || _dataContext == null)
                return;

            _dataContext.Item.afcicmin = grdCycle.Items.Cast<ProductionModel.ProductionUfpModel>().Where(o => o.IsSelected).Sum(s => s.TempoProduzione);
            _dataContext.CalculateCost();

            _grdCycleUserChange = false;
        }

        private async void btnUpdateTimes_Click(object sender, RoutedEventArgs e)
        {
            if (_dataContext == null || _dataContext.ArticlesCycle == null || !_dataContext.ArticlesCycle.Any())
                return;

            await _dataContext.LoadCycles(_dataContext.ArticlesCycle.Where(o => o.IsSelected).ToList());
            btnCycleSelectCNC_Click(null, null);
        }

        private void btnCycleSelectAll_Click(object sender, RoutedEventArgs e)
        {
            if (_dataContext == null)
                return;

            _dataContext.Cycles?.ForEach(a => a.IsSelected = true);

            _dataContext.Item.afcicmin = _dataContext.Cycles?.Where(o => o.IsSelected).Sum(s => s.TempoProduzione);
            _dataContext.CalculateCost();
        }

        private void btnCycleSelectCNC_Click(object? sender, RoutedEventArgs? e)
        {
            if (_dataContext == null)
                return;

            _dataContext.Cycles?.ForEach(a => a.IsSelected = false);
            _dataContext.Cycles?.Where(o => o.FaseCNC).ForEach(a => a.IsSelected = true);

            _dataContext.Item.afcicmin = _dataContext.Cycles?.Where(o => o.IsSelected).Sum(s => s.TempoProduzione);
            _dataContext.CalculateCost();
        }

        private async void patDownloadDraw_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_dataContext == null)
                return;

            var dataContext = (sender as System.Windows.Shapes.Path)!.DataContext as tab_articolo;

            if (dataContext != null)
            {
                var draws = _dataContext.GetArticleDraws(dataContext.ID);

                if (draws != null && draws.Any())
                {
                    foreach (var drw in draws)
                    {
                        var fileData = await StorageHelperCustom.Ufp.DownloadAsync(StorageHelperCustom.Ufp.DISEGNI_FOLDER, drw.Item2);

                        if (fileData != null)
                        {
                            string fullPath = Path.Combine(Path.GetTempPath(), drw.Item2);

                            File.WriteAllBytes(fullPath, fileData);

                            if (File.Exists(fullPath))
                            {
                                Process.Start(new ProcessStartInfo
                                {
                                    FileName = fullPath,
                                    UseShellExecute = true
                                });
                            }
                            else
                            {
                                ErrorHandler.Validation($"Non trovato file - {fullPath}");
                            }
                        }
                        else
                        {
                            ErrorHandler.Validation($"Non trovati dati disegno archiviato - {drw.Item2}");
                        }
                    }
                }
                else
                {
                    ErrorHandler.Validation($"Nessun disegno trovato");
                }
            }
        }

        private void patTempiMedi_Click(object sender, RoutedEventArgs e)
        {
            if (_dataContext == null)
                return;

            var dataContext = (sender as RadHyperlinkButton)!.DataContext as tab_articolo;

            if (dataContext != null)
            {
                var windowViewModel = VulpesServiceProvider.Provider.GetRequiredService<ARTICLE_JOBS_TIMESWindowViewModel>();
                windowViewModel.ArticleID = dataContext.ID;

                var wARTICLEJOBSTIMES = new ARTICLE_JOBS_TIMESWindow(windowViewModel);
                wARTICLEJOBSTIMES.ShowDialog();
            }
        }

        private void numTotal_ValueChanged(object sender, RadRangeBaseValueChangedEventArgs e)
        {
            if (_dataContext == null) return;

            _dataContext.CalculateCost();
        }
    }
}
