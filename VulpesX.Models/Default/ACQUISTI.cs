namespace VulpesX.Models.Default;
 
public partial class ACQUISTI : Base 
{
	private string _ACQSOC = null!;
	public required string ACQSOC { get => _ACQSOC; set { if (_ACQSOC != value) { _ACQSOC = value; NotifyPropertyChanged();} } }
	private string _ACQTIP = null!;
	public required string ACQTIP { get => _ACQTIP; set { if (_ACQTIP != value) { _ACQTIP = value; NotifyPropertyChanged();} } }
	private int _ACQRIG;
	public int ACQRIG { get => _ACQRIG; set { if (_ACQRIG != value) { _ACQRIG = value; NotifyPropertyChanged();} } }
	private string? _ACQDES;
	public string? ACQDES { get => _ACQDES; set { if (_ACQDES != value) { _ACQDES = value; NotifyPropertyChanged();} } }
	private string? _ACQMAT;
	public string? ACQMAT { get => _ACQMAT; set { if (_ACQMAT != value) { _ACQMAT = value; NotifyPropertyChanged();} } }
	private string? _ACQMAR;
	public string? ACQMAR { get => _ACQMAR; set { if (_ACQMAR != value) { _ACQMAR = value; NotifyPropertyChanged();} } }
	private string? _ACQMOD;
	public string? ACQMOD { get => _ACQMOD; set { if (_ACQMOD != value) { _ACQMOD = value; NotifyPropertyChanged();} } }
	private string? _ACQNSE;
	public string? ACQNSE { get => _ACQNSE; set { if (_ACQNSE != value) { _ACQNSE = value; NotifyPropertyChanged();} } }
	private DateTime? _ACQDAF;
	public DateTime? ACQDAF { get => _ACQDAF; set { if (_ACQDAF != value) { _ACQDAF = value; NotifyPropertyChanged();} } }
	private int? _ACQNUF;
	public int? ACQNUF { get => _ACQNUF; set { if (_ACQNUF != value) { _ACQNUF = value; NotifyPropertyChanged();} } }
	private string? _ACQFOR;
	public string? ACQFOR { get => _ACQFOR; set { if (_ACQFOR != value) { _ACQFOR = value; NotifyPropertyChanged();} } }
	private DateTime? _ACQDAG;
	public DateTime? ACQDAG { get => _ACQDAG; set { if (_ACQDAG != value) { _ACQDAG = value; NotifyPropertyChanged();} } }
	private DateTime? _ACQDSG;
	public DateTime? ACQDSG { get => _ACQDSG; set { if (_ACQDSG != value) { _ACQDSG = value; NotifyPropertyChanged();} } }
	private string? _ACQNOT;
	public string? ACQNOT { get => _ACQNOT; set { if (_ACQNOT != value) { _ACQNOT = value; NotifyPropertyChanged();} } }
	private string? _ACQMA1;
	public string? ACQMA1 { get => _ACQMA1; set { if (_ACQMA1 != value) { _ACQMA1 = value; NotifyPropertyChanged();} } }
	private string? _ACQMA2;
	public string? ACQMA2 { get => _ACQMA2; set { if (_ACQMA2 != value) { _ACQMA2 = value; NotifyPropertyChanged();} } }
	private int? _ACQCLI;
	public int? ACQCLI { get => _ACQCLI; set { if (_ACQCLI != value) { _ACQCLI = value; NotifyPropertyChanged();} } }
	private int? _ACQQTA;
	public int? ACQQTA { get => _ACQQTA; set { if (_ACQQTA != value) { _ACQQTA = value; NotifyPropertyChanged();} } }
}