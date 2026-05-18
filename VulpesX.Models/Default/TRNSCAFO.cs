namespace VulpesX.Models.Default;
 
public partial class TRNSCAFO : Base 
{
	private string _scasob = null!;
	public required string scasob { get => _scasob; set { if (_scasob != value) { _scasob = value; NotifyPropertyChanged();} } }
	private int _Scafor;
	public int Scafor { get => _Scafor; set { if (_Scafor != value) { _Scafor = value; NotifyPropertyChanged();} } }
	private DateTime _scadar;
	public DateTime scadar { get => _scadar; set { if (_scadar != value) { _scadar = value; NotifyPropertyChanged();} } }
	private string _scanur = null!;
	public required string scanur { get => _scanur; set { if (_scanur != value) { _scanur = value; NotifyPropertyChanged();} } }
	private DateTime _scadsc;
	public DateTime scadsc { get => _scadsc; set { if (_scadsc != value) { _scadsc = value; NotifyPropertyChanged();} } }
	private string? _scatpa;
	public string? scatpa { get => _scatpa; set { if (_scatpa != value) { _scatpa = value; NotifyPropertyChanged();} } }
	private decimal? _scasal;
	public decimal? scasal { get => _scasal; set { if (_scasal != value) { _scasal = value; NotifyPropertyChanged();} } }
	private string? _scaseg;
	public string? scaseg { get => _scaseg; set { if (_scaseg != value) { _scaseg = value; NotifyPropertyChanged();} } }
	private int? _scaann;
	public int? scaann { get => _scaann; set { if (_scaann != value) { _scaann = value; NotifyPropertyChanged();} } }
	private int? _scames;
	public int? scames { get => _scames; set { if (_scames != value) { _scames = value; NotifyPropertyChanged();} } }
}