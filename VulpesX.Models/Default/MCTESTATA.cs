namespace VulpesX.Models.Default;
 
public partial class MCTESTATA : Base 
{
	private string _MCSOCI = null!;
	public required string MCSOCI { get => _MCSOCI; set { if (_MCSOCI != value) { _MCSOCI = value; NotifyPropertyChanged();} } }
	private int _MCANNO;
	public int MCANNO { get => _MCANNO; set { if (_MCANNO != value) { _MCANNO = value; NotifyPropertyChanged();} } }
	private int _MCNUME;
	public int MCNUME { get => _MCNUME; set { if (_MCNUME != value) { _MCNUME = value; NotifyPropertyChanged();} } }
	private string? _MCTIPO;
	public string? MCTIPO { get => _MCTIPO; set { if (_MCTIPO != value) { _MCTIPO = value; NotifyPropertyChanged();} } }
	private DateTime? _MCDATA;
	public DateTime? MCDATA { get => _MCDATA; set { if (_MCDATA != value) { _MCDATA = value; NotifyPropertyChanged();} } }
	private string? _MCGRUP;
	public string? MCGRUP { get => _MCGRUP; set { if (_MCGRUP != value) { _MCGRUP = value; NotifyPropertyChanged();} } }
	private string? _MCCONT;
	public string? MCCONT { get => _MCCONT; set { if (_MCCONT != value) { _MCCONT = value; NotifyPropertyChanged();} } }
	private string? _MCSOTT;
	public string? MCSOTT { get => _MCSOTT; set { if (_MCSOTT != value) { _MCSOTT = value; NotifyPropertyChanged();} } }
	private string? _MCSBAN;
	public string? MCSBAN { get => _MCSBAN; set { if (_MCSBAN != value) { _MCSBAN = value; NotifyPropertyChanged();} } }
	private int? _MCABI;
	public int? MCABI { get => _MCABI; set { if (_MCABI != value) { _MCABI = value; NotifyPropertyChanged();} } }
	private int? _MCCAB;
	public int? MCCAB { get => _MCCAB; set { if (_MCCAB != value) { _MCCAB = value; NotifyPropertyChanged();} } }
	private string? _MCCCOR;
	public string? MCCCOR { get => _MCCCOR; set { if (_MCCCOR != value) { _MCCCOR = value; NotifyPropertyChanged();} } }
	private string? _MCASSE;
	public string? MCASSE { get => _MCASSE; set { if (_MCASSE != value) { _MCASSE = value; NotifyPropertyChanged();} } }
	private int? _MCNRCL;
	public int? MCNRCL { get => _MCNRCL; set { if (_MCNRCL != value) { _MCNRCL = value; NotifyPropertyChanged();} } }
	private decimal? _MCIMPO;
	public decimal? MCIMPO { get => _MCIMPO; set { if (_MCIMPO != value) { _MCIMPO = value; NotifyPropertyChanged();} } }
	private decimal? _MCEUIM;
	public decimal? MCEUIM { get => _MCEUIM; set { if (_MCEUIM != value) { _MCEUIM = value; NotifyPropertyChanged();} } }
	private decimal? _MCIMAB;
	public decimal? MCIMAB { get => _MCIMAB; set { if (_MCIMAB != value) { _MCIMAB = value; NotifyPropertyChanged();} } }
	private decimal? _MCEUAB;
	public decimal? MCEUAB { get => _MCEUAB; set { if (_MCEUAB != value) { _MCEUAB = value; NotifyPropertyChanged();} } }
	private string? _MCCOVA;
	public string? MCCOVA { get => _MCCOVA; set { if (_MCCOVA != value) { _MCCOVA = value; NotifyPropertyChanged();} } }
	private string? _MCVALU;
	public string? MCVALU { get => _MCVALU; set { if (_MCVALU != value) { _MCVALU = value; NotifyPropertyChanged();} } }
	private decimal? _MCVAIM;
	public decimal? MCVAIM { get => _MCVAIM; set { if (_MCVAIM != value) { _MCVAIM = value; NotifyPropertyChanged();} } }
	private decimal? _MCVAAB;
	public decimal? MCVAAB { get => _MCVAAB; set { if (_MCVAAB != value) { _MCVAAB = value; NotifyPropertyChanged();} } }
	private string? _MCFLST;
	public string? MCFLST { get => _MCFLST; set { if (_MCFLST != value) { _MCFLST = value; NotifyPropertyChanged();} } }
	private string? _MCFLCO;
	public string? MCFLCO { get => _MCFLCO; set { if (_MCFLCO != value) { _MCFLCO = value; NotifyPropertyChanged();} } }
	private string? _MCTICS;
	public string? MCTICS { get => _MCTICS; set { if (_MCTICS != value) { _MCTICS = value; NotifyPropertyChanged();} } }
	private DateTime? _mcscad;
	public DateTime? mcscad { get => _mcscad; set { if (_mcscad != value) { _mcscad = value; NotifyPropertyChanged();} } }
}