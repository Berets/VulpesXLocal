namespace VulpesX.Models.Default;
 
public partial class ASSET00F : Base 
{
	private string _company_id = null!;
	public required string company_id { get => _company_id; set { if (_company_id != value) { _company_id = value; NotifyPropertyChanged();} } }
	private string _id = null!;
	public required string id { get => _id; set { if (_id != value) { _id = value; NotifyPropertyChanged();} } }
	private string _description = null!;
	public required string description { get => _description; set { if (_description != value) { _description = value; NotifyPropertyChanged();} } }
	private string? _product_id;
	public string? product_id { get => _product_id; set { if (_product_id != value) { _product_id = value; NotifyPropertyChanged();} } }
	private int? _customer_id;
	public int? customer_id { get => _customer_id; set { if (_customer_id != value) { _customer_id = value; NotifyPropertyChanged();} } }
	private string? _resource_id;
	public string? resource_id { get => _resource_id; set { if (_resource_id != value) { _resource_id = value; NotifyPropertyChanged();} } }
	private string? _location_id;
	public string? location_id { get => _location_id; set { if (_location_id != value) { _location_id = value; NotifyPropertyChanged();} } }
	private DateTime? _purchase_date;
	public DateTime? purchase_date { get => _purchase_date; set { if (_purchase_date != value) { _purchase_date = value; NotifyPropertyChanged();} } }
	private DateTime? _installation_date;
	public DateTime? installation_date { get => _installation_date; set { if (_installation_date != value) { _installation_date = value; NotifyPropertyChanged();} } }
	private DateTime? _first_use_date;
	public DateTime? first_use_date { get => _first_use_date; set { if (_first_use_date != value) { _first_use_date = value; NotifyPropertyChanged();} } }
	private DateTime? _warranty_expiry_date;
	public DateTime? warranty_expiry_date { get => _warranty_expiry_date; set { if (_warranty_expiry_date != value) { _warranty_expiry_date = value; NotifyPropertyChanged();} } }
	private string? _note;
	public string? note { get => _note; set { if (_note != value) { _note = value; NotifyPropertyChanged();} } }
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
}