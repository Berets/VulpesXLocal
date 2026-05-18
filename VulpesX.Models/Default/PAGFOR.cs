namespace VulpesX.Models.Default;
 
public partial class PAGFOR : Base 
{
	private string _pfocod = null!;
	public required string pfocod { get => _pfocod; set { if (_pfocod != value) { _pfocod = value; NotifyPropertyChanged();} } }
	private string _pfodes = null!;
	public required string pfodes { get => _pfodes; set { if (_pfodes != value) { _pfodes = value; NotifyPropertyChanged();} } }
	private int? _pfogs1;
	public int? pfogs1 { get => _pfogs1; set { if (_pfogs1 != value) { _pfogs1 = value; NotifyPropertyChanged();} } }
	private int? _pfogs2;
	public int? pfogs2 { get => _pfogs2; set { if (_pfogs2 != value) { _pfogs2 = value; NotifyPropertyChanged();} } }
	private int? _pfogs3;
	public int? pfogs3 { get => _pfogs3; set { if (_pfogs3 != value) { _pfogs3 = value; NotifyPropertyChanged();} } }
	private int? _pfogs4;
	public int? pfogs4 { get => _pfogs4; set { if (_pfogs4 != value) { _pfogs4 = value; NotifyPropertyChanged();} } }
	private int? _pfogs5;
	public int? pfogs5 { get => _pfogs5; set { if (_pfogs5 != value) { _pfogs5 = value; NotifyPropertyChanged();} } }
	private int? _pfogs6;
	public int? pfogs6 { get => _pfogs6; set { if (_pfogs6 != value) { _pfogs6 = value; NotifyPropertyChanged();} } }
	private int? _pfogs7;
	public int? pfogs7 { get => _pfogs7; set { if (_pfogs7 != value) { _pfogs7 = value; NotifyPropertyChanged();} } }
	private int? _pfogs8;
	public int? pfogs8 { get => _pfogs8; set { if (_pfogs8 != value) { _pfogs8 = value; NotifyPropertyChanged();} } }
	private int? _pfogs9;
	public int? pfogs9 { get => _pfogs9; set { if (_pfogs9 != value) { _pfogs9 = value; NotifyPropertyChanged();} } }
	private int? _pfomed;
	public int? pfomed { get => _pfomed; set { if (_pfomed != value) { _pfomed = value; NotifyPropertyChanged();} } }
	private string? _pfoppa;
	public string? pfoppa { get => _pfoppa; set { if (_pfoppa != value) { _pfoppa = value; NotifyPropertyChanged();} } }
	private decimal? _pfosco;
	public decimal? pfosco { get => _pfosco; set { if (_pfosco != value) { _pfosco = value; NotifyPropertyChanged();} } }
	private decimal? _pfomag;
	public decimal? pfomag { get => _pfomag; set { if (_pfomag != value) { _pfomag = value; NotifyPropertyChanged();} } }
	private string? _pfotip;
	public string? pfotip { get => _pfotip; set { if (_pfotip != value) { _pfotip = value; NotifyPropertyChanged();} } }
	private string? _pfivacas;
	public string? pfivacas { get => _pfivacas; set { if (_pfivacas != value) { _pfivacas = value; NotifyPropertyChanged();} } }
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