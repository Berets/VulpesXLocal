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
using Telerik.Windows.Documents.Spreadsheet.Expressions.Functions;
using VulpesX.Models.Ufp;
using VulpesX.Shared;
using VulpesX.Shared.Controls.CustomWindows;
using VulpesX.Shared.Utilities;
using VulpesX.ViewModels.Modules.Ufp.CRM.AF;
using VulpesX.ViewModels.Modules.Ufp.General;

namespace VulpesX.Modules.Ufp.CRM.AF
{
    /// <summary>
    /// Interaction logic for ANAFAT_ROWWindow.xaml
    /// </summary>
    public partial class ANAFAT_ROWWindow : FluentDefaultWindow
    {
        private ANAFAT_ROWWindowViewModel _dataContext;

        private bool _acArticlesUserChange = false;

        public ANAFAT_ROWWindow(ANAFAT_ROWWindowViewModel dataContext)
        {
            _dataContext = dataContext;

            _dataContext.ArtSelectMaterialViewModel = VulpesServiceProvider.Provider.GetRequiredService<ARTSelectViewModel>();
            _dataContext.ArtSelectMaterialViewModel.LoadDetails();
            _dataContext.ArtSelectMaterialViewModel.SetFilterJson(_dataContext.Item.afmatsearch);

            _dataContext.ArtSelectCycleViewModel = VulpesServiceProvider.Provider.GetRequiredService<ARTSelectViewModel>();
            _dataContext.ArtSelectCycleViewModel.LoadDetails();
            _dataContext.ArtSelectCycleViewModel.SetFilterJson(_dataContext.Item.afcicsearch);

            InitializeComponent();

            this.Height = (System.Windows.SystemParameters.PrimaryScreenHeight - 200);
            this.Width = (System.Windows.SystemParameters.PrimaryScreenWidth);
            this.DataContext = _dataContext;

            this.PreviewKeyDown += async (s, e) =>
            {
                if (e.Key == Key.Right)
                {
                    if (_dataContext.SelectedIndex + 1 < _dataContext.AFSteps.Count)
                    {
                        _dataContext.SelectedIndex++;
                    }
                }
                if (e.Key == Key.Left)
                {
                    if (_dataContext.SelectedIndex - 1 >= 0)
                    {
                        _dataContext.SelectedIndex--;
                    }
                }
                if (e.Key == Key.F5)
                {
                    if (_dataContext.SelectedIndex == 0)
                    {
                        await _dataContext.ArtSelectMaterialViewModel.Search(true);
                    }
                    if (_dataContext.SelectedIndex == 1)
                    {
                        await _dataContext.ArtSelectCycleViewModel.Search(true);
                    }
                }
            };
            this.Loaded += async (s, e) =>
            {
                await _dataContext.LoadLists();

                _dataContext.Item.Articles = _dataContext.ArticlesCache;
                _dataContext.Item.Customers = _dataContext.CustomersCache;

                if(!_dataContext.IsInsert || _dataContext.IsDuplicate)
                {
                    await _dataContext.ArtSelectMaterialViewModel.Search(false);
                    await _dataContext.ArtSelectCycleViewModel.Search(false);
                }
            };

            switch (_dataContext.Item.afcustomertype)
            {
                case ("I"):
                    rb_afcustomer_ico.IsChecked = true;
                    break;
                default:
                    rb_afcustomer_dir.IsChecked = true;
                    break;
            }

            switch (_dataContext.Item.afcomplexitytype)
            {
                case ("S"):
                    rb_afcomplexity_sta.IsChecked = true;
                    break;
                case ("M"):
                    rb_afcomplexity_med.IsChecked = true;
                    break;
                default:
                    rb_afcomplexity_com.IsChecked = true;
                    break;
            }
        }

        #region Head
        private void acCustomer_SuggestionChosen(object sender, Telerik.Windows.Controls.AutoSuggestBox.SuggestionChosenEventArgs e)
        {
            var customer = e.Suggestion as ABE;

            if (customer != null)
            {
                _dataContext.Item.afcli = customer.abecod;

                if (customer.IsSister ?? false)
                    rb_afcustomer_ico.IsChecked = true;
                else
                    rb_afcustomer_dir.IsChecked = true;
            }
            else
            {
                _dataContext.Item.afcli = null;
            }
        }

