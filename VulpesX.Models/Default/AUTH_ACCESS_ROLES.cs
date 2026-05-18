namespace VulpesX.Models.Default;
 
public partial class AUTH_ACCESS_ROLES : Base 
{
	private string _companyID = null!;
	public required string companyID { get => _companyID; set { if (_companyID != value) { _companyID = value; NotifyPropertyChanged();} } }
	private string _userID = null!;
	public required string userID { get => _userID; set { if (_userID != value) { _userID = value; NotifyPropertyChanged();} } }
	private string? _agentID;
	public string? agentID { get => _agentID; set { if (_agentID != value) { _agentID = value; NotifyPropertyChanged();} } }
	private bool _canOffers;
	public bool canOffers { get => _canOffers; set { if (_canOffers != value) { _canOffers = value; NotifyPropertyChanged();} } }
	private bool _canOffersSignCommercial;
	public bool canOffersSignCommercial { get => _canOffersSignCommercial; set { if (_canOffersSignCommercial != value) { _canOffersSignCommercial = value; NotifyPropertyChanged();} } }
	private bool _canOffersSignTech;
	public bool canOffersSignTech { get => _canOffersSignTech; set { if (_canOffersSignTech != value) { _canOffersSignTech = value; NotifyPropertyChanged();} } }
	private bool _canOrders;
	public bool canOrders { get => _canOrders; set { if (_canOrders != value) { _canOrders = value; NotifyPropertyChanged();} } }
	private bool _canOrdersSignCommercial;
	public bool canOrdersSignCommercial { get => _canOrdersSignCommercial; set { if (_canOrdersSignCommercial != value) { _canOrdersSignCommercial = value; NotifyPropertyChanged();} } }
	private bool _canOrdersSignTech;
	public bool canOrdersSignTech { get => _canOrdersSignTech; set { if (_canOrdersSignTech != value) { _canOrdersSignTech = value; NotifyPropertyChanged();} } }
	private bool _canDDT;
	public bool canDDT { get => _canDDT; set { if (_canDDT != value) { _canDDT = value; NotifyPropertyChanged();} } }
	private bool _canInvoices;
	public bool canInvoices { get => _canInvoices; set { if (_canInvoices != value) { _canInvoices = value; NotifyPropertyChanged();} } }
	private bool _canAccounting;
	public bool canAccounting { get => _canAccounting; set { if (_canAccounting != value) { _canAccounting = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
	private bool _canCompany;
	public bool canCompany { get => _canCompany; set { if (_canCompany != value) { _canCompany = value; NotifyPropertyChanged();} } }
	private bool _canRDA;
	public bool canRDA { get => _canRDA; set { if (_canRDA != value) { _canRDA = value; NotifyPropertyChanged();} } }
	private bool _canApproveRDA;
	public bool canApproveRDA { get => _canApproveRDA; set { if (_canApproveRDA != value) { _canApproveRDA = value; NotifyPropertyChanged();} } }
	private bool _canTransformRDA;
	public bool canTransformRDA { get => _canTransformRDA; set { if (_canTransformRDA != value) { _canTransformRDA = value; NotifyPropertyChanged();} } }
	private bool _canPurchaseOrders;
	public bool canPurchaseOrders { get => _canPurchaseOrders; set { if (_canPurchaseOrders != value) { _canPurchaseOrders = value; NotifyPropertyChanged();} } }
	private bool _canPOSignCommercial;
	public bool canPOSignCommercial { get => _canPOSignCommercial; set { if (_canPOSignCommercial != value) { _canPOSignCommercial = value; NotifyPropertyChanged();} } }
	private bool _canPOSignManagement;
	public bool canPOSignManagement { get => _canPOSignManagement; set { if (_canPOSignManagement != value) { _canPOSignManagement = value; NotifyPropertyChanged();} } }
	private bool _canPOSend;
	public bool canPOSend { get => _canPOSend; set { if (_canPOSend != value) { _canPOSend = value; NotifyPropertyChanged();} } }
	private bool _canTransformOffers;
	public bool canTransformOffers { get => _canTransformOffers; set { if (_canTransformOffers != value) { _canTransformOffers = value; NotifyPropertyChanged();} } }
	private bool _canFeasibility;
	public bool canFeasibility { get => _canFeasibility; set { if (_canFeasibility != value) { _canFeasibility = value; NotifyPropertyChanged();} } }
	private bool _canAssets;
	public bool canAssets { get => _canAssets; set { if (_canAssets != value) { _canAssets = value; NotifyPropertyChanged();} } }
	private bool _canStore;
	public bool canStore { get => _canStore; set { if (_canStore != value) { _canStore = value; NotifyPropertyChanged();} } }
	private bool _canTreasury;
	public bool canTreasury { get => _canTreasury; set { if (_canTreasury != value) { _canTreasury = value; NotifyPropertyChanged();} } }
	private bool _canProduction;
	public bool canProduction { get => _canProduction; set { if (_canProduction != value) { _canProduction = value; NotifyPropertyChanged();} } }
	private bool _canTables;
	public bool canTables { get => _canTables; set { if (_canTables != value) { _canTables = value; NotifyPropertyChanged();} } }
	private bool _canRegistries;
	public bool canRegistries { get => _canRegistries; set { if (_canRegistries != value) { _canRegistries = value; NotifyPropertyChanged();} } }
	private bool _canStats;
	public bool canStats { get => _canStats; set { if (_canStats != value) { _canStats = value; NotifyPropertyChanged();} } }
	private bool _canAccountingAssets;
	public bool canAccountingAssets { get => _canAccountingAssets; set { if (_canAccountingAssets != value) { _canAccountingAssets = value; NotifyPropertyChanged();} } }
	private bool _canCRMActivities;
	public bool canCRMActivities { get => _canCRMActivities; set { if (_canCRMActivities != value) { _canCRMActivities = value; NotifyPropertyChanged();} } }
	private string? _crmRole;
	public string? crmRole { get => _crmRole; set { if (_crmRole != value) { _crmRole = value; NotifyPropertyChanged();} } }
	private bool _canCustomerRating;
	public bool canCustomerRating { get => _canCustomerRating; set { if (_canCustomerRating != value) { _canCustomerRating = value; NotifyPropertyChanged();} } }
}