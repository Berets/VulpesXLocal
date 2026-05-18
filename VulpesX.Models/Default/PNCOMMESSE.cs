namespace VulpesX.Models.Default;
 
public partial class PNCOMMESSE : Base 
{
	private string _PNSOCI = null!;
	public required string PNSOCI { get => _PNSOCI; set { if (_PNSOCI != value) { _PNSOCI = value; NotifyPropertyChanged();} } }
	private int _PNANNO;
	public int PNANNO { get => _PNANNO; set { if (_PNANNO != value) { _PNANNO = value; NotifyPropertyChanged();} } }
	private int _PNNUME;
	public int PNNUME { get => _PNNUME; set { if (_PNNUME != value) { _PNNUME = value; NotifyPropertyChanged();} } }
	private int? _PNMESE;
	public int? PNMESE { get => _PNMESE; set { if (_PNMESE != value) { _PNMESE = value; NotifyPropertyChanged();} } }
	private string? _PNNUDO;
	public string? PNNUDO { get => _PNNUDO; set { if (_PNNUDO != value) { _PNNUDO = value; NotifyPropertyChanged();} } }
	private DateTime? _PNDADO;
	public DateTime? PNDADO { get => _PNDADO; set { if (_PNDADO != value) { _PNDADO = value; NotifyPropertyChanged();} } }
	private string? _PNNUCO;
	public string? PNNUCO { get => _PNNUCO; set { if (_PNNUCO != value) { _PNNUCO = value; NotifyPropertyChanged();} } }
	private string? _PNCSOC;
	public string? PNCSOC { get => _PNCSOC; set { if (_PNCSOC != value) { _PNCSOC = value; NotifyPropertyChanged();} } }
	private string? _PNCCON;
	public string? PNCCON { get => _PNCCON; set { if (_PNCCON != value) { _PNCCON = value; NotifyPropertyChanged();} } }
	private decimal? _PNCQTA;
	public decimal? PNCQTA { get => _PNCQTA; set { if (_PNCQTA != value) { _PNCQTA = value; NotifyPropertyChanged();} } }
	private decimal? _PNEURO;
	public decimal? PNEURO { get => _PNEURO; set { if (_PNEURO != value) { _PNEURO = value; NotifyPropertyChanged();} } }
	private string? _PNSEGN;
	public string? PNSEGN { get => _PNSEGN; set { if (_PNSEGN != value) { _PNSEGN = value; NotifyPropertyChanged();} } }
	private string? _PNNOTE;
	public string? PNNOTE { get => _PNNOTE; set { if (_PNNOTE != value) { _PNNOTE = value; NotifyPropertyChanged();} } }
	private string? _PNFL01;
	public string? PNFL01 { get => _PNFL01; set { if (_PNFL01 != value) { _PNFL01 = value; NotifyPropertyChanged();} } }
	private string? _PNFL02;
	public string? PNFL02 { get => _PNFL02; set { if (_PNFL02 != value) { _PNFL02 = value; NotifyPropertyChanged();} } }
	private int? _PNANRE;
	public int? PNANRE { get => _PNANRE; set { if (_PNANRE != value) { _PNANRE = value; NotifyPropertyChanged();} } }
	private int? _PNNURE;
	public int? PNNURE { get => _PNNURE; set { if (_PNNURE != value) { _PNNURE = value; NotifyPropertyChanged();} } }
	private int? _PNRIRE;
	public int? PNRIRE { get => _PNRIRE; set { if (_PNRIRE != value) { _PNRIRE = value; NotifyPropertyChanged();} } }
	private string? _PNMOVI;
	public string? PNMOVI { get => _PNMOVI; set { if (_PNMOVI != value) { _PNMOVI = value; NotifyPropertyChanged();} } }
	private string? _pncomli;
	public string? pncomli { get => _pncomli; set { if (_pncomli != value) { _pncomli = value; NotifyPropertyChanged();} } }
	private string? _pncoma;
	public string? pncoma { get => _pncoma; set { if (_pncoma != value) { _pncoma = value; NotifyPropertyChanged();} } }
	private string? _pnsocp;
	public string? pnsocp { get => _pnsocp; set { if (_pnsocp != value) { _pnsocp = value; NotifyPropertyChanged();} } }
	private string? _pnmat;
	public string? pnmat { get => _pnmat; set { if (_pnmat != value) { _pnmat = value; NotifyPropertyChanged();} } }
	private int? _pnclie;
	public int? pnclie { get => _pnclie; set { if (_pnclie != value) { _pnclie = value; NotifyPropertyChanged();} } }
}