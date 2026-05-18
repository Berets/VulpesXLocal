namespace VulpesX.Models.Default;
 
public partial class acq_goods_receipts : Base 
{
	private string _company_id = null!;
	public required string company_id { get => _company_id; set { if (_company_id != value) { _company_id = value; NotifyPropertyChanged();} } }
	private long _id;
	public long id { get => _id; set { if (_id != value) { _id = value; NotifyPropertyChanged();} } }
	private int _supplier_id;
	public int supplier_id { get => _supplier_id; set { if (_supplier_id != value) { _supplier_id = value; NotifyPropertyChanged();} } }
	private DateTime _document_date;
	public DateTime document_date { get => _document_date; set { if (_document_date != value) { _document_date = value; NotifyPropertyChanged();} } }
	private string _document_number = null!;
	public required string document_number { get => _document_number; set { if (_document_number != value) { _document_number = value; NotifyPropertyChanged();} } }
	private int _document_row;
	public int document_row { get => _document_row; set { if (_document_row != value) { _document_row = value; NotifyPropertyChanged();} } }
	private string? _product_id;
	public string? product_id { get => _product_id; set { if (_product_id != value) { _product_id = value; NotifyPropertyChanged();} } }
	private decimal? _quantity;
	public decimal? quantity { get => _quantity; set { if (_quantity != value) { _quantity = value; NotifyPropertyChanged();} } }
	private string? _store_id;
	public string? store_id { get => _store_id; set { if (_store_id != value) { _store_id = value; NotifyPropertyChanged();} } }
	private string? _causal_id;
	public string? causal_id { get => _causal_id; set { if (_causal_id != value) { _causal_id = value; NotifyPropertyChanged();} } }
	private string? _lot;
	public string? lot { get => _lot; set { if (_lot != value) { _lot = value; NotifyPropertyChanged();} } }
	private DateTime? _expire;
	public DateTime? expire { get => _expire; set { if (_expire != value) { _expire = value; NotifyPropertyChanged();} } }
	private long? _order_id;
	public long? order_id { get => _order_id; set { if (_order_id != value) { _order_id = value; NotifyPropertyChanged();} } }
	private int? _order_row_id;
	public int? order_row_id { get => _order_row_id; set { if (_order_row_id != value) { _order_row_id = value; NotifyPropertyChanged();} } }
	private string? _note;
	public string? note { get => _note; set { if (_note != value) { _note = value; NotifyPropertyChanged();} } }
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
	private string? _goods_location;
	public string? goods_location { get => _goods_location; set { if (_goods_location != value) { _goods_location = value; NotifyPropertyChanged();} } }
	private string? _unit_id;
	public string? unit_id { get => _unit_id; set { if (_unit_id != value) { _unit_id = value; NotifyPropertyChanged();} } }
	private string? _supplier_lot;
	public string? supplier_lot { get => _supplier_lot; set { if (_supplier_lot != value) { _supplier_lot = value; NotifyPropertyChanged();} } }
}