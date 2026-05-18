namespace VulpesX.Models.Default;
 
public partial class SRM_RDA : Base 
{
	private string _companyID = null!;
	public required string companyID { get => _companyID; set { if (_companyID != value) { _companyID = value; NotifyPropertyChanged();} } }
	private long _id;
	public long id { get => _id; set { if (_id != value) { _id = value; NotifyPropertyChanged();} } }
	private string _product_id = null!;
	public required string product_id { get => _product_id; set { if (_product_id != value) { _product_id = value; NotifyPropertyChanged();} } }
	private decimal _quantity;
	public decimal quantity { get => _quantity; set { if (_quantity != value) { _quantity = value; NotifyPropertyChanged();} } }
	private DateTime? _requested_delivery;
	public DateTime? requested_delivery { get => _requested_delivery; set { if (_requested_delivery != value) { _requested_delivery = value; NotifyPropertyChanged();} } }
	private string? _note;
	public string? note { get => _note; set { if (_note != value) { _note = value; NotifyPropertyChanged();} } }
	private DateTime? _approval_date;
	public DateTime? approval_date { get => _approval_date; set { if (_approval_date != value) { _approval_date = value; NotifyPropertyChanged();} } }
	private string? _approval_user;
	public string? approval_user { get => _approval_user; set { if (_approval_user != value) { _approval_user = value; NotifyPropertyChanged();} } }
	private long? _purchase_order_id;
	public long? purchase_order_id { get => _purchase_order_id; set { if (_purchase_order_id != value) { _purchase_order_id = value; NotifyPropertyChanged();} } }
	private string? _production_order_id;
	public string? production_order_id { get => _production_order_id; set { if (_production_order_id != value) { _production_order_id = value; NotifyPropertyChanged();} } }
	private string? _transform_user;
	public string? transform_user { get => _transform_user; set { if (_transform_user != value) { _transform_user = value; NotifyPropertyChanged();} } }
	private DateTime? _transformed;
	public DateTime? transformed { get => _transformed; set { if (_transformed != value) { _transformed = value; NotifyPropertyChanged();} } }
	private string? _customer_order;
	public string? customer_order { get => _customer_order; set { if (_customer_order != value) { _customer_order = value; NotifyPropertyChanged();} } }
	private decimal? _original_needed;
	public decimal? original_needed { get => _original_needed; set { if (_original_needed != value) { _original_needed = value; NotifyPropertyChanged();} } }
	private bool? _is_customer_material;
	public bool? is_customer_material { get => _is_customer_material; set { if (_is_customer_material != value) { _is_customer_material = value; NotifyPropertyChanged();} } }
	private decimal? _customer_received_quantity;
	public decimal? customer_received_quantity { get => _customer_received_quantity; set { if (_customer_received_quantity != value) { _customer_received_quantity = value; NotifyPropertyChanged();} } }
	private DateTime _added;
	public DateTime added { get => _added; set { if (_added != value) { _added = value; NotifyPropertyChanged();} } }
	private string _addedUserID = null!;
	public required string addedUserID { get => _addedUserID; set { if (_addedUserID != value) { _addedUserID = value; NotifyPropertyChanged();} } }
	private DateTime? _updated;
	public DateTime? updated { get => _updated; set { if (_updated != value) { _updated = value; NotifyPropertyChanged();} } }
	private string? _updatedUserID;
	public string? updatedUserID { get => _updatedUserID; set { if (_updatedUserID != value) { _updatedUserID = value; NotifyPropertyChanged();} } }
	private DateTime? _canceled;
	public DateTime? canceled { get => _canceled; set { if (_canceled != value) { _canceled = value; NotifyPropertyChanged();} } }
	private string? _canceledUserID;
	public string? canceledUserID { get => _canceledUserID; set { if (_canceledUserID != value) { _canceledUserID = value; NotifyPropertyChanged();} } }
	private string? _canceledNote;
	public string? canceledNote { get => _canceledNote; set { if (_canceledNote != value) { _canceledNote = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
	private int? _supplier_id;
	public int? supplier_id { get => _supplier_id; set { if (_supplier_id != value) { _supplier_id = value; NotifyPropertyChanged();} } }
	private int? _order_year;
	public int? order_year { get => _order_year; set { if (_order_year != value) { _order_year = value; NotifyPropertyChanged();} } }
	private int? _order_number;
	public int? order_number { get => _order_number; set { if (_order_number != value) { _order_number = value; NotifyPropertyChanged();} } }
	private int? _order_row;
	public int? order_row { get => _order_row; set { if (_order_row != value) { _order_row = value; NotifyPropertyChanged();} } }
}