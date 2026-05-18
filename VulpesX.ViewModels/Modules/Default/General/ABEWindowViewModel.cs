using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VulpesX.DAL.Accounting;
using VulpesX.DAL.CRM;
using VulpesX.DAL.General;
using VulpesX.DAL.SRM;
using VulpesX.DAL.Store;
using VulpesX.DAL.Tables.Accounting;
using VulpesX.Models;
using VulpesX.Models.Default;
using VulpesX.Models.Default.Partials;
using VulpesX.Models.Ufp;
using VulpesX.Shared;
using VulpesX.Shared.Generics;

namespace VulpesX.ViewModels.Modules.Default.General
{
    public abstract class ABEWindowViewModel : Base
    {
        public required string CompanyID { get; set; }

        public ABEWindowViewModel()
        {
            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;
        }

        public required ABE Data { get; set; }
        public bool IsInsert { get; set; }
        public int AssignedCustomerID { get; set; }


        public AZIENDA? AZIENDA { get; set; }
        public FORNAMMI? SupplierData { get; set; }
        public CLIAMMI? CustomerData { get; set; }
        public FORNITORI? SupplierCommercialData { get; set; }
        public CLIENTI? CustomerCommercialData { get; set; }

        private ObservableCollection<DESTINATARI>? recipients;
        public ObservableCollection<DESTINATARI>? Recipients
        {
            get { return recipients; }
            set
            {
                recipients = value;
                foreach (var item in recipients ?? new ObservableCollection<DESTINATARI>())
                {
                    item.Cities = Data.Cities;
                    item.States = Data.States;
                    item.AgentsList = Agents;
                    item.Isos = Data.ISOList;
                    item.Stores = Stores;
                    item.StoreCausals = StoreCausals;
                }
            }
        }

        private ObservableCollection<DESFOR>? supplierRecipients;
        public ObservableCollection<DESFOR>? SupplierRecipients
        {
            get { return supplierRecipients; }
            set
            {
                supplierRecipients = value;
                foreach (var item in supplierRecipients ?? new ObservableCollection<DESFOR>())
                {
                    item.Cities = Data.Cities;
                    item.States = Data.States;
                }
            }
        }

        public ObservableCollection<NOTECLI1>? CustomerNotes { get; set; }
        public ObservableCollection<NOTEFOR>? SupplierNotes { get; set; }
        public ObservableCollection<RFFTB00F>? SupplierReferences { get; set; }
        public ObservableCollection<ANDEFRES>? CustomerReferences { get; set; }

        private ObservableCollection<SUPPLIER_GROUPS>? counterpartsRows;
        public ObservableCollection<SUPPLIER_GROUPS>? CounterpartsRows
        {
            get { return counterpartsRows; }
            set
            {
                counterpartsRows = value;
                foreach (var item in counterpartsRows ?? new ObservableCollection<SUPPLIER_GROUPS>())
                {
                    item.AccountCache = AccountCache;
                    item.SubaccountCache = SubaccountCache;
                    item.GroupsList = GroupsList;
                    item.CausalsList = CausalsList;
                }
            }
        }

        private ObservableCollection<CUSTOMER_GROUPS>? counterpartsRowsCustomer;
        public ObservableCollection<CUSTOMER_GROUPS>? CounterpartsRowsCustomer
        {
            get { return counterpartsRowsCustomer; }
            set
            {
                counterpartsRowsCustomer = value;
                foreach (var item in counterpartsRowsCustomer ?? new ObservableCollection<CUSTOMER_GROUPS>())
                {
                    item.AccountCache = AccountCache;
                    item.SubaccountCache = SubaccountCache;
                    item.GroupsList = GroupsList;
                    item.CausalsList = CausalsList;
                }
            }
        }

    
        public ObservableCollection<GenericIDDescription>? NoteTypes { get; set; }
        public ObservableCollection<GenericIDDescription>? EntityTypes { get; set; }
        public ObservableCollection<GenericIDDescription>? EntityUseTypes { get; set; }
        public ObservableCollection<GenericIDDescription>? Sexes { get; set; }
        public ObservableCollection<GenericIDDescription>? SupplierSubjectTypes { get; set; }
        public List<PDCCONTI>? AccountCache { get; set; }
        public List<PDCSOTTO>? SubaccountCache { get; set; }
        public ObservableCollection<PDCGRUPPI>? GroupsList { get; set; }

        public COMUNI? SelectedCustomerRIBACity { get; set; }
        public TAB_STATES? SelectedCustomerRIBAState { get; set; }

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

