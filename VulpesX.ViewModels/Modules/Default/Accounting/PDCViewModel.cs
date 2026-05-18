using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Accounting;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Accounting
{
    public abstract class PDCViewModel : Base
    {
        public abstract required string CompanyID { get; set; }

        public PDCViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
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

        private ObservableCollection<PDCGRUPPI>? items;
        public ObservableCollection<PDCGRUPPI>? Items
        {
            get => items; set
            {
                items = value;
                NotifyPropertyChanged();
            }
        }

        public abstract Task Load();

        public abstract PDCGRUPPI? GetGroup(string GroupID);

        public abstract bool CanDeleteGroup(PDCGRUPPI Group);

        public abstract bool DeleteGroup(PDCGRUPPI Group);

        public abstract bool CanDeleteAccount(PDCCONTI Account);

        public abstract bool DeleteAccount(PDCCONTI Account);

        public abstract bool CanDeleteSubaccount(PDCSOTTO Subaccount);

        public abstract bool DeleteSubaccount(PDCSOTTO Subaccount);

        public abstract bool CanDeleteYear(PDCANNI Year);

        public abstract bool DeleteYear(PDCANNI Year);
    }

    public class PDCViewModelDefault : PDCViewModel
    {
        public override required string CompanyID { get; set; }

        public PDCViewModelDefault()
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
                    return VulpesServiceProvider.Provider.GetRequiredService<IPDCGRUPPIRepository>().GetTreeList(CompanyID);

                });

                Items = result;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public override PDCGRUPPI? GetGroup(string GroupID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPDCGRUPPIRepository>().Get(GroupID);
        }

        public override bool CanDeleteGroup(PDCGRUPPI Group)
        {

            return VulpesServiceProvider.Provider.GetRequiredService<IPDCGRUPPIRepository>().CanDelete(Group);
        }

        public override bool DeleteGroup(PDCGRUPPI Group)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPDCGRUPPIRepository>().Delete(Group);
        }

        public override bool CanDeleteAccount(PDCCONTI Account)
        {

            return VulpesServiceProvider.Provider.GetRequiredService<IPDCCONTIRepository>().CanDelete(Account);
        }

        public override bool DeleteAccount(PDCCONTI Account)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPDCCONTIRepository>().Delete(Account);
        }

        public override bool CanDeleteSubaccount(PDCSOTTO Subaccount)
        {

            return VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().CanDelete(Subaccount);
        }

        public override bool DeleteSubaccount(PDCSOTTO Subaccount)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().Delete(Subaccount);
        }

        public override bool CanDeleteYear(PDCANNI Year)
        {

            return VulpesServiceProvider.Provider.GetRequiredService<IPDCANNIRepository>().CanDelete(Year);
        }

        public override bool DeleteYear(PDCANNI Year)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPDCANNIRepository>().Delete(Year);
        }
    }

    public class PDCViewModelUfp : PDCViewModel
    {
        public override required string CompanyID { get; set; }

        public PDCViewModelUfp()
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
                    return VulpesServiceProvider.Provider.GetRequiredService<IPDCGRUPPIRepository>().GetTreeList(CompanyID);

                });

                Items = result;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public override PDCGRUPPI? GetGroup(string GroupID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPDCGRUPPIRepository>().Get(GroupID);
        }

        public override bool CanDeleteGroup(PDCGRUPPI Group)
        {

            return VulpesServiceProvider.Provider.GetRequiredService<IPDCGRUPPIRepository>().CanDelete(Group);
        }

        public override bool DeleteGroup(PDCGRUPPI Group)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPDCGRUPPIRepository>().Delete(Group);
        }

        public override bool CanDeleteAccount(PDCCONTI Account)
        {

            return VulpesServiceProvider.Provider.GetRequiredService<IPDCCONTIRepository>().CanDelete(Account);
        }

        public override bool DeleteAccount(PDCCONTI Account)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPDCCONTIRepository>().Delete(Account);
        }

        public override bool CanDeleteSubaccount(PDCSOTTO Subaccount)
        {

            return VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().CanDelete(Subaccount);
        }

        public override bool DeleteSubaccount(PDCSOTTO Subaccount)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>().Delete(Subaccount);
        }

        public override bool CanDeleteYear(PDCANNI Year)
        {

            return VulpesServiceProvider.Provider.GetRequiredService<IPDCANNIRepository>().CanDelete(Year);
        }

        public override bool DeleteYear(PDCANNI Year)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPDCANNIRepository>().Delete(Year);
        }
    }

}
