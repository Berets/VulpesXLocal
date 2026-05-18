namespace VulpesX.Models.Default;
 
public partial class PROGCLILEVEL1 : Base 
{
	private string _Pcodsoc = null!;
	public required string Pcodsoc { get => _Pcodsoc; set { if (_Pcodsoc != value) { _Pcodsoc = value; NotifyPropertyChanged();} } }
	private int _Pcodcli;
	public int Pcodcli { get => _Pcodcli; set { if (_Pcodcli != value) { _Pcodcli = value; NotifyPropertyChanged();} } }
	private int _Pprog;
	public int Pprog { get => _Pprog; set { if (_Pprog != value) { _Pprog = value; NotifyPropertyChanged();} } }
	private int _Priga;
	public int Priga { get => _Priga; set { if (_Priga != value) { _Priga = value; NotifyPropertyChanged();} } }
	private string _Ptipo = null!;
	public required string Ptipo { get => _Ptipo; set { if (_Ptipo != value) { _Ptipo = value; NotifyPropertyChanged();} } }
	private string? _gener;
	public string? gener { get => _gener; set { if (_gener != value) { _gener = value; NotifyPropertyChanged();} } }
	private string? _ProtDB;
	public string? ProtDB { get => _ProtDB; set { if (_ProtDB != value) { _ProtDB = value; NotifyPropertyChanged();} } }
	private string? _Utbdprot;
	public string? Utbdprot { get => _Utbdprot; set { if (_Utbdprot != value) { _Utbdprot = value; NotifyPropertyChanged();} } }
	private string? _Pswdbprot;
	public string? Pswdbprot { get => _Pswdbprot; set { if (_Pswdbprot != value) { _Pswdbprot = value; NotifyPropertyChanged();} } }
	private string? _Versdbprot;
	public string? Versdbprot { get => _Versdbprot; set { if (_Versdbprot != value) { _Versdbprot = value; NotifyPropertyChanged();} } }
	private string? _ProtSer;
	public string? ProtSer { get => _ProtSer; set { if (_ProtSer != value) { _ProtSer = value; NotifyPropertyChanged();} } }
	private int? _Upg;
	public int? Upg { get => _Upg; set { if (_Upg != value) { _Upg = value; NotifyPropertyChanged();} } }
	private int? _Build2;
	public int? Build2 { get => _Build2; set { if (_Build2 != value) { _Build2 = value; NotifyPropertyChanged();} } }
	private string? _framew;
	public string? framew { get => _framew; set { if (_framew != value) { _framew = value; NotifyPropertyChanged();} } }
}