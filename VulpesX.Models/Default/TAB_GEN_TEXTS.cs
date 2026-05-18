namespace VulpesX.Models.Default;
 
public partial class TAB_GEN_TEXTS : Base 
{
	private string _TTxsoc = null!;
	public required string TTxsoc { get => _TTxsoc; set { if (_TTxsoc != value) { _TTxsoc = value; NotifyPropertyChanged();} } }
	private string _TTxcod = null!;
	public required string TTxcod { get => _TTxcod; set { if (_TTxcod != value) { _TTxcod = value; NotifyPropertyChanged();} } }
	private string _TTXtip = null!;
	public required string TTXtip { get => _TTXtip; set { if (_TTXtip != value) { _TTXtip = value; NotifyPropertyChanged();} } }
	private string _TTxdes = null!;
	public required string TTxdes { get => _TTxdes; set { if (_TTxdes != value) { _TTxdes = value; NotifyPropertyChanged();} } }
	private string _TTXNote = null!;
	public required string TTXNote { get => _TTXNote; set { if (_TTXNote != value) { _TTXNote = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
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