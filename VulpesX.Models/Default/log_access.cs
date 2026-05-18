namespace VulpesX.Models.Default;
 
public partial class log_access : Base 
{
	private DateTime _istant;
	public DateTime istant { get => _istant; set { if (_istant != value) { _istant = value; NotifyPropertyChanged();} } }
	private string _username = null!;
	public required string username { get => _username; set { if (_username != value) { _username = value; NotifyPropertyChanged();} } }
	private string? _client_name;
	public string? client_name { get => _client_name; set { if (_client_name != value) { _client_name = value; NotifyPropertyChanged();} } }
	private DateTime? _client_time;
	public DateTime? client_time { get => _client_time; set { if (_client_time != value) { _client_time = value; NotifyPropertyChanged();} } }
	private DateTime? _server_time;
	public DateTime? server_time { get => _server_time; set { if (_server_time != value) { _server_time = value; NotifyPropertyChanged();} } }
	private string? _result;
	public string? result { get => _result; set { if (_result != value) { _result = value; NotifyPropertyChanged();} } }
	private string? _app_version;
	public string? app_version { get => _app_version; set { if (_app_version != value) { _app_version = value; NotifyPropertyChanged();} } }
}