using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.CRM;
using VulpesX.DAL.CRM.AF;
using VulpesX.DAL.General;
using VulpesX.DAL.SRM;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Models.Models.CRM.AF;
using VulpesX.Models.Models.Production;
using VulpesX.Models.Ufp;
using VulpesX.Shared;
using VulpesX.Shared.Generics;
using VulpesX.ViewModels.Modules.Ufp.General;

namespace VulpesX.ViewModels.Modules.Ufp.CRM.AF
{
    public class ANAFAT_ROWWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public ObservableCollection<GenericIDDescription> AFSteps { get; set; }

        public ANAFAT_ROWWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.ACCESS!.USRID;

            AFSteps = new ObservableCollection<GenericIDDescription>()
            {
                new GenericIDDescription() { ID = "Material", Description = "Materiale" },
                new GenericIDDescription() { ID = "Cycle", Description = "Tempi" },
                new GenericIDDescription() { ID = "Extra", Description = "Extra" },
            };
        }

        public bool IsInsert { get; set; }
        public bool IsDuplicate { get; set; } = false;

        private ANAFAT_ROW item = null!;
        public required ANAFAT_ROW Item { get { return item; } set { item = value; NotifyPropertyChanged(); } }

        private List<Tuple<string, string>>? articleDraws;
        public List<Tuple<string, string>>? ArticleDraws
        {
            get => articleDraws;
            set
            {
                articleDraws = value;

                NotifyPropertyChanged();
            }
        }

        private int selectedIndex = 0;
        public int SelectedIndex
        {
            get
            {
                return this.selectedIndex;
            }
            set
            {
                if (this.selectedIndex != value)
                {
                    this.selectedIndex = value;
                    this.NotifyPropertyChanged("SelectedIndex");
                }
            }
        }

        private ObservableCollection<tab_articolo>? articlesCache;
        public ObservableCollection<tab_articolo>? ArticlesCache
        {
            get => articlesCache;
            set
            {
                articlesCache = value;

                NotifyPropertyChanged("ArticlesCache");
            }
        }

        private ObservableCollection<ABE>? customersCache;
        public ObservableCollection<ABE>? CustomersCache
        {
            get => customersCache;
            set
            {
                customersCache = value;

                NotifyPropertyChanged("CustomersCache");
            }
        }

        private bool _isBusyList;
        public bool IsBusyList
        {
            get { return _isBusyList; }
            set { _isBusyList = value; NotifyPropertyChanged(); }
        }

        public required ObservableCollection<ANAFAT_CONST> Consts { get; set; }

        private ANAFAT_CONST? selectedConst;
        public ANAFAT_CONST? SelectedConst { get { return selectedConst; } set { selectedConst = value; NotifyPropertyChanged(); } }

        #region Material
        private bool _isBusyMaterial;
        public bool IsBusyMaterial
        {
            get { return _isBusyMaterial; }
            set { _isBusyMaterial = value; NotifyPropertyChanged(); }
        }

        private ARTSelectViewModel artSelectMaterialViewModel = null!;
        public ARTSelectViewModel ArtSelectMaterialViewModel
        {
            get => artSelectMaterialViewModel;
            set
            {
                artSelectMaterialViewModel = value;

                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<ArticlePriceListRowModel>? priceLists;
        public ObservableCollection<ArticlePriceListRowModel>? PriceLists
        {
            get => priceLists;
            set
            {
                priceLists = value;

                NotifyPropertyChanged();
            }
        }

        public event Action<List<string>>? ColumnsReady;

        public async Task LoadPriceLists(List<tab_articolo> Articles)
        {
            IsBusyMaterial = true;

            try
            {
                var allSupplierKeys = new LinkedList<string>();

                var result = await Task.Run(() =>
                {
                    var ret = new List<ArticlePriceListRowModel>();
                    var allCells = new List<ArticlePriceListCellModel>();

                    var lisForRepo = VulpesServiceProvider.Provider.GetRequiredService<ISRM_LISFORRepository>();
                    foreach (var art in Articles)
                    {
                        var pl = lisForRepo.GetListLISFOR0FActive(CompanyID, art.ID, Item.afdata);

                        var plm = new ArticlePriceListRowModel();
                        plm["ID"] = art.ID;
                        plm["Article"] = art;

                        foreach (var pll in (pl ?? new ObservableCollection<LISFOR0F>()).OrderByDescending(o => o.LFFDAT).GroupBy(g => g.LFFORN))
                        {
                            var first = pll.First();

                            string key = $"{first.LFFORN}\n{first.Supplier?.abers1?.TrimEnd()}";

                            var cell = new ArticlePriceListCellModel
                            {
                                Key = key,
                                Article = art,
                                SupplierID = first.LFFORN,
                                Price = first.LFPREZ,
                                Date = first.LFFDAT,
                                Color = "D",
                                IsSelected = false,
                            };

                            plm[key] = cell;
                            allCells.Add(cell);

                            if (!allSupplierKeys.Contains(key))
                                allSupplierKeys.AddLast(key);
                        }

                        if ((pl ?? new ObservableCollection<LISFOR0F>()).Any())
                            ret.Add(plm);
                    }

                    if (allCells.Any())
                    {
                        var maxPrice = allCells.Max(c => c.Price);
                        var minPrice = allCells.Min(c => c.Price);

                        // Only color if there's actually a difference
                        if (maxPrice != minPrice)
                        {
                            foreach (var cell in allCells)
                            {
                                if (cell.Price == maxPrice)
                                {
                                    cell.Color = "R";
                                }
                                else if (cell.Price == minPrice)
                                {
                                    cell.Color = "G";
                                }
                                else
                                {
                                    cell.Color = "D";
                                }
                            }
                        }
                    }

                    return new { ret };
                });

                ColumnsReady?.Invoke(allSupplierKeys.ToList());
                PriceLists = new ObservableCollection<ArticlePriceListRowModel>(result.ret);
            }
            finally
            {
                IsBusyMaterial = false;
            }
        }
        #endregion

        #region Cycle
        private bool _isBusyCycle;
        public bool IsBusyCycle
        {
            get { return _isBusyCycle; }
            set { _isBusyCycle = value; NotifyPropertyChanged(); }
        }

        private ARTSelectViewModel artSelectCycleViewModel = null!;
        public ARTSelectViewModel ArtSelectCycleViewModel
        {
            get => artSelectCycleViewModel;
            set
            {
                artSelectCycleViewModel = value;

                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<tab_articolo>? articlesCycle;
        public ObservableCollection<tab_articolo>? ArticlesCycle
        {
            get => articlesCycle;
            set
            {
                articlesCycle = value;

                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<ProductionModel.ProductionUfpModel>? cycles;
        public ObservableCollection<ProductionModel.ProductionUfpModel>? Cycles
        {
            get => cycles;
            set
            {
                cycles = value;

                NotifyPropertyChanged("Cycles");
            }
        }

        public async Task LoadCycles(List<tab_articolo> Articles)
        {
            if (Articles.Count == 1)
            {
                IsBusyCycle = true;

                try
                {
                    var result = await Task.Run(() =>
                    {
                        return VulpesServiceProvider.Provider.GetRequiredService<ITEMPI_MEDI_VISTARepository>().GetList(CompanyID, Articles.First().ID);
                    });

                    Cycles = result;
                }
                finally
                {
                    IsBusyCycle = false;
                }
            }
            else
            {
                IsBusyCycle = true;

                try
                {
                    var result = await Task.Run(() =>
                    {
                        return VulpesServiceProvider.Provider.GetRequiredService<ITEMPI_MEDI_VISTARepository>().GetList(CompanyID, Item.Article!.ID, Articles.ToList());
                    });

                    Cycles = result;
                }
                finally
                {
                    IsBusyCycle = false;
                }
            }
        }
        #endregion

        public async Task LoadLists()
        {
            IsBusyList = true;
            try
            {
                var result = await Task.Run(() =>
                {
                    var customers = VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().GetLightList(CompanyID, "C");
                    var articles = VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>().GetSimpleProductionList("1");

                    return new { articles, customers };
                });

                ArticlesCache = result.articles;
                customersCache = result.customers;
            }
            finally
            {
                IsBusyList = false;
            }
        }

        public tab_articolo? GetArticle(string ProductID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>().Get(ProductID);
        }

        public List<Tuple<string, string>>? GetArticleDraws(string ProductID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>().GetDraws(ProductID);
        }

        public void CalculateCost()
        {
            if (selectedConst != null)
                VulpesServiceProvider.Provider.GetRequiredService<IANAFAT_ROWRepository>().CalculateCosts(ref selectedConst, ref item);
        }

        public async Task<bool> Save()
        {
            IsBusyList = true;

            try
            {
                var result = await Task.Run(() =>
                {
                    if (IsInsert || IsDuplicate)
                    {
                        var numRegRepo = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>();

                        Item.addedUserID = UserID;
                        Item.afid = numRegRepo.GetNumber(CompanyID, Item.afyear, Constants.FEASABILITY, true);

                        return VulpesServiceProvider.Provider.GetRequiredService<IANAFAT_ROWRepository>().Insert(Item);
                    }
                    else
                    {
                        Item.updateUserID = UserID;
                        return VulpesServiceProvider.Provider.GetRequiredService<IANAFAT_ROWRepository>().Update(Item);
                    }

                });

                return result;
            }
            finally
            {
                IsBusyList = false;
            }
        }
    }
}
