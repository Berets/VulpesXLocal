namespace VulpesX.Models.Default;
 
public partial class TUTENTI : Base 
{
	private string _UTECOD = null!;
	public required string UTECOD { get => _UTECOD; set { if (_UTECOD != value) { _UTECOD = value; NotifyPropertyChanged();} } }
	private string? _UTEPWD;
	public string? UTEPWD { get => _UTEPWD; set { if (_UTEPWD != value) { _UTEPWD = value; NotifyPropertyChanged();} } }
	private string? _UTETIP;
	public string? UTETIP { get => _UTETIP; set { if (_UTETIP != value) { _UTETIP = value; NotifyPropertyChanged();} } }
	private string? _UTETRA;
	public string? UTETRA { get => _UTETRA; set { if (_UTETRA != value) { _UTETRA = value; NotifyPropertyChanged();} } }
	private string? _UTEMAI;
	public string? UTEMAI { get => _UTEMAI; set { if (_UTEMAI != value) { _UTEMAI = value; NotifyPropertyChanged();} } }
	private byte[]? _utefel;
	public byte[]? utefel { get => _utefel; set { if (_utefel != value) { _utefel = value; NotifyPropertyChanged();} } }
	private string? _utemat;
	public string? utemat { get => _utemat; set { if (_utemat != value) { _utemat = value; NotifyPropertyChanged();} } }
}