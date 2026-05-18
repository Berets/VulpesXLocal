using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.CRM;
using VulpesX.DAL.General;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;
using VulpesX.Shared.Generics;

namespace VulpesX.ViewModels.Modules.Default.CRM
{
    public class LISCLIViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }
        public required Guid? CompanyUID { get; set; }

        public LISCLIViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            CompanyUID = UserContext.Instance.ACCESS!.SelectedCompany!.SOCUID;
            UserID = UserContext.Instance.ACCESS!.USRID;
        }

        public ObservableCollection<ABE>? Customers { get; set; }

        private ABE? selectedCustomer;
        public ABE? SelectedCustomer { get => selectedCustomer; set { selectedCustomer = value; NotifyPropertyChanged("SelectedCustomer"); } }

        private ObservableCollection<GenericIDDescription>? recipients;
        public ObservableCollection<GenericIDDescription>? Recipients { get => recipients; set { recipients = value; NotifyPropertyChanged("Recipients"); } }

        private GenericIDDescription? selectedRecipient;
        public GenericIDDescription? SelectedRecipient { get => selectedRecipient; set { selectedRecipient = value; NotifyPropertyChanged("SelectedRecipient"); } }

        private ObservableCollection<CRM_LISCLI>? items;
        public ObservableCollection<CRM_LISCLI>? Items { get { return items; } set { items = value; NotifyPropertyChanged("Items"); } }

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { _isBusy = value; NotifyPropertyChanged(); }
        }

        public async Task Load(string StatusID)
        {
            IsBusy = true;

            try
            {
                int recipientID = 0;
                int.TryParse(SelectedRecipient?.ID, out recipientID);

                var result = await Task.Run(() =>
                {
                    return VulpesServiceProvider.Provider.GetRequiredService<ICRM_LISCLIRepository>().GetList(CompanyID, SelectedCustomer?.abecod, recipientID > 0 ? recipientID : null, StatusID);
                });

                Items = result;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public ObservableCollection<GenericIDDescription>? GetDESTINATARIs(int EntityID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IDESTINATARIRepository>().GetSimpleList(EntityID, true);
        }

        public bool Update(CRM_LISCLI Item)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICRM_LISCLIRepository>().Update(Item);
        }

        public bool CopyFrom(int Source, int Target)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICRM_LISCLIRepository>().CopyFrom(CompanyID, Source, Target, UserID);
        }
    }
}
