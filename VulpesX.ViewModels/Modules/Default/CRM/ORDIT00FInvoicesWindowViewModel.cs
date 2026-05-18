using DocumentFormat.OpenXml.Office2010.ExcelAc;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Accounting;
using VulpesX.DAL.CRM;
using VulpesX.DAL.General;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.CRM
{
    public class ORDIT00FInvoicesWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required Guid? CompanyUID { get; set; }
        public required string UserID { get; set; }

        public ORDIT00FInvoicesWindowViewModel()
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
            return VulpesServiceProvider.Provider.GetRequiredService<ISUPPLIER_GROUPSRepository>().GetList(CompanyID,EntityID);
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

        private ObservableCollection<FATTD00F>? _items;
        public ObservableCollection<FATTD00F>? Items { get { return _items; } set { _items = value; NotifyPropertyChanged(); } }

        private ObservableCollection<FATTT00F>? _heads;
        public ObservableCollection<FATTT00F>? Heads { get { return _heads; } set { _heads = value; NotifyPropertyChanged(); } }

        public async Task Load()
        {
            IsBusy = true;

            try
            {

                var fatttRepo = VulpesServiceProvider.Provider.GetRequiredService<IFATTT00FRepository>();

                Heads = new ObservableCollection<FATTT00F>();

                var result = await Task.Run(() =>
                {
                    var listToAdd = new List<FATTT00F>();
                    Parallel.ForEach(Items ?? new(), (invoice) =>
                    {
                        var head = fatttRepo.Get(invoice.ftsoci, invoice.FTANNO, invoice.FTNUOR);

                        if(head != null)
                            listToAdd.Add(head);
                    });

                    return listToAdd;
                });

                Heads = new ObservableCollection<FATTT00F>(result);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public bool Delete(FATTT00F Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IFATTT00FRepository>().Delete(Item);
        }
    }
}
