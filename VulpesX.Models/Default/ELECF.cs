namespace VulpesX.Models.Default;
 
public partial class ELECF : Base 
{
	private string _ECFSOC = null!;
	public required string ECFSOC { get => _ECFSOC; set { if (_ECFSOC != value) { _ECFSOC = value; NotifyPropertyChanged();} } }
	private int _ECFANNO;
	public int ECFANNO { get => _ECFANNO; set { if (_ECFANNO != value) { _ECFANNO = value; NotifyPropertyChanged();} } }
	private string _ECFTIP = null!;
	public required string ECFTIP { get => _ECFTIP; set { if (_ECFTIP != value) { _ECFTIP = value; NotifyPropertyChanged();} } }
	private int _ECFCOD;
	public int ECFCOD { get => _ECFCOD; set { if (_ECFCOD != value) { _ECFCOD = value; NotifyPropertyChanged();} } }
	private string? _ECFPIVA;
	public string? ECFPIVA { get => _ECFPIVA; set { if (_ECFPIVA != value) { _ECFPIVA = value; NotifyPropertyChanged();} } }
	private string? _ECFCFI;
	public string? ECFCFI { get => _ECFCFI; set { if (_ECFCFI != value) { _ECFCFI = value; NotifyPropertyChanged();} } }
	private decimal? _ECFIMP;
	public decimal? ECFIMP { get => _ECFIMP; set { if (_ECFIMP != value) { _ECFIMP = value; NotifyPropertyChanged();} } }
	private decimal? _ECFIMI;
	public decimal? ECFIMI { get => _ECFIMI; set { if (_ECFIMI != value) { _ECFIMI = value; NotifyPropertyChanged();} } }
	private decimal? _ECFNIM;
	public decimal? ECFNIM { get => _ECFNIM; set { if (_ECFNIM != value) { _ECFNIM = value; NotifyPropertyChanged();} } }
	private decimal? _ECFESE;
	public decimal? ECFESE { get => _ECFESE; set { if (_ECFESE != value) { _ECFESE = value; NotifyPropertyChanged();} } }
	private decimal? _ECFIME;
	public decimal? ECFIME { get => _ECFIME; set { if (_ECFIME != value) { _ECFIME = value; NotifyPropertyChanged();} } }
	private decimal? _ECFIMN;
	public decimal? ECFIMN { get => _ECFIMN; set { if (_ECFIMN != value) { _ECFIMN = value; NotifyPropertyChanged();} } }
	private decimal? _ECFINI;
	public decimal? ECFINI { get => _ECFINI; set { if (_ECFINI != value) { _ECFINI = value; NotifyPropertyChanged();} } }
	private decimal? _ECFNOI;
	public decimal? ECFNOI { get => _ECFNOI; set { if (_ECFNOI != value) { _ECFNOI = value; NotifyPropertyChanged();} } }
	private decimal? _ECFNOE;
	public decimal? ECFNOE { get => _ECFNOE; set { if (_ECFNOE != value) { _ECFNOE = value; NotifyPropertyChanged();} } }
	private decimal? _ECFNOF;
	public decimal? ECFNOF { get => _ECFNOF; set { if (_ECFNOF != value) { _ECFNOF = value; NotifyPropertyChanged();} } }
	private DateTime? _ECFDAES;
	public DateTime? ECFDAES { get => _ECFDAES; set { if (_ECFDAES != value) { _ECFDAES = value; NotifyPropertyChanged();} } }
	private string? _ECFDEF;
	public string? ECFDEF { get => _ECFDEF; set { if (_ECFDEF != value) { _ECFDEF = value; NotifyPropertyChanged();} } }
}