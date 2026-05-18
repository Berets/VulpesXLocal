using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Accounting;
using VulpesX.DAL.Auth;
using VulpesX.DAL.General;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Services.Tables.Accounting;
using VulpesX.Shared;
using VulpesX.Shared.Generics;

namespace VulpesX.ViewModels.Modules.Default.Accounting
{
    public class PNRIGHEWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }
        public required bool IsInsert { get; set; }
        public AUTH_ACCESS_ROLES? Roles { get; set; }

        public PNRIGHEWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;
            Roles = VulpesServiceProvider.Provider.GetRequiredService<IAUTH_ACCESS_ROLESRepository>().Get(CompanyID, UserID);
            AccountCache = VulpesServiceProvider.Provider.GetRequiredService<IPDCCONTIRepository>().GetBasicList();
            SubaccountCache = VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().GetBasicList(CompanyID);
            GroupsList = VulpesServiceProvider.Provider.GetRequiredService<IPDCGRUPPIRepository>().GetList();
            CostCentersList = VulpesServiceProvider.Provider.GetRequiredService<ITCECO00FRepository>().GetList(CompanyID, false);
            RatesList = VulpesServiceProvider.Provider.GetRequiredService<IAliquoteRepository>().GetList();
            IVABooksList = VulpesServiceProvider.Provider.GetRequiredService<ILIBRIIVARepository>().GetList();
            Rows = new ObservableCollection<PNRIGHE>();
            IVARows = new ObservableCollection<PNIVA>();

            PaymentsListCustomerCache = new ObservableCollection<GenericIDDescription>(VulpesServiceProvider.Provider.GetRequiredService<IPAGCLIRepository>().GetList()?.Select(s => new GenericIDDescription() { ID = s.pclcod, Description = s.pcldes?.Trim() })?.ToList() ?? new List<GenericIDDescription>());
            PaymentsListSupplierCache = new ObservableCollection<GenericIDDescription>(VulpesServiceProvider.Provider.GetRequiredService<IPAGFORRepository>().GetList()?.Select(s => new GenericIDDescription() { ID = s.pfocod, Description = s.pfodes?.Trim() })?.ToList() ?? new List<GenericIDDescription>());
            CustomersCache = VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().GetLightList("C");
            SuppliersCache = VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().GetLightList("F");
        }

        public bool ViewUpdate
        {
            set
            {
                NotifyPropertyChanged("Balance");
                NotifyPropertyChanged("BalanceSign");
                NotifyPropertyChanged("BalanceForeground");
            }
        }

        public required PNTESTATA Head { get; set; }
        public CAUCONT? HeadSelectedCausal { get; set; }

        private ObservableCollection<PNRIGHE> rows = new();
        public ObservableCollection<PNRIGHE> Rows
        {
            get { return rows; }
            set
            {
                rows = value;
                foreach (var item in rows)
                {
                    item.SubaccountChanged += OnSubaccountChanged;
                    item.AccountCache = AccountCache;
                    item.SubaccountCache = SubaccountCache;
                    item.GroupsList = GroupsList;
                    item.CostCentersList = CostCentersList;
                    item.Testata = Head;
                }
            }
        }

        private ObservableCollection<PNIVA>? iVARows;
        public ObservableCollection<PNIVA>? IVARows
        {
            get { return iVARows; }
            set
            {
                foreach (var row in value ?? new ObservableCollection<PNIVA>())
                {
                    row.RatesList = RatesList;
                    row.IVABooksList = IVABooksList;
                }
                iVARows = value;
            }
        }

        public List<PDCCONTI>? AccountCache { get; set; }
        public List<PDCSOTTO>? SubaccountCache { get; set; }
        public ObservableCollection<PDCGRUPPI>? GroupsList { get; set; }
        public ObservableCollection<TCECO00F>? CostCentersList { get; set; }
        public ObservableCollection<ASSOGGETAMENTI>? RatesList { get; set; }
        public ObservableCollection<LIBRIIVA>? IVABooksList { get; set; }
        public ObservableCollection<GenericIDDescription> Signs { get; set; } = new ObservableCollection<GenericIDDescription>
        {
            new GenericIDDescription(){ ID = "D", Description = "Dare" },
            new GenericIDDescription(){ ID = "A", Description = "Avere" }
        };
        public ObservableCollection<GenericIDDescription> IVASigns { get; set; } = new ObservableCollection<GenericIDDescription>
        {
            new GenericIDDescription(){ ID = "+", Description = "+" },
            new GenericIDDescription(){ ID = "-", Description = "-" }
        };


        public ObservableCollection<GenericIDDescription>? PaymentsListCustomerCache { get; set; }
        public ObservableCollection<GenericIDDescription>? PaymentsListSupplierCache { get; set; }
        public ObservableCollection<ABE>? CustomersCache { get; set; }
        public ObservableCollection<ABE>? SuppliersCache { get; set; }


        public bool IsReadonly { get; set; }
        public bool IsEnabled => !IsReadonly;

        #region Balance
        public string? BalanceSign { get; set; }
        public decimal Balance
        {
            get
            {
                var dare = Rows != null ? Rows.Where(w => w.N1SEGN == "D").Sum(sum => sum.N1IMEU) ?? 0 : 0;
                var avere = Rows != null ? Rows.Where(w => w.N1SEGN == "A").Sum(sum => sum.N1IMEU) ?? 0 : 0;
                if (dare > avere)
                {
                    BalanceSign = "D";
                    NotifyPropertyChanged("BalanceSign");
                    return dare - avere;
                }
                else
                {
                    if (avere > dare)
                    {
                        BalanceSign = "A";
                        NotifyPropertyChanged("BalanceSign");
                        return avere - dare;
                    }
                    else
                    {
                        BalanceSign = string.Empty;
                        NotifyPropertyChanged("BalanceSign");
                        return 0;
                    }
                }
            }
        }
        public string BalanceForeground => Balance == 0 ? "W" : "O";
        #endregion

        public void OnSubaccountChanged(object? sender, EventArgs? e)
        {
            var item = sender as PNRIGHE;
            if (item != null)
            {
                if (item.SelectedSubaccount != null)
                {
                    if (item.SelectedSubaccount.P3CLFO == "C")
                    {
                        item.PaymentsList = PaymentsListCustomerCache;
                        item.EntitiesList = CustomersCache;
                    }
                    else
                    {
                        item.PaymentsList = PaymentsListSupplierCache;
                        item.EntitiesList = SuppliersCache;
                    }
                }
            }
        }

        public ObservableCollection<PNRIGHE>? GetPNRIGHE()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPNRIGHERepository>().GetList(Head.N1SOCI, Head.N1ANNO, Head.N1REGI);
        }

        public ObservableCollection<PNIVA>? GetPNIVA()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPNIVARepository>().GetList(Head.N1SOCI, Head.N1ANNO, Head.N1REGI);
        }

        public ObservableCollection<SUPPLIER_GROUPS>? GetSUPPLIER_GROUPS()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ISUPPLIER_GROUPSRepository>().GetList(Head.N1SOCI, Head.N1CLFO ?? 0, Head.pncaus ?? string.Empty);
        }

        public ObservableCollection<CAUCONT_GROUPS>? GetCAUCONT_GROUPS()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICAUCONT_GROUPSRepository>().GetList(Head.N1SOCI, Head.pncaus ?? string.Empty);
        }

        public ASSOGGETAMENTI? GetASSOGGETAMENTI(string? Assoggettamento, string? Aliquota)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IAliquoteRepository>().Get(Assoggettamento ?? string.Empty, Aliquota ?? string.Empty);
        }

        public CLIENTI? GetCLIENTI()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICLIENTIRepository>().Get(Head.N1CLFO ?? 0);
        }

        public CLIAMMI? GetCLIAMMI()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICLIAMMIRepository>().Get(Head.N1SOCI, Head.N1CLFO ?? 0);
        }

        public FORNAMMI? GetFORNAMMI()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IFORNAMMIRepository>().Get(Head.N1SOCI, Head.N1CLFO ?? 0);
        }

        public LIBRIIVA? GetLIBRIIVA()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ILIBRIIVARepository>().Get(HeadSelectedCausal?.cauliv ?? string.Empty);
        }


        public PDCSOTTO? GetPDCSOTTO(string? GroupID, string? AccountID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().GetFirst(GroupID ?? string.Empty, AccountID ?? string.Empty, Head.N1FLCF ?? string.Empty, CompanyID);
        }

        public string? ValidatePNRIGHE(PNRIGHE Model)
        {
            if (HeadSelectedCausal == null)
                return "Causale obbligatoria";

            return VulpesServiceProvider.Provider.GetRequiredService<IPNRIGHERepository>().Validate(Model, HeadSelectedCausal);
        }
        public string? ValidatePNIVA(PNIVA Model)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPNIVARepository>().Validate(Model, true);
        }

        public ObservableCollection<PNCLIENTI>? GetPNCLIENTI(int Riga, int CustomerID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPNCLIENTIRepository>().GetList(Head.N1SOCI, Head.N1ANNO, Head.N1REGI, Riga, CustomerID);
        }

        public ObservableCollection<PNFORNITORI>? GetPNFORNITORI(int Riga, int SupplierID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPNFORNITORIRepository>().GetList(Head.N1SOCI, Head.N1ANNO, Head.N1REGI, Riga, SupplierID);
        }

        public PDCANNI? GetPDCANNI(string GroupID, string AccountID, string SubaccountID, int Year)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPDCANNIRepository>().GetFull(CompanyID, GroupID, AccountID, SubaccountID, Year);
        }

        public string? ValidateModel()
        {
            if (HeadSelectedCausal == null)
                return "Causale obbligatoria";
            if (Head.AccountingCausal == null)
                return "Causale testata obbligatoria";

            return VulpesServiceProvider.Provider.GetRequiredService<IPNRIGHERepository>().ValidateModel(HeadSelectedCausal, IVARows, Rows, Balance, Head.AccountingCausal, IsInsert);
        }

        public bool Save()
        {
            if (IsInsert)
            {
                Head.addedUserID = UserID;
                return VulpesServiceProvider.Provider.GetRequiredService<IPNTESTATARepository>().Insert(Head, Rows, HeadSelectedCausal!, IVARows ?? new ObservableCollection<PNIVA>());
            }
            else
            {
                Head.updatedUserID = UserID;
                return VulpesServiceProvider.Provider.GetRequiredService<IPNTESTATARepository>().Update(Head, Rows, HeadSelectedCausal!, IVARows ?? new ObservableCollection<PNIVA>());
            }
        }
    }
}
