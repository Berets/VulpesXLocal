namespace VulpesX.Models.Default;
 
public partial class TAB_ACC_TIPPAG : Base 
{
	private string _inccod = null!;
	public required string inccod { get => _inccod; set { if (_inccod != value) { _inccod = value; NotifyPropertyChanged();} } }
	private string _incdes = null!;
	public required string incdes { get => _incdes; set { if (_incdes != value) { _incdes = value; NotifyPropertyChanged();} } }
	private string? _incods;
	public string? incods { get => _incods; set { if (_incods != value) { _incods = value; NotifyPropertyChanged();} } }
	private string? _incsup;
	public string? incsup { get => _incsup; set { if (_incsup != value) { _incsup = value; NotifyPropertyChanged();} } }
	private string? _incpor;
	public string? incpor { get => _incpor; set { if (_incpor != value) { _incpor = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}