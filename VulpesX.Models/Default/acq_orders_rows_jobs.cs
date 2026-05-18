namespace VulpesX.Models.Default;
 
public partial class acq_orders_rows_jobs : Base 
{
	private string _company_id = null!;
	public required string company_id { get => _company_id; set { if (_company_id != value) { _company_id = value; NotifyPropertyChanged();} } }
	private long _order_id;
	public long order_id { get => _order_id; set { if (_order_id != value) { _order_id = value; NotifyPropertyChanged();} } }
	private int _order_row_id;
	public int order_row_id { get => _order_row_id; set { if (_order_row_id != value) { _order_row_id = value; NotifyPropertyChanged();} } }
	private string _job_id = null!;
	public required string job_id { get => _job_id; set { if (_job_id != value) { _job_id = value; NotifyPropertyChanged();} } }
	private decimal _quantity_needed;
	public decimal quantity_needed { get => _quantity_needed; set { if (_quantity_needed != value) { _quantity_needed = value; NotifyPropertyChanged();} } }
	private decimal _quantity_received;
	public decimal quantity_received { get => _quantity_received; set { if (_quantity_received != value) { _quantity_received = value; NotifyPropertyChanged();} } }
	private decimal? _quantity_original;
	public decimal? quantity_original { get => _quantity_original; set { if (_quantity_original != value) { _quantity_original = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}