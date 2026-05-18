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
using VulpesX.DAL.Tables.Accounting;
using VulpesX.DAL.Tables.Article;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Models.Models;
using VulpesX.Shared;
using VulpesX.Shared.Generics;

namespace VulpesX.ViewModels.Modules.Default.CRM
{
    public class ORDID00FWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public ORDID00FWindowViewModel()
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
        public required ORDIT00F Head { get; set; }

        public bool IsReadonly => Head.canceled != null || Head.flgchi == "E";
        public bool IsEnabled => !IsReadonly;
        public bool HasMergedSigns { get; set; }

        private ObservableCollection<ORDID00F>? rows;
        public ObservableCollection<ORDID00F>? Rows
        {
            get { return rows; }
            set
            {
                rows = value;
                if (rows != null && rows.Count > 0)
                {
                    Parallel.ForEach(rows ?? new(), (item) =>
                    {
                        item.UMsCache = UMsCache;
                        item.Products = Products;
                        item.AllAccounts = AllAccounts;
                        item.AllSubccounts = AllSubccounts;
                        item.GroupsList = GroupsList;
                        item.RatesList = RatesList;
                        item.AgentsList = AgentsList;
                        item.CustomerID = Head.OTCLIE ?? 0;
                        item.RecipientID = Head.DESTIN ?? 0;
                        item.OrderDate = Head.OTDAOR!.Value;
                        item.IsHeadReadonly = IsReadonly;
                        item.CanEdit = !item.HasDDT && (string.IsNullOrWhiteSpace(item.ODSTA) || item.ODSTA != "*");
                        item.QuantityValueChanged += OnQuantityValueChanged;
                    });
                }
            }
        }
        public ObservableCollection<PDCGRUPPI>? GroupsList { get; set; }
        public ObservableCollection<AGENTI>? AgentsList { get; set; }
        public ObservableCollection<tab_articolo>? Products { get; set; }
        public ObservableCollection<ASSOGGETAMENTI>? RatesList { get; set; }
        public ObservableCollection<PDCCONTI>? AllAccounts { get; set; }
        public ObservableCollection<PDCSOTTO>? AllSubccounts { get; set; }
        public ObservableCollection<GenericIDDescription>? UMsCache { get; set; }

        #region Computed
        public decimal Imponibile => (Rows?.Sum(sum => sum.NetPrice) - ScontiCliente) ?? 0;
        public decimal ScontiCliente => Math.Round((Rows ?? new()).Sum(sum => sum.NetPrice) * (Head.OTSCCL ?? 0) / 100, 2);
        public decimal Sconti => Rows?.Sum(sum => sum.Discount) ?? 0;
        public decimal Maggiorazioni => Rows?.Sum(sum => sum.Surcharge) ?? 0;
        public decimal Omaggi => Rows?.Where(w => w.ODTQTA == "O").Sum(sum => sum.NetPrice) ?? 0;
        public decimal Total => Rows?.Sum(sum => sum.AmountDisplay) ?? 0;
        #endregion

        public void OnQuantityValueChanged(object? sender, EventArgs? e)
        {
            var item = sender as ORDID00F;

            if (item != null)
            {
                if (item.ODPREZ == 0)
                {
                    var lastSup = GetCurrentCustomer(item);
                    if (lastSup != null)
                    {
                        item.ODPREZ = lastSup.Price;
                        item.ODSCO1 = lastSup.Discount1;
                        item.ODTSC1 = lastSup.DiscountType1;
                        item.ODSCO2 = lastSup.Discount2;
                        item.ODTSC2 = lastSup.DiscountType2;
                        item.ODSCO3 = lastSup.Discount3;
                        item.ODTSC3 = lastSup.DiscountType3;
                    }
                    else
                    {
                        var lastGen = GetCurrentGeneric(item);
                        if (lastGen != null)
                        {
                            item.ODPREZ = lastGen.Price;
                            item.ODSCO1 = lastGen.Discount1;
                            item.ODTSC1 = lastGen.DiscountType1;
                            item.ODSCO2 = lastGen.Discount2;
                            item.ODTSC2 = lastGen.DiscountType2;
                            item.ODSCO3 = lastGen.Discount3;
                            item.ODTSC3 = lastGen.DiscountType3;
                        }
                    }
                }
            }
        }

        public GenericPriceInfo? GetCurrentCustomer(ORDID00F Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICRM_LISCLIRepository>().GetCurrent(Item.otsoci, Item.ODCODA ?? string.Empty, Item.CustomerID, Item.RecipientID, Item.OrderDate, Item.ODQTAV, Item.odunit ?? string.Empty);
        }

        public GenericPriceInfo? GetCurrentGeneric(ORDID00F Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICRM_LISGENRepository>().GetCurrent(Item.otsoci, Item.ODCODA ?? string.Empty, Item.OrderDate, Item.ODQTAV, Item.odunit ?? string.Empty);
        }

        public CLIENTI? GetCLIENTI(int EntityID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICLIENTIRepository>().Get(EntityID);
        }

        public string? Validate(ORDID00F Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IORDID00FRepository>().Validate(Item, true);
        }

        public string? ValidateModel()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IORDID00FRepository>().ValidateModel(Rows ?? new());
        }

        public bool UpdateAll()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IORDID00FRepository>().UpdateAll(Head, Rows ?? new());
        }

        public CLIAMMI? GetCLIAMMI(int EntityID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICLIAMMIRepository>().Get(CompanyID, EntityID);
        }

        public DESTINATARI? GetDESTINATARI(int EntityID, int Sequence)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IDESTINATARIRepository>().Get(EntityID, Sequence);
        }

        public void LoadDetails()
        {
            AllAccounts = VulpesServiceProvider.Provider.GetRequiredService<IPDCCONTIRepository>().GetSimpleList();
            AllSubccounts = VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().GetList(CompanyID);
            GroupsList = VulpesServiceProvider.Provider.GetRequiredService<IPDCGRUPPIRepository>().GetList();
            UMsCache = VulpesServiceProvider.Provider.GetRequiredService<IUnitaRepository>().GetSimpleList(CompanyID);
            Products = VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>().GetSimpleList(CompanyID);
            RatesList = VulpesServiceProvider.Provider.GetRequiredService<IAliquoteRepository>().GetList();
            AgentsList = VulpesServiceProvider.Provider.GetRequiredService<IAGENTIRepository>().GetList();
        }
    }
}
