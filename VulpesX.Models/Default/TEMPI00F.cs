namespace VulpesX.Models.Default;
 
public partial class TEMPI00F : Base 
{
	private string _TESOCI = null!;
	public required string TESOCI { get => _TESOCI; set { if (_TESOCI != value) { _TESOCI = value; NotifyPropertyChanged();} } }
	private int _TEANNP;
	public int TEANNP { get => _TEANNP; set { if (_TEANNP != value) { _TEANNP = value; NotifyPropertyChanged();} } }
	private int _TEORDP;
	public int TEORDP { get => _TEORDP; set { if (_TEORDP != value) { _TEORDP = value; NotifyPropertyChanged();} } }
	private int _TENULP;
	public int TENULP { get => _TENULP; set { if (_TENULP != value) { _TENULP = value; NotifyPropertyChanged();} } }
	private int _TENSEQ;
	public int TENSEQ { get => _TENSEQ; set { if (_TENSEQ != value) { _TENSEQ = value; NotifyPropertyChanged();} } }
	private int _TERIGA;
	public int TERIGA { get => _TERIGA; set { if (_TERIGA != value) { _TERIGA = value; NotifyPropertyChanged();} } }
	private DateTime? _TEDATA;
	public DateTime? TEDATA { get => _TEDATA; set { if (_TEDATA != value) { _TEDATA = value; NotifyPropertyChanged();} } }
	private string? _TETIME;
	public string? TETIME { get => _TETIME; set { if (_TETIME != value) { _TETIME = value; NotifyPropertyChanged();} } }
	private string? _TEATTI;
	public string? TEATTI { get => _TEATTI; set { if (_TEATTI != value) { _TEATTI = value; NotifyPropertyChanged();} } }
	private string? _TECOPA;
	public string? TECOPA { get => _TECOPA; set { if (_TECOPA != value) { _TECOPA = value; NotifyPropertyChanged();} } }
	private string? _TEREPA;
	public string? TEREPA { get => _TEREPA; set { if (_TEREPA != value) { _TEREPA = value; NotifyPropertyChanged();} } }
	private string? _TEFASE;
	public string? TEFASE { get => _TEFASE; set { if (_TEFASE != value) { _TEFASE = value; NotifyPropertyChanged();} } }
	private string? _TEMATR;
	public string? TEMATR { get => _TEMATR; set { if (_TEMATR != value) { _TEMATR = value; NotifyPropertyChanged();} } }
	private string? _TECAAV;
	public string? TECAAV { get => _TECAAV; set { if (_TECAAV != value) { _TECAAV = value; NotifyPropertyChanged();} } }
	private string? _TETIAV;
	public string? TETIAV { get => _TETIAV; set { if (_TETIAV != value) { _TETIAV = value; NotifyPropertyChanged();} } }
	private decimal? _TEQCON;
	public decimal? TEQCON { get => _TEQCON; set { if (_TEQCON != value) { _TEQCON = value; NotifyPropertyChanged();} } }
	private decimal? _TEQNCO;
	public decimal? TEQNCO { get => _TEQNCO; set { if (_TEQNCO != value) { _TEQNCO = value; NotifyPropertyChanged();} } }
	private string? _TECASC;
	public string? TECASC { get => _TECASC; set { if (_TECASC != value) { _TECASC = value; NotifyPropertyChanged();} } }
	private string? _TERIPA;
	public string? TERIPA { get => _TERIPA; set { if (_TERIPA != value) { _TERIPA = value; NotifyPropertyChanged();} } }
	private decimal? _TEQRIP;
	public decimal? TEQRIP { get => _TEQRIP; set { if (_TEQRIP != value) { _TEQRIP = value; NotifyPropertyChanged();} } }
	private string? _TEMTR2;
	public string? TEMTR2 { get => _TEMTR2; set { if (_TEMTR2 != value) { _TEMTR2 = value; NotifyPropertyChanged();} } }
	private string? _TEMTR3;
	public string? TEMTR3 { get => _TEMTR3; set { if (_TEMTR3 != value) { _TEMTR3 = value; NotifyPropertyChanged();} } }
	private string? _TEFILL;
	public string? TEFILL { get => _TEFILL; set { if (_TEFILL != value) { _TEFILL = value; NotifyPropertyChanged();} } }
	private string? _TEFLAG;
	public string? TEFLAG { get => _TEFLAG; set { if (_TEFLAG != value) { _TEFLAG = value; NotifyPropertyChanged();} } }
	private string? _TENOTE;
	public string? TENOTE { get => _TENOTE; set { if (_TENOTE != value) { _TENOTE = value; NotifyPropertyChanged();} } }
	private string? _TECOPC;
	public string? TECOPC { get => _TECOPC; set { if (_TECOPC != value) { _TECOPC = value; NotifyPropertyChanged();} } }
	private string? _TEMAGA;
	public string? TEMAGA { get => _TEMAGA; set { if (_TEMAGA != value) { _TEMAGA = value; NotifyPropertyChanged();} } }
	private int? _TEINTE;
	public int? TEINTE { get => _TEINTE; set { if (_TEINTE != value) { _TEINTE = value; NotifyPropertyChanged();} } }
	private string? _tesoc1;
	public string? tesoc1 { get => _tesoc1; set { if (_tesoc1 != value) { _tesoc1 = value; NotifyPropertyChanged();} } }
	private string? _tesoc2;
	public string? tesoc2 { get => _tesoc2; set { if (_tesoc2 != value) { _tesoc2 = value; NotifyPropertyChanged();} } }
	private string? _tesoc3;
	public string? tesoc3 { get => _tesoc3; set { if (_tesoc3 != value) { _tesoc3 = value; NotifyPropertyChanged();} } }
	private string? _tesoc4;
	public string? tesoc4 { get => _tesoc4; set { if (_tesoc4 != value) { _tesoc4 = value; NotifyPropertyChanged();} } }
	private string? _temacc;
	public string? temacc { get => _temacc; set { if (_temacc != value) { _temacc = value; NotifyPropertyChanged();} } }
	private string? _tetema;
	public string? tetema { get => _tetema; set { if (_tetema != value) { _tetema = value; NotifyPropertyChanged();} } }
}