namespace VulpesX.Models.Default;
 
public partial class MGLIF00F : Base 
{
	private string _SOCIEL = null!;
	public required string SOCIEL { get => _SOCIEL; set { if (_SOCIEL != value) { _SOCIEL = value; NotifyPropertyChanged();} } }
	private string _CODARL = null!;
	public required string CODARL { get => _CODARL; set { if (_CODARL != value) { _CODARL = value; NotifyPropertyChanged();} } }
	private int _ANNOLL;
	public int ANNOLL { get => _ANNOLL; set { if (_ANNOLL != value) { _ANNOLL = value; NotifyPropertyChanged();} } }
	private string? _ATTIV;
	public string? ATTIV { get => _ATTIV; set { if (_ATTIV != value) { _ATTIV = value; NotifyPropertyChanged();} } }
	private int? _NRAZI;
	public int? NRAZI { get => _NRAZI; set { if (_NRAZI != value) { _NRAZI = value; NotifyPropertyChanged();} } }
	private string? _CODMAL;
	public string? CODMAL { get => _CODMAL; set { if (_CODMAL != value) { _CODMAL = value; NotifyPropertyChanged();} } }
	private decimal? _QULIFL;
	public decimal? QULIFL { get => _QULIFL; set { if (_QULIFL != value) { _QULIFL = value; NotifyPropertyChanged();} } }
	private decimal? _COMESL;
	public decimal? COMESL { get => _COMESL; set { if (_COMESL != value) { _COMESL = value; NotifyPropertyChanged();} } }
	private decimal? _VATOTL;
	public decimal? VATOTL { get => _VATOTL; set { if (_VATOTL != value) { _VATOTL = value; NotifyPropertyChanged();} } }
	private int? _ANNOCL;
	public int? ANNOCL { get => _ANNOCL; set { if (_ANNOCL != value) { _ANNOCL = value; NotifyPropertyChanged();} } }
	private string? _FAMILL;
	public string? FAMILL { get => _FAMILL; set { if (_FAMILL != value) { _FAMILL = value; NotifyPropertyChanged();} } }
	private decimal? _TOENTL;
	public decimal? TOENTL { get => _TOENTL; set { if (_TOENTL != value) { _TOENTL = value; NotifyPropertyChanged();} } }
	private string? _ORASIL;
	public string? ORASIL { get => _ORASIL; set { if (_ORASIL != value) { _ORASIL = value; NotifyPropertyChanged();} } }
	private string? _UTENIL;
	public string? UTENIL { get => _UTENIL; set { if (_UTENIL != value) { _UTENIL = value; NotifyPropertyChanged();} } }
	private string? _TERMIL;
	public string? TERMIL { get => _TERMIL; set { if (_TERMIL != value) { _TERMIL = value; NotifyPropertyChanged();} } }
	private string? _ORASVL;
	public string? ORASVL { get => _ORASVL; set { if (_ORASVL != value) { _ORASVL = value; NotifyPropertyChanged();} } }
	private string? _UTENVL;
	public string? UTENVL { get => _UTENVL; set { if (_UTENVL != value) { _UTENVL = value; NotifyPropertyChanged();} } }
	private string? _TERMVL;
	public string? TERMVL { get => _TERMVL; set { if (_TERMVL != value) { _TERMVL = value; NotifyPropertyChanged();} } }
}