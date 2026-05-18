using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Accounting;
using VulpesX.DAL.General;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.DAL.Tables.CRM;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Services.Tables.Accounting;
using VulpesX.Shared;
using VulpesX.Shared.Generics;

namespace VulpesX.ViewModels.Modules.Default.Tables.Accounting
{
    public abstract class CAUCONTWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public CAUCONTWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
        }

        public required CAUCONT Data { get; set; }
        public bool IsInsert { get; set; }

        public ObservableCollection<GenericIDDescription>? Signs { get; set; }
        public ObservableCollection<GenericIDDescription>? IVASigns { get; set; }
        public ObservableCollection<ASSOGGETAMENTI>? Rates { get; set; }
        public ASSOGGETAMENTI? SelectedRate { get; set; }
        public ObservableCollection<LIBRIIVA>? IVABooks { get; set; }
        public LIBRIIVA? SelectedIVABook { get; set; }
        public ObservableCollection<TCECO00F>? CostCenters { get; set; }
        public TCECO00F? SelectedCostCenter { get; set; }
        public ASSOGGETAMENTI? SelectedCompanyRate { get; set; }
        public ObservableCollection<PDCGRUPPI>? GroupsList { get; set; }
        public List<PDCCONTI>? AccountCache { get; set; }
        public List<PDCSOTTO>? SubaccountCache { get; set; }
        public ObservableCollection<CAUCONT>? CausalsList { get; set; }

        private ObservableCollection<CAUCONT_GROUPS>? counterpartsRows;
        public ObservableCollection<CAUCONT_GROUPS>? CounterpartsRows
        {
            get { return counterpartsRows; }
            set
            {
                counterpartsRows = value;
            }
        }

        public ObservableCollection<CAUCONT>? Causals { get; set; }

        private CAUCONT? selectedCausal;
        public CAUCONT? SelectedCausal { get => selectedCausal; set { selectedCausal = value; NotifyPropertyChanged("SelectedCausal"); } }

        public ObservableCollection<CAUFAT00F>? CausalsAuto { get; set; }

        private CAUFAT00F? selectedCausalAuto;
        public CAUFAT00F? SelectedCausalAuto { get => selectedCausalAuto; set { selectedCausalAuto = value; NotifyPropertyChanged("SelectedCausalAuto"); } }

        public abstract ObservableCollection<CAUCONT_GROUPS>? GetCAUCONT_GROUPS();

        public abstract ObservableCollection<PDCGRUPPI>? GetPDCGRUPPI();

        public abstract List<PDCCONTI>? GetPDCCONTI();

        public abstract List<PDCSOTTO>? GetPDCSOTTO();

        public abstract ObservableCollection<CAUCONT>? GetCAUCONT();

        public abstract ObservableCollection<TCECO00F>? GetTCECO00F();

        public abstract string? ValidateCAUCONT_GROUPS(CAUCONT_GROUPS Group, bool IsInsert);

        public abstract ObservableCollection<CAUCONT>? GetCAUCONTs();

        public abstract ObservableCollection<CAUFAT00F>? GetCAUFAT00Fs();

        public abstract string? Validate();

        public abstract bool Save();
    }

    public class CAUCONTWindowViewModelDefault : CAUCONTWindowViewModel
    {
        public CAUCONTWindowViewModelDefault()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;

            Signs = CommonsService.StandardAccountingSigns;
            IVASigns = CommonsService.StandardIVASigns;
            Rates = VulpesServiceProvider.Provider.GetRequiredService<IAliquoteRepository>().GetList();
            IVABooks = VulpesServiceProvider.Provider.GetRequiredService<ILIBRIIVARepository>().GetList();
        }

        public override ObservableCollection<CAUCONT_GROUPS>? GetCAUCONT_GROUPS()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICAUCONT_GROUPSRepository>().GetList(CompanyID, Data.caucod);
        }

        public override ObservableCollection<PDCGRUPPI>? GetPDCGRUPPI()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPDCGRUPPIRepository>().GetList();
        }

        public override List<PDCCONTI>? GetPDCCONTI()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPDCCONTIRepository>().GetBasicList();
        }

        public override List<PDCSOTTO>? GetPDCSOTTO()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().GetBasicList(CompanyID);
        }

        public override ObservableCollection<CAUCONT>? GetCAUCONT()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICAUCONTRepository>().GetSimpleList();
        }

        public override ObservableCollection<TCECO00F>? GetTCECO00F()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ITCECO00FRepository>().GetList(CompanyID, false);
        }

        public override string? ValidateCAUCONT_GROUPS(CAUCONT_GROUPS Group, bool IsInsert)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICAUCONT_GROUPSRepository>().Validate(Group, IsInsert);
        }

        public override ObservableCollection<CAUCONT>? GetCAUCONTs()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICAUCONTRepository>().GetList();
        }

        public override ObservableCollection<CAUFAT00F>? GetCAUFAT00Fs()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICAUFAT00FRepository>().GetList();
        }

        public override string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICAUCONTRepository>().Validate(Data, IsInsert);
        }

        public override bool Save()
        {
            if (IsInsert)
                return VulpesServiceProvider.Provider.GetRequiredService<ICAUCONTRepository>().Insert(Data, CounterpartsRows, CompanyID);
            else
                return VulpesServiceProvider.Provider.GetRequiredService<ICAUCONTRepository>().Update(Data, CounterpartsRows, CompanyID);
        }
    }

    public class CAUCONTWindowViewModelUfp : CAUCONTWindowViewModel
    {
        public CAUCONTWindowViewModelUfp()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;

            Signs = CommonsService.StandardAccountingSigns;
            IVASigns = CommonsService.StandardIVASigns;
            Rates = VulpesServiceProvider.Provider.GetRequiredService<IAliquoteRepository>().GetList();
            IVABooks = VulpesServiceProvider.Provider.GetRequiredService<ILIBRIIVARepository>().GetList();
        }

        public override ObservableCollection<CAUCONT_GROUPS>? GetCAUCONT_GROUPS()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICAUCONT_GROUPSRepository>().GetList(CompanyID, Data.caucod);
        }

        public override ObservableCollection<PDCGRUPPI>? GetPDCGRUPPI()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPDCGRUPPIRepository>().GetList();
        }

        public override List<PDCCONTI>? GetPDCCONTI()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPDCCONTIRepository>().GetBasicList();
        }

        public override List<PDCSOTTO>? GetPDCSOTTO()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().GetBasicList(CompanyID);
        }

        public override ObservableCollection<CAUCONT>? GetCAUCONT()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICAUCONTRepository>().GetSimpleList();
        }

        public override ObservableCollection<TCECO00F>? GetTCECO00F()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ITCECO00FRepository>().GetList(CompanyID, false);
        }

        public override string? ValidateCAUCONT_GROUPS(CAUCONT_GROUPS Group, bool IsInsert)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICAUCONT_GROUPSRepository>().Validate(Group, IsInsert);
        }

        public override ObservableCollection<CAUCONT>? GetCAUCONTs()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICAUCONTRepository>().GetList();
        }

        public override ObservableCollection<CAUFAT00F>? GetCAUFAT00Fs()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICAUFAT00FRepository>().GetList();
        }

        public override string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICAUCONTRepository>().Validate(Data, IsInsert);
        }

        public override bool Save()
        {
            if (IsInsert)
                return VulpesServiceProvider.Provider.GetRequiredService<ICAUCONTRepository>().Insert(Data, CounterpartsRows, CompanyID);
            else
                return VulpesServiceProvider.Provider.GetRequiredService<ICAUCONTRepository>().Update(Data, CounterpartsRows, CompanyID);
        }
    }

}
