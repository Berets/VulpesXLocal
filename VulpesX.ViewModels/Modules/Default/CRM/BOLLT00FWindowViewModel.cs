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
using VulpesX.DAL.Shipping;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.DAL.Tables.General;
using VulpesX.DAL.Tables.Shipping;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Models.Default.Partials;
using VulpesX.Services.Tables.General;
using VulpesX.Shared;
using VulpesX.Shared.Generics;

namespace VulpesX.ViewModels.Modules.Default.CRM
{
    public class BOLLT00FWindowViewModel : Base
    {
        public required string CompanyID { get; set; }
        public required string UserID { get; set; }

        public BOLLT00FWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
            UserID = UserContext.Instance.ACCESS!.USRID;
        }

        public required BOLLT00F Data { get; set; }
        public bool IsInsert { get; set; }
        public bool IsReadonly { get; set; }
        public bool IsEnabled => !IsReadonly;
        public string EntityDescription => Data.BTFLCF == "C" ? "Cliente" : "Fornitore";

        public ABE? SelectedCustomer { get; set; }
        public ObservableCollection<ABE>? Customers { get; set; }
        public GenericIntIDDescription? SelectedReference { get; set; }

        private ObservableCollection<GenericIntIDDescription>? references;
        public ObservableCollection<GenericIntIDDescription>? References
        {
            get { return references; }
            set
            {
                references = value;
                NotifyPropertyChanged("References");
            }
        }

        private CAUSBOLL? selectedCausal;
        public CAUSBOLL? SelectedCausal
        {
            get { return selectedCausal; }
            set
            {
                selectedCausal = value;
                NotifyPropertyChanged("SelectedCausal");
            }
        }
        public ObservableCollection<CAUSBOLL>? Causals { get; set; }
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
        public LINGUA? SelectedLanguage { get; set; }
        public ObservableCollection<LINGUA>? Languages { get; set; }
        private VETTORI? selectedFirstCarrier;
        public VETTORI? SelectedFirstCarrier
        {
            get { return selectedFirstCarrier; }
            set
            {
                selectedFirstCarrier = value;
                NotifyPropertyChanged("SelectedFirstCarrier");
            }
        }
        private VETTORI? selectedSecondCarrier;
        public VETTORI? SelectedSecondCarrier
        {
            get { return selectedSecondCarrier; }
            set
            {
                selectedSecondCarrier = value;
                NotifyPropertyChanged("SelectedSecondCarrier");
            }
        }
        public ObservableCollection<VETTORI>? Carriers { get; set; }
        private CLIENTI? selectedCustomerContacts;
        public CLIENTI? SelectedCustomerContacts
        {
            get => selectedCustomerContacts;
            set { selectedCustomerContacts = value; NotifyPropertyChanged("SelectedCustomercontacts"); }
        }
        public ObservableCollection<AREE>? Areas { get; set; }
        private AREE? selectedArea;
        public AREE? SelectedArea { get => selectedArea; set { selectedArea = value; NotifyPropertyChanged("SelectedArea"); } }
        public ObservableCollection<REGIONI>? Regions { get; set; }
        private REGIONI? selectedRegion;
        public REGIONI? SelectedRegion { get => selectedRegion; set { selectedRegion = value; NotifyPropertyChanged("SelectedRegion"); } }
        public ObservableCollection<ZONE>? Zones { get; set; }
        private ZONE? selectedZone;
        public ZONE? SelectedZone { get => selectedZone; set { selectedZone = value; NotifyPropertyChanged("SelectedZone"); } }
        public ObservableCollection<MERCEOLOGICO>? Sectors { get; set; }
        private MERCEOLOGICO? selectedSector;
        public MERCEOLOGICO? SelectedSector { get => selectedSector; set { selectedSector = value; NotifyPropertyChanged("SelectedSector"); } }
        public ObservableCollection<FILIALI>? Branches { get; set; }
        private FILIALI? selectedBranch;
        public FILIALI? SelectedBranch { get => selectedBranch; set { selectedBranch = value; NotifyPropertyChanged("SelectedBranch"); } }
        public ObservableCollection<RIVENDITORI>? Dealers { get; set; }
        private RIVENDITORI? selectedDealer;
        public RIVENDITORI? SelectedDealer { get => selectedDealer; set { selectedDealer = value; NotifyPropertyChanged("SelectedDealer"); } }
        public ObservableCollection<TAB_GEN_TEXTS>? Texts { get; set; }
        private TAB_GEN_TEXTS? selectedHeadText;
        public TAB_GEN_TEXTS? SelectedHeadText { get => selectedHeadText; set { selectedHeadText = value; NotifyPropertyChanged("SelectedHeadText"); } }
        private TAB_GEN_TEXTS? selectedFootText;
        public TAB_GEN_TEXTS? SelectedFootText { get => selectedFootText; set { selectedFootText = value; NotifyPropertyChanged("SelectedFootText"); } }

