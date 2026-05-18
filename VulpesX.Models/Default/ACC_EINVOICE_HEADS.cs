namespace VulpesX.Models.Default;
 
public partial class ACC_EINVOICE_HEADS : Base 
{
	private string _fattsoc = null!;
	public required string fattsoc { get => _fattsoc; set { if (_fattsoc != value) { _fattsoc = value; NotifyPropertyChanged();} } }
	private string _fattnum = null!;
	public required string fattnum { get => _fattnum; set { if (_fattnum != value) { _fattnum = value; NotifyPropertyChanged();} } }
	private DateTime _fattdata;
	public DateTime fattdata { get => _fattdata; set { if (_fattdata != value) { _fattdata = value; NotifyPropertyChanged();} } }
	private string _fattpiva = null!;
	public required string fattpiva { get => _fattpiva; set { if (_fattpiva != value) { _fattpiva = value; NotifyPropertyChanged();} } }
	private string? _fattabers1;
	public string? fattabers1 { get => _fattabers1; set { if (_fattabers1 != value) { _fattabers1 = value; NotifyPropertyChanged();} } }
	private int? _fattfor;
	public int? fattfor { get => _fattfor; set { if (_fattfor != value) { _fattfor = value; NotifyPropertyChanged();} } }
	private decimal? _fattimpo;
	public decimal? fattimpo { get => _fattimpo; set { if (_fattimpo != value) { _fattimpo = value; NotifyPropertyChanged();} } }
	private decimal? _fattimposta;
	public decimal? fattimposta { get => _fattimposta; set { if (_fattimposta != value) { _fattimposta = value; NotifyPropertyChanged();} } }
	private decimal? _fatttot;
	public decimal? fatttot { get => _fatttot; set { if (_fatttot != value) { _fatttot = value; NotifyPropertyChanged();} } }
	private int? _fattannoreg;
	public int? fattannoreg { get => _fattannoreg; set { if (_fattannoreg != value) { _fattannoreg = value; NotifyPropertyChanged();} } }
	private int? _fattnumreg;
	public int? fattnumreg { get => _fattnumreg; set { if (_fattnumreg != value) { _fattnumreg = value; NotifyPropertyChanged();} } }
	private DateTime? _fattdataric;
	public DateTime? fattdataric { get => _fattdataric; set { if (_fattdataric != value) { _fattdataric = value; NotifyPropertyChanged();} } }
	private string? _fattnomefileric;
	public string? fattnomefileric { get => _fattnomefileric; set { if (_fattnomefileric != value) { _fattnomefileric = value; NotifyPropertyChanged();} } }
	private string? _fattcaus;
	public string? fattcaus { get => _fattcaus; set { if (_fattcaus != value) { _fattcaus = value; NotifyPropertyChanged();} } }
	private string? _fattcausaledes;
	public string? fattcausaledes { get => _fattcausaledes; set { if (_fattcausaledes != value) { _fattcausaledes = value; NotifyPropertyChanged();} } }
	private string? _fattregi;
	public string? fattregi { get => _fattregi; set { if (_fattregi != value) { _fattregi = value; NotifyPropertyChanged();} } }
	private string? _fattpec;
	public string? fattpec { get => _fattpec; set { if (_fattpec != value) { _fattpec = value; NotifyPropertyChanged();} } }
	private string? _fattcoddest;
	public string? fattcoddest { get => _fattcoddest; set { if (_fattcoddest != value) { _fattcoddest = value; NotifyPropertyChanged();} } }
	private string? _fattiso;
	public string? fattiso { get => _fattiso; set { if (_fattiso != value) { _fattiso = value; NotifyPropertyChanged();} } }
	private decimal? _fattimpobol;
	public decimal? fattimpobol { get => _fattimpobol; set { if (_fattimpobol != value) { _fattimpobol = value; NotifyPropertyChanged();} } }
	private string? _fattfornazi;
	public string? fattfornazi { get => _fattfornazi; set { if (_fattfornazi != value) { _fattfornazi = value; NotifyPropertyChanged();} } }
	private string? _fattforcap;
	public string? fattforcap { get => _fattforcap; set { if (_fattforcap != value) { _fattforcap = value; NotifyPropertyChanged();} } }
	private string? _fattforloca;
	public string? fattforloca { get => _fattforloca; set { if (_fattforloca != value) { _fattforloca = value; NotifyPropertyChanged();} } }
	private string? _fattforinde;
	public string? fattforinde { get => _fattforinde; set { if (_fattforinde != value) { _fattforinde = value; NotifyPropertyChanged();} } }
	private string? _fattforprov;
	public string? fattforprov { get => _fattforprov; set { if (_fattforprov != value) { _fattforprov = value; NotifyPropertyChanged();} } }
	private string? _fattclicap;
	public string? fattclicap { get => _fattclicap; set { if (_fattclicap != value) { _fattclicap = value; NotifyPropertyChanged();} } }
	private string? _fattclinaz;
	public string? fattclinaz { get => _fattclinaz; set { if (_fattclinaz != value) { _fattclinaz = value; NotifyPropertyChanged();} } }
	private string? _fattclipro;
	public string? fattclipro { get => _fattclipro; set { if (_fattclipro != value) { _fattclipro = value; NotifyPropertyChanged();} } }
	private string? _fattcliloca;
	public string? fattcliloca { get => _fattcliloca; set { if (_fattcliloca != value) { _fattcliloca = value; NotifyPropertyChanged();} } }
	private string? _fattcliinde;
	public string? fattcliinde { get => _fattcliinde; set { if (_fattcliinde != value) { _fattcliinde = value; NotifyPropertyChanged();} } }
	private string? _fattclipiva;
	public string? fattclipiva { get => _fattclipiva; set { if (_fattclipiva != value) { _fattclipiva = value; NotifyPropertyChanged();} } }
	private string? _fattstampa;
	public string? fattstampa { get => _fattstampa; set { if (_fattstampa != value) { _fattstampa = value; NotifyPropertyChanged();} } }
	private decimal? _fattarrotondamento;
	public decimal? fattarrotondamento { get => _fattarrotondamento; set { if (_fattarrotondamento != value) { _fattarrotondamento = value; NotifyPropertyChanged();} } }
	private string? _FATTTIPODOC;
	public string? FATTTIPODOC { get => _FATTTIPODOC; set { if (_FATTTIPODOC != value) { _FATTTIPODOC = value; NotifyPropertyChanged();} } }
	private string? _fattidsdi;
	public string? fattidsdi { get => _fattidsdi; set { if (_fattidsdi != value) { _fattidsdi = value; NotifyPropertyChanged();} } }
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
	private string? _fattclirags;
	public string? fattclirags { get => _fattclirags; set { if (_fattclirags != value) { _fattclirags = value; NotifyPropertyChanged();} } }
	private string _fattdire = null!;
	public required string fattdire { get => _fattdire; set { if (_fattdire != value) { _fattdire = value; NotifyPropertyChanged();} } }
	private DateTime? _fattdatasca;
	public DateTime? fattdatasca { get => _fattdatasca; set { if (_fattdatasca != value) { _fattdatasca = value; NotifyPropertyChanged();} } }
	private DateTime? _accounted;
	public DateTime? accounted { get => _accounted; set { if (_accounted != value) { _accounted = value; NotifyPropertyChanged();} } }
	private string? _accounted_UserID;
	public string? accounted_UserID { get => _accounted_UserID; set { if (_accounted_UserID != value) { _accounted_UserID = value; NotifyPropertyChanged();} } }
	private int? _fattcli;
	public int? fattcli { get => _fattcli; set { if (_fattcli != value) { _fattcli = value; NotifyPropertyChanged();} } }
	private string? _fattclicf;
	public string? fattclicf { get => _fattclicf; set { if (_fattclicf != value) { _fattclicf = value; NotifyPropertyChanged();} } }
	private long _id;
	public long id { get => _id; set { if (_id != value) { _id = value; NotifyPropertyChanged();} } }
	private int? _fattalle;
	public int? fattalle { get => _fattalle; set { if (_fattalle != value) { _fattalle = value; NotifyPropertyChanged();} } }
}