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
using VulpesX.Services.Tables.Accounting;
using VulpesX.Shared;
using VulpesX.Shared.Generics;

namespace VulpesX.ViewModels.Modules.Default.CRM
{
    public class FATTD00FWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required Guid? CompanyUID { get; set; }
        public required string UserID { get; set; }

        public FATTD00FWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            CompanyUID = UserContext.Instance.ACCESS!.SelectedCompany!.SOCUID;
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
        public required FATTT00F Head { get; set; }
        public bool IsReadonly => !string.IsNullOrWhiteSpace(Head.FTNUMFEL);
        public bool IsEnabled => !IsReadonly;

        private ObservableCollection<FATTD00F>? rows;
        public ObservableCollection<FATTD00F>? Rows
        {
            get { return rows; }
            set
            {
                rows = value;
                Parallel.ForEach(rows ?? new(), item =>
                {
                    item.UMsCache = UMsCache;
                    item.Products = Products;
                    item.AccountCache = AccountCache;
                    item.SubaccountCache = SubaccountCache;
                    item.GroupsList = GroupsList;
                    item.NaturaList = NaturaList;
                    item.RatesList = RatesList;
                    item.Agents = Agents;
                    item.CostCentersList = CostCentersList;
                    item.CustomerID = Head.FTCODC ?? 0;
                    item.RecipientID = Head.FTCODD ?? 0;
                    item.InvoiceDate = Head.FTDAOR!.Value;
                    item.QuantityValueChanged += OnQuantityValueChanged;
                });
            }
        }

        public void OnQuantityValueChanged(object? sender, EventArgs? e)
        {
            var item = sender as FATTD00F;

            if (item != null)
            {
                if (!item.FDPREZ.HasValue || item.FDPREZ.Value == 0)
                {
                    var lastSup = GetCurrentCustomer(item);
                    if (lastSup != null)
                    {
                        item.FDPREZ = lastSup.Price;
                        item.FDSCO1 = lastSup.Discount1;
                        item.FDTSC1 = lastSup.DiscountType1;
                        item.FDSCO2 = lastSup.Discount2;
                        item.FDTSC2 = lastSup.DiscountType2;
                        item.FDSCO3 = lastSup.Discount3;
                        item.FDTSC3 = lastSup.DiscountType3;
                    }
                    else
                    {
                        var lastGen = GetCurrentGeneric(item);
                        if (lastGen != null)
                        {
                            item.FDPREZ = lastGen.Price;
                            item.FDSCO1 = lastGen.Discount1;
                            item.FDTSC1 = lastGen.DiscountType1;
                            item.FDSCO2 = lastGen.Discount2;
                            item.FDTSC2 = lastGen.DiscountType2;
                            item.FDSCO3 = lastGen.Discount3;
                            item.FDTSC3 = lastGen.DiscountType3;
                        }
                    }
                }
            }
        }

        public ObservableCollection<PDCGRUPPI>? GroupsList { get; set; }
        public List<PDCCONTI>? AccountCache { get; set; }
        public List<PDCSOTTO>? SubaccountCache { get; set; }
        public ObservableCollection<tab_articolo>? Products { get; set; }

        public ObservableCollection<ASSOGGETAMENTI>? RatesList { get; set; }
        public ObservableCollection<FE_IVADOC>? NaturaList { get; set; }
        public ObservableCollection<AGENTI>? Agents { get; set; }
        public ObservableCollection<TCECO00F>? CostCentersList { get; set; }
        public ObservableCollection<GenericIDDescription>? UMsCache { get; set; }

        #region Computed
        public decimal Imponibile => (Rows?.Sum(sum => sum.NetPrice) - ScontiCliente) ?? 0;
        public decimal ScontiCliente => Math.Round((Rows?.Sum(sum => sum.NetPrice) ?? 0) * (Head.FTSCCL ?? 0) / 100, 2);
        public decimal Sconti => Rows?.Sum(sum => sum.Discount) ?? 0;
        public decimal Maggiorazioni => Rows?.Sum(sum => sum.Surcharge) ?? 0;
        public decimal Omaggi => Rows?.Where(w => w.FDTQTA == "O").Sum(sum => sum.NetPrice) ?? 0;
        public decimal Total => Rows?.Sum(sum => sum.AmountDisplay) ?? 0;
        #endregion


        public void LoadDetails()
        {
            AccountCache = VulpesServiceProvider.Provider.GetRequiredService<IPDCCONTIRepository>().GetBasicList();
            SubaccountCache = VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().GetBasicList(CompanyID);
            GroupsList = VulpesServiceProvider.Provider.GetRequiredService<IPDCGRUPPIRepository>().GetList();
            UMsCache = VulpesServiceProvider.Provider.GetRequiredService<IUnitaRepository>().GetSimpleList(CompanyID);
            Products = VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>().GetSimpleList(CompanyID);
            RatesList = VulpesServiceProvider.Provider.GetRequiredService<IAliquoteRepository>().GetList();
            NaturaList = VulpesServiceProvider.Provider.GetRequiredService<IFE_IVADOCRepository>().GetList();
            CostCentersList = VulpesServiceProvider.Provider.GetRequiredService<ITCECO00FRepository>().GetList(CompanyID, false);

            Agents = VulpesServiceProvider.Provider.GetRequiredService<IAGENTIRepository>().GetList();
        }

        public ObservableCollection<FATTD00F>? GetList()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IFATTD00FRepository>().GetList(Head.ftsoci, Head.FTANNO, Head.FTNUOR);
        }

        public CLIAMMI? GetCLIAMMI(int EntityID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICLIAMMIRepository>().Get(CompanyID, EntityID);
        }

        public DESTINATARI? GetDESTINATARI(int EntityID, int Sequence)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IDESTINATARIRepository>().Get(EntityID, Sequence);
        }

        public GenericPriceInfo? GetCurrentCustomer(FATTD00F Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICRM_LISCLIRepository>().GetCurrent(Item.ftsoci, Item.FDCODA ?? string.Empty, Item.CustomerID, Item.RecipientID, Item.InvoiceDate, Item.FDQTAV ?? 0, Item.FDUNIM ?? string.Empty);
        }

        public GenericPriceInfo? GetCurrentGeneric(FATTD00F Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICRM_LISGENRepository>().GetCurrent(Item.ftsoci, Item.FDCODA ?? string.Empty, Item.InvoiceDate, Item.FDQTAV ?? 0, Item.FDUNIM ?? string.Empty);
        }

        public string? Validate(FATTD00F Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IFATTD00FRepository>().Validate(Item, true);
        }

        public string? ValidateModel()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IFATTD00FRepository>().ValidateModel(Rows ?? new());
        }

        public CLIENTI? GetCLIENTI(int EntityID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICLIENTIRepository>().Get(EntityID);
        }

        public bool UpdateAll()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IFATTD00FRepository>().UpdateAll(Head, Rows ?? new());
        }
    }
}