        private void acCustomer_TextChanged(object sender, Telerik.Windows.Controls.AutoSuggestBox.TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(acCustomer.Text))
            {
                _dataContext.Item.afcli = null;

                acCustomer.ItemsSource = _dataContext.Item.Customers;
            }
            else
            {
                if (e.Reason == Telerik.Windows.Controls.AutoSuggestBox.TextChangeReason.UserInput)
                {
                    _dataContext.Item.afcli = null;

                    acCustomer.ItemsSource = _dataContext.Item.Customers?.Where(o => o.FullDescriptionSearchable.ToLower().Contains(acCustomer.Text.ToLower()));
                }
            }
        }

        private void acArticle_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _acArticlesUserChange = true;
        }

        private void acArticle_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = ((RadAutoCompleteBox)sender).ChildrenOfType<TextBox>().First();
            Dispatcher.BeginInvoke(new Action(() => { textBox.SelectAll(); }));
        }

        private void acArticle_LostFocus(object sender, RoutedEventArgs e)
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

        private async void acArticle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_dataContext == null)
                return;

            if (e.AddedItems.Count > 0)
            {
                var article = e.AddedItems[0] as tab_articolo;

                if (article != null)
                {
                    _dataContext.Item.afartid = article.ID;
                    _dataContext.Item.Article = _dataContext.GetArticle(article.ID);
                    _dataContext.ArticleDraws = _dataContext.GetArticleDraws(_dataContext.Item.Article!.ID);

                    if (!_acArticlesUserChange)
                        return;

                    #region Filter MP
                    if (!string.IsNullOrEmpty(article.artmp1?.TrimEnd()))
                    {
                        _dataContext.Item.afmatid = article.artmp1;
                        _dataContext.Item.Material = _dataContext.GetArticle(article.artmp1);

                        _dataContext.ArtSelectMaterialViewModel.TIPTA00F = null;
                        _dataContext.ArtSelectMaterialViewModel.TIPMATPRI = null;
                        _dataContext.ArtSelectMaterialViewModel.MATERIEPRIME = null;
                        _dataContext.ArtSelectMaterialViewModel.Filter_artdi1_From = null;
                        _dataContext.ArtSelectMaterialViewModel.Filter_artdi1_To = null;
                        _dataContext.ArtSelectMaterialViewModel.Filter_ID = article.artmp1;
                    }
                    else
                    {
                        _dataContext.Item.afmatid = null;
                        _dataContext.Item.Material = null;

                        _dataContext.ArtSelectMaterialViewModel.TIPTA00F = _dataContext.ArtSelectMaterialViewModel.TIPTA00Fs?.Where(o => o.tipcod.TrimEnd() == "3").FirstOrDefault();
                        _dataContext.ArtSelectMaterialViewModel.TIPMATPRI = _dataContext.ArtSelectMaterialViewModel.TIPMATPRIs?.Where(o => o.tmpcod == _dataContext.Item.Article?.arttipmat).FirstOrDefault();
                        _dataContext.ArtSelectMaterialViewModel.MATERIEPRIME = _dataContext.ArtSelectMaterialViewModel.MATERIEPRIMEs?.Where(o => o.matpcod == _dataContext.Item.Article?.artmap).FirstOrDefault();
                        _dataContext.ArtSelectMaterialViewModel.Filter_artdi1_From = _dataContext.Item.Article?.artdi1 - 1;
                        _dataContext.ArtSelectMaterialViewModel.Filter_artdi1_To = _dataContext.Item.Article?.artdi1 + 1;
                        _dataContext.ArtSelectMaterialViewModel.Filter_ID = null;
                    }
                    await _dataContext.ArtSelectMaterialViewModel.Search(true);
                    #endregion

                    #region Filter Cycle
                    _dataContext.ArtSelectCycleViewModel.TIPTA00F = _dataContext.ArtSelectCycleViewModel.TIPTA00Fs?.Where(o => o.tipcod.TrimEnd() == _dataContext.Item.Article?.arttip).FirstOrDefault();
                    _dataContext.ArtSelectCycleViewModel.ANALOGIE = _dataContext.ArtSelectCycleViewModel.ANALOGIEs?.Where(o => o.angcod == _dataContext.Item.Article?.artfam).FirstOrDefault();
                    _dataContext.ArtSelectCycleViewModel.RIVESTIMENTI = _dataContext.ArtSelectCycleViewModel.RIVESTIMENTIs?.Where(o => o.rivecod == _dataContext.Item.Article?.artcor).FirstOrDefault();
                    _dataContext.ArtSelectCycleViewModel.TIPMATPRI = _dataContext.ArtSelectCycleViewModel.TIPMATPRIs?.Where(o => o.tmpcod == _dataContext.Item.Article?.arttipmat).FirstOrDefault();
                    _dataContext.ArtSelectCycleViewModel.MATERIEPRIME = _dataContext.ArtSelectCycleViewModel.MATERIEPRIMEs?.Where(o => o.matpcod == _dataContext.Item.Article?.artmap).FirstOrDefault();
                    _dataContext.ArtSelectCycleViewModel.DENTI = _dataContext.ArtSelectCycleViewModel.DENTIs?.Where(o => o.Dencod == _dataContext.Item.Article?.artden).FirstOrDefault();
                    _dataContext.ArtSelectCycleViewModel.DIAMETRO = _dataContext.ArtSelectCycleViewModel.DIAMETROs?.Where(o => o.Diamcod == _dataContext.Item.Article?.artdiam).FirstOrDefault();
                    _dataContext.ArtSelectCycleViewModel.LD = _dataContext.ArtSelectCycleViewModel.LDs?.Where(o => o.Ldcod == _dataContext.Item.Article?.artld).FirstOrDefault();
                    _dataContext.ArtSelectCycleViewModel.FORILUBRIFICATI = _dataContext.ArtSelectCycleViewModel.FORILUBRIFICATIs?.Where(o => o.FLcod == _dataContext.Item.Article?.artfori).FirstOrDefault();
                    _dataContext.ArtSelectCycleViewModel.ATTACCO = _dataContext.ArtSelectCycleViewModel.ATTACCOs?.Where(o => o.attacod == _dataContext.Item.Article?.artatco).FirstOrDefault();

                    _dataContext.ArtSelectCycleViewModel.Filter_artdi3_From = _dataContext.Item.Article?.artdi3;
                    _dataContext.ArtSelectCycleViewModel.Filter_artdi3_To = _dataContext.Item.Article?.artdi3;
                    _dataContext.ArtSelectCycleViewModel.Filter_artdi1_From = _dataContext.Item.Article?.artdi1;
                    _dataContext.ArtSelectCycleViewModel.Filter_artdi1_To = _dataContext.Item.Article?.artdi1;
                    _dataContext.ArtSelectCycleViewModel.Filter_artlun_From = _dataContext.Item.Article?.artlun;
                    _dataContext.ArtSelectCycleViewModel.Filter_artlun_To = _dataContext.Item.Article?.artlun;
                    _dataContext.ArtSelectCycleViewModel.Filter_artlar_From = _dataContext.Item.Article?.artlar;
                    _dataContext.ArtSelectCycleViewModel.Filter_artlar_To = _dataContext.Item.Article?.artlar;
                    _dataContext.ArtSelectCycleViewModel.Filter_artluncod_From = _dataContext.Item.Article?.artluncod;
                    _dataContext.ArtSelectCycleViewModel.Filter_artluncod_To = _dataContext.Item.Article?.artluncod;
                    _dataContext.ArtSelectCycleViewModel.Filter_artlinuta_From = _dataContext.Item.Article?.artlinuta;
                    _dataContext.ArtSelectCycleViewModel.Filter_artlinuta_To = _dataContext.Item.Article?.artlinuta;
                    _dataContext.ArtSelectCycleViewModel.Filter_artalt_From = _dataContext.Item.Article?.artalt;
                    _dataContext.ArtSelectCycleViewModel.Filter_artalt_To = _dataContext.Item.Article?.artalt;

                    _dataContext.ArtSelectCycleViewModel.Filter_ID = _dataContext.Item.Article?.ID;
                    await _dataContext.ArtSelectCycleViewModel.Search(true);
                    #endregion

                    //RESET NEXT VALUES
                    _dataContext.Item.afcicid = article.ID;
                    _dataContext.Item.afmatpre = 0;
                    _dataContext.Item.afcicmin = 0;
                    _dataContext.Item.afcicpre = 0;

                    _dataContext.CalculateCost();
                }

                _acArticlesUserChange = false;
            }
        }

        private async void patDownloadDraw_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var dataContext = (sender as System.Windows.Shapes.Path)!.DataContext as Tuple<string, string>;

            if (dataContext != null)
            {
                if (!string.IsNullOrEmpty(dataContext.Item2))
                {
                    var fileData = await StorageHelperCustom.Ufp.DownloadAsync(StorageHelperCustom.Ufp.DISEGNI_FOLDER, dataContext.Item2);

                    if (fileData != null)
                    {
                        string fullPath = Path.Combine(Path.GetTempPath(), dataContext.Item2);

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
                        ErrorHandler.Validation($"Non trovati dati disegno archiviato - {dataContext.Item2}");
                    }
                }
                else
                {
                    ErrorHandler.Validation($"Nessun disegno trovato");
                }
            }
        }
        #endregion

        #region Buttons
        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            if (_dataContext.SelectedConst != null)
            {
                //SAVE FILTERS
                _dataContext.Item.afmatsearch = _dataContext.ArtSelectMaterialViewModel.GetFilterJson();
                _dataContext.Item.afcicsearch = _dataContext.ArtSelectCycleViewModel.GetFilterJson();

                //SAVE CONSTANTS
                _dataContext.Item.afconstdata = _dataContext.SelectedConst.afdata;
                _dataContext.Item.afconstver = _dataContext.SelectedConst.afver;

                //UPDATE CYCLE SELECTED
                _dataContext.Item.afcicid = string.Join(",", (_dataContext.ArticlesCycle ?? new()).Where(o => o.IsSelected).Select(s => $"{s.ID?.TrimEnd()}"));
                _dataContext.Item.afcicseq = string.Join(",", (_dataContext.Cycles ?? new ()).Where(o => o.IsSelected).Select(s => $"{s.RepartoID?.TrimEnd()}-{s.FaseID?.TrimEnd()}"));

                var result = await _dataContext.Save();

                if (result)
                {
                    this.DialogResult = true;
                }
            }
            else
            {
                ErrorHandler.Validation("Selezionare una versione della tabella range pezzi");
            }
        }
        #endregion

        #region RadioButtons
        private void rb_afcustomer_Checked(object sender, RoutedEventArgs e)
        {
            _dataContext.Item.afcustomertype = null;

            if (rb_afcustomer_ico.IsChecked ?? false)
            {
                _dataContext.Item.afcustomertype = "I";
            }
            if (rb_afcustomer_dir.IsChecked ?? false)
            {
                _dataContext.Item.afcustomertype = "D";
            }
            _dataContext.CalculateCost();
        }

        private void rb_afcomplexity_Checked(object sender, RoutedEventArgs e)
        {
            _dataContext.Item.afcomplexitytype = null;

            if (rb_afcomplexity_sta.IsChecked ?? false)
            {
                _dataContext.Item.afcomplexitytype = "S";
            }
            if (rb_afcomplexity_med.IsChecked ?? false)
            {
                _dataContext.Item.afcomplexitytype = "M";
            }
            if (rb_afcomplexity_com.IsChecked ?? false)
            {
                _dataContext.Item.afcomplexitytype = "C";
            }
            _dataContext.CalculateCost();
        }
        #endregion

        #region Totale
        private void rcbConst_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _dataContext.SelectedConst = rcbConst.SelectedItem as ANAFAT_CONST;

            _dataContext.CalculateCost();
        }

        #endregion

    }
}