        private BankItem? selectedBank;
        public BankItem? SelectedBank
        {
            get { return selectedBank; }
            set
            {
                selectedBank = value;
                NotifyPropertyChanged("SelectedBank");
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

        public ObservableCollection<ASSOGGETAMENTI>? RatesList { get; set; }
        public ASSOGGETAMENTI? SelectedSupplierRate { get; set; }
        public ObservableCollection<PAGFOR>? SupplierPayments { get; set; }
        public PAGFOR? SelectedSupplierPayment { get; set; }
        public ObservableCollection<SCADENZE>? Expires { get; set; }
        public SCADENZE? SelectedSupplierExpire { get; set; }
        public ObservableCollection<SPEDIZIONE>? Shipments { get; set; }
        public SPEDIZIONE? SelectedSupplierShipment { get; set; }
        public SPEDIZIONE? SelectedCustomerShipment { get; set; }
        public ObservableCollection<CONSEGNA>? Deliveries { get; set; }
        public CONSEGNA? SelectedSupplierDelivery { get; set; }
        public CONSEGNA? SelectedCustomerDelivery { get; set; }
        public ObservableCollection<ZONE>? Zones { get; set; }
        public ZONE? SelectedSupplierZone { get; set; }
        public ObservableCollection<store_stores>? Stores { get; set; }
        public store_stores? SelectedSupplierStore { get; set; }
        public ObservableCollection<VETTORI>? Carriers { get; set; }
        public VETTORI? SelectedSupplierCarrier { get; set; }
        public VETTORI? SelectedCustomerCarrier { get; set; }
        public ObservableCollection<RIVENDITORI>? Dealers { get; set; }

        public RIVENDITORI? SelectedSupplierDealer { get; set; }
        public RIVENDITORI? SelectedCustomerDealer { get; set; }

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

        private BankItem? selectedCustomerInternalBank;
        public BankItem? SelectedCustomerInternalBank
        {
            get { return selectedCustomerInternalBank; }
            set
            {
                selectedCustomerInternalBank = value;
                NotifyPropertyChanged("SelectedCustomerInternalBank");
            }
        }

        public ObservableCollection<PAGCLI>? CustomerPayments { get; set; }
        public PAGCLI? SelectedCustomerPayment { get; set; }
        public SCADENZE? SelectedCustomerExpire { get; set; }
        public ObservableCollection<AFFIDABILITA>? Reliabilities { get; set; }
        public AFFIDABILITA? SelectedCustomerReliability { get; set; }
        public ObservableCollection<AREE>? Areas { get; set; }
        public AREE? SelectedCustomerArea { get; set; }
        public ObservableCollection<FILIALI>? Branches { get; set; }
        public FILIALI? SelectedCustomerBranch { get; set; }
        public ObservableCollection<CATEGORIA>? Categories { get; set; }
        public CATEGORIA? SelectedCustomerCategory { get; set; }
        public ObservableCollection<CLAZIONE>? Classifications { get; set; }
        public CLAZIONE? SelectedCustomerClassification { get; set; }
        public ASSOGGETAMENTI? SelectedCustomerRate { get; set; }
        public ObservableCollection<AGENTI>? Agents { get; set; }

        private AGENTI? selectedCustomerFirstAgent;
        public AGENTI? SelectedCustomerFirstAgent { get => selectedCustomerFirstAgent; set { selectedCustomerFirstAgent = value; NotifyPropertyChanged("SelectedCustomerFirstAgent"); } }

        private AGENTI? selectedCustomerSecondAgent;
        public AGENTI? SelectedCustomerSecondAgent { get => selectedCustomerSecondAgent; set { selectedCustomerSecondAgent = value; NotifyPropertyChanged("SelectedCustomerSecondAgent"); } }

        public ObservableCollection<SOLLECITI>? Reminders { get; set; }
        public SOLLECITI? SelectedCustomerReminder { get; set; }
        public ZONE? SelectedCustomerZone { get; set; }
        public ObservableCollection<DEPOSITI>? Deposits { get; set; }
        public DEPOSITI? SelectedCustomerDeposit { get; set; }
        public ObservableCollection<REGIONI>? Regions { get; set; }
        public REGIONI? SelectedCustomerRegion { get; set; }
        public ObservableCollection<MERCEOLOGICO>? Commodities { get; set; }
        public MERCEOLOGICO? SelectedCustomerCommodity { get; set; }
        public COMUNI? SelectedSupplierCommercialBirthCity { get; set; }
        public TAB_STATES? SelectedSupplierCommercialBirthState { get; set; }
        public COMUNI? SelectedSupplierCommercialResidentialCity { get; set; }
        public TAB_STATES? SelectedSupplierCommercialResidentialState { get; set; }
        public COMUNI? SelectedSupplierCommercialFiscalCity { get; set; }
        public TAB_STATES? SelectedSupplierCommercialFiscalState { get; set; }
        public REGIONI? SelectedSupplierCommercialRegion { get; set; }
        public ObservableCollection<IMBALLI>? Packings { get; set; }
        public IMBALLI? SelectedSupplierCommercialPacking { get; set; }
        public ObservableCollection<CAUCONT>? CausalsList { get; set; }
        public ObservableCollection<GenericIDDescription>? Signs { get; set; }
        public ObservableCollection<GenericIDDescription>? CommissionTypes { get; set; }
        public IMBALLI? SelectedCustomerCommercialPacking { get; set; }

        private ObservableCollection<ABE_EXTERN>? externalCodes;
        public ObservableCollection<ABE_EXTERN>? ExternalCodes
        {
            get => externalCodes;
            set
            {
                externalCodes = value;
                foreach (var item in value ?? new ObservableCollection<ABE_EXTERN>())
                {
                    foreach (var dest in item.Destinations ?? new ObservableCollection<ABE_EXTERN_DESTS>())
                    {
                        dest.DestinationsList = Recipients;
                    }
                }
                NotifyPropertyChanged("ExternalCodes");
            }
        }

        public FATTPERSTXT? SelectedInvoicePersonalizationFile { get; set; }

        private ObservableCollection<FATTPERSTXT>? invoicePersonalizationFiles;
        public ObservableCollection<FATTPERSTXT>? InvoicePersonalizationFiles
        {
            get => invoicePersonalizationFiles;
            set
            {
                invoicePersonalizationFiles = value;

                NotifyPropertyChanged("InvoicePersonalizationFiles");
            }
        }

        private ABE? selectedObsoleto;
        public ABE? SelectedObsoleto
        {
            get => selectedObsoleto;
            set
            {
                selectedObsoleto = value;

                NotifyPropertyChanged("SelectedObsoleto");
            }
        }

        private ObservableCollection<ABE>? obsoleti;
        public ObservableCollection<ABE>? Obsoleti
        {
            get => obsoleti;
            set
            {
                obsoleti = value;

                NotifyPropertyChanged("Obsoleti");
            }
        }

        //UFP
        public ObservableCollection<GenericIDDescription>? CertificateTypes { get; set; }
        public ObservableCollection<store_causals>? StoreCausals { get; set; }

        private ANACERT? certificate;
        public ANACERT? Certificate
        {
            get { return certificate; }
            set
            {
                certificate = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<ANACERTLEVEL1>? certificates;
        public ObservableCollection<ANACERTLEVEL1>? Certificates
        {
            get => certificates;
            set
            {
                certificates = value;

                NotifyPropertyChanged("Certificates");
            }
        }

        private SCADCLI? expireCustomer;
        public SCADCLI? ExpireCustomer
        {
            get { return expireCustomer; }
            set
            {
                expireCustomer = value;
                NotifyPropertyChanged();
            }
        }



        private ObservableCollection<CAUSVEN>? purchaseCausals;
        public ObservableCollection<CAUSVEN>? PurchaseCausals
        {
            get => purchaseCausals;
            set
            {
                purchaseCausals = value;

                NotifyPropertyChanged();
            }
        }

        private CAUSVEN? purchaseCausal;
        public CAUSVEN? PurchaseCausal
        {
            get { return purchaseCausal; }
            set
            {
                purchaseCausal = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<VALUTE>? valute;
        public ObservableCollection<VALUTE>? Valute
        {
            get => valute;
            set
            {
                valute = value;

                NotifyPropertyChanged();
            }
        }

        private VALUTE? selectedSupplierValuta;
        public VALUTE? SelectedSupplierValuta
        {
            get { return selectedSupplierValuta; }
            set
            {
                selectedSupplierValuta = value;
                NotifyPropertyChanged();
            }
        }

        private VALUTE? selectedCustomerValuta;
        public VALUTE? SelectedCustomerValuta
        {
            get { return selectedCustomerValuta; }
            set
            {
                selectedCustomerValuta = value;
                NotifyPropertyChanged();
            }
        }

        public abstract void LoadLists();

        public abstract int GetFreeID();

        public abstract string? DestinatariValidate(DESTINATARI Model, bool IsInsert);

        public abstract string? DestinatariCanDelete(DESTINATARI Model);

        public abstract string? DesForValidate(DESFOR Model, bool IsInsert);

        public abstract string? RFTTB00FValidate(RFFTB00F Model, bool IsInsert);

        public abstract string? ANDEFRESValidate(ANDEFRES Model, bool IsInsert);

        public abstract string? CUSTOMER_GROUPValidate(CUSTOMER_GROUPS Model, bool IsInsert);

        public abstract string? SUPPLIER_GROUPValidate(SUPPLIER_GROUPS Model, bool IsInsert);

        public abstract string? ABE_EXTERNValidate(ABE_EXTERN Model, bool IsInsert);

        public abstract string? ABE_EXTERN_DESTValidate(ABE_EXTERN_DESTS Model, bool IsInsert);

        public abstract string? Validate();

        public abstract bool Insert();

        public abstract bool Update();
    }

    public class ABEWindowViewModelDefault : ABEWindowViewModel
    {
        private IABERepository _abeRepository;
        private IDESTINATARIRepository _destinatariRepository;
        private IDESFORRepository _desforRepository;
        private IRFFTB00FRepository _rfb00FRepository;
        private IANDEFRESRepository _andrefresRepository;
        private ISUPPLIER_GROUPSRepository _supplierGroupsRepository;
        private ICUSTOMER_GROUPSRepository _customerGroupsRepository;
        private IABE_EXTERNRepository _abeExternRepository;
        private IABE_EXTERN_DESTRepository _abeExternDestRepository;
        private IPDCGRUPPIRepository _pdcGruppiRepository;
        private IPDCCONTIRepository _pdcContiRepository;
        private IPDCSOTTORepository _pdcSottoRepository;
        private IAGENTIRepository _agentiRepository;
        private IAliquoteRepository _aliquoteRepository;
        private IPAGFORRepository _PAGFORRepository;
        private IPAGCLIRepository _PAGLIRepository;

        public ABEWindowViewModelDefault()
        {
            _abeRepository = VulpesServiceProvider.Provider.GetRequiredService<IABERepository>();
            _desforRepository = VulpesServiceProvider.Provider.GetRequiredService<IDESFORRepository>();
            _destinatariRepository = VulpesServiceProvider.Provider.GetRequiredService<IDESTINATARIRepository>();
            _rfb00FRepository = VulpesServiceProvider.Provider.GetRequiredService<IRFFTB00FRepository>();
            _andrefresRepository = VulpesServiceProvider.Provider.GetRequiredService<IANDEFRESRepository>();
            _supplierGroupsRepository = VulpesServiceProvider.Provider.GetRequiredService<ISUPPLIER_GROUPSRepository>();
            _customerGroupsRepository = VulpesServiceProvider.Provider.GetRequiredService<ICUSTOMER_GROUPSRepository>();
            _abeExternDestRepository = VulpesServiceProvider.Provider.GetRequiredService<IABE_EXTERN_DESTRepository>();
            _abeExternRepository = VulpesServiceProvider.Provider.GetRequiredService<IABE_EXTERNRepository>();
            _pdcGruppiRepository = VulpesServiceProvider.Provider.GetRequiredService<IPDCGRUPPIRepository>();
            _pdcContiRepository = VulpesServiceProvider.Provider.GetRequiredService<IPDCCONTIRepository>();
            _pdcSottoRepository = VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>();
            _agentiRepository = VulpesServiceProvider.Provider.GetRequiredService<IAGENTIRepository>();
            _aliquoteRepository = VulpesServiceProvider.Provider.GetRequiredService<IAliquoteRepository>();
            _PAGFORRepository = VulpesServiceProvider.Provider.GetRequiredService<IPAGFORRepository>();
            _PAGLIRepository = VulpesServiceProvider.Provider.GetRequiredService<IPAGCLIRepository>();

            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;


            EntityTypes = new ObservableCollection<GenericIDDescription>() {
                    new GenericIDDescription { ID = "1", Description = "Soggetto solo partita IVA" },
                    new GenericIDDescription { ID = "2", Description = "Soggetto solo codice fiscale" },
                    new GenericIDDescription { ID = "4", Description = "Soggetto partita IVA e codice fiscale" },
                    new GenericIDDescription { ID = "3", Description = "Soggetto estero" }};
            EntityUseTypes = new ObservableCollection<GenericIDDescription>() {
                    new GenericIDDescription { ID = "C", Description = "Cliente" },
                    new GenericIDDescription { ID = "F", Description = "Fornitore" },
                    new GenericIDDescription { ID = "P", Description = "Prospect" },
                    new GenericIDDescription { ID = "E", Description = "Entrambi" }};
            Sexes = new ObservableCollection<GenericIDDescription>() {
                    new GenericIDDescription { ID = "M", Description = "Maschio" },
                    new GenericIDDescription { ID = "F", Description = "Femmina" },
                    new GenericIDDescription { ID = null, Description = "Nessuno" }};
            NoteTypes = new ObservableCollection<GenericIDDescription>() {
                    new GenericIDDescription { ID = "O", Description = "Ordini" },
                    new GenericIDDescription { ID = "B", Description = "Bolle" },
                    new GenericIDDescription { ID = "F", Description = "Fatture" },
                    new GenericIDDescription { ID = "G", Description = "Generale" }};
            SupplierSubjectTypes = new ObservableCollection<GenericIDDescription>() {
                    new GenericIDDescription { ID = null, Description = "Soggetto IVA normale" },
                    new GenericIDDescription { ID = "4", Description = "Dogane" }};

            RatesList = _aliquoteRepository.GetList();
            SupplierPayments = _PAGFORRepository.GetList();
            CustomerPayments = _PAGLIRepository.GetList();
            Expires = VulpesServiceProvider.Provider.GetRequiredService<ISCADENZERepository>().GetList();
            Shipments = VulpesServiceProvider.Provider.GetRequiredService<ISPEDIZIONERepository>().GetList();
            Deliveries = VulpesServiceProvider.Provider.GetRequiredService<ICONSEGNARepository>().GetList();
            Zones = VulpesServiceProvider.Provider.GetRequiredService<IZONERepository>().GetList();
            Carriers = VulpesServiceProvider.Provider.GetRequiredService<IVETTORIRepository>().GetList();
            Dealers = VulpesServiceProvider.Provider.GetRequiredService<IRIVENDITORIRepository>().GetList();
            Reliabilities = VulpesServiceProvider.Provider.GetRequiredService<IAFFIDABILITARepository>().GetList();
            Areas = VulpesServiceProvider.Provider.GetRequiredService<IAREERepository>().GetList();
            Categories = VulpesServiceProvider.Provider.GetRequiredService<ICATEGORIARepository>().GetList();
            Classifications = VulpesServiceProvider.Provider.GetRequiredService<ICLAZIONERepository>().GetList();
            Agents = _agentiRepository.GetList();
            Reminders = VulpesServiceProvider.Provider.GetRequiredService<ISOLLECITIRepository>().GetList();
            Commodities = VulpesServiceProvider.Provider.GetRequiredService<IMERCEOLOGICORepository>().GetList();
            Regions = VulpesServiceProvider.Provider.GetRequiredService<IREGIONIRepository>().GetList();
            Deposits = VulpesServiceProvider.Provider.GetRequiredService<IDEPOSITIRepository>().GetList();
            Packings = VulpesServiceProvider.Provider.GetRequiredService<IIMBALLIRepository>().GetList();
            AccountCache = _pdcContiRepository.GetBasicList();
            SubaccountCache = _pdcSottoRepository.GetBasicList(CompanyID);
            GroupsList = _pdcGruppiRepository.GetList();
            CausalsList = VulpesServiceProvider.Provider.GetRequiredService<ICAUCONTRepository>().GetSimpleList();
            Signs = CommonsService.StandardAccountingSigns;
            CommissionTypes = CommonsService.StandardValueTypes;
        }

        public override void LoadLists()
        {
            AZIENDA = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Get(CompanyID);

            Stores = VulpesServiceProvider.Provider.GetRequiredService<Istore_storesRepository>().GetList(CompanyID, false);
            InternalBanks = VulpesServiceProvider.Provider.GetRequiredService<IABICABRepository>().GetSimpleList(CompanyID, "N");
            Banks = VulpesServiceProvider.Provider.GetRequiredService<IABICABRepository>().GetSimpleList(CompanyID, null);
            Branches = VulpesServiceProvider.Provider.GetRequiredService<IFILIALIRepository>().GetList(CompanyID);

            Data.ISOList = VulpesServiceProvider.Provider.GetRequiredService<IISORepository>().GetList();
            Data.Cities = VulpesServiceProvider.Provider.GetRequiredService<ICOMUNIRepository>().GetList();
            Data.States = VulpesServiceProvider.Provider.GetRequiredService<ITAB_STATESRepository>().GetList();
            Data.Countries = VulpesServiceProvider.Provider.GetRequiredService<INAZIONIRepository>().GetList();
            Data.CompanyTypes = VulpesServiceProvider.Provider.GetRequiredService<ISOCIETARepository>().GetList();

            Recipients = VulpesServiceProvider.Provider.GetRequiredService<IDESTINATARIRepository>().GetList(Data.abecod);
            SupplierRecipients = VulpesServiceProvider.Provider.GetRequiredService<IDESFORRepository>().GetList(Data.abecod);
            ExternalCodes = VulpesServiceProvider.Provider.GetRequiredService<IABE_EXTERNRepository>().GetList(Data.abecod);
            InvoicePersonalizationFiles = VulpesServiceProvider.Provider.GetRequiredService<IFATTPERSTXTRepository>().GetList(CompanyID);

            Obsoleti = _abeRepository.GetLightListObsoleti();
        }

        public override int GetFreeID()
        {
            return _abeRepository.GetFreeIDList()?.FirstOrDefault() ?? 1;
        }

        public override string? DestinatariValidate(DESTINATARI Model, bool IsInsert)
        {
            return _destinatariRepository.Validate(Model, IsInsert);
        }

        public override string? DestinatariCanDelete(DESTINATARI Model)
        {
            return _destinatariRepository.CanDelete(Model);
        }

        public override string? DesForValidate(DESFOR Model, bool IsInsert)
        {
            return _desforRepository.Validate(Model, IsInsert);
        }

        public override string? RFTTB00FValidate(RFFTB00F Model, bool IsInsert)
        {
            return _rfb00FRepository.Validate(Model, IsInsert);
        }

        public override string? ANDEFRESValidate(ANDEFRES Model, bool IsInsert)
        {
            return _andrefresRepository.Validate(Model, IsInsert);
        }

        public override string? CUSTOMER_GROUPValidate(CUSTOMER_GROUPS Model, bool IsInsert)
        {
            return _customerGroupsRepository.Validate(Model, IsInsert);
        }

        public override string? SUPPLIER_GROUPValidate(SUPPLIER_GROUPS Model, bool IsInsert)
        {
            return _supplierGroupsRepository.Validate(Model, IsInsert);
        }

        public override string? ABE_EXTERNValidate(ABE_EXTERN Model, bool IsInsert)
        {
            return _abeExternRepository.Validate(Model, IsInsert);
        }

        public override string? ABE_EXTERN_DESTValidate(ABE_EXTERN_DESTS Model, bool IsInsert)
        {
            return _abeExternDestRepository.Validate(Model, IsInsert);
        }

        public override string? Validate()
        {
            return _abeRepository.Validate(Data, SupplierData, CustomerData, SupplierCommercialData, CustomerCommercialData, Data?.ISO != null && Data.ISO.isopiv.HasValue ? Data.ISO.isopiv.Value : 0, IsInsert);
        }

        public override bool Insert()
        {
            return _abeRepository.Insert(CompanyID, Data!, Recipients, SupplierRecipients,
                        CustomerNotes, SupplierNotes,
                        CustomerData, SupplierData,
                        CustomerCommercialData, SupplierCommercialData,
                        SupplierReferences, CustomerReferences, CounterpartsRows, CounterpartsRowsCustomer,
                        ExternalCodes, null, null, null);
        }

        public override bool Update()
        {
            return _abeRepository.Update(CompanyID, Data!, Recipients, SupplierRecipients,
                        CustomerNotes, SupplierNotes,
                        CustomerData, SupplierData,
                        CustomerCommercialData, SupplierCommercialData,
                        SupplierReferences, CustomerReferences, CounterpartsRows, CounterpartsRowsCustomer,
                        ExternalCodes, null, null, null);
        }
    }

    public class ABEWindowViewModelUfp : ABEWindowViewModel
    {
        private IABERepository _abeRepository;
        private IDESTINATARIRepository _destinatariRepository;
        private IDESFORRepository _desforRepository;
        private IRFFTB00FRepository _rfb00FRepository;
        private IANDEFRESRepository _andrefresRepository;
        private ISUPPLIER_GROUPSRepository _supplierGroupsRepository;
        private IABE_EXTERNRepository _abeExternRepository;
        private IABE_EXTERN_DESTRepository _abeExternDestRepository;
        private IPDCGRUPPIRepository _pdcGruppiRepository;
        private IPDCCONTIRepository _pdcContiRepository;
        private IPDCSOTTORepository _pdcSottoRepository;
        private IAGENTIRepository _agentiRepository;
        private IAliquoteRepository _aliquoteRepository;
        private IPAGFORRepository _PAGFORRepository;
        private IPAGCLIRepository _PAGLIRepository;

        public ABEWindowViewModelUfp()
        {
            _abeRepository = VulpesServiceProvider.Provider.GetRequiredService<IABERepository>();
            _desforRepository = VulpesServiceProvider.Provider.GetRequiredService<IDESFORRepository>();
            _destinatariRepository = VulpesServiceProvider.Provider.GetRequiredService<IDESTINATARIRepository>();
            _rfb00FRepository = VulpesServiceProvider.Provider.GetRequiredService<IRFFTB00FRepository>();
            _andrefresRepository = VulpesServiceProvider.Provider.GetRequiredService<IANDEFRESRepository>();
            _supplierGroupsRepository = VulpesServiceProvider.Provider.GetRequiredService<ISUPPLIER_GROUPSRepository>();
            _abeExternDestRepository = VulpesServiceProvider.Provider.GetRequiredService<IABE_EXTERN_DESTRepository>();
            _abeExternRepository = VulpesServiceProvider.Provider.GetRequiredService<IABE_EXTERNRepository>();
            _pdcGruppiRepository = VulpesServiceProvider.Provider.GetRequiredService<IPDCGRUPPIRepository>();
            _pdcContiRepository = VulpesServiceProvider.Provider.GetRequiredService<IPDCCONTIRepository>();
            _pdcSottoRepository = VulpesServiceProvider.Provider.GetRequiredService<IPDCSOTTORepository>();
            _agentiRepository = VulpesServiceProvider.Provider.GetRequiredService<IAGENTIRepository>();
            _aliquoteRepository = VulpesServiceProvider.Provider.GetRequiredService<IAliquoteRepository>();
            _PAGFORRepository = VulpesServiceProvider.Provider.GetRequiredService<IPAGFORRepository>();
            _PAGLIRepository = VulpesServiceProvider.Provider.GetRequiredService<IPAGCLIRepository>();

            CompanyID = UserContext.Instance.ACCESS!.SelectedCompany!.SOMCOD;

            EntityTypes = new ObservableCollection<GenericIDDescription>() {
                    new GenericIDDescription { ID = "1", Description = "Soggetto solo partita IVA" },
                    new GenericIDDescription { ID = "2", Description = "Soggetto solo codice fiscale" },
                    new GenericIDDescription { ID = "4", Description = "Soggetto partita IVA e codice fiscale" },
                    new GenericIDDescription { ID = "3", Description = "Soggetto estero" }};
            EntityUseTypes = new ObservableCollection<GenericIDDescription>() {
                    new GenericIDDescription { ID = "C", Description = "Cliente" },
                    new GenericIDDescription { ID = "F", Description = "Fornitore" },
                    new GenericIDDescription { ID = "P", Description = "Prospect" },
                    new GenericIDDescription { ID = "E", Description = "Entrambi" }};
            Sexes = new ObservableCollection<GenericIDDescription>() {
                    new GenericIDDescription { ID = "M", Description = "Maschio" },
                    new GenericIDDescription { ID = "F", Description = "Femmina" },
                    new GenericIDDescription { ID = null, Description = "Nessuno" }};
            NoteTypes = new ObservableCollection<GenericIDDescription>() {
                    new GenericIDDescription { ID = "O", Description = "Ordini" },
                    new GenericIDDescription { ID = "B", Description = "Bolle" },
                    new GenericIDDescription { ID = "F", Description = "Fatture" },
                    new GenericIDDescription { ID = "G", Description = "Generale" },
                    new GenericIDDescription { ID = "P", Description = "Offerte" },
                    new GenericIDDescription { ID = "L", Description = "Legali" },
                    new GenericIDDescription { ID = "M", Description = "Marcatura" },
                    new GenericIDDescription { ID = "Z", Description = "Produzione" },
                    new GenericIDDescription { ID = "C", Description = "Marcatura C/Lavoro" }};
            SupplierSubjectTypes = new ObservableCollection<GenericIDDescription>() {
                    new GenericIDDescription { ID = null, Description = "Soggetto IVA normale" },
                    new GenericIDDescription { ID = "4", Description = "Dogane" }};

            CertificateTypes = new ObservableCollection<GenericIDDescription>(){
                    new GenericIDDescription("CS","Avio - Dichiarazione Conformità Semplice"),
                    new GenericIDDescription("UC","Avio - Dichiarazione Conformità Utensile Critico"),
                    new GenericIDDescription("RD","Avio - Scheda Rilievi Dimensionali"),
                    new GenericIDDescription("FA","Avio - Scheda Fai"),};


            RatesList = _aliquoteRepository.GetList();
            SupplierPayments = _PAGFORRepository.GetList();
            CustomerPayments = _PAGLIRepository.GetList();
            Expires = VulpesServiceProvider.Provider.GetRequiredService<ISCADENZERepository>().GetList();
            Shipments = VulpesServiceProvider.Provider.GetRequiredService<ISPEDIZIONERepository>().GetList();
            Deliveries = VulpesServiceProvider.Provider.GetRequiredService<ICONSEGNARepository>().GetList();
            Zones = VulpesServiceProvider.Provider.GetRequiredService<IZONERepository>().GetList();
            Carriers = VulpesServiceProvider.Provider.GetRequiredService<IVETTORIRepository>().GetList();
            Dealers = VulpesServiceProvider.Provider.GetRequiredService<IRIVENDITORIRepository>().GetList();
            Reliabilities = VulpesServiceProvider.Provider.GetRequiredService<IAFFIDABILITARepository>().GetList();
            Areas = VulpesServiceProvider.Provider.GetRequiredService<IAREERepository>().GetList();
            Categories = VulpesServiceProvider.Provider.GetRequiredService<ICATEGORIARepository>().GetList();
            Classifications = VulpesServiceProvider.Provider.GetRequiredService<ICLAZIONERepository>().GetList();
            Agents = _agentiRepository.GetList();
            Reminders = VulpesServiceProvider.Provider.GetRequiredService<ISOLLECITIRepository>().GetList();
            Commodities = VulpesServiceProvider.Provider.GetRequiredService<IMERCEOLOGICORepository>().GetList();
            Regions = VulpesServiceProvider.Provider.GetRequiredService<IREGIONIRepository>().GetList();
            Deposits = VulpesServiceProvider.Provider.GetRequiredService<IDEPOSITIRepository>().GetList();
            Packings = VulpesServiceProvider.Provider.GetRequiredService<IIMBALLIRepository>().GetList();
            AccountCache = _pdcContiRepository.GetBasicList();
            SubaccountCache = _pdcSottoRepository.GetBasicList(CompanyID);
            GroupsList = _pdcGruppiRepository.GetList();
            CausalsList = VulpesServiceProvider.Provider.GetRequiredService<ICAUCONTRepository>().GetSimpleList();
            Signs = CommonsService.StandardAccountingSigns;
            CommissionTypes = CommonsService.PercentageValueTypes;

        }

        public override void LoadLists()
        {
            AZIENDA = VulpesServiceProvider.Provider.GetRequiredService<IAZIENDARepository>().Get(CompanyID);

            Stores = VulpesServiceProvider.Provider.GetRequiredService<Istore_storesRepository>().GetList(CompanyID, false);
            StoreCausals = VulpesServiceProvider.Provider.GetRequiredService<ISTORE_CAUSALSRepository>().GetList(CompanyID, "+");
            InternalBanks = VulpesServiceProvider.Provider.GetRequiredService<IABICABRepository>().GetSimpleList(CompanyID, "N");
            Banks = VulpesServiceProvider.Provider.GetRequiredService<IABICABRepository>().GetSimpleList(CompanyID, null);
            Branches = VulpesServiceProvider.Provider.GetRequiredService<IFILIALIRepository>().GetList(CompanyID);
            PurchaseCausals = VulpesServiceProvider.Provider.GetRequiredService<ICAUSVENService>().GetList();
            Valute = VulpesServiceProvider.Provider.GetRequiredService<IVALUTERepository>().GetList();

            Data.ISOList = VulpesServiceProvider.Provider.GetRequiredService<IISORepository>().GetList();
            Data.Cities = VulpesServiceProvider.Provider.GetRequiredService<ICOMUNIRepository>().GetList();
            Data.States = VulpesServiceProvider.Provider.GetRequiredService<ITAB_STATESRepository>().GetList();
            Data.Countries = VulpesServiceProvider.Provider.GetRequiredService<INAZIONIRepository>().GetList();
            Data.CompanyTypes = VulpesServiceProvider.Provider.GetRequiredService<ISOCIETARepository>().GetList();

            Recipients = VulpesServiceProvider.Provider.GetRequiredService<IDESTINATARIRepository>().GetList(Data.abecod);
            SupplierRecipients = VulpesServiceProvider.Provider.GetRequiredService<IDESFORRepository>().GetList(Data.abecod);

            Obsoleti = _abeRepository.GetLightListObsoleti();
        }

        public override int GetFreeID()
        {
            return _abeRepository.GetFreeIDList()?.FirstOrDefault() ?? 1;
        }

        public override string? DestinatariValidate(DESTINATARI Model, bool IsInsert)
        {
            return _destinatariRepository.Validate(Model, IsInsert);
        }

        public override string? DestinatariCanDelete(DESTINATARI Model)
        {
            return _destinatariRepository.CanDelete(Model);
        }

        public override string? DesForValidate(DESFOR Model, bool IsInsert)
        {
            return _desforRepository.Validate(Model, IsInsert);
        }

        public override string? RFTTB00FValidate(RFFTB00F Model, bool IsInsert)
        {
            return _rfb00FRepository.Validate(Model, IsInsert);
        }

        public override string? ANDEFRESValidate(ANDEFRES Model, bool IsInsert)
        {
            return _andrefresRepository.Validate(Model, IsInsert);
        }

        public override string? CUSTOMER_GROUPValidate(CUSTOMER_GROUPS Model, bool IsInsert)
        {
            throw new NotImplementedException();
        }

        public override string? SUPPLIER_GROUPValidate(SUPPLIER_GROUPS Model, bool IsInsert)
        {
            return _supplierGroupsRepository.Validate(Model, IsInsert);
        }

        public override string? ABE_EXTERNValidate(ABE_EXTERN Model, bool IsInsert)
        {
            throw new NotImplementedException();
        }

        public override string? ABE_EXTERN_DESTValidate(ABE_EXTERN_DESTS Model, bool IsInsert)
        {
            throw new NotImplementedException();
        }

        public override string? Validate()
        {
            return _abeRepository.Validate(Data, SupplierData, CustomerData, SupplierCommercialData, CustomerCommercialData, Data?.ISO != null && Data.ISO.isopiv.HasValue ? Data.ISO.isopiv.Value : 0, IsInsert);
        }

        public override bool Insert()
        {
            return _abeRepository.Insert(CompanyID, Data!, Recipients, SupplierRecipients,
                        CustomerNotes, SupplierNotes,
                        CustomerData, SupplierData,
                        CustomerCommercialData, SupplierCommercialData,
                        SupplierReferences, CustomerReferences, CounterpartsRows, null,
                        null, Certificate, Certificates, ExpireCustomer);
        }

        public override bool Update()
        {
            return _abeRepository.Update(CompanyID, Data!, Recipients, SupplierRecipients,
                        CustomerNotes, SupplierNotes,
                        CustomerData, SupplierData,
                        CustomerCommercialData, SupplierCommercialData,
                        SupplierReferences, CustomerReferences, CounterpartsRows, null,
                        null, Certificate, Certificates, ExpireCustomer);
        }
    }

}
