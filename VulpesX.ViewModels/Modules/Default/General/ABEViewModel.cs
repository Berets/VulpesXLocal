using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Accounting;
using VulpesX.DAL.General;
using VulpesX.DAL.Store;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Models.Ufp;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.General
{
    public abstract class ABEViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public ABEViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.ACCESS!.USRID;
        }

        private ObservableCollection<ABE>? _items;
        public ObservableCollection<ABE>? Items { get { return _items; } set { _items = value; NotifyPropertyChanged(); } }

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { _isBusy = value; NotifyPropertyChanged(); }
        }

        public abstract Task Load();

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

        public abstract bool Delete(ABE Item, string? CancelReason, string? UserID);

        public abstract ANACERT? GetANACERT(int ID);

        public abstract ObservableCollection<ANACERTLEVEL1>? GetANACERTLEVEL1(int ID);

        public abstract SCADCLI? GetSCADCLI(int ID);
    }

    public class ABEViewModelDefault : ABEViewModel
    {
        public ABEViewModelDefault()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.ACCESS!.USRID;
        }


        public override async Task Load()
        {
            IsBusy = true;

            try
            {
                var result = await Task.Run(() =>
                {
                    return VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().GetList();
                });

                Items = result;
            }
            finally
            {
                IsBusy = false;
            }
        }

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

        public override bool Delete(ABE Item, string? CancelReason, string? UserID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().Delete(Item, CancelReason, UserID);
        }

        public override ANACERT? GetANACERT(int ID)
        {
            throw new NotImplementedException();
        }

        public override ObservableCollection<ANACERTLEVEL1>? GetANACERTLEVEL1(int ID)
        {
            throw new NotImplementedException();
        }

        public override SCADCLI? GetSCADCLI(int ID)
        {
            throw new NotImplementedException();
        }
    }

    public class ABEViewModelUfp : ABEViewModel
    {
        public ABEViewModelUfp()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
        }

        public override async Task Load()
        {
            IsBusy = true;

            try
            {
                var result = await Task.Run(() =>
                {
                    return VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().GetCompanyList(CompanyID);
                });

                Items = result;
            }
            finally
            {
                IsBusy = false;
            }
        }

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
            return VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().Get(CompanyID, ID);
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

        public override bool Delete(ABE Item, string? CancelReason, string? UserID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().Delete(Item, CancelReason, UserID);
        }

        public override ANACERT? GetANACERT(int ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IANACERTRepository>().Get(ID);
        }

        public override ObservableCollection<ANACERTLEVEL1>? GetANACERTLEVEL1(int ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IANACERTLEVEL1Repository>().GetList(ID);
        }

        public override SCADCLI? GetSCADCLI(int ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ISCADCLIService>().Get(ID);
        }
    }

}
