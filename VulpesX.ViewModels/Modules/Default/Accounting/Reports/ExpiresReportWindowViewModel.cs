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
using VulpesX.Models.Models.Reports.Accounting;
using VulpesX.Shared;
using VulpesX.Shared.Generics;

namespace VulpesX.ViewModels.Modules.Default.Accounting.Reports
{
    public class ExpiresReportWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public ExpiresReportWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.UserName;
        }

        private ObservableCollection<ABE>? entities;
        public ObservableCollection<ABE>? Entities { get => entities; set { entities = value; NotifyPropertyChanged("Entities"); } }

        private ABE? selectedEntity { get; set; }
        public ABE? SelectedEntity { get => selectedEntity; set { selectedEntity = value; NotifyPropertyChanged("SelectedEntity"); } }

        public ObservableCollection<GenericIDDescription> EntityTypes => CommonsService.SingleEntityTypes;
        public ObservableCollection<GenericIDDescription> GroupingTypes => new ObservableCollection<GenericIDDescription>() {
            new GenericIDDescription(){ ID = "C", Description = "Cliente/fornitore" },
            new GenericIDDescription(){ ID = "S", Description = "Data scadenza" },
            new GenericIDDescription(){ ID = "P", Description = "Tipo incasso/pagamento" }
        };

        public GenericIDDescription? SelectedGroupingType { get; set; }

        private GenericIDDescription? selectedEntityType;
        public GenericIDDescription? SelectedEntityType
        {
            get => selectedEntityType;
            set
            {
                if (value != null && value.ID != selectedEntityType?.ID)
                    SelectedEntity = null;

                selectedEntityType = value;

                NotifyPropertyChanged("SelectedEntityType");
            }
        }

        private string? selectedEntityPaymentDescription;
        public string? SelectedEntityPaymentDescription { get => selectedEntityPaymentDescription; set { selectedEntityPaymentDescription = value; NotifyPropertyChanged("SelectedEntityPaymentDescription"); } }

        private string? entityDescription;
        public string? EntityDescription { get => entityDescription; set { entityDescription = value; NotifyPropertyChanged("EntityDescription"); } }

        private ObservableCollection<GenericIDDescription>? paymentTypes;
        public ObservableCollection<GenericIDDescription>? PaymentTypes { get => paymentTypes; set { paymentTypes = value; NotifyPropertyChanged("PaymentTypes"); } }
        
        private GenericIDDescription? selectedPaymentType;
        public GenericIDDescription? SelectedPaymentType { get => selectedPaymentType; set { selectedPaymentType = value; NotifyPropertyChanged("SelectedPaymentType"); } }

        public DateTime? ExpireDate { get; set; }
        public bool RunEqualization { get; set; } = true;

        public ObservableCollection<ABE>? GetABE(string EntityType)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().GetLightList(EntityType);
        }

        public ObservableCollection<GenericIDDescription>? GetTIPINC()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ITAB_ACC_TIPINCRepository>().GetGenericList();
        }

        public ExpiresReport? GetSuppliers()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPNFORNITORIRepository>().ExpiresReport(CompanyID, SelectedEntity!, SelectedPaymentType!, ExpireDate, SelectedGroupingType!.ID!);
        }

        public ExpiresReport? GetCustomers()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IPNCLIENTIRepository>().ExpiresReport(CompanyID, SelectedEntity!, SelectedPaymentType!, ExpireDate, SelectedGroupingType!.ID!);
        }

        public bool RunEqualizationStoredProcedure()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IAccountingRepository>().RunEqualization(CompanyID, SelectedEntityType!.ID!, new DateTime(9999, 12, 31));
        }
    }
}
