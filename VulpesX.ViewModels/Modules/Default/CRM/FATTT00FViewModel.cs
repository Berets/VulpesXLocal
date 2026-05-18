using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Wordprocessing;
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
using VulpesX.DAL.Tables.Accounting;
using VulpesX.DAL.Tables.CRM;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Models.Models.CRM;
using VulpesX.Models.Reports.CRM;
using VulpesX.Shared;
using VulpesX.Shared.Generics;

namespace VulpesX.ViewModels.Modules.Default.CRM
{
    public class FATTT00FViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required Guid? CompanyUID { get; set; }
        public required string UserID { get; set; }

        public FATTT00FViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            CompanyUID = UserContext.Instance.ACCESS!.SelectedCompany!.SOCUID;
            UserID = UserContext.Instance.ACCESS!.USRID;
        }


        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { _isBusy = value; NotifyPropertyChanged(); }
        }

        private ObservableCollection<FATTT00F>? items;
        public ObservableCollection<FATTT00F>? Items
        {
            get { return items; }
            set
            {
                items = value; NotifyPropertyChanged("Items");
            }
        }

        private InvoicingTrend? trend;
        public InvoicingTrend? Trend
        {
            get => trend;
            set
            {
                trend = value;
                NotifyPropertyChanged("Trend");
            }
        }

        public ObservableCollection<Tuple<int, decimal, decimal>>? YearlyTrendData
        {
            get
            {
                var result = new ObservableCollection<Tuple<int, decimal, decimal>>();

                foreach (var year in (trend?.Months ?? new()).Select(s => s.Year).Distinct())
                {
                    result.Add(new Tuple<int, decimal, decimal>(year, trend?.Average(year) ?? 0, trend?.Median(year) ?? 0));
                }
                return result;
            }
        }

        public async Task Load(DateTime From, DateTime To)
        {
            IsBusy = true;

            try
            {
                var result = await Task.Run(() =>
                {
                    var fattRepo = VulpesServiceProvider.Provider.GetRequiredService<IFATTT00FRepository>();

                    var items = fattRepo.GetList(CompanyID, From, To);
                    var trend = fattRepo.GetTrend(CompanyID);

                    return new { items, trend };
                });

                Items = result.items;
                Trend = result.trend;
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

        public FATTT00F? GetHead(int Year, int ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IFATTT00FRepository>().GetHead(CompanyID, Year, ID);
        }

        public FATTT00F? Get(int Year, int ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IFATTT00FRepository>().Get(CompanyID, Year, ID);
        }

        public FATTT00F? GetFull(int Year, int ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IFATTT00FRepository>().GetFull(CompanyID, Year, ID, false, false);
        }

        public FATTT00F? GetSingle(int Year, int ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IFATTT00FRepository>().GetSingle(CompanyID, Year, ID);
        }

        public FATTT00F? GetPrintFull(int Year, int ID)
        {
            var companySettings = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Get(CompanyID);

            return VulpesServiceProvider.Provider.GetRequiredService<IFATTT00FRepository>().GetPrintFull(CompanyID, Year, ID, companySettings?.azpnotinv ?? false, companySettings?.azagedfat ?? false, companySettings?.AZCUSINV ?? false);
        }

        public InvoiceReport? PrintInvoice(FATTT00F Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IFATTT00FRepository>().PrintInvoice(Item);
        }

        public bool Update(FATTT00F Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IFATTT00FRepository>().Update(Item);
        }

        public bool Delete(FATTT00F Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IFATTT00FRepository>().Delete(Item);
        }

        public string? GenerateInvoiceXML(int Year, int ID, bool IsRegenerate)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IFATTT00FRepository>().GenerateInvoiceXML(CompanyID, Year, ID, IsRegenerate);
        }

        public bool InsertTFATTT00FLEVEL1(TFATTT00FLEVEL1 Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ITFATTT00FLEVEL1Repository>().Insert(Item);
        }

        public bool InsertFATTAL00F(FATTAL00F Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IFATTAL00FRepository>().Insert(Item);
        }


        public ACC_PLAFOND? GetLast(int EntityID, int Year, DateTime Date)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IACC_PLAFONDRepository>().GetLast(CompanyID, EntityID, Year, Date, false);
        }

        public ACC_PLAFOND_PARMS? GetACC_PLAFOND_PARMS()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IACC_PLAFOND_PARMSRepository>().Get(CompanyID);
        }

        public CAUFAT00F? GetCAUFAT00F(string ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICAUFAT00FRepository>().Get(ID);
        }

        public bool CheckPrices(int Year, int ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IFATTD00FRepository>().CheckPrices(CompanyID, Year, ID);
        }

        public string? Validate(FATTT00F Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IFATTT00FRepository>().Validate(Item, false);
        }

        public int GetNumerator(int Year, GenericIDDescription Numerator)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>().GetNumber(CompanyID, Year, Numerator, true);
        }

        public ObservableCollection<FATTD00F>? GetList(int Year, int ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IFATTD00FRepository>().GetList(CompanyID, Year, ID);
        }

        public bool Update(FATTD00F Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IFATTD00FRepository>().Update(Item);
        }

        public bool Update(ACC_PLAFOND Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IACC_PLAFONDRepository>().Update(Item);
        }

        public bool Accounting(FATTT00F Item, ESERCIZIO AccountingYear)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IFATTT00FRepository>().Accounting(Item, AccountingYear, Item.FTDAOR!.Value, UserID);
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
