namespace VulpesX.Models.Default;
 
public partial class store_stocks_engage : Base 
{
	private string _company_id = null!;
	public required string company_id { get => _company_id; set { if (_company_id != value) { _company_id = value; NotifyPropertyChanged();} } }
	private long _id;
	public long id { get => _id; set { if (_id != value) { _id = value; NotifyPropertyChanged();} } }
	private string _store_id = null!;
	public required string store_id { get => _store_id; set { if (_store_id != value) { _store_id = value; NotifyPropertyChanged();} } }
	private string _product_id = null!;
	public required string product_id { get => _product_id; set { if (_product_id != value) { _product_id = value; NotifyPropertyChanged();} } }
	private string? _job_id;
	public string? job_id { get => _job_id; set { if (_job_id != value) { _job_id = value; NotifyPropertyChanged();} } }
	private string? _order_id;
	public string? order_id { get => _order_id; set { if (_order_id != value) { _order_id = value; NotifyPropertyChanged();} } }
	private string? _ward_id;
	public string? ward_id { get => _ward_id; set { if (_ward_id != value) { _ward_id = value; NotifyPropertyChanged();} } }
	private string? _document_id;
	public string? document_id { get => _document_id; set { if (_document_id != value) { _document_id = value; NotifyPropertyChanged();} } }
	private decimal? _quantity;
	public decimal? quantity { get => _quantity; set { if (_quantity != value) { _quantity = value; NotifyPropertyChanged();} } }
	private DateTime? _date_engaged;
	public DateTime? date_engaged { get => _date_engaged; set { if (_date_engaged != value) { _date_engaged = value; NotifyPropertyChanged();} } }
	private DateTime? _date_unloaded;
	public DateTime? date_unloaded { get => _date_unloaded; set { if (_date_unloaded != value) { _date_unloaded = value; NotifyPropertyChanged();} } }
	private DateTime? _added;
	public DateTime? added { get => _added; set { if (_added != value) { _added = value; NotifyPropertyChanged();} } }
	private string? _add_user;
	public string? add_user { get => _add_user; set { if (_add_user != value) { _add_user = value; NotifyPropertyChanged();} } }
	private DateTime? _updated;
	public DateTime? updated { get => _updated; set { if (_updated != value) { _updated = value; NotifyPropertyChanged();} } }
	private string? _update_user;
	public string? update_user { get => _update_user; set { if (_update_user != value) { _update_user = value; NotifyPropertyChanged();} } }
	private DateTime? _canceled;
	public DateTime? canceled { get => _canceled; set { if (_canceled != value) { _canceled = value; NotifyPropertyChanged();} } }
	private string? _cancel_user;
	public string? cancel_user { get => _cancel_user; set { if (_cancel_user != value) { _cancel_user = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
	private string? _lot;
	public string? lot { get => _lot; set { if (_lot != value) { _lot = value; NotifyPropertyChanged();} } }
	private int? _ddt_year;
	public int? ddt_year { get => _ddt_year; set { if (_ddt_year != value) { _ddt_year = value; NotifyPropertyChanged();} } }
	private int? _ddt_number;
	public int? ddt_number { get => _ddt_number; set { if (_ddt_number != value) { _ddt_number = value; NotifyPropertyChanged();} } }
	private int? _ddt_row;
	public int? ddt_row { get => _ddt_row; set { if (_ddt_row != value) { _ddt_row = value; NotifyPropertyChanged();} } }
	private int? _order_year;
	public int? order_year { get => _order_year; set { if (_order_year != value) { _order_year = value; NotifyPropertyChanged();} } }
	private int? _order_number;
	public int? order_number { get => _order_number; set { if (_order_number != value) { _order_number = value; NotifyPropertyChanged();} } }
	private int? _order_row;
	public int? order_row { get => _order_row; set { if (_order_row != value) { _order_row = value; NotifyPropertyChanged();} } }
}