        public void LoadDetails()
        {
            Payments = VulpesServiceProvider.Provider.GetRequiredService<IPAGCLIRepository>().GetList();
            Deliveries = VulpesServiceProvider.Provider.GetRequiredService<ICONSEGNARepository>().GetList();
            Shipments = VulpesServiceProvider.Provider.GetRequiredService<ISPEDIZIONERepository>().GetList();
            Packings = VulpesServiceProvider.Provider.GetRequiredService<IIMBALLIRepository>().GetList();
            Carriers = VulpesServiceProvider.Provider.GetRequiredService<IVETTORIRepository>().GetList();
            Languages = VulpesServiceProvider.Provider.GetRequiredService<ILINGUARepository>().GetList();
            Areas = VulpesServiceProvider.Provider.GetRequiredService<IAREERepository>().GetList();
            Regions = VulpesServiceProvider.Provider.GetRequiredService<IREGIONIRepository>().GetList();
            Zones = VulpesServiceProvider.Provider.GetRequiredService<IZONERepository>().GetList();
            Sectors = VulpesServiceProvider.Provider.GetRequiredService<IMERCEOLOGICORepository>().GetList();
            Dealers = VulpesServiceProvider.Provider.GetRequiredService<IRIVENDITORIRepository>().GetList();

            Customers = VulpesServiceProvider.Provider.GetRequiredService<IABERepository>().GetLightList(Data.BTFLCF!);
            Causals = VulpesServiceProvider.Provider.GetRequiredService<ICAUSBOLLRepository>().GetList(Data.BTFLCF!);
            Texts = VulpesServiceProvider.Provider.GetRequiredService<ITAB_GEN_TEXTSRepository>().GetList(CompanyID, TAB_GEN_TEXTS.DDT);
            Branches = VulpesServiceProvider.Provider.GetRequiredService<IFILIALIRepository>().GetList(CompanyID);
        }

        public AZIENDA? GetAZIENDA()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Get(CompanyID);
        }

        public ObservableCollection<BankItem>? GetABICABs(string? PaymentID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IABICABRepository>().GetSimpleList(CompanyID, PaymentID);
        }

        public DESTINATARI? GetDESTINATARI(int EntityID, int Sequence)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IDESTINATARIRepository>().Get(EntityID, Sequence);
        }

        public ObservableCollection<GenericIntIDDescription>? GetDESTINATARIs(int EntityID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IDESTINATARIRepository>().GetSimpleIntList(EntityID, false);
        }

        public ObservableCollection<GenericIntIDDescription>? GetDESFORs(int EntityID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IDESFORRepository>().GetSimpleIntList(EntityID, false);
        }

        public CLIAMMI? GetCLIAMMI(int EntityID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICLIAMMIRepository>().Get(CompanyID, EntityID);
        }

        public CLIENTI? GetCLIENTI(int EntityID)
        {
            return VulpesServiceProvider.Provider.GetRequiredService<ICLIENTIRepository>().Get(EntityID);
        }

        public string? Validate()
        {
            return VulpesServiceProvider.Provider.GetRequiredService<IBOLLT00FRepository>().Validate(Data, IsInsert);
        }

        public bool Save(BOLLT00F_history? History)
        {
            if (IsInsert)
            {
                var numRegRepo = VulpesServiceProvider.Provider.GetRequiredService<INUMREGRepository>();

                Data.BTBOLL = numRegRepo.GetNumber(CompanyID, Data.BTANNO, Constants.DDT_TEMP, true);
                Data.addedUserID = UserID;

                return VulpesServiceProvider.Provider.GetRequiredService<IBOLLT00FRepository>().Insert(Data);
            }
            else
            {
                Data.updatedUserID = UserID;

                if (Data.BTNUBD > 0 && History != null)
                {
                    // store previous revision
                    VulpesServiceProvider.Provider.GetRequiredService<IBOLLT00F_historyRepository>().Insert(History);
                }

                return VulpesServiceProvider.Provider.GetRequiredService<IBOLLT00FRepository>().Update(Data);
            }
        }
    }
}
