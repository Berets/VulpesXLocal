namespace VulpesX.Models.Ufp;
using VulpesX.Shared;
public partial class LISFOR0F : Base 
{
	private string _LFSOC = null!;
	public required string LFSOC { get => _LFSOC; set { if (_LFSOC != value) { _LFSOC = value; NotifyPropertyChanged();} } }
	private string _LFCOAR = null!;
	public required string LFCOAR { get => _LFCOAR; set { if (_LFCOAR != value) { _LFCOAR = value; NotifyPropertyChanged();} } }
	private int _LFFORN;
	public int LFFORN { get => _LFFORN; set { if (_LFFORN != value) { _LFFORN = value; NotifyPropertyChanged();} } }
	private DateTime _LFFDAT;
	public DateTime LFFDAT { get => _LFFDAT; set { if (_LFFDAT != value) { _LFFDAT = value; NotifyPropertyChanged();} } }
	private int? _LFPRIO;
	public int? LFPRIO { get => _LFPRIO; set { if (_LFPRIO != value) { _LFPRIO = value; NotifyPropertyChanged();} } }
	private decimal? _LFSCO1;
	public decimal? LFSCO1 { get => _LFSCO1; set { if (_LFSCO1 != value) { _LFSCO1 = value; NotifyPropertyChanged();} } }
	private decimal? _LFSCO2;
	public decimal? LFSCO2 { get => _LFSCO2; set { if (_LFSCO2 != value) { _LFSCO2 = value; NotifyPropertyChanged();} } }
	private decimal? _LFSCO3;
	public decimal? LFSCO3 { get => _LFSCO3; set { if (_LFSCO3 != value) { _LFSCO3 = value; NotifyPropertyChanged();} } }
	private decimal? _LFMAGG;
	public decimal? LFMAGG { get => _LFMAGG; set { if (_LFMAGG != value) { _LFMAGG = value; NotifyPropertyChanged();} } }
	private string? _LFTSC1;
	public string? LFTSC1 { get => _LFTSC1; set { if (_LFTSC1 != value) { _LFTSC1 = value; NotifyPropertyChanged();} } }
	private string? _LFTSC2;
	public string? LFTSC2 { get => _LFTSC2; set { if (_LFTSC2 != value) { _LFTSC2 = value; NotifyPropertyChanged();} } }
	private string? _LFTSC3;
	public string? LFTSC3 { get => _LFTSC3; set { if (_LFTSC3 != value) { _LFTSC3 = value; NotifyPropertyChanged();} } }
	private string? _LFTMAG;
	public string? LFTMAG { get => _LFTMAG; set { if (_LFTMAG != value) { _LFTMAG = value; NotifyPropertyChanged();} } }
	private string? _LFUNMI;
	public string? LFUNMI { get => _LFUNMI; set { if (_LFUNMI != value) { _LFUNMI = value; NotifyPropertyChanged();} } }
	private decimal? _LFPREZ;
	public decimal? LFPREZ { get => _LFPREZ; set { if (_LFPREZ != value) { _LFPREZ = value; NotifyPropertyChanged();} } }
	private string? _LFTPRE;
	public string? LFTPRE { get => _LFTPRE; set { if (_LFTPRE != value) { _LFTPRE = value; NotifyPropertyChanged();} } }
	private string? _LFNOTE;
	public string? LFNOTE { get => _LFNOTE; set { if (_LFNOTE != value) { _LFNOTE = value; NotifyPropertyChanged();} } }
	private string? _LFLISATT;
	public string? LFLISATT { get => _LFLISATT; set { if (_LFLISATT != value) { _LFLISATT = value; NotifyPropertyChanged();} } }
}