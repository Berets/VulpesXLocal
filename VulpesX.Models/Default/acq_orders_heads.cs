namespace VulpesX.Models.Default;
 
public partial class acq_orders_heads : Base 
{
	private string _company_id = null!;
	public required string company_id { get => _company_id; set { if (_company_id != value) { _company_id = value; NotifyPropertyChanged();} } }
	private long _id;
	public long id { get => _id; set { if (_id != value) { _id = value; NotifyPropertyChanged();} } }
	private DateTime? _order_date;
	public DateTime? order_date { get => _order_date; set { if (_order_date != value) { _order_date = value; NotifyPropertyChanged();} } }
	private int _supplier_id;
	public int supplier_id { get => _supplier_id; set { if (_supplier_id != value) { _supplier_id = value; NotifyPropertyChanged();} } }
	private string? _note;
	public string? note { get => _note; set { if (_note != value) { _note = value; NotifyPropertyChanged();} } }
	private DateTime? _sent;
	public DateTime? sent { get => _sent; set { if (_sent != value) { _sent = value; NotifyPropertyChanged();} } }
	private string? _payment_id;
	public string? payment_id { get => _payment_id; set { if (_payment_id != value) { _payment_id = value; NotifyPropertyChanged();} } }
	private string? _delivery_id;
	public string? delivery_id { get => _delivery_id; set { if (_delivery_id != value) { _delivery_id = value; NotifyPropertyChanged();} } }
	private string? _shipping_id;
	public string? shipping_id { get => _shipping_id; set { if (_shipping_id != value) { _shipping_id = value; NotifyPropertyChanged();} } }
	private string? _send_user;
	public string? send_user { get => _send_user; set { if (_send_user != value) { _send_user = value; NotifyPropertyChanged();} } }
	private DateTime? _closed;
	public DateTime? closed { get => _closed; set { if (_closed != value) { _closed = value; NotifyPropertyChanged();} } }
	private DateTime? _added;
	public DateTime? added { get => _added; set { if (_added != value) { _added = value; NotifyPropertyChanged();} } }
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
	private DateTime? _commercial_signed;
	public DateTime? commercial_signed { get => _commercial_signed; set { if (_commercial_signed != value) { _commercial_signed = value; NotifyPropertyChanged();} } }
	private string? _commercial_signer;
	public string? commercial_signer { get => _commercial_signer; set { if (_commercial_signer != value) { _commercial_signer = value; NotifyPropertyChanged();} } }
	private DateTime? _management_signed;
	public DateTime? management_signed { get => _management_signed; set { if (_management_signed != value) { _management_signed = value; NotifyPropertyChanged();} } }
	private string? _management_signer;
	public string? management_signer { get => _management_signer; set { if (_management_signer != value) { _management_signer = value; NotifyPropertyChanged();} } }
	private int? _bank_abi;
	public int? bank_abi { get => _bank_abi; set { if (_bank_abi != value) { _bank_abi = value; NotifyPropertyChanged();} } }
	private int? _bank_cab;
	public int? bank_cab { get => _bank_cab; set { if (_bank_cab != value) { _bank_cab = value; NotifyPropertyChanged();} } }
	private string? _bank_account;
	public string? bank_account { get => _bank_account; set { if (_bank_account != value) { _bank_account = value; NotifyPropertyChanged();} } }
	private bool _use_supplier_codes;
	public bool use_supplier_codes { get => _use_supplier_codes; set { if (_use_supplier_codes != value) { _use_supplier_codes = value; NotifyPropertyChanged();} } }
}