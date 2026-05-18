namespace VulpesX.Models.Default;
 
public partial class TEMPS00F : Base 
{
	private string _TSSOCI = null!;
	public required string TSSOCI { get => _TSSOCI; set { if (_TSSOCI != value) { _TSSOCI = value; NotifyPropertyChanged();} } }
	private int _TSANNP;
	public int TSANNP { get => _TSANNP; set { if (_TSANNP != value) { _TSANNP = value; NotifyPropertyChanged();} } }
	private int _TSORDP;
	public int TSORDP { get => _TSORDP; set { if (_TSORDP != value) { _TSORDP = value; NotifyPropertyChanged();} } }
	private int _TSNSEQ;
	public int TSNSEQ { get => _TSNSEQ; set { if (_TSNSEQ != value) { _TSNSEQ = value; NotifyPropertyChanged();} } }
	private string _TSREPA = null!;
	public required string TSREPA { get => _TSREPA; set { if (_TSREPA != value) { _TSREPA = value; NotifyPropertyChanged();} } }
	private string _TSFASE = null!;
	public required string TSFASE { get => _TSFASE; set { if (_TSFASE != value) { _TSFASE = value; NotifyPropertyChanged();} } }
	private string _TSMACC = null!;
	public required string TSMACC { get => _TSMACC; set { if (_TSMACC != value) { _TSMACC = value; NotifyPropertyChanged();} } }
	private DateTime _TSDATA;
	public DateTime TSDATA { get => _TSDATA; set { if (_TSDATA != value) { _TSDATA = value; NotifyPropertyChanged();} } }
	private int _TSNSQD;
	public int TSNSQD { get => _TSNSQD; set { if (_TSNSQD != value) { _TSNSQD = value; NotifyPropertyChanged();} } }
	private string? _TSATTI;
	public string? TSATTI { get => _TSATTI; set { if (_TSATTI != value) { _TSATTI = value; NotifyPropertyChanged();} } }
	private string? _TSTIPO;
	public string? TSTIPO { get => _TSTIPO; set { if (_TSTIPO != value) { _TSTIPO = value; NotifyPropertyChanged();} } }
	private string? _TSCOPA;
	public string? TSCOPA { get => _TSCOPA; set { if (_TSCOPA != value) { _TSCOPA = value; NotifyPropertyChanged();} } }
	private string? _TSCOPR;
	public string? TSCOPR { get => _TSCOPR; set { if (_TSCOPR != value) { _TSCOPR = value; NotifyPropertyChanged();} } }
	private decimal? _TSTEMP;
	public decimal? TSTEMP { get => _TSTEMP; set { if (_TSTEMP != value) { _TSTEMP = value; NotifyPropertyChanged();} } }
	private decimal? _TSCOME;
	public decimal? TSCOME { get => _TSCOME; set { if (_TSCOME != value) { _TSCOME = value; NotifyPropertyChanged();} } }
	private decimal? _TSCOMT;
	public decimal? TSCOMT { get => _TSCOMT; set { if (_TSCOMT != value) { _TSCOMT = value; NotifyPropertyChanged();} } }
	private decimal? _TSCOMA;
	public decimal? TSCOMA { get => _TSCOMA; set { if (_TSCOMA != value) { _TSCOMA = value; NotifyPropertyChanged();} } }
	private decimal? _TSCOLA;
	public decimal? TSCOLA { get => _TSCOLA; set { if (_TSCOLA != value) { _TSCOLA = value; NotifyPropertyChanged();} } }
	private decimal? _TSCOAG;
	public decimal? TSCOAG { get => _TSCOAG; set { if (_TSCOAG != value) { _TSCOAG = value; NotifyPropertyChanged();} } }
	private decimal? _TSCOD1;
	public decimal? TSCOD1 { get => _TSCOD1; set { if (_TSCOD1 != value) { _TSCOD1 = value; NotifyPropertyChanged();} } }
	private decimal? _TSCOD2;
	public decimal? TSCOD2 { get => _TSCOD2; set { if (_TSCOD2 != value) { _TSCOD2 = value; NotifyPropertyChanged();} } }
	private string? _TSFILL;
	public string? TSFILL { get => _TSFILL; set { if (_TSFILL != value) { _TSFILL = value; NotifyPropertyChanged();} } }
	private string? _TSMATR;
	public string? TSMATR { get => _TSMATR; set { if (_TSMATR != value) { _TSMATR = value; NotifyPropertyChanged();} } }
	private string? _TSMTR2;
	public string? TSMTR2 { get => _TSMTR2; set { if (_TSMTR2 != value) { _TSMTR2 = value; NotifyPropertyChanged();} } }
	private string? _TSMTR3;
	public string? TSMTR3 { get => _TSMTR3; set { if (_TSMTR3 != value) { _TSMTR3 = value; NotifyPropertyChanged();} } }
	private decimal? _TSTQTA;
	public decimal? TSTQTA { get => _TSTQTA; set { if (_TSTQTA != value) { _TSTQTA = value; NotifyPropertyChanged();} } }
	private int? _TSNULP;
	public int? TSNULP { get => _TSNULP; set { if (_TSNULP != value) { _TSNULP = value; NotifyPropertyChanged();} } }
	private int? _TSTEMH;
	public int? TSTEMH { get => _TSTEMH; set { if (_TSTEMH != value) { _TSTEMH = value; NotifyPropertyChanged();} } }
	private int? _TSTEMM;
	public int? TSTEMM { get => _TSTEMM; set { if (_TSTEMM != value) { _TSTEMM = value; NotifyPropertyChanged();} } }
	private int? _TSTEMS;
	public int? TSTEMS { get => _TSTEMS; set { if (_TSTEMS != value) { _TSTEMS = value; NotifyPropertyChanged();} } }
}