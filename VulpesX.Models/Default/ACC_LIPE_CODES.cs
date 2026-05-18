namespace VulpesX.Models.Default;
 
public partial class ACC_LIPE_CODES : Base 
{
	private string _id = null!;
	public required string id { get => _id; set { if (_id != value) { _id = value; NotifyPropertyChanged();} } }
	private string _description = null!;
	public required string description { get => _description; set { if (_description != value) { _description = value; NotifyPropertyChanged();} } }
	private string _codetype = null!;
	public required string codetype { get => _codetype; set { if (_codetype != value) { _codetype = value; NotifyPropertyChanged();} } }
	private int _sequence;
	public int sequence { get => _sequence; set { if (_sequence != value) { _sequence = value; NotifyPropertyChanged();} } }
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