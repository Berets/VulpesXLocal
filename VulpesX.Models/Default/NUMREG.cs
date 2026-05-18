namespace VulpesX.Models.Default;
 
public partial class NUMREG : Base 
{
	private string _PERSOC = null!;
	public required string PERSOC { get => _PERSOC; set { if (_PERSOC != value) { _PERSOC = value; NotifyPropertyChanged();} } }
	private string _PERCOD = null!;
	public required string PERCOD { get => _PERCOD; set { if (_PERCOD != value) { _PERCOD = value; NotifyPropertyChanged();} } }
	private int _PERANN;
	public int PERANN { get => _PERANN; set { if (_PERANN != value) { _PERANN = value; NotifyPropertyChanged();} } }
	private int? _PERNUM;
	public int? PERNUM { get => _PERNUM; set { if (_PERNUM != value) { _PERNUM = value; NotifyPropertyChanged();} } }
	private string? _PERDE1;
	public string? PERDE1 { get => _PERDE1; set { if (_PERDE1 != value) { _PERDE1 = value; NotifyPropertyChanged();} } }
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