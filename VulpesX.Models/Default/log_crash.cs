namespace VulpesX.Models.Default;
 
public partial class log_crash : Base 
{
	private string _app_domain = null!;
	public required string app_domain { get => _app_domain; set { if (_app_domain != value) { _app_domain = value; NotifyPropertyChanged();} } }
	private DateTime _istant;
	public DateTime istant { get => _istant; set { if (_istant != value) { _istant = value; NotifyPropertyChanged();} } }
	private DateTime? _client_time;
	public DateTime? client_time { get => _client_time; set { if (_client_time != value) { _client_time = value; NotifyPropertyChanged();} } }
	private string? _client_name;
	public string? client_name { get => _client_name; set { if (_client_name != value) { _client_name = value; NotifyPropertyChanged();} } }
	private string? _message;
	public string? message { get => _message; set { if (_message != value) { _message = value; NotifyPropertyChanged();} } }
	private string? _stack_trace;
	public string? stack_trace { get => _stack_trace; set { if (_stack_trace != value) { _stack_trace = value; NotifyPropertyChanged();} } }
	private string? _inner_message;
	public string? inner_message { get => _inner_message; set { if (_inner_message != value) { _inner_message = value; NotifyPropertyChanged();} } }
	private string? _inner_stack_trace;
	public string? inner_stack_trace { get => _inner_stack_trace; set { if (_inner_stack_trace != value) { _inner_stack_trace = value; NotifyPropertyChanged();} } }
}