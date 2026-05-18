namespace VulpesX.Models.Default;
 
public partial class CODMAG : Base 
{
	private string _codmag = null!;
	public required string codmag { get => _codmag; set { if (_codmag != value) { _codmag = value; NotifyPropertyChanged();} } }
	private string? _coddes;
	public string? coddes { get => _coddes; set { if (_coddes != value) { _coddes = value; NotifyPropertyChanged();} } }
	private string? _codflg;
	public string? codflg { get => _codflg; set { if (_codflg != value) { _codflg = value; NotifyPropertyChanged();} } }
	private string? _codrag;
	public string? codrag { get => _codrag; set { if (_codrag != value) { _codrag = value; NotifyPropertyChanged();} } }
	private string? _codind;
	public string? codind { get => _codind; set { if (_codind != value) { _codind = value; NotifyPropertyChanged();} } }
	private string? _codloc;
	public string? codloc { get => _codloc; set { if (_codloc != value) { _codloc = value; NotifyPropertyChanged();} } }
	private int? _codcap;
	public int? codcap { get => _codcap; set { if (_codcap != value) { _codcap = value; NotifyPropertyChanged();} } }
	private string? _codpr1;
	public string? codpr1 { get => _codpr1; set { if (_codpr1 != value) { _codpr1 = value; NotifyPropertyChanged();} } }
	private int? _codpr2;
	public int? codpr2 { get => _codpr2; set { if (_codpr2 != value) { _codpr2 = value; NotifyPropertyChanged();} } }
	private string? _magsoc;
	public string? magsoc { get => _magsoc; set { if (_magsoc != value) { _magsoc = value; NotifyPropertyChanged();} } }
}