namespace VulpesX.Models.Default;
 
public partial class ORDVD00F : Base 
{
	private int _OVANNO;
	public int OVANNO { get => _OVANNO; set { if (_OVANNO != value) { _OVANNO = value; NotifyPropertyChanged();} } }
	private int _OVNUOR;
	public int OVNUOR { get => _OVNUOR; set { if (_OVNUOR != value) { _OVNUOR = value; NotifyPropertyChanged();} } }
	private int _OVRIGA;
	public int OVRIGA { get => _OVRIGA; set { if (_OVRIGA != value) { _OVRIGA = value; NotifyPropertyChanged();} } }
	private DateTime _OVDATE;
	public DateTime OVDATE { get => _OVDATE; set { if (_OVDATE != value) { _OVDATE = value; NotifyPropertyChanged();} } }
	private int _OVTIME;
	public int OVTIME { get => _OVTIME; set { if (_OVTIME != value) { _OVTIME = value; NotifyPropertyChanged();} } }
	private string? _OVCODA;
	public string? OVCODA { get => _OVCODA; set { if (_OVCODA != value) { _OVCODA = value; NotifyPropertyChanged();} } }
	private decimal? _OVQTAV;
	public decimal? OVQTAV { get => _OVQTAV; set { if (_OVQTAV != value) { _OVQTAV = value; NotifyPropertyChanged();} } }
	private string? _OVTQTA;
	public string? OVTQTA { get => _OVTQTA; set { if (_OVTQTA != value) { _OVTQTA = value; NotifyPropertyChanged();} } }
	private decimal? _OVPREZ;
	public decimal? OVPREZ { get => _OVPREZ; set { if (_OVPREZ != value) { _OVPREZ = value; NotifyPropertyChanged();} } }
	private decimal? _OVSCO1;
	public decimal? OVSCO1 { get => _OVSCO1; set { if (_OVSCO1 != value) { _OVSCO1 = value; NotifyPropertyChanged();} } }
	private decimal? _OVSCO2;
	public decimal? OVSCO2 { get => _OVSCO2; set { if (_OVSCO2 != value) { _OVSCO2 = value; NotifyPropertyChanged();} } }
	private decimal? _OVMAGG;
	public decimal? OVMAGG { get => _OVMAGG; set { if (_OVMAGG != value) { _OVMAGG = value; NotifyPropertyChanged();} } }
	private string? _OVTPRE;
	public string? OVTPRE { get => _OVTPRE; set { if (_OVTPRE != value) { _OVTPRE = value; NotifyPropertyChanged();} } }
	private string? _OVTSC1;
	public string? OVTSC1 { get => _OVTSC1; set { if (_OVTSC1 != value) { _OVTSC1 = value; NotifyPropertyChanged();} } }
	private string? _OVTSC2;
	public string? OVTSC2 { get => _OVTSC2; set { if (_OVTSC2 != value) { _OVTSC2 = value; NotifyPropertyChanged();} } }
	private string? _OVTMAG;
	public string? OVTMAG { get => _OVTMAG; set { if (_OVTMAG != value) { _OVTMAG = value; NotifyPropertyChanged();} } }
	private DateTime? _OVDACO;
	public DateTime? OVDACO { get => _OVDACO; set { if (_OVDACO != value) { _OVDACO = value; NotifyPropertyChanged();} } }
	private decimal? _OVSCO3;
	public decimal? OVSCO3 { get => _OVSCO3; set { if (_OVSCO3 != value) { _OVSCO3 = value; NotifyPropertyChanged();} } }
	private string? _OVTSC3;
	public string? OVTSC3 { get => _OVTSC3; set { if (_OVTSC3 != value) { _OVTSC3 = value; NotifyPropertyChanged();} } }
	private int? _OVCOCL;
	public int? OVCOCL { get => _OVCOCL; set { if (_OVCOCL != value) { _OVCOCL = value; NotifyPropertyChanged();} } }
}