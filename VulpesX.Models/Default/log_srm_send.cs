namespace VulpesX.Models.Default;
 
public partial class log_srm_send : Base 
{
	private string _company_id = null!;
	public required string company_id { get => _company_id; set { if (_company_id != value) { _company_id = value; NotifyPropertyChanged();} } }
	private DateTime _istant;
	public DateTime istant { get => _istant; set { if (_istant != value) { _istant = value; NotifyPropertyChanged();} } }
	private string _document_type = null!;
	public required string document_type { get => _document_type; set { if (_document_type != value) { _document_type = value; NotifyPropertyChanged();} } }
	private long _document_number;
	public long document_number { get => _document_number; set { if (_document_number != value) { _document_number = value; NotifyPropertyChanged();} } }
	private string? _client_name;
	public string? client_name { get => _client_name; set { if (_client_name != value) { _client_name = value; NotifyPropertyChanged();} } }
	private DateTime? _client_time;
	public DateTime? client_time { get => _client_time; set { if (_client_time != value) { _client_time = value; NotifyPropertyChanged();} } }
	private string? _sent_to;
	public string? sent_to { get => _sent_to; set { if (_sent_to != value) { _sent_to = value; NotifyPropertyChanged();} } }
	private string? _sent_cc;
	public string? sent_cc { get => _sent_cc; set { if (_sent_cc != value) { _sent_cc = value; NotifyPropertyChanged();} } }
	private string? _sent_object;
	public string? sent_object { get => _sent_object; set { if (_sent_object != value) { _sent_object = value; NotifyPropertyChanged();} } }
	private string? _sent_from;
	public string? sent_from { get => _sent_from; set { if (_sent_from != value) { _sent_from = value; NotifyPropertyChanged();} } }
	private string? _sent_attachments;
	public string? sent_attachments { get => _sent_attachments; set { if (_sent_attachments != value) { _sent_attachments = value; NotifyPropertyChanged();} } }
	private string? _result;
	public string? result { get => _result; set { if (_result != value) { _result = value; NotifyPropertyChanged();} } }
	private string? _sendUserID = null!;
	public string? sendUserID { get => _sendUserID; set { if (_sendUserID != value) { _sendUserID = value; NotifyPropertyChanged();} } }
}