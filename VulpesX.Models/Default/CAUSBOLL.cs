namespace VulpesX.Models.Default;
 
public partial class CAUSBOLL : Base 
{
	private string _bolcod = null!;
	public required string bolcod { get => _bolcod; set { if (_bolcod != value) { _bolcod = value; NotifyPropertyChanged();} } }
	private string? _boldes;
	public string? boldes { get => _boldes; set { if (_boldes != value) { _boldes = value; NotifyPropertyChanged();} } }
	private string? _BOLCAU;
	public string? BOLCAU { get => _BOLCAU; set { if (_BOLCAU != value) { _BOLCAU = value; NotifyPropertyChanged();} } }
	private string? _bolmag;
	public string? bolmag { get => _bolmag; set { if (_bolmag != value) { _bolmag = value; NotifyPropertyChanged();} } }
	private string? _bolfat;
	public string? bolfat { get => _bolfat; set { if (_bolfat != value) { _bolfat = value; NotifyPropertyChanged();} } }
	private string? _bolfac;
	public string? bolfac { get => _bolfac; set { if (_bolfac != value) { _bolfac = value; NotifyPropertyChanged();} } }
	private string? _bolcli;
	public string? bolcli { get => _bolcli; set { if (_bolcli != value) { _bolcli = value; NotifyPropertyChanged();} } }
	private string? _bolfor;
	public string? bolfor { get => _bolfor; set { if (_bolfor != value) { _bolfor = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
	private string? _bolnum;
	public string? bolnum { get => _bolnum; set { if (_bolnum != value) { _bolnum = value; NotifyPropertyChanged();} } }
	private string? _bolpre;
	public string? bolpre { get => _bolpre; set { if (_bolpre != value) { _bolpre = value; NotifyPropertyChanged();} } }
}