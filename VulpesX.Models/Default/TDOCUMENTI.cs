namespace VulpesX.Models.Default;
 
public partial class TDOCUMENTI : Base 
{
	private string _docucod = null!;
	public required string docucod { get => _docucod; set { if (_docucod != value) { _docucod = value; NotifyPropertyChanged();} } }
	private string? _docudes;
	public string? docudes { get => _docudes; set { if (_docudes != value) { _docudes = value; NotifyPropertyChanged();} } }
	private string? _docucaus;
	public string? docucaus { get => _docucaus; set { if (_docucaus != value) { _docucaus = value; NotifyPropertyChanged();} } }
	private string? _docuritesott;
	public string? docuritesott { get => _docuritesott; set { if (_docuritesott != value) { _docuritesott = value; NotifyPropertyChanged();} } }
	private string? _docuriteco;
	public string? docuriteco { get => _docuriteco; set { if (_docuriteco != value) { _docuriteco = value; NotifyPropertyChanged();} } }
	private string? _docuritegru;
	public string? docuritegru { get => _docuritegru; set { if (_docuritegru != value) { _docuritegru = value; NotifyPropertyChanged();} } }
	private string? _docucausrite;
	public string? docucausrite { get => _docucausrite; set { if (_docucausrite != value) { _docucausrite = value; NotifyPropertyChanged();} } }
	private string? _docuflg;
	public string? docuflg { get => _docuflg; set { if (_docuflg != value) { _docuflg = value; NotifyPropertyChanged();} } }
}