using DocumentFormat.OpenXml.Drawing;
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
using VulpesX.DAL.Tables.Shipping;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Models.Reports.Shipping;
using VulpesX.Shared;
using VulpesX.Shared.Generics;
using static VulpesX.Shared.Utilities.TelerikGridService;

namespace VulpesX.ViewModels.Modules.Default.CRM
{
    public class BOLLT00FViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }
        public required Guid? CompanyUID { get; set; }
        public required string EntityType { get; set; }

        public BOLLT00FViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            CompanyUID = UserContext.Instance.ACCESS!.SelectedCompany!.SOCUID;
            UserID = UserContext.Instance.ACCESS!.USRID;
        }


        private ObservableCollection<BOLLT00F>? items;
        public ObservableCollection<BOLLT00F>? Items { get { return items; } set { items = value; NotifyPropertyChanged("Items"); } }

        private bool isSelectable;
        public bool IsSelectable { get => isSelectable; set { isSelectable = value; NotifyPropertyChanged("IsSelectable"); } }

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { _isBusy = value; NotifyPropertyChanged(); }
        }

        private DateTime year;
        public DateTime Year
        {
            get => year; set
            {
                year = value;
                NotifyPropertyChanged();
            }
        }

        private string? selectedStatusID;
        public string? SelectedStatusID
        {
            get => selectedStatusID; set
            {
                selectedStatusID = value;
                NotifyPropertyChanged();
            }
        }

        private string? textFilter;
        public string? TextFilter
        {
            get => textFilter; set
            {
                textFilter = value;
                NotifyPropertyChanged();
            }
        }

        private int pageSize;
        public int PageSize
        {
            get => pageSize; set
            {
                pageSize = value;
                NotifyPropertyChanged();
            }
        }

        private int pageRequested;
        public int PageRequested
        {
            get => pageRequested; set
            {
                pageRequested = value;
                NotifyPropertyChanged();
            }
        }

        private int totalCount;
        public int TotalCount
        {
            get => totalCount; set
            {
                totalCount = value;
                NotifyPropertyChanged();
            }
        }

        public async Task Load(List<GenericIDDescription> Sorts, List<FilterEntry> Wheres)
        {
            IsBusy = true;

            try
            {
                var result = await Task.Run(() =>
                {
                    int itemsCount = 0;

                    var items = VulpesServiceProvider.Provider.GetRequiredService<IBOLLT00FRepository>().GetList(CompanyID, Year.Year, SelectedStatusID, EntityType, TextFilter, PageSize, PageRequested, Sorts, Wheres, out itemsCount);

                    return new { items, itemsCount };
                });

                Items = result.items;
                TotalCount = result.itemsCount;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public AZIENDA? GetAZIENDA()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Get(CompanyID);
        }

        public BOLLT00F? Get(int Year, int ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IBOLLT00FRepository>().Get(CompanyID, Year, ID);
        }

        public BOLLT00F? GetFull(int Year, int ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IBOLLT00FRepository>().GetFull(CompanyID, Year, ID, false, false);
        }

        public bool CancelAndFreeLinkedOrders(BOLLT00F Item, string CancelReason)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IBOLLT00FRepository>().CancelAndFreeLinkedOrders(Item, UserID, CancelReason);
        }

        public BOLLT00F? GetPrintFull(int Year, int ID)
        {
            var companySettings = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Get(CompanyID);

            return VulpesServiceProvider.Provider.GetRequiredService<IBOLLT00FRepository>().GetPrintFull(CompanyID, Year, ID, companySettings?.azpnotddt ?? false, companySettings?.azagedddt ?? false, companySettings?.AZCUSDDT ?? false);
        }

        public DDTReport? PrintDDT(BOLLT00F Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IBOLLT00FRepository>().PrintDDT(Item);
        }

        public CAUSBOLL? GetCAUSBOLL(string ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICAUSBOLLRepository>().Get(ID);
        }

        public store_causals? GetStore_Causals(string ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ISTORE_CAUSALSRepository>().Get(CompanyID, ID);
        }

        public Tuple<decimal, decimal, decimal>? GetRowsTotalQuantity(int Year, int ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IBOLLD00FRepository>().GetRowsTotalQuantity(CompanyID, Year, ID);
        }

        public bool GenerateByDDT(List<BOLLT00F> DDTs, int Year)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IFATTT00FRepository>().GenerateByDDT(DDTs, Year, UserID);
        }

        #region ABE
        public ABE? GetABE(int EntityID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().Get(EntityID);
        }

        public ObservableCollection<PDCGRUPPI>? GetGruppi()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPDCGRUPPIRepository>().GetList();
        }

        public List<PDCCONTI>? GetConti()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPDCCONTIRepository>().GetBasicList();
        }

        public List<PDCSOTTO>? GetSotto()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().GetBasicList(CompanyID);
        }

        public FORNITORI? GetFORNITORI(int EntityID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IFORNITORIRepository>().Get(EntityID);
        }

        public FORNAMMI? GetFORNAMMI(int EntityID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IFORNAMMIRepository>().Get(CompanyID, EntityID);
        }

        public CLIENTI? GetCLIENTI(int EntityID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICLIENTIRepository>().Get(EntityID);
        }

        public CLIAMMI? GetCLIAMMI(int EntityID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICLIAMMIRepository>().Get(CompanyID, EntityID);
        }

        public ObservableCollection<NOTECLI1>? GetNOTECLI1s(int EntityID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<INOTECLI1Repository>().GetList(EntityID);
        }

        public ObservableCollection<NOTEFOR>? GetNOTEFORs(int EntityID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<INOTEFORRepository>().GetList(EntityID);
        }

        public ObservableCollection<RFFTB00F>? GetRFFTB00Fs(int EntityID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IRFFTB00FRepository>().GetList(EntityID);
        }

        public ObservableCollection<ANDEFRES>? GetANDEFREs(int EntityID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IANDEFRESRepository>().GetList(EntityID);
        }

        public ObservableCollection<SUPPLIER_GROUPS>? GetSUPPLIER_GROUPs(int EntityID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ISUPPLIER_GROUPSRepository>().GetList(CompanyID, EntityID);
        }

        public ObservableCollection<CUSTOMER_GROUPS>? GetCUSTOMER_GROUPs(int EntityID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICUSTOMER_GROUPSRepository>().GetList(CompanyID, EntityID);
        }
        #endregion
    }
}
