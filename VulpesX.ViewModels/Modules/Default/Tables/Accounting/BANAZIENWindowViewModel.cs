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
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Models.Default.Partials;
using VulpesX.Services.Tables.Accounting;
using VulpesX.Shared;
using VulpesX.Shared.Generics;

namespace VulpesX.ViewModels.Modules.Default.Tables.Accounting
{
    public abstract class BANAZIENWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public BANAZIENWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
        }

        public required BANAZIEN Data { get; set; }
        public bool IsInsert { get; set; }

        private ObservableCollection<BankItem>? abicabs;
        public ObservableCollection<BankItem>? ABICABs { get => abicabs; set { abicabs = value; NotifyPropertyChanged(); } }

        private BankItem? selectedABICAB;
        public BankItem? SelectedABICAB { get => selectedABICAB; set { selectedABICAB = value; NotifyPropertyChanged(); } }

        public ObservableCollection<ISO>? ISOs { get; set; }
        public ISO? SelectedISO { get; set; }

        public ObservableCollection<CAUCONT>? Causals { get; set; }
        public CAUCONT? SelectedFromEffectsCausal { get; set; }
        public CAUCONT? SelectedToEffectsCausal { get; set; }
        public CAUCONT? SelectedToAnticipationCausal { get; set; }
        public CAUCONT? SelectedFromAnticipationCausal { get; set; }

        public abstract ObservableCollection<BankItem>? GetABICAB();

        public abstract ObservableCollection<PDCGRUPPI>? GetPDCGRUPPI();

        public abstract ObservableCollection<PDCCONTI>? GetPDCCONTI();

        public abstract ObservableCollection<PDCSOTTO>? GetPDCSOTTO();


        public abstract string? Validate();

        public abstract bool Save();
    }

    public class BANAZIENWindowViewModelDeafult : BANAZIENWindowViewModel
    {
        public BANAZIENWindowViewModelDeafult()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            ISOs = VulpesServiceProvider.Provider.GetRequiredService<IISORepository>().GetList();
            Causals = VulpesServiceProvider.Provider.GetRequiredService<ICAUCONTRepository>().GetSimpleList("*");
        }

        public override ObservableCollection<BankItem>? GetABICAB()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IABICABRepository>().GetSimpleList(CompanyID, null);
        }

        public override ObservableCollection<PDCGRUPPI>? GetPDCGRUPPI()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPDCGRUPPIRepository>().GetList();
        }

        public override ObservableCollection<PDCCONTI>? GetPDCCONTI()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPDCCONTIRepository>().GetList();
        }

        public override ObservableCollection<PDCSOTTO>? GetPDCSOTTO()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().GetList(CompanyID);
        }


        public override string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IBANAZIENRepository>().Validate(Data, IsInsert);
        }

        public override bool Save()
        {
            if (IsInsert)
                return VulpesServiceProvider.Provider.GetRequiredService<IBANAZIENRepository>().Insert(Data);
            else
                return VulpesServiceProvider.Provider.GetRequiredService<IBANAZIENRepository>().Update(Data);
        }
    }

    public class BANAZIENWindowViewModelUfp : BANAZIENWindowViewModel
    {
        public BANAZIENWindowViewModelUfp()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            ISOs = VulpesServiceProvider.Provider.GetRequiredService<IISORepository>().GetList();
            Causals = VulpesServiceProvider.Provider.GetRequiredService<ICAUCONTRepository>().GetSimpleList("*");
        }

        public override ObservableCollection<BankItem>? GetABICAB()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IABICABRepository>().GetSimpleList(CompanyID, null);
        }

        public override ObservableCollection<PDCGRUPPI>? GetPDCGRUPPI()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPDCGRUPPIRepository>().GetList();
        }

        public override ObservableCollection<PDCCONTI>? GetPDCCONTI()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPDCCONTIRepository>().GetList();
        }

        public override ObservableCollection<PDCSOTTO>? GetPDCSOTTO()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().GetList(CompanyID);
        }


        public override string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IBANAZIENRepository>().Validate(Data, IsInsert);
        }

        public override bool Save()
        {
            if (IsInsert)
                return VulpesServiceProvider.Provider.GetRequiredService<IBANAZIENRepository>().Insert(Data);
            else
                return VulpesServiceProvider.Provider.GetRequiredService<IBANAZIENRepository>().Update(Data);
        }
    }


}
