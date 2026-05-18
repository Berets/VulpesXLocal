namespace VulpesX.Models.Default;
 
public partial class acq_orders_rows_rdas : Base 
{
	private string _company_id = null!;
	public required string company_id { get => _company_id; set { if (_company_id != value) { _company_id = value; NotifyPropertyChanged();} } }
	private long _order_id;
	public long order_id { get => _order_id; set { if (_order_id != value) { _order_id = value; NotifyPropertyChanged();} } }
	private int _order_row_id;
	public int order_row_id { get => _order_row_id; set { if (_order_row_id != value) { _order_row_id = value; NotifyPropertyChanged();} } }
	private long _rda_id;
	public long rda_id { get => _rda_id; set { if (_rda_id != value) { _rda_id = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}