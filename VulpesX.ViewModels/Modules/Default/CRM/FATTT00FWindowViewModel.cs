using DocumentFormat.OpenXml.EMMA;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.CRM;
using VulpesX.DAL.General;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.DAL.Tables.CRM;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Models.Default.Partials;
using VulpesX.Services.Tables.Accounting;
using VulpesX.Shared;

namespace VulpesX.ViewModels.Modules.Default.CRM
{
    public class FATTT00FWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required Guid? CompanyUID { get; set; }
        public required string UserID { get; set; }

        public FATTT00FWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            CompanyUID = UserContext.Instance.ACCESS!.SelectedCompany!.SOCUID;
            UserID = UserContext.Instance.ACCESS!.USRID;
        }

        public required  FATTT00F Data { get; set; }
        public bool IsInsert { get; set; }
        public bool IsReadonly => !string.IsNullOrWhiteSpace(Data.FTNUMFEL);
        public bool IsEnabled => !IsReadonly;

        public ABE? SelectedCustomer { get; set; }
        public ObservableCollection<ABE>? Customers { get; set; }
        public DESTINATARI? SelectedReference { get; set; }

        private ObservableCollection<DESTINATARI>? references;
        public ObservableCollection<DESTINATARI>? References
        {
            get { return references; }
            set
            {
                references = value;
                NotifyPropertyChanged("References");
            }
        }

        private CAUFAT00F? selectedCausal;
        public CAUFAT00F? SelectedCausal
        {
            get { return selectedCausal; }
            set
            {
                selectedCausal = value;
                NotifyPropertyChanged("SelectedCausal");
            }
        }

        public ObservableCollection<CAUFAT00F>? Causals { get; set; }

        private PAGCLI? selectedPayment;
        public PAGCLI? SelectedPayment
        {
            get { return selectedPayment; }
            set
            {
                selectedPayment = value;
                NotifyPropertyChanged("SelectedPayment");
            }
        }

        public ObservableCollection<PAGCLI>? Payments { get; set; }

        private FE_TIPODOC? selectedFEDocType;
        public FE_TIPODOC? SelectedFEDocType
        {
            get
            { return selectedFEDocType; }
            set
            {
                selectedFEDocType = value;
                NotifyPropertyChanged("SelectedFEDocType");
            }
        }

        public ObservableCollection<FE_TIPODOC>? FEDocTypes { get; set; }
        private BankItem? selectedCustomerBank;
        public BankItem? SelectedCustomerBank
        {
            get { return selectedCustomerBank; }
            set
            {
                selectedCustomerBank = value;
                NotifyPropertyChanged("SelectedCustomerBank");
            }
        }

        private ObservableCollection<BankItem>? banks;
        public ObservableCollection<BankItem>? Banks
        {
            get { return banks; }
            set
            {
                banks = value;
                NotifyPropertyChanged("Banks");
            }
        }

        private CONSEGNA? selectedDelivery;
        public CONSEGNA? SelectedDelivery
        {
            get { return selectedDelivery; }
            set
            {
                selectedDelivery = value;
                NotifyPropertyChanged("SelectedDelivery");
            }
        }

        public ObservableCollection<CONSEGNA>? Deliveries { get; set; }

        private SPEDIZIONE? selectedShipment;
        public SPEDIZIONE? SelectedShipment
        {
            get { return selectedShipment; }
            set
            {
                selectedShipment = value;
                NotifyPropertyChanged("SelectedShipment");
            }
        }

        public ObservableCollection<SPEDIZIONE>? Shipments { get; set; }
    
        private IMBALLI? selectedPacking;
        public IMBALLI? SelectedPacking { get => selectedPacking; set { selectedPacking = value; NotifyPropertyChanged("SelectedPacking"); } }
        public ObservableCollection<IMBALLI>? Packings { get; set; }

        private VETTORI? selectedCarrier;
        public VETTORI? SelectedCarrier
        {
            get { return selectedCarrier; }
            set
            {
                selectedCarrier = value;
                NotifyPropertyChanged("SelectedCarrier");
            }
        }
        public ObservableCollection<VETTORI>? Carriers { get; set; }

        public ObservableCollection<TCECO00F>? CostCenters { get; set; }

        private TCECO00F? selectedCostCenter;
        public TCECO00F? SelectedCostCenter
        {
            get { return selectedCostCenter; }
            set
            {
                selectedCostCenter = value;
                NotifyPropertyChanged("SelectedCostCenter");
            }
        }

        public void LoadDetails()
        {
            Customers = VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().GetCustomersLightListActive("C");
            Causals = VulpesServiceProvider.Provider.GetRequiredService<ICAUFAT00FRepository>().GetList();
            Payments = VulpesServiceProvider.Provider.GetRequiredService<IPAGCLIRepository>().GetList();
            FEDocTypes = VulpesServiceProvider.Provider.GetRequiredService<IFE_TIPODOCRepository>().GetList();
            Deliveries = VulpesServiceProvider.Provider.GetRequiredService<ICONSEGNARepository>().GetList();
            Shipments = VulpesServiceProvider.Provider.GetRequiredService<ISPEDIZIONERepository>().GetList();
            Packings = VulpesServiceProvider.Provider.GetRequiredService<IIMBALLIRepository>().GetList();
            Carriers = VulpesServiceProvider.Provider.GetRequiredService<IVETTORIRepository>().GetList();
            CostCenters = VulpesServiceProvider.Provider.GetRequiredService<ITCECO00FRepository>().GetList(CompanyID, false);
        }

        public ObservableCollection<BankItem>? GetABICABs(string? PaymentID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IABICABRepository>().GetSimpleList(CompanyID, PaymentID);
        }

        public ObservableCollection<DESTINATARI>? GetDESTINATARIs(int EntityID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IDESTINATARIRepository>().GetList(EntityID);
        }

        public CLIAMMI? GetCLIAMMI(int EntityID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICLIAMMIRepository>().Get(CompanyID,EntityID);
        }

        public CLIENTI? GetCLIENTI(int EntityID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICLIENTIRepository>().Get(EntityID);
        }

        public string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IFATTT00FRepository>().Validate(Data, IsInsert);
        }

        public bool Save()
        {
            if (IsInsert)
            {
                var numRegRepo = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>();

                Data.FTNUOR = numRegRepo.GetNumber(CompanyID, Data.FTANNO, Constants.INVOICE_TEMP, true);
                Data.addedUserID = UserID;

                return VulpesServiceProvider.Provider.GetRequiredService<IFATTT00FRepository>().Insert(Data);
            }
            else
            {
                Data.updatedUserID = UserID;

                return VulpesServiceProvider.Provider.GetRequiredService<IFATTT00FRepository>().Update(Data);
            }
        }
    }
}
