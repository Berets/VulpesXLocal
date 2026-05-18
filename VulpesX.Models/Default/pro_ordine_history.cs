namespace VulpesX.Models.Default;
 
public partial class pro_ordine_history : Base 
{
	private string _company_id = null!;
	public required string company_id { get => _company_id; set { if (_company_id != value) { _company_id = value; NotifyPropertyChanged();} } }
	private string _id = null!;
	public required string id { get => _id; set { if (_id != value) { _id = value; NotifyPropertyChanged();} } }
	private DateTime _istant;
	public DateTime istant { get => _istant; set { if (_istant != value) { _istant = value; NotifyPropertyChanged();} } }
	private string _operation = null!;
	public required string operation { get => _operation; set { if (_operation != value) { _operation = value; NotifyPropertyChanged();} } }
	private bool _cancelled_engages;
	public bool cancelled_engages { get => _cancelled_engages; set { if (_cancelled_engages != value) { _cancelled_engages = value; NotifyPropertyChanged();} } }
	private decimal _production_quantity;
	public decimal production_quantity { get => _production_quantity; set { if (_production_quantity != value) { _production_quantity = value; NotifyPropertyChanged();} } }
	private decimal _engaged_quantity;
	public decimal engaged_quantity { get => _engaged_quantity; set { if (_engaged_quantity != value) { _engaged_quantity = value; NotifyPropertyChanged();} } }
	private string? _previous_state;
	public string? previous_state { get => _previous_state; set { if (_previous_state != value) { _previous_state = value; NotifyPropertyChanged();} } }
	private string _username = null!;
	public required string username { get => _username; set { if (_username != value) { _username = value; NotifyPropertyChanged();} } }
	private string _client_name = null!;
	public required string client_name { get => _client_name; set { if (_client_name != value) { _client_name = value; NotifyPropertyChanged();} } }
}