namespace VulpesX.Models.Default;
 
public partial class acq_orders_rows : Base 
{
	private string _company_id = null!;
	public required string company_id { get => _company_id; set { if (_company_id != value) { _company_id = value; NotifyPropertyChanged();} } }
	private long _id;
	public long id { get => _id; set { if (_id != value) { _id = value; NotifyPropertyChanged();} } }
	private int _sequence;
	public int sequence { get => _sequence; set { if (_sequence != value) { _sequence = value; NotifyPropertyChanged();} } }
	private string? _product_id;
	public string? product_id { get => _product_id; set { if (_product_id != value) { _product_id = value; NotifyPropertyChanged();} } }
	private string? _store_id;
	public string? store_id { get => _store_id; set { if (_store_id != value) { _store_id = value; NotifyPropertyChanged();} } }
	private decimal? _quantity;
	public decimal? quantity { get => _quantity; set { if (_quantity != value) { _quantity = value; NotifyPropertyChanged();} } }
	private decimal? _discount;
	public decimal? discount { get => _discount; set { if (_discount != value) { _discount = value; NotifyPropertyChanged();} } }
	private decimal? _price;
	public decimal? price { get => _price; set { if (_price != value) { _price = value; NotifyPropertyChanged();} } }
	private DateTime? _delivery_requested;
	public DateTime? delivery_requested { get => _delivery_requested; set { if (_delivery_requested != value) { _delivery_requested = value; NotifyPropertyChanged();} } }
	private string? _note;
	public string? note { get => _note; set { if (_note != value) { _note = value; NotifyPropertyChanged();} } }
	private string? _discount_type;
	public string? discount_type { get => _discount_type; set { if (_discount_type != value) { _discount_type = value; NotifyPropertyChanged();} } }
	private decimal? _quantity_received;
	public decimal? quantity_received { get => _quantity_received; set { if (_quantity_received != value) { _quantity_received = value; NotifyPropertyChanged();} } }
	private decimal? _price_second_um;
	public decimal? price_second_um { get => _price_second_um; set { if (_price_second_um != value) { _price_second_um = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
	private string _tax_code = null!;
	public required string tax_code { get => _tax_code; set { if (_tax_code != value) { _tax_code = value; NotifyPropertyChanged();} } }
	private string _tax_rate = null!;
	public required string tax_rate { get => _tax_rate; set { if (_tax_rate != value) { _tax_rate = value; NotifyPropertyChanged();} } }
	private string? _price_type;
	public string? price_type { get => _price_type; set { if (_price_type != value) { _price_type = value; NotifyPropertyChanged();} } }
	private bool _is_closed;
	public bool is_closed { get => _is_closed; set { if (_is_closed != value) { _is_closed = value; NotifyPropertyChanged();} } }
	private string? _unit_id;
	public string? unit_id { get => _unit_id; set { if (_unit_id != value) { _unit_id = value; NotifyPropertyChanged();} } }
}