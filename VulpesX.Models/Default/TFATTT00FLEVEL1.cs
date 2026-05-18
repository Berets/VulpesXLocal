namespace VulpesX.Models.Default;
 
public partial class TFATTT00FLEVEL1 : Base 
{
	private string _ftsoci = null!;
	public required string ftsoci { get => _ftsoci; set { if (_ftsoci != value) { _ftsoci = value; NotifyPropertyChanged();} } }
	private int _FTANNO;
	public int FTANNO { get => _FTANNO; set { if (_FTANNO != value) { _FTANNO = value; NotifyPropertyChanged();} } }
	private int _FTNUOR;
	public int FTNUOR { get => _FTNUOR; set { if (_FTNUOR != value) { _FTNUOR = value; NotifyPropertyChanged();} } }
	private DateTime _ftdataora;
	public DateTime ftdataora { get => _ftdataora; set { if (_ftdataora != value) { _ftdataora = value; NotifyPropertyChanged();} } }
	private string? _ftsdid;
	public string? ftsdid { get => _ftsdid; set { if (_ftsdid != value) { _ftsdid = value; NotifyPropertyChanged();} } }
	private string? _ftfilename;
	public string? ftfilename { get => _ftfilename; set { if (_ftfilename != value) { _ftfilename = value; NotifyPropertyChanged();} } }
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