using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Accounting;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Models.Default.Partials;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Accounting
{
    public abstract class PortafoglioDistWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public PortafoglioDistWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;
        }

        public required PNPORTAFOGLIO_DIST Data { get; set; }
        public bool IsInsert { get; set; }

        public bool IsReadonly => Data.accounting_date.HasValue;
        public bool IsEnabled => !IsReadonly;

        private ObservableCollection<BankItem>? internalBanks;
        public ObservableCollection<BankItem>? InternalBanks
        {
            get { return internalBanks; }
            set
            {
                internalBanks = value;
                NotifyPropertyChanged("InternalBanks");
            }
        }

        private BankItem? selectedInternalBank;
        public BankItem? SelectedInternalBank
        {
            get { return selectedInternalBank; }
            set
            {
                selectedInternalBank = value;
                NotifyPropertyChanged("SelectedInternalBank");
            }
        }

        public abstract string? Validate();

        public abstract bool Save();

        public abstract ObservableCollection<BankItem>? GetABICABs();
    }

    public class PortafoglioDistWindowViewModelDefault : PortafoglioDistWindowViewModel
    {
        public PortafoglioDistWindowViewModelDefault()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;
        }

        public override string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPNPORTAFOGLIO_DISTRepository>().Validate(Data, IsInsert);
        }

        public override bool Save()
        {
            if (IsInsert)
            {
                Data.addedUserID = UserID;
                return VulpesServiceProvider.Provider.GetRequiredService<IPNPORTAFOGLIO_DISTRepository>().InsertAll(Data);

            }
            else
            {
                Data.updatedUserID = UserID;
                return VulpesServiceProvider.Provider.GetRequiredService<IPNPORTAFOGLIO_DISTRepository>().UpdateAll(Data);
            }
        }

        public override ObservableCollection<BankItem>? GetABICABs()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IABICABRepository>().GetSimpleList(CompanyID, "N");
        }
    }

    public class PortafoglioDistWindowViewModelUfp : PortafoglioDistWindowViewModel
    {
        public PortafoglioDistWindowViewModelUfp()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;
        }

        public override string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPNPORTAFOGLIO_DISTRepository>().Validate(Data, IsInsert);
        }

        public override bool Save()
        {
            if (IsInsert)
            {
                Data.addedUserID = UserID;
                return VulpesServiceProvider.Provider.GetRequiredService<IPNPORTAFOGLIO_DISTRepository>().InsertAll(Data);

            }
            else
            {
                Data.updatedUserID = UserID;
                return VulpesServiceProvider.Provider.GetRequiredService<IPNPORTAFOGLIO_DISTRepository>().UpdateAll(Data);
            }
        }

        public override ObservableCollection<BankItem>? GetABICABs()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IABICABRepository>().GetSimpleList(CompanyID, "N");
        }
    }
}
