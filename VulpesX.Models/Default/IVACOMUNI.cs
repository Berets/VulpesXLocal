namespace VulpesX.Models.Default;
 
public partial class IVACOMUNI : Base 
{
	private string _Icsoc = null!;
	public required string Icsoc { get => _Icsoc; set { if (_Icsoc != value) { _Icsoc = value; NotifyPropertyChanged();} } }
	private int _icannoct;
	public int icannoct { get => _icannoct; set { if (_icannoct != value) { _icannoct = value; NotifyPropertyChanged();} } }
	private int _icanncm;
	public int icanncm { get => _icanncm; set { if (_icanncm != value) { _icanncm = value; NotifyPropertyChanged();} } }
	private string? _icragsoc;
	public string? icragsoc { get => _icragsoc; set { if (_icragsoc != value) { _icragsoc = value; NotifyPropertyChanged();} } }
	private string? _icpiva;
	public string? icpiva { get => _icpiva; set { if (_icpiva != value) { _icpiva = value; NotifyPropertyChanged();} } }
	private int? _icattcod;
	public int? icattcod { get => _icattcod; set { if (_icattcod != value) { _icattcod = value; NotifyPropertyChanged();} } }
	private string? _iccodfisdic;
	public string? iccodfisdic { get => _iccodfisdic; set { if (_iccodfisdic != value) { _iccodfisdic = value; NotifyPropertyChanged();} } }
	private string? _iccodcar;
	public string? iccodcar { get => _iccodcar; set { if (_iccodcar != value) { _iccodcar = value; NotifyPropertyChanged();} } }
	private string? _iccodfissoc;
	public string? iccodfissoc { get => _iccodfissoc; set { if (_iccodfissoc != value) { _iccodfissoc = value; NotifyPropertyChanged();} } }
	private decimal? _ictotimpa;
	public decimal? ictotimpa { get => _ictotimpa; set { if (_ictotimpa != value) { _ictotimpa = value; NotifyPropertyChanged();} } }
	private decimal? _ictotimpna;
	public decimal? ictotimpna { get => _ictotimpna; set { if (_ictotimpna != value) { _ictotimpna = value; NotifyPropertyChanged();} } }
	private decimal? _ictotesea;
	public decimal? ictotesea { get => _ictotesea; set { if (_ictotesea != value) { _ictotesea = value; NotifyPropertyChanged();} } }
	private decimal? _ictotcintra;
	public decimal? ictotcintra { get => _ictotcintra; set { if (_ictotcintra != value) { _ictotcintra = value; NotifyPropertyChanged();} } }
	private decimal? _ictotimpp;
	public decimal? ictotimpp { get => _ictotimpp; set { if (_ictotimpp != value) { _ictotimpp = value; NotifyPropertyChanged();} } }
	private decimal? _ictotimpnp;
	public decimal? ictotimpnp { get => _ictotimpnp; set { if (_ictotimpnp != value) { _ictotimpnp = value; NotifyPropertyChanged();} } }
	private decimal? _ictotesep;
	public decimal? ictotesep { get => _ictotesep; set { if (_ictotesep != value) { _ictotesep = value; NotifyPropertyChanged();} } }
	private decimal? _ictotacqintp;
	public decimal? ictotacqintp { get => _ictotacqintp; set { if (_ictotacqintp != value) { _ictotacqintp = value; NotifyPropertyChanged();} } }
	private decimal? _ictotivaese;
	public decimal? ictotivaese { get => _ictotivaese; set { if (_ictotivaese != value) { _ictotivaese = value; NotifyPropertyChanged();} } }
	private decimal? _ictotivadet;
	public decimal? ictotivadet { get => _ictotivadet; set { if (_ictotivadet != value) { _ictotivadet = value; NotifyPropertyChanged();} } }
	private decimal? _ictotivad;
	public decimal? ictotivad { get => _ictotivad; set { if (_ictotivad != value) { _ictotivad = value; NotifyPropertyChanged();} } }
	private decimal? _ictotivac;
	public decimal? ictotivac { get => _ictotivac; set { if (_ictotivac != value) { _ictotivac = value; NotifyPropertyChanged();} } }
	private string? _icfortip;
	public string? icfortip { get => _icfortip; set { if (_icfortip != value) { _icfortip = value; NotifyPropertyChanged();} } }
	private string? _icflag;
	public string? icflag { get => _icflag; set { if (_icflag != value) { _icflag = value; NotifyPropertyChanged();} } }
}