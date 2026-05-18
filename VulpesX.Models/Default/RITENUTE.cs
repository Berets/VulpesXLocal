namespace VulpesX.Models.Default;
 
public partial class RITENUTE : Base 
{
	private string _ritcod = null!;
	public required string ritcod { get => _ritcod; set { if (_ritcod != value) { _ritcod = value; NotifyPropertyChanged();} } }
	private string _ritdes = null!;
	public required string ritdes { get => _ritdes; set { if (_ritdes != value) { _ritdes = value; NotifyPropertyChanged();} } }
	private string? _ritgr1;
	public string? ritgr1 { get => _ritgr1; set { if (_ritgr1 != value) { _ritgr1 = value; NotifyPropertyChanged();} } }
	private string? _ritco1;
	public string? ritco1 { get => _ritco1; set { if (_ritco1 != value) { _ritco1 = value; NotifyPropertyChanged();} } }
	private string? _ritso1;
	public string? ritso1 { get => _ritso1; set { if (_ritso1 != value) { _ritso1 = value; NotifyPropertyChanged();} } }
	private string? _ritca1;
	public string? ritca1 { get => _ritca1; set { if (_ritca1 != value) { _ritca1 = value; NotifyPropertyChanged();} } }
	private string? _ritfl1;
	public string? ritfl1 { get => _ritfl1; set { if (_ritfl1 != value) { _ritfl1 = value; NotifyPropertyChanged();} } }
	private string? _ritgr2;
	public string? ritgr2 { get => _ritgr2; set { if (_ritgr2 != value) { _ritgr2 = value; NotifyPropertyChanged();} } }
	private string? _ritco2;
	public string? ritco2 { get => _ritco2; set { if (_ritco2 != value) { _ritco2 = value; NotifyPropertyChanged();} } }
	private string? _ritso2;
	public string? ritso2 { get => _ritso2; set { if (_ritso2 != value) { _ritso2 = value; NotifyPropertyChanged();} } }
	private string? _ritca2;
	public string? ritca2 { get => _ritca2; set { if (_ritca2 != value) { _ritca2 = value; NotifyPropertyChanged();} } }
	private string? _ritfl2;
	public string? ritfl2 { get => _ritfl2; set { if (_ritfl2 != value) { _ritfl2 = value; NotifyPropertyChanged();} } }
	private string? _ritpag;
	public string? ritpag { get => _ritpag; set { if (_ritpag != value) { _ritpag = value; NotifyPropertyChanged();} } }
	private string? _rtsez;
	public string? rtsez { get => _rtsez; set { if (_rtsez != value) { _rtsez = value; NotifyPropertyChanged();} } }
	private string? _rttipo;
	public string? rttipo { get => _rttipo; set { if (_rttipo != value) { _rttipo = value; NotifyPropertyChanged();} } }
	private int? _rtmese;
	public int? rtmese { get => _rtmese; set { if (_rtmese != value) { _rtmese = value; NotifyPropertyChanged();} } }
	private string? _rtTipRed;
	public string? rtTipRed { get => _rtTipRed; set { if (_rtTipRed != value) { _rtTipRed = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}