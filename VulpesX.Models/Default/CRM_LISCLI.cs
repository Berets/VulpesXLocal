namespace VulpesX.Models.Default;
 
public partial class CRM_LISCLI : Base 
{
	private long _id;
	public long id { get => _id; set { if (_id != value) { _id = value; NotifyPropertyChanged();} } }
	private int _customerID;
	public int customerID { get => _customerID; set { if (_customerID != value) { _customerID = value; NotifyPropertyChanged();} } }
	private string _companyID = null!;
	public required string companyID { get => _companyID; set { if (_companyID != value) { _companyID = value; NotifyPropertyChanged();} } }
	private string _productID = null!;
	public required string productID { get => _productID; set { if (_productID != value) { _productID = value; NotifyPropertyChanged();} } }
	private DateTime _fromDate;
	public DateTime fromDate { get => _fromDate; set { if (_fromDate != value) { _fromDate = value; NotifyPropertyChanged();} } }
	private DateTime _toDate;
	public DateTime toDate { get => _toDate; set { if (_toDate != value) { _toDate = value; NotifyPropertyChanged();} } }
	private decimal _fromQuantity;
	public decimal fromQuantity { get => _fromQuantity; set { if (_fromQuantity != value) { _fromQuantity = value; NotifyPropertyChanged();} } }
	private decimal _toQuantity;
	public decimal toQuantity { get => _toQuantity; set { if (_toQuantity != value) { _toQuantity = value; NotifyPropertyChanged();} } }
	private decimal _price;
	public decimal price { get => _price; set { if (_price != value) { _price = value; NotifyPropertyChanged();} } }
	private string? _description;
	public string? description { get => _description; set { if (_description != value) { _description = value; NotifyPropertyChanged();} } }
	private decimal? _discount1;
	public decimal? discount1 { get => _discount1; set { if (_discount1 != value) { _discount1 = value; NotifyPropertyChanged();} } }
	private decimal? _discount2;
	public decimal? discount2 { get => _discount2; set { if (_discount2 != value) { _discount2 = value; NotifyPropertyChanged();} } }
	private decimal? _discount3;
	public decimal? discount3 { get => _discount3; set { if (_discount3 != value) { _discount3 = value; NotifyPropertyChanged();} } }
	private string? _discountType1;
	public string? discountType1 { get => _discountType1; set { if (_discountType1 != value) { _discountType1 = value; NotifyPropertyChanged();} } }
	private string? _discountType2;
	public string? discountType2 { get => _discountType2; set { if (_discountType2 != value) { _discountType2 = value; NotifyPropertyChanged();} } }
	private string? _discountType3;
	public string? discountType3 { get => _discountType3; set { if (_discountType3 != value) { _discountType3 = value; NotifyPropertyChanged();} } }
	private decimal? _surcharge;
	public decimal? surcharge { get => _surcharge; set { if (_surcharge != value) { _surcharge = value; NotifyPropertyChanged();} } }
	private string? _surchargeType;
	public string? surchargeType { get => _surchargeType; set { if (_surchargeType != value) { _surchargeType = value; NotifyPropertyChanged();} } }
	private DateTime? _added;
	public DateTime? added { get => _added; set { if (_added != value) { _added = value; NotifyPropertyChanged();} } }
	private DateTime? _updated;
	public DateTime? updated { get => _updated; set { if (_updated != value) { _updated = value; NotifyPropertyChanged();} } }
	private DateTime? _canceled;
	public DateTime? canceled { get => _canceled; set { if (_canceled != value) { _canceled = value; NotifyPropertyChanged();} } }
	private string? _addedUserID;
	public string? addedUserID { get => _addedUserID; set { if (_addedUserID != value) { _addedUserID = value; NotifyPropertyChanged();} } }
	private string? _updatedUserID;
	public string? updatedUserID { get => _updatedUserID; set { if (_updatedUserID != value) { _updatedUserID = value; NotifyPropertyChanged();} } }
	private string? _canceledUserID;
	public string? canceledUserID { get => _canceledUserID; set { if (_canceledUserID != value) { _canceledUserID = value; NotifyPropertyChanged();} } }
	private string? _canceledNote;
	public string? canceledNote { get => _canceledNote; set { if (_canceledNote != value) { _canceledNote = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
	private string? _customerProductID;
	public string? customerProductID { get => _customerProductID; set { if (_customerProductID != value) { _customerProductID = value; NotifyPropertyChanged();} } }
	private string? _customerProductDescription;
	public string? customerProductDescription { get => _customerProductDescription; set { if (_customerProductDescription != value) { _customerProductDescription = value; NotifyPropertyChanged();} } }
	private int? _recipientID;
	public int? recipientID { get => _recipientID; set { if (_recipientID != value) { _recipientID = value; NotifyPropertyChanged();} } }
	private string? _unit_id;
	public string? unit_id { get => _unit_id; set { if (_unit_id != value) { _unit_id = value; NotifyPropertyChanged();} } }
}