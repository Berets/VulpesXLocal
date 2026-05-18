namespace VulpesX.Models.Default;
 
public partial class GXOB_purchases : Base 
{
	private string _customer_vat = null!;
	public required string customer_vat { get => _customer_vat; set { if (_customer_vat != value) { _customer_vat = value; NotifyPropertyChanged();} } }
	private string _service_id = null!;
	public required string service_id { get => _service_id; set { if (_service_id != value) { _service_id = value; NotifyPropertyChanged();} } }
	private DateTime _istant;
	public DateTime istant { get => _istant; set { if (_istant != value) { _istant = value; NotifyPropertyChanged();} } }
	private string? _service_details;
	public string? service_details { get => _service_details; set { if (_service_details != value) { _service_details = value; NotifyPropertyChanged();} } }
	private string? _note;
	public string? note { get => _note; set { if (_note != value) { _note = value; NotifyPropertyChanged();} } }
	private DateTime? _added;
	public DateTime? added { get => _added; set { if (_added != value) { _added = value; NotifyPropertyChanged();} } }
	private DateTime? _canceled;
	public DateTime? canceled { get => _canceled; set { if (_canceled != value) { _canceled = value; NotifyPropertyChanged();} } }
	private string? _cancel_reason;
	public string? cancel_reason { get => _cancel_reason; set { if (_cancel_reason != value) { _cancel_reason = value; NotifyPropertyChanged();} } }
	private string? _remote_ip;
	public string? remote_ip { get => _remote_ip; set { if (_remote_ip != value) { _remote_ip = value; NotifyPropertyChanged();} } }
	private string? _company_id;
	public string? company_id { get => _company_id; set { if (_company_id != value) { _company_id = value; NotifyPropertyChanged();} } }
	private string? _request_result;
	public string? request_result { get => _request_result; set { if (_request_result != value) { _request_result = value; NotifyPropertyChanged();} } }
	private DateTime? _billed;
	public DateTime? billed { get => _billed; set { if (_billed != value) { _billed = value; NotifyPropertyChanged();} } }
	private string? _invoice_id;
	public string? invoice_id { get => _invoice_id; set { if (_invoice_id != value) { _invoice_id = value; NotifyPropertyChanged();} } }
}