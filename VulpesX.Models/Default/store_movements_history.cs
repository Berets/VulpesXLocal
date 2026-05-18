namespace VulpesX.Models.Default;
 
public partial class store_movements_history : Base 
{
	private string _company_id = null!;
	public required string company_id { get => _company_id; set { if (_company_id != value) { _company_id = value; NotifyPropertyChanged();} } }
	private long _id;
	public long id { get => _id; set { if (_id != value) { _id = value; NotifyPropertyChanged();} } }
	private int _sequence;
	public int sequence { get => _sequence; set { if (_sequence != value) { _sequence = value; NotifyPropertyChanged();} } }
	private DateTime? _date;
	public DateTime? date { get => _date; set { if (_date != value) { _date = value; NotifyPropertyChanged();} } }
	private string _store_id = null!;
	public required string store_id { get => _store_id; set { if (_store_id != value) { _store_id = value; NotifyPropertyChanged();} } }
	private string _product_id = null!;
	public required string product_id { get => _product_id; set { if (_product_id != value) { _product_id = value; NotifyPropertyChanged();} } }
	private string? _causal_id;
	public string? causal_id { get => _causal_id; set { if (_causal_id != value) { _causal_id = value; NotifyPropertyChanged();} } }
	private decimal? _quantity;
	public decimal? quantity { get => _quantity; set { if (_quantity != value) { _quantity = value; NotifyPropertyChanged();} } }
	private string? _document_id;
	public string? document_id { get => _document_id; set { if (_document_id != value) { _document_id = value; NotifyPropertyChanged();} } }
	private DateTime? _document_date;
	public DateTime? document_date { get => _document_date; set { if (_document_date != value) { _document_date = value; NotifyPropertyChanged();} } }
	private int? _document_row;
	public int? document_row { get => _document_row; set { if (_document_row != value) { _document_row = value; NotifyPropertyChanged();} } }
	private string? _note;
	public string? note { get => _note; set { if (_note != value) { _note = value; NotifyPropertyChanged();} } }
	private DateTime? _added;
	public DateTime? added { get => _added; set { if (_added != value) { _added = value; NotifyPropertyChanged();} } }
	private string? _add_user;
	public string? add_user { get => _add_user; set { if (_add_user != value) { _add_user = value; NotifyPropertyChanged();} } }
	private string? _order_id;
	public string? order_id { get => _order_id; set { if (_order_id != value) { _order_id = value; NotifyPropertyChanged();} } }
	private long? _engage_id;
	public long? engage_id { get => _engage_id; set { if (_engage_id != value) { _engage_id = value; NotifyPropertyChanged();} } }
	private string? _lot;
	public string? lot { get => _lot; set { if (_lot != value) { _lot = value; NotifyPropertyChanged();} } }
	private DateTime? _expire;
	public DateTime? expire { get => _expire; set { if (_expire != value) { _expire = value; NotifyPropertyChanged();} } }
	private long? _goods_receipt_id;
	public long? goods_receipt_id { get => _goods_receipt_id; set { if (_goods_receipt_id != value) { _goods_receipt_id = value; NotifyPropertyChanged();} } }
	private string? _goods_location;
	public string? goods_location { get => _goods_location; set { if (_goods_location != value) { _goods_location = value; NotifyPropertyChanged();} } }
	private int? _supplier_id;
	public int? supplier_id { get => _supplier_id; set { if (_supplier_id != value) { _supplier_id = value; NotifyPropertyChanged();} } }
	private int? _document_year;
	public int? document_year { get => _document_year; set { if (_document_year != value) { _document_year = value; NotifyPropertyChanged();} } }
	private string? _supplier_lot;
	public string? supplier_lot { get => _supplier_lot; set { if (_supplier_lot != value) { _supplier_lot = value; NotifyPropertyChanged();} } }
	private decimal _price;
	public decimal price { get => _price; set { if (_price != value) { _price = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}