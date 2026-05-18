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
using VulpesX.DAL.SRM;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.DAL.Tables.Article;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Models.Models;
using VulpesX.Shared;
using VulpesX.Shared.Generics;

namespace VulpesX.ViewModels.Modules.Default.CRM
{
    public class OFFED00FWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public OFFED00FWindowViewModel()
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
        public required OFFET00F Head { get; set; }
        public bool IsReadonly => Head.canceled.HasValue || Head.oflgchi == "C" || Head.oflgchi == "O";
        public bool HasMergedSigns { get; set; }

        private ObservableCollection<OFFED00F>? rows;
        public ObservableCollection<OFFED00F>? Rows
        {
            get { return rows; }
            set
            {
                rows = value;
                if (rows != null && rows.Count > 0)
                {
                    Parallel.ForEach(rows, (item) =>
                    {
                        item.UMsCache = UMsCache;
                        item.Products = Products;
                        item.AllAccounts = AllAccounts;
                        item.AllSubccounts = AllSubccounts;
                        item.GroupsList = GroupsList;
                        item.RatesList = RatesList;
                        item.AgentsList = AgentsList;
                        item.CustomerID = Head.OFTCOCL ?? 0;
                        item.RecipientID = Head.OFTDEST ?? 0;
                        item.OfferDate = Head.OFTDAOR!.Value;
                        item.IsHeadReadonly = IsReadonly;
                        item.QuantityValueChanged += OnQuantityValueChanged;
                    });
                }
            }
        }

        public ObservableCollection<PDCGRUPPI>? GroupsList { get; set; }
        public ObservableCollection<AGENTI>? AgentsList { get; set; }
        public ObservableCollection<tab_articolo>? Products { get; set; }
        public ObservableCollection<GenericIDDescription>? UMsCache { get; set; }
        public ObservableCollection<ASSOGGETAMENTI>? RatesList { get; set; }
        public ObservableCollection<PDCCONTI>? AllAccounts { get; set; }
        public ObservableCollection<PDCSOTTO>? AllSubccounts { get; set; }

        #region Computed
        public decimal Imponibile => Rows != null && rows?.Count > 0 ? Rows.Sum(sum => sum.NetPrice) - ScontiCliente : 0;
        public decimal ScontiCliente => Rows != null && rows?.Count > 0 ? Math.Round(Rows.Sum(sum => sum.NetPrice) * (Head.OFTSCCL ?? 0) / 100, 2) : 0;
        public decimal Sconti => Rows != null && rows?.Count > 0 ? Rows.Sum(sum => sum.Discount) : 0;
        public decimal Maggiorazioni => Rows != null && rows?.Count > 0 ? Rows.Sum(sum => sum.Surcharge) : 0;
        public decimal Omaggi => Rows != null && rows?.Count > 0 ? Rows.Where(w => w.OFDTQTA == "O").Sum(sum => sum.NetPrice) : 0;
        public decimal Total => Rows != null && rows?.Count > 0 ? Rows.Sum(sum => sum.AmountDisplay) : 0;
        #endregion

        public void OnQuantityValueChanged(object? sender, EventArgs? e)
        {
            var item = sender as OFFED00F;

            if (item != null)
            {
                if (item.OFDPREZ == 0)
                {
                    var lastSup = GetCurrentCustomer(item);
                    if (lastSup != null)
                    {
                        item.OFDPREZ = lastSup.Price;
                        item.OFDSCO1 = lastSup.Discount1;
                        item.OFDTSC1 = lastSup.DiscountType1;
                        item.OFDSCO2 = lastSup.Discount2;
                        item.OFDTSC2 = lastSup.DiscountType2;
                        item.OFDSCO3 = lastSup.Discount3;
                        item.OFDTSC3 = lastSup.DiscountType3;
                    }
                    else
                    {
                        var lastGen = GetCurrentGeneric(item);
                        if (lastGen != null)
                        {
                            item.OFDPREZ = lastGen.Price;
                            item.OFDSCO1 = lastGen.Discount1;
                            item.OFDTSC1 = lastGen.DiscountType1;
                            item.OFDSCO2 = lastGen.Discount2;
                            item.OFDTSC2 = lastGen.DiscountType2;
                            item.OFDSCO3 = lastGen.Discount3;
                            item.OFDTSC3 = lastGen.DiscountType3;
                        }
                    }
                }
            }
        }

        public void LoadDetails()
        {
            AllAccounts = VulpesServiceProvider.Provider.GetRequiredService<IPDCCONTIRepository>().GetSimpleList();
            AllSubccounts = VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().GetList(CompanyID);
            GroupsList = VulpesServiceProvider.Provider.GetRequiredService<IPDCGRUPPIRepository>().GetList();
            RatesList = VulpesServiceProvider.Provider.GetRequiredService<IAliquoteRepository>().GetList();
            UMsCache = VulpesServiceProvider.Provider.GetRequiredService<IUnitaRepository>().GetSimpleList(CompanyID);
            Products = VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>().GetSimpleList(CompanyID);
            AgentsList = VulpesServiceProvider.Provider.GetRequiredService<IAGENTIRepository>().GetList();
        }

        public GenericPriceInfo? GetCurrentCustomer(OFFED00F Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICRM_LISCLIRepository>().GetCurrent(Item.oftsoci, Item.OFDCODA, Item.CustomerID, Item.RecipientID, Item.OfferDate, Item.OFDQTAV, Item.ofdunim ?? string.Empty);
        }

        public GenericPriceInfo? GetCurrentGeneric(OFFED00F Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICRM_LISGENRepository>().GetCurrent(Item.oftsoci, Item.OFDCODA, Item.OfferDate, Item.OFDQTAV, Item.ofdunim ?? string.Empty);
        }

        public string? Validate(OFFED00F Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IOFFED00FRepository>().Validate(Item, true);
        }

        public DESTINATARI? GetDESTINATARI(int EntityID, int Sequence)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IDESTINATARIRepository>().Get(EntityID, Sequence);
        }

        public CLIAMMI? GetCLIAMMI(int EntityID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICLIAMMIRepository>().Get(CompanyID, EntityID);
        }

        public CLIENTI? GetCLIENTI(int EntityID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICLIENTIRepository>().Get(EntityID);
        }

        public string? ValidateModel()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IOFFED00FRepository>().ValidateModel(Rows);
        }

        public bool UpdateAll()
        {
            Head.updatedUserID = UserID;
            return VulpesServiceProvider.Provider.GetRequiredService<IOFFED00FRepository>().UpdateAll(Head, Rows ?? new ObservableCollection<OFFED00F>());
        }
    }
}
