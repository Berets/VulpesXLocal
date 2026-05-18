namespace VulpesX.Models.Default;
 
public partial class MGFIFW0F : Base 
{
	private string _SOCIEFW = null!;
	public required string SOCIEFW { get => _SOCIEFW; set { if (_SOCIEFW != value) { _SOCIEFW = value; NotifyPropertyChanged();} } }
	private string _CODARFW = null!;
	public required string CODARFW { get => _CODARFW; set { if (_CODARFW != value) { _CODARFW = value; NotifyPropertyChanged();} } }
	private int _ANNOLFW;
	public int ANNOLFW { get => _ANNOLFW; set { if (_ANNOLFW != value) { _ANNOLFW = value; NotifyPropertyChanged();} } }
	private string? _ATTIVFW;
	public string? ATTIVFW { get => _ATTIVFW; set { if (_ATTIVFW != value) { _ATTIVFW = value; NotifyPropertyChanged();} } }
	private int? _NRAZIFW;
	public int? NRAZIFW { get => _NRAZIFW; set { if (_NRAZIFW != value) { _NRAZIFW = value; NotifyPropertyChanged();} } }
	private string? _CODMAFW;
	public string? CODMAFW { get => _CODMAFW; set { if (_CODMAFW != value) { _CODMAFW = value; NotifyPropertyChanged();} } }
	private decimal? _QULIFFW;
	public decimal? QULIFFW { get => _QULIFFW; set { if (_QULIFFW != value) { _QULIFFW = value; NotifyPropertyChanged();} } }
	private decimal? _COMESFW;
	public decimal? COMESFW { get => _COMESFW; set { if (_COMESFW != value) { _COMESFW = value; NotifyPropertyChanged();} } }
	private decimal? _VATOTFW;
	public decimal? VATOTFW { get => _VATOTFW; set { if (_VATOTFW != value) { _VATOTFW = value; NotifyPropertyChanged();} } }
	private int? _ANNOCFW;
	public int? ANNOCFW { get => _ANNOCFW; set { if (_ANNOCFW != value) { _ANNOCFW = value; NotifyPropertyChanged();} } }
	private string? _FAMILFW;
	public string? FAMILFW { get => _FAMILFW; set { if (_FAMILFW != value) { _FAMILFW = value; NotifyPropertyChanged();} } }
	private decimal? _TOENTFW;
	public decimal? TOENTFW { get => _TOENTFW; set { if (_TOENTFW != value) { _TOENTFW = value; NotifyPropertyChanged();} } }
	private string? _ORASIFW;
	public string? ORASIFW { get => _ORASIFW; set { if (_ORASIFW != value) { _ORASIFW = value; NotifyPropertyChanged();} } }
	private string? _UTENIFW;
	public string? UTENIFW { get => _UTENIFW; set { if (_UTENIFW != value) { _UTENIFW = value; NotifyPropertyChanged();} } }
	private string? _TERMIFW;
	public string? TERMIFW { get => _TERMIFW; set { if (_TERMIFW != value) { _TERMIFW = value; NotifyPropertyChanged();} } }
	private string? _ORASVFW;
	public string? ORASVFW { get => _ORASVFW; set { if (_ORASVFW != value) { _ORASVFW = value; NotifyPropertyChanged();} } }
	private string? _UTENVFW;
	public string? UTENVFW { get => _UTENVFW; set { if (_UTENVFW != value) { _UTENVFW = value; NotifyPropertyChanged();} } }
	private string? _TERMVFW;
	public string? TERMVFW { get => _TERMVFW; set { if (_TERMVFW != value) { _TERMVFW = value; NotifyPropertyChanged();} } }
}