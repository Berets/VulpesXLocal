namespace VulpesX.Models.Default;
 
public partial class NODE_3 : Base 
{
	private string _nodesoc = null!;
	public required string nodesoc { get => _nodesoc; set { if (_nodesoc != value) { _nodesoc = value; NotifyPropertyChanged();} } }
	private int _nodeann;
	public int nodeann { get => _nodeann; set { if (_nodeann != value) { _nodeann = value; NotifyPropertyChanged();} } }
	private string _nodetip = null!;
	public required string nodetip { get => _nodetip; set { if (_nodetip != value) { _nodetip = value; NotifyPropertyChanged();} } }
	private int _nodefil;
	public int nodefil { get => _nodefil; set { if (_nodefil != value) { _nodefil = value; NotifyPropertyChanged();} } }
	private int _nodenum1;
	public int nodenum1 { get => _nodenum1; set { if (_nodenum1 != value) { _nodenum1 = value; NotifyPropertyChanged();} } }
	private int _nodenum2;
	public int nodenum2 { get => _nodenum2; set { if (_nodenum2 != value) { _nodenum2 = value; NotifyPropertyChanged();} } }
	private int _nodenum3;
	public int nodenum3 { get => _nodenum3; set { if (_nodenum3 != value) { _nodenum3 = value; NotifyPropertyChanged();} } }
	private string? _nodegruppo;
	public string? nodegruppo { get => _nodegruppo; set { if (_nodegruppo != value) { _nodegruppo = value; NotifyPropertyChanged();} } }
	private string? _nodeconto;
	public string? nodeconto { get => _nodeconto; set { if (_nodeconto != value) { _nodeconto = value; NotifyPropertyChanged();} } }
	private string? _nodesotto;
	public string? nodesotto { get => _nodesotto; set { if (_nodesotto != value) { _nodesotto = value; NotifyPropertyChanged();} } }
	private decimal? _nodevalue;
	public decimal? nodevalue { get => _nodevalue; set { if (_nodevalue != value) { _nodevalue = value; NotifyPropertyChanged();} } }
	private DateTime? _nodedat;
	public DateTime? nodedat { get => _nodedat; set { if (_nodedat != value) { _nodedat = value; NotifyPropertyChanged();} } }
	private int? _nodeseq;
	public int? nodeseq { get => _nodeseq; set { if (_nodeseq != value) { _nodeseq = value; NotifyPropertyChanged();} } }
	private decimal? _nodeprevalue;
	public decimal? nodeprevalue { get => _nodeprevalue; set { if (_nodeprevalue != value) { _nodeprevalue = value; NotifyPropertyChanged();} } }
	private decimal? _nodeprecalc;
	public decimal? nodeprecalc { get => _nodeprecalc; set { if (_nodeprecalc != value) { _nodeprecalc = value; NotifyPropertyChanged();} } }
	private decimal? _nodebudget;
	public decimal? nodebudget { get => _nodebudget; set { if (_nodebudget != value) { _nodebudget = value; NotifyPropertyChanged();} } }
}