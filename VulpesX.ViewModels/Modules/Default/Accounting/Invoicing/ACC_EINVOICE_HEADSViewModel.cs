using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL;
using VulpesX.DAL.Accounting;
using VulpesX.DAL.Accounting.eInvoice;
using VulpesX.DAL.General;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Accounting.Invoicing
{
    public abstract class ACC_EINVOICE_HEADSViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required Guid? CompanyGuidID { get; set; }
        public required string UserID { get; set; }

        public ACC_EINVOICE_HEADSViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            CompanyGuidID = UserContext.Instance.ACCESS!.SelectedCompany!.SOCUID;
            UserID = UserContext.Instance.UserName;
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                NotifyPropertyChanged();
            }
        }

        private bool _isBusySent;
        public bool IsBusySent
        {
            get { return _isBusySent; }
            set
            {
                _isBusySent = value;
                NotifyPropertyChanged();
            }
        }

        public DateTime? Period { get; set; }

        private ObservableCollection<ACC_EINVOICE_HEADS>? items;
        public ObservableCollection<ACC_EINVOICE_HEADS>? Items
        {
            get { return items; }
            set
            {
                items = value;

                Parallel.ForEach(items ?? new ObservableCollection<ACC_EINVOICE_HEADS>(), item =>
                {
                    item.Suppliers = Suppliers;
                });
                NotifyPropertyChanged("Items");
            }
        }

        public ObservableCollection<ABE>? Suppliers { get; set; }

        public ObservableCollection<PDCGRUPPI>? GroupCache { get; set; }

        public List<PDCCONTI>? AccountCache { get; set; }

        public List<PDCSOTTO>? SubaccountCache { get; set; }


        public DateTime? PeriodSent { get; set; }

        private ObservableCollection<ACC_EINVOICE_HEADS>? itemsSent;
        public ObservableCollection<ACC_EINVOICE_HEADS>? ItemsSent
        {
            get { return itemsSent; }
            set
            {
                itemsSent = value;
                NotifyPropertyChanged("ItemsSent");
            }
        }

        public abstract string? GetApiKey();

        public abstract Task Load();

        public abstract Task LoadSent();

        public abstract void LoadDetails();

        public abstract ACC_EINVOICE_HEADS? Get(long ID);

        public abstract ACC_EINVOICE_HEADS? GetFull(long ID);

        public abstract bool Update(ACC_EINVOICE_HEADS Item);

        public abstract bool Delete(ACC_EINVOICE_HEADS Item, Guid? CompanyID);

        public abstract tab_articolo? GetArticle(string ID);


        //ABE
        public abstract ObservableCollection<PDCGRUPPI>? GetGruppi();

        public abstract List<PDCCONTI>? GetConti();

        public abstract List<PDCSOTTO>? GetSotto();

        public abstract ABE? GetData(int ID);

        public abstract FORNAMMI? GetFORNAMMI(int ID);

        public abstract FORNITORI? GetFORNITORI(int ID);

        public abstract ObservableCollection<NOTEFOR>? GetNOTEFOR(int ID);

        public abstract CLIAMMI? GetCLIAMMI(int ID);

        public abstract CLIENTI? GetCLIENTI(int ID);

        public abstract ObservableCollection<NOTECLI1>? GetNOTECLI(int ID);

        public abstract ObservableCollection<RFFTB00F>? GetRFFTB00F(int ID);

        public abstract ObservableCollection<ANDEFRES>? GetANDEFRES(int ID);

        public abstract ObservableCollection<SUPPLIER_GROUPS>? GetSUPPLIER_GROUPS(int ID);

        public abstract ObservableCollection<CUSTOMER_GROUPS>? GetCUSTOMER_GROUPS(int ID);

        public abstract PNTESTATA? GetPNTESTATA(int Year, int ID);

        public abstract ObservableCollection<ABE>? GetABE(string N1FLCF);

        public abstract ObservableCollection<CAUCONT>? GetCAUCONT(string StateID);
    }

    public class ACC_EINVOICE_HEADSViewModelDefault : ACC_EINVOICE_HEADSViewModel
    {
        public ACC_EINVOICE_HEADSViewModelDefault()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;
        }

        public override string? GetApiKey()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Get(CompanyID)?.azapikey;
        }

        public override async Task Load()
        {
            IsBusy = true;

            await Task.Run(() =>
                       Items = VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_HEADSRepository>().GetList(CompanyID, Period ?? VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime(), "R"));

            IsBusy = false;
        }

        public override async Task LoadSent()
        {
            IsBusy = true;

            await Task.Run(() =>
                       ItemsSent = VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_HEADSRepository>().GetList(CompanyID, Period ?? VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime(), "S"));

            IsBusy = false;
        }

        public override void LoadDetails()
        {
            AccountCache = VulpesServiceProvider.Provider.GetRequiredService<IPDCCONTIRepository>().GetBasicList();
            SubaccountCache = VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().GetBasicList(CompanyID);
            GroupCache = VulpesServiceProvider.Provider.GetRequiredService<IPDCGRUPPIRepository>().GetList();
            Suppliers = VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().GetLightList("F");
        }
        public override ACC_EINVOICE_HEADS? Get(long ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_HEADSRepository>().Get(ID);
        }

        public override ACC_EINVOICE_HEADS? GetFull(long ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_HEADSRepository>().GetFull(ID);
        }

        public override bool Update(ACC_EINVOICE_HEADS Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_HEADSRepository>().Update(Item);
        }

        public override bool Delete(ACC_EINVOICE_HEADS Item, Guid? CompanyID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_HEADSRepository>().Delete(Item, CompanyID);
        }

        public override tab_articolo? GetArticle(string ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>().GetSingle(CompanyID, ID);
        }

        //ABE
        public override ObservableCollection<PDCGRUPPI>? GetGruppi()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPDCGRUPPIRepository>().GetList();
        }

        public override List<PDCCONTI>? GetConti()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPDCCONTIRepository>().GetBasicList();
        }

        public override List<PDCSOTTO>? GetSotto()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().GetBasicList(CompanyID);
        }

        public override ABE? GetData(int ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().Get(ID);
        }

        public override FORNAMMI? GetFORNAMMI(int ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IFORNAMMIRepository>().Get(CompanyID, ID);
        }

        public override FORNITORI? GetFORNITORI(int ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IFORNITORIRepository>().Get(ID);
        }

        public override ObservableCollection<NOTEFOR>? GetNOTEFOR(int ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<INOTEFORRepository>().GetList(ID);
        }

        public override CLIAMMI? GetCLIAMMI(int ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICLIAMMIRepository>().Get(CompanyID, ID);
        }

        public override CLIENTI? GetCLIENTI(int ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICLIENTIRepository>().Get(ID);
        }

        public override ObservableCollection<NOTECLI1>? GetNOTECLI(int ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<INOTECLI1Repository>().GetList(ID);
        }

        public override ObservableCollection<RFFTB00F>? GetRFFTB00F(int ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IRFFTB00FRepository>().GetList(ID);
        }

        public override ObservableCollection<ANDEFRES>? GetANDEFRES(int ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IANDEFRESRepository>().GetList(ID);
        }

        public override ObservableCollection<SUPPLIER_GROUPS>? GetSUPPLIER_GROUPS(int ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ISUPPLIER_GROUPSRepository>().GetList(CompanyID, ID);
        }

        public override ObservableCollection<CUSTOMER_GROUPS>? GetCUSTOMER_GROUPS(int ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICUSTOMER_GROUPSRepository>().GetList(CompanyID, ID);
        }

        public override PNTESTATA? GetPNTESTATA(int Year, int ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPNTESTATARepository>().Get(CompanyID, Year, ID);
        }

        public override ObservableCollection<ABE>? GetABE(string N1FLCF)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().GetLightList(N1FLCF);
        }

        public override ObservableCollection<CAUCONT>? GetCAUCONT(string StateID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICAUCONTRepository>().GetSimpleList(StateID);
        }
    }

    public class ACC_EINVOICE_HEADSViewModelUfp : ACC_EINVOICE_HEADSViewModel
    {
        public ACC_EINVOICE_HEADSViewModelUfp()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;
        }

        public override string? GetApiKey()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPERSBOLLRepository>().Get(CompanyID)?.perapikey;
        }

        public override async Task Load()
        {
            IsBusy = true;

            await Task.Run(() =>
                       Items = VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_HEADSRepository>().GetList(CompanyID, Period ?? VulpesServiceProvider.Provider.GetRequiredService<DateTimeService>().GetDatabaseServerDateTime(), "R"));

            IsBusy = false;
        }

        public override async Task LoadSent()
        {
            await Task.Yield();

            throw new NotImplementedException();
        }

        public override void LoadDetails()
        {
            AccountCache = VulpesServiceProvider.Provider.GetRequiredService<IPDCCONTIRepository>().GetBasicList();
            SubaccountCache = VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().GetBasicList(CompanyID);
            GroupCache = VulpesServiceProvider.Provider.GetRequiredService<IPDCGRUPPIRepository>().GetList();
            Suppliers = VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().GetLightList("F");
        }

        public override ACC_EINVOICE_HEADS? Get(long ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_HEADSRepository>().Get(ID);
        }

        public override ACC_EINVOICE_HEADS? GetFull(long ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_HEADSRepository>().GetFull(ID);
        }

        public override bool Update(ACC_EINVOICE_HEADS Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_HEADSRepository>().Update(Item);
        }

        public override bool Delete(ACC_EINVOICE_HEADS Item, Guid? CompanyID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IACC_EINVOICE_HEADSRepository>().Delete(Item, CompanyID);
        }

        public override tab_articolo? GetArticle(string ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>().GetSingle(CompanyID, ID);
        }


        //ABE
        public override ObservableCollection<PDCGRUPPI>? GetGruppi()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPDCGRUPPIRepository>().GetList();
        }

        public override List<PDCCONTI>? GetConti()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPDCCONTIRepository>().GetBasicList();
        }

        public override List<PDCSOTTO>? GetSotto()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().GetBasicList(CompanyID);
        }

        public override ABE? GetData(int ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().Get(ID);
        }

        public override FORNAMMI? GetFORNAMMI(int ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IFORNAMMIRepository>().Get(CompanyID, ID);
        }

        public override FORNITORI? GetFORNITORI(int ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IFORNITORIRepository>().Get(ID);
        }

        public override ObservableCollection<NOTEFOR>? GetNOTEFOR(int ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<INOTEFORRepository>().GetList(ID);
        }

        public override CLIAMMI? GetCLIAMMI(int ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICLIAMMIRepository>().Get(CompanyID, ID);
        }

        public override CLIENTI? GetCLIENTI(int ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICLIENTIRepository>().Get(ID);
        }

        public override ObservableCollection<NOTECLI1>? GetNOTECLI(int ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<INOTECLI1Repository>().GetList(ID);
        }

        public override ObservableCollection<RFFTB00F>? GetRFFTB00F(int ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IRFFTB00FRepository>().GetList(ID);
        }

        public override ObservableCollection<ANDEFRES>? GetANDEFRES(int ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IANDEFRESRepository>().GetList(ID);
        }

        public override ObservableCollection<SUPPLIER_GROUPS>? GetSUPPLIER_GROUPS(int ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ISUPPLIER_GROUPSRepository>().GetList(CompanyID, ID);
        }

        public override ObservableCollection<CUSTOMER_GROUPS>? GetCUSTOMER_GROUPS(int ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICUSTOMER_GROUPSRepository>().GetList(CompanyID, ID);
        }

        public override PNTESTATA? GetPNTESTATA(int Year, int ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPNTESTATARepository>().Get(CompanyID, Year, ID);
        }

        public override ObservableCollection<ABE>? GetABE(string N1FLCF)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().GetLightList(N1FLCF);
        }

        public override ObservableCollection<CAUCONT>? GetCAUCONT(string StateID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICAUCONTRepository>().GetSimpleList(StateID);
        }
    }
}
