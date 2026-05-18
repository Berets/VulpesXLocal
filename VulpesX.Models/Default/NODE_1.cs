namespace VulpesX.Models.Default;
 
public partial class NODE_1 : Base 
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
	private string? _nodedes;
	public string? nodedes { get => _nodedes; set { if (_nodedes != value) { _nodedes = value; NotifyPropertyChanged();} } }
	private string? _nodetot;
	public string? nodetot { get => _nodetot; set { if (_nodetot != value) { _nodetot = value; NotifyPropertyChanged();} } }
	private string? _nodetis;
	public string? nodetis { get => _nodetis; set { if (_nodetis != value) { _nodetis = value; NotifyPropertyChanged();} } }
	private string? _nodetas;
	public string? nodetas { get => _nodetas; set { if (_nodetas != value) { _nodetas = value; NotifyPropertyChanged();} } }
	private DateTime? _nodedat;
	public DateTime? nodedat { get => _nodedat; set { if (_nodedat != value) { _nodedat = value; NotifyPropertyChanged();} } }
	private string? _nodeseg;
	public string? nodeseg { get => _nodeseg; set { if (_nodeseg != value) { _nodeseg = value; NotifyPropertyChanged();} } }
	private int? _nodeseq;
	public int? nodeseq { get => _nodeseq; set { if (_nodeseq != value) { _nodeseq = value; NotifyPropertyChanged();} } }
	private string? _nodecec;
	public string? nodecec { get => _nodecec; set { if (_nodecec != value) { _nodecec = value; NotifyPropertyChanged();} } }
}