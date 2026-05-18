namespace VulpesX.Models.Default;
 
public partial class store_stores : Base 
{
	private string _company_id = null!;
	public required string company_id { get => _company_id; set { if (_company_id != value) { _company_id = value; NotifyPropertyChanged();} } }
	private string _id = null!;
	public required string id { get => _id; set { if (_id != value) { _id = value; NotifyPropertyChanged();} } }
	private string? _description;
	public string? description { get => _description; set { if (_description != value) { _description = value; NotifyPropertyChanged();} } }
	private string? _type;
	public string? type { get => _type; set { if (_type != value) { _type = value; NotifyPropertyChanged();} } }
	private bool _is_default_final_load;
	public bool is_default_final_load { get => _is_default_final_load; set { if (_is_default_final_load != value) { _is_default_final_load = value; NotifyPropertyChanged();} } }
	private bool _is_default_half;
	public bool is_default_half { get => _is_default_half; set { if (_is_default_half != value) { _is_default_half = value; NotifyPropertyChanged();} } }
	private bool _is_default_raw;
	public bool is_default_raw { get => _is_default_raw; set { if (_is_default_raw != value) { _is_default_raw = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
	private bool _is_default_infinite;
	public bool is_default_infinite { get => _is_default_infinite; set { if (_is_default_infinite != value) { _is_default_infinite = value; NotifyPropertyChanged();} } }
	private DateTime? _added;
	public DateTime? added { get => _added; set { if (_added != value) { _added = value; NotifyPropertyChanged();} } }
	private DateTime? _updated;
	public DateTime? updated { get => _updated; set { if (_updated != value) { _updated = value; NotifyPropertyChanged();} } }
	private DateTime? _canceled;
	public DateTime? canceled { get => _canceled; set { if (_canceled != value) { _canceled = value; NotifyPropertyChanged();} } }
	private string? _addedUserID;
	public string? addedUserID { get => _addedUserID; set { if (_addedUserID != value) { _addedUserID = value; NotifyPropertyChanged();} } }
	private string? _updatedUserID;
	public string? updatedUserID { get => _updatedUserID; set { if (_updatedUserID != value) { _updatedUserID = value; NotifyPropertyChanged();} } }
	private string? _canceledUserID;
	public string? canceledUserID { get => _canceledUserID; set { if (_canceledUserID != value) { _canceledUserID = value; NotifyPropertyChanged();} } }
	private string? _canceledNote;
	public string? canceledNote { get => _canceledNote; set { if (_canceledNote != value) { _canceledNote = value; NotifyPropertyChanged();} } }
}