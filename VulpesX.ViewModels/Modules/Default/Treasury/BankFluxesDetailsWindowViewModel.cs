using DocumentFormat.OpenXml.Wordprocessing;
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
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Treasury
{
    public class BankFluxesDetailsWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public int Year { get; set; }
        public string? MonthFlag { get; set; }
        public required string GroupID;
        public required string AccountID;
        public required string SubaccountID;
        public required string Bank;
        public required string Agency;

        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public decimal StartBalance { get; set; }
        public string? StartBalanceSign { get; set; }

        public BankFluxesDetailsWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;
        }



        private ObservableCollection<PNRIGHE>? items;
        public ObservableCollection<PNRIGHE>? Items
        {
            get => items;
            set
            {
                items = value;
                
                NotifyPropertyChanged("Items");
            }
        }



        public string BankFullText => $"{Bank.Trim()} - {Agency.Trim()} ({GroupID}/{AccountID}/{SubaccountID})";

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

        public async Task Load()
        {
            IsBusy = true;

            await Task.Run(() =>
                       Items = VulpesServiceProvider.Provider.GetRequiredService<IAccountingRepository>().GetHeadTemporaryPeriodList(CompanyID, Year, MonthFlag, From, To, GroupID, AccountID, SubaccountID));

            IsBusy = false;

        }

        public bool RemoveTemporaryFlag(PNRIGHE Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPNRIGHERepository>().RemoveTemporaryFlag(Item);
        }

        public bool ChangeBankFlag(PNRIGHE Item, bool FlagValue)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPNRIGHERepository>().ChangeBankFlag(Item.N1SOCI, Item.N1ANNO, Item.N1REGI, Item.N1RIGA, FlagValue);
        }

        public PNTESTATA? Get(int Year, int ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPNTESTATARepository>().Get(CompanyID,Year,ID);
        }

        public bool PrintedOnGeneralJournal(int Year, int ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPNRIGHERepository>().PrintedOnGeneralJournal(CompanyID, Year, ID);
        }

        public ObservableCollection<CAUCONT>? GetCAUCONTs()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICAUCONTRepository>().GetSimpleList();
        }

        public ObservableCollection<ABE>? GetABEs(string EntityType)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().GetLightList(EntityType);
        }
    }
}
