namespace VulpesX.Models.Default;
 
public partial class ACC_ASSETS_TYPES : Base 
{
	private string _socij = null!;
	public required string socij { get => _socij; set { if (_socij != value) { _socij = value; NotifyPropertyChanged();} } }
	private string _JTICE = null!;
	public required string JTICE { get => _JTICE; set { if (_JTICE != value) { _JTICE = value; NotifyPropertyChanged();} } }
	private string _TCDES = null!;
	public required string TCDES { get => _TCDES; set { if (_TCDES != value) { _TCDES = value; NotifyPropertyChanged();} } }
	private decimal? _TCLINE;
	public decimal? TCLINE { get => _TCLINE; set { if (_TCLINE != value) { _TCLINE = value; NotifyPropertyChanged();} } }
	private string? _TCCALC;
	public string? TCCALC { get => _TCCALC; set { if (_TCCALC != value) { _TCCALC = value; NotifyPropertyChanged();} } }
	private bool _tcalc;
	public bool tcalc { get => _tcalc; set { if (_tcalc != value) { _tcalc = value; NotifyPropertyChanged();} } }
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
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}