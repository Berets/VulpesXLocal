namespace VulpesX.Models.Default;
 
public partial class PAGCLI : Base 
{
	private string _pclcod = null!;
	public required string pclcod { get => _pclcod; set { if (_pclcod != value) { _pclcod = value; NotifyPropertyChanged();} } }
	private string _pcldes = null!;
	public required string pcldes { get => _pcldes; set { if (_pcldes != value) { _pcldes = value; NotifyPropertyChanged();} } }
	private int? _pclgs1;
	public int? pclgs1 { get => _pclgs1; set { if (_pclgs1 != value) { _pclgs1 = value; NotifyPropertyChanged();} } }
	private int? _pclgs2;
	public int? pclgs2 { get => _pclgs2; set { if (_pclgs2 != value) { _pclgs2 = value; NotifyPropertyChanged();} } }
	private int? _pclgs3;
	public int? pclgs3 { get => _pclgs3; set { if (_pclgs3 != value) { _pclgs3 = value; NotifyPropertyChanged();} } }
	private int? _pclgs4;
	public int? pclgs4 { get => _pclgs4; set { if (_pclgs4 != value) { _pclgs4 = value; NotifyPropertyChanged();} } }
	private int? _pclgs5;
	public int? pclgs5 { get => _pclgs5; set { if (_pclgs5 != value) { _pclgs5 = value; NotifyPropertyChanged();} } }
	private int? _pclgs6;
	public int? pclgs6 { get => _pclgs6; set { if (_pclgs6 != value) { _pclgs6 = value; NotifyPropertyChanged();} } }
	private int? _pclgs7;
	public int? pclgs7 { get => _pclgs7; set { if (_pclgs7 != value) { _pclgs7 = value; NotifyPropertyChanged();} } }
	private int? _pclgs8;
	public int? pclgs8 { get => _pclgs8; set { if (_pclgs8 != value) { _pclgs8 = value; NotifyPropertyChanged();} } }
	private int? _pclgs9;
	public int? pclgs9 { get => _pclgs9; set { if (_pclgs9 != value) { _pclgs9 = value; NotifyPropertyChanged();} } }
	private int? _pclmed;
	public int? pclmed { get => _pclmed; set { if (_pclmed != value) { _pclmed = value; NotifyPropertyChanged();} } }
	private string? _pclppa;
	public string? pclppa { get => _pclppa; set { if (_pclppa != value) { _pclppa = value; NotifyPropertyChanged();} } }
	private decimal? _pclsco;
	public decimal? pclsco { get => _pclsco; set { if (_pclsco != value) { _pclsco = value; NotifyPropertyChanged();} } }
	private decimal? _pclmag;
	public decimal? pclmag { get => _pclmag; set { if (_pclmag != value) { _pclmag = value; NotifyPropertyChanged();} } }
	private string? _pcltip;
	public string? pcltip { get => _pcltip; set { if (_pcltip != value) { _pcltip = value; NotifyPropertyChanged();} } }
	private int? _pclgio;
	public int? pclgio { get => _pclgio; set { if (_pclgio != value) { _pclgio = value; NotifyPropertyChanged();} } }
	private string? _pclold;
	public string? pclold { get => _pclold; set { if (_pclold != value) { _pclold = value; NotifyPropertyChanged();} } }
	private string? _pcivacas;
	public string? pcivacas { get => _pcivacas; set { if (_pcivacas != value) { _pcivacas = value; NotifyPropertyChanged();} } }
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