using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2010.Excel;
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

namespace VulpesX.ViewModels.Modules.Default.CRM
{
    public class ORDIT00FDDTsWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required Guid? CompanyUID { get; set; }
        public required string UserID { get; set; }

        public ORDIT00FDDTsWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            CompanyUID = UserContext.Instance.ACCESS!.SelectedCompany!.SOCUID;
            UserID = UserContext.Instance.ACCESS!.USRID;
        }

        public required ORDIT00F Head { get; set; }

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

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { _isBusy = value; NotifyPropertyChanged(); }
        }

        private ObservableCollection<BOLLD00F>? _items;
        public ObservableCollection<BOLLD00F>? Items { get { return _items; } set { _items = value; NotifyPropertyChanged(); } }

        private ObservableCollection<BOLLT00F>? _heads;
        public ObservableCollection<BOLLT00F>? Heads { get { return _heads; } set { _heads = value; NotifyPropertyChanged(); } }

        public async Task Load()
        {
            IsBusy = true;

            try
            {

                var bolltRepo = VulpesServiceProvider.Provider.GetRequiredService<IBOLLT00FRepository>();

                var result = await Task.Run(() =>
                {
                    var listToAdd = new List<BOLLT00F>();
                    Parallel.ForEach(Items ?? new(), (invoice) =>
                    {
                        var head = bolltRepo.Get(invoice.bolsoc, invoice.BTANNO, invoice.BTBOLL);

                        if (head != null)
                            listToAdd.Add(head);
                    });

                    return listToAdd;
                });

                Heads = new ObservableCollection<BOLLT00F>(result);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public bool Delete(BOLLT00F Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IBOLLT00FRepository>().Delete(Item);
        }

        public BOLLT00F? Get(int Year, int ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IBOLLT00FRepository>().Get(CompanyID, Year, ID);
        }

        public BOLLT00F? GetFull(int Year, int ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IBOLLT00FRepository>().GetFull(CompanyID, Year, ID, false, false);
        }

        public BOLLT00F? GetPrintFull(int Year, int ID)
        {
            var companySettings = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Get(CompanyID);

            if (companySettings == null)
                return null;

            return VulpesServiceProvider.Provider.GetRequiredService<IBOLLT00FRepository>().GetPrintFull(CompanyID, Year, ID, companySettings.azpnotddt, companySettings.azagedddt, companySettings.AZCUSDDT);
        }

        public DDTReport? PrintDDT(BOLLT00F Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IBOLLT00FRepository>().PrintDDT(Item);

        }

        public CAUSBOLL? GetCAUSBOLL(string ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICAUSBOLLRepository>().Get(ID);
        }

        public Tuple<decimal, decimal, decimal>? GetRowsTotalQuantity(int Year, int ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IBOLLD00FRepository>().GetRowsTotalQuantity(CompanyID, Year, ID);
        }

        public store_causals? GetStore_Causals(string CausalID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ISTORE_CAUSALSRepository>().Get(CompanyID, CausalID);
        }

        public bool CancelAndFreeLinkedOrders(BOLLT00F Item, string CancelReason)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IBOLLT00FRepository>().CancelAndFreeLinkedOrders(Item, UserID, CancelReason);
        }
    }
}
