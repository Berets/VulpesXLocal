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
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Models.Default.Partials;
using VulpesX.Shared;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VulpesX.ViewModels.Modules.Default.Accounting
{
    public abstract class PortafoglioViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public PortafoglioViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;
        }


        private DateTime _from;
        public DateTime From
        {
            get { return _from; }
            set
            {
                _from = value;
                NotifyPropertyChanged();
            }
        }

        private DateTime _to;
        public DateTime To
        {
            get { return _to; }
            set
            {
                _to = value;
                NotifyPropertyChanged();
            }
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

        private bool _isBusyHistory;
        public bool IsBusyHistory
        {
            get { return _isBusyHistory; }
            set
            {
                _isBusyHistory = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<PNPORTAFOGLIO>? items;
        public ObservableCollection<PNPORTAFOGLIO>? Items
        {
            get { return items; }
            set
            {
                items = value;
                NotifyPropertyChanged("Items"); NotifyPropertyChanged("TotalAmount");
            }
        }

        private ObservableCollection<PNPORTAFOGLIO_DIST>? itemsDist;
        public ObservableCollection<PNPORTAFOGLIO_DIST>? ItemsDist
        {
            get { return itemsDist; }
            set
            {
                itemsDist = value;
                NotifyPropertyChanged("ItemsDist");
            }
        }

        private ObservableCollection<BankItem>? banksCache;
        public ObservableCollection<BankItem>? BanksCache
        {
            get { return banksCache; }
            set
            {
                banksCache = value;
                NotifyPropertyChanged("BanksCache");
            }
        }

        public decimal TotalAmount => items != null ? items.Sum(sum => sum.N6IMEU) ?? 0 : 0;

        private decimal totalAmountSelected;
        public decimal TotalAmountSelected
        {
            get => totalAmountSelected;
            set { totalAmountSelected = value; NotifyPropertyChanged("TotalAmountSelected"); }
        }

        public abstract Task Load(int Year, string StatusID);

        public abstract Task Load(string StatusID);

        public abstract Task LoadHistory(int Year);

        public abstract ObservableCollection<ESERCIZIO>? GetESERCIZIOs();

        public abstract PNPORTAFOGLIO_DIST? GetFull(long ID);

        public abstract bool Update(PNPORTAFOGLIO Item);

        public abstract bool Duplicate(PNPORTAFOGLIO Item);

        public abstract bool Delete(PNPORTAFOGLIO Item);

        public abstract bool DeleteAll(PNPORTAFOGLIO_DIST Item);

        public abstract bool GenerateFileCBI(PNPORTAFOGLIO_DIST Item, string FilePath, bool IsFileOpen);

        public abstract bool Accounting(PNPORTAFOGLIO_DIST Item, DateTime Date, ESERCIZIO Year);

        public abstract long GetDistinctLongID(int Year);

        public abstract int GetDistinctIntID(int Year);
    }

    public class PortafoglioViewModelDefault : PortafoglioViewModel
    {
        public PortafoglioViewModelDefault()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;
        }

        public override async Task Load(int Year, string StatusID)
        {
            IsBusy = true;

            await Task.Run(() =>
            {
                Items = VulpesServiceProvider.Provider.GetRequiredService<IPNPORTAFOGLIORepository>().GetList(CompanyID, Year, StatusID);
                BanksCache = VulpesServiceProvider.Provider.GetRequiredService<IABICABRepository>().GetSimpleList(CompanyID, null);
            });

            IsBusy = false;

        }

        public override async Task Load(string StatusID)
        {
            IsBusy = true;

            await Task.Run(() =>
            {
                Items = VulpesServiceProvider.Provider.GetRequiredService<IPNPORTAFOGLIORepository>().GetList(CompanyID, From, To, StatusID);
                BanksCache = VulpesServiceProvider.Provider.GetRequiredService<IABICABRepository>().GetSimpleList(CompanyID, null);
            });

            IsBusy = false;

        }

        public override async Task LoadHistory(int Year)
        {
            IsBusyHistory = true;

            await Task.Run(() =>
            {
                ItemsDist = VulpesServiceProvider.Provider.GetRequiredService<IPNPORTAFOGLIO_DISTRepository>().GetList(CompanyID, Year);
            });

            IsBusyHistory = false;

        }

        public override ObservableCollection<ESERCIZIO>? GetESERCIZIOs()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IESERCIZIORepository>().GetListOpen(CompanyID);
        }

        public override PNPORTAFOGLIO_DIST? GetFull(long ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPNPORTAFOGLIO_DISTRepository>().GetFull(CompanyID, ID);
        }

        public override bool Update(PNPORTAFOGLIO Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPNPORTAFOGLIORepository>().Update(Item);
        }

        public override bool Delete(PNPORTAFOGLIO Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPNPORTAFOGLIORepository>().Delete(Item);
        }

        public override bool Duplicate(PNPORTAFOGLIO Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPNPORTAFOGLIORepository>().Duplicate(Item);
        }

        public override bool DeleteAll(PNPORTAFOGLIO_DIST Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPNPORTAFOGLIO_DISTRepository>().DeleteAll(Item);
        }

        public override bool GenerateFileCBI(PNPORTAFOGLIO_DIST Item, string FilePath, bool IsFileOpen)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPNPORTAFOGLIO_DISTRepository>().GenerateFileCBI(Item, UserContext.Instance.ACCESS!.SelectedCompany!.SOMDES, FilePath, IsFileOpen);
        }

        public override bool Accounting(PNPORTAFOGLIO_DIST Item, DateTime Date, ESERCIZIO Year)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPNPORTAFOGLIO_DISTRepository>().Accounting(Item, Date, Year, UserID);
        }

        public override long GetDistinctLongID(int Year)
        {
            var numregRepo = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>();
            return numregRepo.GetFullLongID(Year, numregRepo.GetNumber(CompanyID, Year, Constants.WALLET_DIST, true));
        }

        public override int GetDistinctIntID(int Year)
        {
            throw new NotImplementedException();
        }
    }

    public class PortafoglioViewModelUfp : PortafoglioViewModel
    {
        public PortafoglioViewModelUfp()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;
        }

        public override async Task Load(int Year, string StatusID)
        {
            IsBusy = true;

            await Task.Run(() =>
            {
                Items = VulpesServiceProvider.Provider.GetRequiredService<IPNPORTAFOGLIORepository>().GetList(CompanyID, Year, StatusID);
                BanksCache = VulpesServiceProvider.Provider.GetRequiredService<IABICABRepository>().GetSimpleList(CompanyID, null);
            });

            IsBusy = false;

        }

        public override async Task Load(string StatusID)
        {
            IsBusy = true;

            await Task.Run(() =>
            {
                Items = VulpesServiceProvider.Provider.GetRequiredService<IPNPORTAFOGLIORepository>().GetList(CompanyID, From, To, StatusID);
                BanksCache = VulpesServiceProvider.Provider.GetRequiredService<IABICABRepository>().GetSimpleList(CompanyID, null);
            });

            IsBusy = false;

        }

        public override async Task LoadHistory(int Year)
        {
            IsBusyHistory = true;

            await Task.Run(() =>
                       ItemsDist = VulpesServiceProvider.Provider.GetRequiredService<IPNPORTAFOGLIO_DISTRepository>().GetList(CompanyID, Year));

            IsBusyHistory = false;

        }

        public override ObservableCollection<ESERCIZIO>? GetESERCIZIOs()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IESERCIZIORepository>().GetListOpen(CompanyID);
        }

        public override PNPORTAFOGLIO_DIST? GetFull(long ID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPNPORTAFOGLIO_DISTRepository>().GetFull(CompanyID, ID);
        }

        public override bool Update(PNPORTAFOGLIO Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPNPORTAFOGLIORepository>().Update(Item);
        }

        public override bool Delete(PNPORTAFOGLIO Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPNPORTAFOGLIORepository>().Delete(Item);
        }

        public override bool Duplicate(PNPORTAFOGLIO Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPNPORTAFOGLIORepository>().Duplicate(Item);
        }

        public override bool DeleteAll(PNPORTAFOGLIO_DIST Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPNPORTAFOGLIO_DISTRepository>().DeleteAll(Item);
        }


        public override bool GenerateFileCBI(PNPORTAFOGLIO_DIST Item, string FilePath, bool IsFileOpen)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPNPORTAFOGLIO_DISTRepository>().GenerateFileCBI(Item, UserContext.Instance.ACCESS!.SelectedCompany!.SOMDES, FilePath, IsFileOpen);
        }

        public override bool Accounting(PNPORTAFOGLIO_DIST Item, DateTime Date, ESERCIZIO Year)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPNPORTAFOGLIO_DISTRepository>().Accounting(Item, Date, Year, UserID);
        }

        public override long GetDistinctLongID(int Year)
        {
            throw new NotImplementedException();
        }

        public override int GetDistinctIntID(int Year)
        {
            var numregRepo = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>();
            return numregRepo.GetFullIntID(Year, numregRepo.GetNumber(CompanyID, Year, Constants.WALLET_DIST, true));
        }
    }

}
