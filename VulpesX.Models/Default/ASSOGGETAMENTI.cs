namespace VulpesX.Models.Default;
 
public partial class ASSOGGETAMENTI : Base 
{
	private string _asscod = null!;
	public required string asscod { get => _asscod; set { if (_asscod != value) { _asscod = value; NotifyPropertyChanged();} } }
	private string _assali = null!;
	public required string assali { get => _assali; set { if (_assali != value) { _assali = value; NotifyPropertyChanged();} } }
	private string _assdes = null!;
	public required string assdes { get => _assdes; set { if (_assdes != value) { _assdes = value; NotifyPropertyChanged();} } }
	private string? _asscli;
	public string? asscli { get => _asscli; set { if (_asscli != value) { _asscli = value; NotifyPropertyChanged();} } }
	private string? _assfor;
	public string? assfor { get => _assfor; set { if (_assfor != value) { _assfor = value; NotifyPropertyChanged();} } }
	private int? _asspin;
	public int? asspin { get => _asspin; set { if (_asspin != value) { _asspin = value; NotifyPropertyChanged();} } }
	private string? _assoma;
	public string? assoma { get => _assoma; set { if (_assoma != value) { _assoma = value; NotifyPropertyChanged();} } }
	private string? _asspla;
	public string? asspla { get => _asspla; set { if (_asspla != value) { _asspla = value; NotifyPropertyChanged();} } }
	private string? _asstif;
	public string? asstif { get => _asstif; set { if (_asstif != value) { _asstif = value; NotifyPropertyChanged();} } }
	private string? _asstic;
	public string? asstic { get => _asstic; set { if (_asstic != value) { _asstic = value; NotifyPropertyChanged();} } }
	private string? _asself;
	public string? asself { get => _asself; set { if (_asself != value) { _asself = value; NotifyPropertyChanged();} } }
	private string? _asselc;
	public string? asselc { get => _asselc; set { if (_asselc != value) { _asselc = value; NotifyPropertyChanged();} } }
	private string? _asstipiva;
	public string? asstipiva { get => _asstipiva; set { if (_asstipiva != value) { _asstipiva = value; NotifyPropertyChanged();} } }
	private string? _asscomiva;
	public string? asscomiva { get => _asscomiva; set { if (_asscomiva != value) { _asscomiva = value; NotifyPropertyChanged();} } }
	private string? _asssplpay;
	public string? asssplpay { get => _asssplpay; set { if (_asssplpay != value) { _asssplpay = value; NotifyPropertyChanged();} } }
	private string? _assnatufe;
	public string? assnatufe { get => _assnatufe; set { if (_assnatufe != value) { _assnatufe = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
	private string? _assomacod;
	public string? assomacod { get => _assomacod; set { if (_assomacod != value) { _assomacod = value; NotifyPropertyChanged();} } }
	private string? _assomaali;
	public string? assomaali { get => _assomaali; set { if (_assomaali != value) { _assomaali = value; NotifyPropertyChanged();} } }
	private string? _assventcod;
	public string? assventcod { get => _assventcod; set { if (_assventcod != value) { _assventcod = value; NotifyPropertyChanged();} } }
	private string? _assventali;
	public string? assventali { get => _assventali; set { if (_assventali != value) { _assventali = value; NotifyPropertyChanged();} } }
}