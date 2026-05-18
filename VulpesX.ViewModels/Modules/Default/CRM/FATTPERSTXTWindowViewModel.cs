using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.CRM;
using VulpesX.DAL.General;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.CRM
{
    public class FATTPERSTXTWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public FATTPERSTXTWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.ACCESS!.USRID;
        }


        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { _isBusy = value; NotifyPropertyChanged(); }
        }

        private FATTPERSTXT? selectedType;
        public FATTPERSTXT? SelectedType
        {
            get { return selectedType; }
            set
            {
                selectedType = value;
                NotifyPropertyChanged("SelectedType");
            }
        }

        public ObservableCollection<FATTPERSTXT>? Types { get; set; }

        private ABE? selectedCustomer;
        public ABE? SelectedCustomer
        {
            get { return selectedCustomer; }
            set
            {
                selectedCustomer = value;
                NotifyPropertyChanged("SelectedCustomer");
            }
        }

        private ObservableCollection<ABE>? customers;
        public ObservableCollection<ABE>? Customers
        {
            get { return customers; }
            set
            {
                customers = value;
                NotifyPropertyChanged("Customers");
            }
        }

        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public string? Path { get; set; }

        public void LoadDetails()
        {
            Types = VulpesServiceProvider.Provider.GetRequiredService<IFATTPERSTXTRepository>().GetList(CompanyID);
        }

        public ObservableCollection<ABE>? GetCustomersLightListActiveForExternals()
        {
            if (SelectedType != null)
                return VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().GetCustomersLightListActiveForExternals(SelectedType.txtdes ?? string.Empty);

            return null;
        }

        public async Task<bool> Generate()
        {
            IsBusy = true;

            if (SelectedType!.txtdes == "FILCONAD")
            {
                var result = await Task.Run(() =>
                {
                    return VulpesServiceProvider.Provider.GetRequiredService<IFATTPERSTXTRepository>().GenerateFILCONAD(CompanyID, SelectedCustomer!.abecod, From!.Value, To!.Value, Path!);
                });

                return result;
            }

            else
            {
                IsBusy = false;
                return false;
            }
        }
    }
}
