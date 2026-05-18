using DocumentFormat.OpenXml.EMMA;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Accounting;
using VulpesX.DAL.CRM;
using VulpesX.DAL.General;
using VulpesX.DAL.Shipping;
using VulpesX.DAL.Store;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.DAL.Tables.Article;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Models.Models;
using VulpesX.Services.Tables.Accounting;
using VulpesX.Shared;
using VulpesX.Shared.Generics;

namespace VulpesX.ViewModels.Modules.Default.CRM
{
    public class BOLLD00FWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public BOLLD00FWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.ACCESS!.USRID;
        }

        public bool ViewUpdate
        {
            set
            {
                NotifyPropertyChanged("Imponibile");
                NotifyPropertyChanged("Sconti");
                NotifyPropertyChanged("Maggiorazioni");
                NotifyPropertyChanged("Omaggi");
                NotifyPropertyChanged("ScontiCliente");
                NotifyPropertyChanged("Total");
            }
        }

        public required BOLLT00F Head { get; set; }
        public bool IsReadonly { get; set; }
        public bool IsEnabled => !IsReadonly;
        public bool IsInsert { get; set; }

        private ObservableCollection<BOLLD00F>? rows;
        public ObservableCollection<BOLLD00F>? Rows
        {
            get { return rows; }
            set
            {
                rows = value;
                if (rows != null && rows.Count > 0)
                {
                    var lotsCache = new List<store_stocks_lots>();
                    Parallel.ForEach(rows.Select(s => s.BOCODA).Distinct().ToList(), (prd) =>
                    {
                        if (!string.IsNullOrEmpty(prd))
                        {
                            var data = VulpesServiceProvider.Provider.GetRequiredService<Istore_stocks_lotsRepository>().GetListByProduct(rows.First().bolsoc, prd);

                            if (data != null)
                                lotsCache.AddRange(data.ToList());
                        }
                    });
                    Parallel.ForEach(rows, (item) =>
                    {
                        item.UMsCache = UMsCache;
                        item.Products = Products;
                        item.AllAccounts = AllAccounts;
                        item.AllSubccounts = AllSubccounts;
                        item.Groups = Groups;
                        item.Rates = Rates;
                        item.Stores = Stores;
                        item.Agents = Agents;
                        item.CustomerID = Head.BTCODC ?? 0;
                        item.RecipientID = Head.BTCODD ?? 0;
                        item.DDTDate = Head.BTDATP!.Value;
                        item.QuantityValueChanged += OnQuantityValueChanged;

                        foreach (var eng in item.EngagesRows ?? new ObservableCollection<BOLLD00F1>())
                        {
                            eng.Lots = new ObservableCollection<store_stocks_lots>(lotsCache.Where(w => w.product_id == item.BOCODA).ToList());
                        }
                    });
                }
            }
        }

        public ObservableCollection<ASSOGGETAMENTI>? Rates { get; set; }
        public ObservableCollection<PDCGRUPPI>? Groups { get; set; }
        public ObservableCollection<PDCCONTI>? AllAccounts { get; set; }
        public ObservableCollection<PDCSOTTO>? AllSubccounts { get; set; }
        public ObservableCollection<GenericIDDescription>? UMsCache { get; set; }
        public ObservableCollection<tab_articolo>? Products { get; set; }
        public ObservableCollection<store_stores>? Stores { get; set; }
        public ObservableCollection<AGENTI>? Agents { get; set; }

        #region Computed
        public decimal Imponibile => (Rows?.Sum(sum => sum.NetPrice) - ScontiCliente) ?? 0;
        public decimal ScontiCliente => Math.Round((Rows?.Sum(sum => sum.NetPrice) ?? 0) * (Head.BTSCCL ?? 0) / 100, 2);
        public decimal Sconti => Rows?.Sum(sum => sum.Discount) ?? 0;
        public decimal Maggiorazioni => Rows?.Sum(sum => sum.Surcharge) ?? 0;
        public decimal Omaggi => Rows?.Where(w => w.BOTQTA == "O").Sum(sum => sum.NetPrice) ?? 0;
        public decimal Total => Rows?.Sum(sum => sum.AmountDisplay) ?? 0;

        private decimal neededQuantity;
        public decimal NeededQuantity
        {
            get => neededQuantity;
            set
            {
                neededQuantity = value;
                NotifyPropertyChanged("NeededQuantity");
            }
        }
        private decimal engagedQuantity;
        public decimal EngagedQuantity
        {
            get => engagedQuantity;
            set
            {
                engagedQuantity = value;
                NotifyPropertyChanged("EngagedQuantity");
            }
        }
        #endregion

        public void LoadDetails()
        {
            AllAccounts = VulpesServiceProvider.Provider.GetRequiredService<IPDCCONTIRepository>().GetSimpleList();
            AllSubccounts = VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().GetList(CompanyID);
            Groups = VulpesServiceProvider.Provider.GetRequiredService<IPDCGRUPPIRepository>().GetList();
            UMsCache = VulpesServiceProvider.Provider.GetRequiredService<IUnitaRepository>().GetSimpleList(CompanyID);
            Products = VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>().GetSimpleList(CompanyID);
            Rates = VulpesServiceProvider.Provider.GetRequiredService<IAliquoteRepository>().GetList();
            Products = VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>().GetSimpleList(CompanyID);
            Stores = VulpesServiceProvider.Provider.GetRequiredService<Istore_storesRepository>().GetList(CompanyID, false);
            Agents = VulpesServiceProvider.Provider.GetRequiredService<IAGENTIRepository>().GetList();
        }

        public void OnQuantityValueChanged(object? sender, EventArgs? e)
        {
            var item = sender as BOLLD00F;

            if (item != null)
            {
                if (item.boprez == 0)
                {
                    var lastSup = GetCurrentCustomer(item);
                    if (lastSup != null)
                    {
                        item.boprez = lastSup.Price;
                        item.bosco1 = lastSup.Discount1;
                        item.botsc1 = lastSup.DiscountType1;
                        item.bosco2 = lastSup.Discount2;
                        item.botsc2 = lastSup.DiscountType2;
                        item.bosco3 = lastSup.Discount3;
                        item.botsc3 = lastSup.DiscountType3;
                    }
                    else
                    {
                        var lastGen = GetCurrentGeneric(item);
                        if (lastGen != null)
                        {
                            item.boprez = lastGen.Price;
                            item.bosco1 = lastGen.Discount1;
                            item.botsc1 = lastGen.DiscountType1;
                            item.bosco2 = lastGen.Discount2;
                            item.botsc2 = lastGen.DiscountType2;
                            item.bosco3 = lastGen.Discount3;
                            item.botsc3 = lastGen.DiscountType3;
                        }
                    }
                }
            }
        }

        public GenericPriceInfo? GetCurrentCustomer(BOLLD00F Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICRM_LISCLIRepository>().GetCurrent(Item.bolsoc, Item.BOCODA ?? string.Empty, Item.CustomerID, Item.RecipientID, Item.DDTDate ?? DateTime.MinValue, Item.BOQTAV, Item.BOUNIM ?? string.Empty);
        }

        public GenericPriceInfo? GetCurrentGeneric(BOLLD00F Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICRM_LISGENRepository>().GetCurrent(Item.bolsoc, Item.BOCODA ?? string.Empty, Item.DDTDate ?? DateTime.MinValue, Item.BOQTAV, Item.BOUNIM ?? string.Empty);
        }


        public ObservableCollection<store_stocks_lots>? GetListByProduct(string ProductID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Istore_stocks_lotsRepository>().GetListByProduct(CompanyID, ProductID);
        }

        public string? Validate(BOLLD00F1 Item, decimal QuantityAlreadySelected, decimal QuantityNeeded, decimal QuantitySameLotAlreadyUsed, string UM)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IBOLLD00F1Repository>().Validate(Item, QuantityAlreadySelected, QuantityNeeded, QuantitySameLotAlreadyUsed, true, UM);
        }

        public string? Validate(BOLLD00F Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IBOLLD00FRepository>().Validate(Item, true);
        }

        public string? ValidateModel()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IBOLLD00FRepository>().ValidateModel(Rows);
        }

        public CLIENTI? GetCLIENTI(int EntityID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICLIENTIRepository>().Get(EntityID);
        }

        public AZIENDA? GetAZIENDA()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Get(CompanyID);
        }

        public bool Save(BOLLT00F_history? History)
        {
            if (Head.BTNUBD > 0)
            {
                //Edited a definitive DDT
                // store previous revision
                if (History != null)
                    VulpesServiceProvider.Provider.GetRequiredService<IBOLLT00F_historyRepository>().Insert(History);

                return VulpesServiceProvider.Provider.GetRequiredService<IBOLLD00FRepository>().UpdateAllDefinitive(Head, Rows ?? new(), UserID);
            }
            else
            {
                if (!IsInsert)
                {
                    Head.updatedUserID = UserID;
                }

                return VulpesServiceProvider.Provider.GetRequiredService<IBOLLD00FRepository>().UpdateAll(Head, Rows ?? new(), UserID);
            }
        }
    }
}
