namespace VulpesX.Models.Default;
 
public partial class SOLLECITI : Base 
{
	private int _solcod;
	public int solcod { get => _solcod; set { if (_solcod != value) { _solcod = value; NotifyPropertyChanged();} } }
	private string _soldes = null!;
	public required string soldes { get => _soldes; set { if (_soldes != value) { _soldes = value; NotifyPropertyChanged();} } }
	private string? _tpflg;
	public string? tpflg { get => _tpflg; set { if (_tpflg != value) { _tpflg = value; NotifyPropertyChanged();} } }
	private string? _tptit;
	public string? tptit { get => _tptit; set { if (_tptit != value) { _tptit = value; NotifyPropertyChanged();} } }
	private string? _tptest;
	public string? tptest { get => _tptest; set { if (_tptest != value) { _tptest = value; NotifyPropertyChanged();} } }
	private string? _tpsoc;
	public string? tpsoc { get => _tpsoc; set { if (_tpsoc != value) { _tpsoc = value; NotifyPropertyChanged();} } }
	private int? _tpgra;
	public int? tpgra { get => _tpgra; set { if (_tpgra != value) { _tpgra = value; NotifyPropertyChanged();} } }
	private string? _tpleg;
	public string? tpleg { get => _tpleg; set { if (_tpleg != value) { _tpleg = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}