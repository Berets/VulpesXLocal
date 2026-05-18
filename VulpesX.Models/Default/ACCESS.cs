namespace VulpesX.Models.Default;
 
public partial class ACCESS : Base 
{
	private string _USRID = null!;
	public required string USRID { get => _USRID; set { if (_USRID != value) { _USRID = value; NotifyPropertyChanged();} } }
	private string _USRPWD = null!;
	public required string USRPWD { get => _USRPWD; set { if (_USRPWD != value) { _USRPWD = value; NotifyPropertyChanged();} } }
	private string _USROLD = null!;
	public required string USROLD { get => _USROLD; set { if (_USROLD != value) { _USROLD = value; NotifyPropertyChanged();} } }
	private string? _USRPRECOM;
	public string? USRPRECOM { get => _USRPRECOM; set { if (_USRPRECOM != value) { _USRPRECOM = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
	private string? _USRPRETEMA;
	public string? USRPRETEMA { get => _USRPRETEMA; set { if (_USRPRETEMA != value) { _USRPRETEMA = value; NotifyPropertyChanged();} } }
	private string? _USRPREVIEW;
	public string? USRPREVIEW { get => _USRPREVIEW; set { if (_USRPREVIEW != value) { _USRPREVIEW = value; NotifyPropertyChanged();} } }
	private bool _usrwidqta;
	public bool usrwidqta { get => _usrwidqta; set { if (_usrwidqta != value) { _usrwidqta = value; NotifyPropertyChanged();} } }
	private int? _usrwidqtatim;
	public int? usrwidqtatim { get => _usrwidqtatim; set { if (_usrwidqtatim != value) { _usrwidqtatim = value; NotifyPropertyChanged();} } }
	private bool? _usrappreset;
	public bool? usrappreset { get => _usrappreset; set { if (_usrappreset != value) { _usrappreset = value; NotifyPropertyChanged();} } }
}