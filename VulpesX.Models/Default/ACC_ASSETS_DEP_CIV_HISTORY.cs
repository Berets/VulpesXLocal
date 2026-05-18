namespace VulpesX.Models.Default;
 
public partial class ACC_ASSETS_DEP_CIV_HISTORY : Base 
{
	private string _bcsoci = null!;
	public required string bcsoci { get => _bcsoci; set { if (_bcsoci != value) { _bcsoci = value; NotifyPropertyChanged();} } }
	private int _bcanco;
	public int bcanco { get => _bcanco; set { if (_bcanco != value) { _bcanco = value; NotifyPropertyChanged();} } }
	private int _bcann4;
	public int bcann4 { get => _bcann4; set { if (_bcann4 != value) { _bcann4 = value; NotifyPropertyChanged();} } }
	private string _bcgrup = null!;
	public required string bcgrup { get => _bcgrup; set { if (_bcgrup != value) { _bcgrup = value; NotifyPropertyChanged();} } }
	private string _bccont = null!;
	public required string bccont { get => _bccont; set { if (_bccont != value) { _bccont = value; NotifyPropertyChanged();} } }
	private string _bcsotc = null!;
	public required string bcsotc { get => _bcsotc; set { if (_bcsotc != value) { _bcsotc = value; NotifyPropertyChanged();} } }
	private int _bcinv2;
	public int bcinv2 { get => _bcinv2; set { if (_bcinv2 != value) { _bcinv2 = value; NotifyPropertyChanged();} } }
	private int _bcinv;
	public int bcinv { get => _bcinv; set { if (_bcinv != value) { _bcinv = value; NotifyPropertyChanged();} } }
	private decimal? _bcval;
	public decimal? bcval { get => _bcval; set { if (_bcval != value) { _bcval = value; NotifyPropertyChanged();} } }
	private decimal? _bcpea;
	public decimal? bcpea { get => _bcpea; set { if (_bcpea != value) { _bcpea = value; NotifyPropertyChanged();} } }
	private decimal? _bcanne;
	public decimal? bcanne { get => _bcanne; set { if (_bcanne != value) { _bcanne = value; NotifyPropertyChanged();} } }
	private DateTime? _bcdaip;
	public DateTime? bcdaip { get => _bcdaip; set { if (_bcdaip != value) { _bcdaip = value; NotifyPropertyChanged();} } }
	private DateTime? _bcdafp;
	public DateTime? bcdafp { get => _bcdafp; set { if (_bcdafp != value) { _bcdafp = value; NotifyPropertyChanged();} } }
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