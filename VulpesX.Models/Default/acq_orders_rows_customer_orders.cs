namespace VulpesX.Models.Default;
 
public partial class acq_orders_rows_customer_orders : Base 
{
	private string _company_id = null!;
	public required string company_id { get => _company_id; set { if (_company_id != value) { _company_id = value; NotifyPropertyChanged();} } }
	private long _order_id;
	public long order_id { get => _order_id; set { if (_order_id != value) { _order_id = value; NotifyPropertyChanged();} } }
	private int _order_row_id;
	public int order_row_id { get => _order_row_id; set { if (_order_row_id != value) { _order_row_id = value; NotifyPropertyChanged();} } }
	private int _customer_order_year;
	public int customer_order_year { get => _customer_order_year; set { if (_customer_order_year != value) { _customer_order_year = value; NotifyPropertyChanged();} } }
	private int _customer_order_number;
	public int customer_order_number { get => _customer_order_number; set { if (_customer_order_number != value) { _customer_order_number = value; NotifyPropertyChanged();} } }
	private int _customer_order_row;
	public int customer_order_row { get => _customer_order_row; set { if (_customer_order_row != value) { _customer_order_row = value; NotifyPropertyChanged();} } }
	private decimal _quantity_needed;
	public decimal quantity_needed { get => _quantity_needed; set { if (_quantity_needed != value) { _quantity_needed = value; NotifyPropertyChanged();} } }
	private decimal _quantity_received;
	public decimal quantity_received { get => _quantity_received; set { if (_quantity_received != value) { _quantity_received = value; NotifyPropertyChanged();} } }
	private decimal? _quantity_original;
	public decimal? quantity_original { get => _quantity_original; set { if (_quantity_original != value) { _quantity_original = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}