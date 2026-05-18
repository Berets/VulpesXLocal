namespace VulpesX.Models.Default;
 
public partial class store_causals : Base 
{
	private string _company_id = null!;
	public required string company_id { get => _company_id; set { if (_company_id != value) { _company_id = value; NotifyPropertyChanged();} } }
	private string _id = null!;
	public required string id { get => _id; set { if (_id != value) { _id = value; NotifyPropertyChanged();} } }
	private string? _description;
	public string? description { get => _description; set { if (_description != value) { _description = value; NotifyPropertyChanged();} } }
	private string? _sign;
	public string? sign { get => _sign; set { if (_sign != value) { _sign = value; NotifyPropertyChanged();} } }
	private bool _is_default_raw_unload;
	public bool is_default_raw_unload { get => _is_default_raw_unload; set { if (_is_default_raw_unload != value) { _is_default_raw_unload = value; NotifyPropertyChanged();} } }
	private bool _is_default_final_unload;
	public bool is_default_final_unload { get => _is_default_final_unload; set { if (_is_default_final_unload != value) { _is_default_final_unload = value; NotifyPropertyChanged();} } }
	private bool _is_default_final_load;
	public bool is_default_final_load { get => _is_default_final_load; set { if (_is_default_final_load != value) { _is_default_final_load = value; NotifyPropertyChanged();} } }
	private bool _is_default_external_discharge;
	public bool is_default_external_discharge { get => _is_default_external_discharge; set { if (_is_default_external_discharge != value) { _is_default_external_discharge = value; NotifyPropertyChanged();} } }
	private bool _is_default_external_charge;
	public bool is_default_external_charge { get => _is_default_external_charge; set { if (_is_default_external_charge != value) { _is_default_external_charge = value; NotifyPropertyChanged();} } }
	private bool _is_default_raw_load;
	public bool is_default_raw_load { get => _is_default_raw_load; set { if (_is_default_raw_load != value) { _is_default_raw_load = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
	private bool _is_default_half_load;
	public bool is_default_half_load { get => _is_default_half_load; set { if (_is_default_half_load != value) { _is_default_half_load = value; NotifyPropertyChanged();} } }
	private bool _is_default_half_unload;
	public bool is_default_half_unload { get => _is_default_half_unload; set { if (_is_default_half_unload != value) { _is_default_half_unload = value; NotifyPropertyChanged();} } }
	private bool _is_default_infinite_unload;
	public bool is_default_infinite_unload { get => _is_default_infinite_unload; set { if (_is_default_infinite_unload != value) { _is_default_infinite_unload = value; NotifyPropertyChanged();} } }
	private string? _cost_center_id;
	public string? cost_center_id { get => _cost_center_id; set { if (_cost_center_id != value) { _cost_center_id = value; NotifyPropertyChanged();} } }
	private string? _link_causal_id;
	public string? link_causal_id { get => _link_causal_id; set { if (_link_causal_id != value) { _link_causal_id = value; NotifyPropertyChanged();} } }
	private string? _link_store_id;
	public string? link_store_id { get => _link_store_id; set { if (_link_store_id != value) { _link_store_id = value; NotifyPropertyChanged();} } }
}