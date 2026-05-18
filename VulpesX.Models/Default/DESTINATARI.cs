namespace VulpesX.Models.Default;
 
public partial class DESTINATARI : Base 
{
	private int _cliecod;
	public int cliecod { get => _cliecod; set { if (_cliecod != value) { _cliecod = value; NotifyPropertyChanged();} } }
	private int _codesti;
	public int codesti { get => _codesti; set { if (_codesti != value) { _codesti = value; NotifyPropertyChanged();} } }
	private string _ragisoc = null!;
	public required string ragisoc { get => _ragisoc; set { if (_ragisoc != value) { _ragisoc = value; NotifyPropertyChanged();} } }
	private string? _DEINDI;
	public string? DEINDI { get => _DEINDI; set { if (_DEINDI != value) { _DEINDI = value; NotifyPropertyChanged();} } }
	private int? _DECAP;
	public int? DECAP { get => _DECAP; set { if (_DECAP != value) { _DECAP = value; NotifyPropertyChanged();} } }
	private string? _deloc;
	public string? deloc { get => _deloc; set { if (_deloc != value) { _deloc = value; NotifyPropertyChanged();} } }
	private string? _depro;
	public string? depro { get => _depro; set { if (_depro != value) { _depro = value; NotifyPropertyChanged();} } }
	private string? _person;
	public string? person { get => _person; set { if (_person != value) { _person = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
	private string? _decoag1;
	public string? decoag1 { get => _decoag1; set { if (_decoag1 != value) { _decoag1 = value; NotifyPropertyChanged();} } }
	private string? _decoag2;
	public string? decoag2 { get => _decoag2; set { if (_decoag2 != value) { _decoag2 = value; NotifyPropertyChanged();} } }
	private decimal? _deage1p;
	public decimal? deage1p { get => _deage1p; set { if (_deage1p != value) { _deage1p = value; NotifyPropertyChanged();} } }
	private decimal? _deage2p;
	public decimal? deage2p { get => _deage2p; set { if (_deage2p != value) { _deage2p = value; NotifyPropertyChanged();} } }
	private string? _deage1pt;
	public string? deage1pt { get => _deage1pt; set { if (_deage1pt != value) { _deage1pt = value; NotifyPropertyChanged();} } }
	private string? _deage2pt;
	public string? deage2pt { get => _deage2pt; set { if (_deage2pt != value) { _deage2pt = value; NotifyPropertyChanged();} } }
	private string? _isocod;
	public string? isocod { get => _isocod; set { if (_isocod != value) { _isocod = value; NotifyPropertyChanged();} } }
}