using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.General;
using VulpesX.DAL.Production;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.Production
{
    public class ProductionOrderWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public ProductionOrderWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.ACCESS!.USRID;
        }

        public required pro_ordine Data { get; set; }
        public bool IsInsert { get; set; }
        public bool IsReadonly => true;

        public ABE? SelectedCustomer { get; set; }
        public ObservableCollection<ABE>? Customers { get; set; }

        public tab_articolo? SelectedProduct { get; set; }
        public ObservableCollection<tab_articolo>? Products { get; set; }

        private tab_articolo? selectedRevision;
        public tab_articolo? SelectedRevision
        {
            get { return selectedRevision; }
            set
            {
                selectedRevision = value;
                NotifyPropertyChanged("SelectedRevision");
            }
        }

        private ObservableCollection<tab_articolo>? revisions;
        public ObservableCollection<tab_articolo>? Revisions
        {
            get { return revisions; }
            set
            {
                revisions = value;
                NotifyPropertyChanged("Revisions");
            }
        }

        public void LoadDetails()
        {
            Customers = VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().GetCustomersLightListActive("C");
            Products = VulpesServiceProvider.Provider.GetRequiredService<Itab_articoloRepository>().GetSimpleList(CompanyID);
        }

        public ObservableCollection<tab_articolo>? GetRevisioni(string ProductID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Itab_articolo_composizioneRepository>().GetRevisioni(CompanyID, ProductID);
        }

        public string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<Ipro_ordineRepository>().Validate(Data, IsInsert);
        }

        public bool Save()
        {
            if (IsInsert)
            {
                Data.LogAddedUserID = UserID;
                return VulpesServiceProvider.Provider.GetRequiredService<Ipro_ordineRepository>().Insert(Data);
            }
            else
            {
                Data.LogUpdatedUserID = UserID;
                return VulpesServiceProvider.Provider.GetRequiredService<Ipro_ordineRepository>().Update(Data);
            }
        }
    }
}
