namespace VulpesX.Models.Default;
 
public partial class MGFIF00F : Base 
{
	private string _SOCIEF = null!;
	public required string SOCIEF { get => _SOCIEF; set { if (_SOCIEF != value) { _SOCIEF = value; NotifyPropertyChanged();} } }
	private string _CODARF = null!;
	public required string CODARF { get => _CODARF; set { if (_CODARF != value) { _CODARF = value; NotifyPropertyChanged();} } }
	private int _ANNOLF;
	public int ANNOLF { get => _ANNOLF; set { if (_ANNOLF != value) { _ANNOLF = value; NotifyPropertyChanged();} } }
	private string? _ATTIVF;
	public string? ATTIVF { get => _ATTIVF; set { if (_ATTIVF != value) { _ATTIVF = value; NotifyPropertyChanged();} } }
	private int? _NRAZIF;
	public int? NRAZIF { get => _NRAZIF; set { if (_NRAZIF != value) { _NRAZIF = value; NotifyPropertyChanged();} } }
	private string? _CODMAF;
	public string? CODMAF { get => _CODMAF; set { if (_CODMAF != value) { _CODMAF = value; NotifyPropertyChanged();} } }
	private decimal? _QULIFF;
	public decimal? QULIFF { get => _QULIFF; set { if (_QULIFF != value) { _QULIFF = value; NotifyPropertyChanged();} } }
	private decimal? _COMESF;
	public decimal? COMESF { get => _COMESF; set { if (_COMESF != value) { _COMESF = value; NotifyPropertyChanged();} } }
	private decimal? _VATOTF;
	public decimal? VATOTF { get => _VATOTF; set { if (_VATOTF != value) { _VATOTF = value; NotifyPropertyChanged();} } }
	private int? _ANNOCF;
	public int? ANNOCF { get => _ANNOCF; set { if (_ANNOCF != value) { _ANNOCF = value; NotifyPropertyChanged();} } }
	private string? _FAMILF;
	public string? FAMILF { get => _FAMILF; set { if (_FAMILF != value) { _FAMILF = value; NotifyPropertyChanged();} } }
	private decimal? _TOENTF;
	public decimal? TOENTF { get => _TOENTF; set { if (_TOENTF != value) { _TOENTF = value; NotifyPropertyChanged();} } }
	private string? _ORASIF;
	public string? ORASIF { get => _ORASIF; set { if (_ORASIF != value) { _ORASIF = value; NotifyPropertyChanged();} } }
	private string? _UTENIF;
	public string? UTENIF { get => _UTENIF; set { if (_UTENIF != value) { _UTENIF = value; NotifyPropertyChanged();} } }
	private string? _TERMIF;
	public string? TERMIF { get => _TERMIF; set { if (_TERMIF != value) { _TERMIF = value; NotifyPropertyChanged();} } }
	private string? _ORASVF;
	public string? ORASVF { get => _ORASVF; set { if (_ORASVF != value) { _ORASVF = value; NotifyPropertyChanged();} } }
	private string? _UTENVF;
	public string? UTENVF { get => _UTENVF; set { if (_UTENVF != value) { _UTENVF = value; NotifyPropertyChanged();} } }
	private string? _TERMVF;
	public string? TERMVF { get => _TERMVF; set { if (_TERMVF != value) { _TERMVF = value; NotifyPropertyChanged();} } }
}