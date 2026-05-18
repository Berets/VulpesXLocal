namespace VulpesX.Models.Default;
 
public partial class NOTASPESE1 : Base 
{
	private string _nossoc = null!;
	public required string nossoc { get => _nossoc; set { if (_nossoc != value) { _nossoc = value; NotifyPropertyChanged();} } }
	private string _nosdip = null!;
	public required string nosdip { get => _nosdip; set { if (_nosdip != value) { _nosdip = value; NotifyPropertyChanged();} } }
	private DateTime _nosdat;
	public DateTime nosdat { get => _nosdat; set { if (_nosdat != value) { _nosdat = value; NotifyPropertyChanged();} } }
	private int _nosrig;
	public int nosrig { get => _nosrig; set { if (_nosrig != value) { _nosrig = value; NotifyPropertyChanged();} } }
	private DateTime? _nosdai;
	public DateTime? nosdai { get => _nosdai; set { if (_nosdai != value) { _nosdai = value; NotifyPropertyChanged();} } }
	private string? _nostsp;
	public string? nostsp { get => _nostsp; set { if (_nostsp != value) { _nostsp = value; NotifyPropertyChanged();} } }
	private string? _nosdep;
	public string? nosdep { get => _nosdep; set { if (_nosdep != value) { _nosdep = value; NotifyPropertyChanged();} } }
	private int? _nosqta;
	public int? nosqta { get => _nosqta; set { if (_nosqta != value) { _nosqta = value; NotifyPropertyChanged();} } }
	private decimal? _nosuni;
	public decimal? nosuni { get => _nosuni; set { if (_nosuni != value) { _nosuni = value; NotifyPropertyChanged();} } }
	private decimal? _nosicl;
	public decimal? nosicl { get => _nosicl; set { if (_nosicl != value) { _nosicl = value; NotifyPropertyChanged();} } }
	private string? _noscc;
	public string? noscc { get => _noscc; set { if (_noscc != value) { _noscc = value; NotifyPropertyChanged();} } }
	private string? _noscod;
	public string? noscod { get => _noscod; set { if (_noscod != value) { _noscod = value; NotifyPropertyChanged();} } }
	private string? _noscso;
	public string? noscso { get => _noscso; set { if (_noscso != value) { _noscso = value; NotifyPropertyChanged();} } }
	private int? _noclfo;
	public int? noclfo { get => _noclfo; set { if (_noclfo != value) { _noclfo = value; NotifyPropertyChanged();} } }
}