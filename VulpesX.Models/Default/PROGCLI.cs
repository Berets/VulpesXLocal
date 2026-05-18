namespace VulpesX.Models.Default;
 
public partial class PROGCLI : Base 
{
	private string _Pcodsoc = null!;
	public required string Pcodsoc { get => _Pcodsoc; set { if (_Pcodsoc != value) { _Pcodsoc = value; NotifyPropertyChanged();} } }
	private int _Pcodcli;
	public int Pcodcli { get => _Pcodcli; set { if (_Pcodcli != value) { _Pcodcli = value; NotifyPropertyChanged();} } }
	private int _Pprog;
	public int Pprog { get => _Pprog; set { if (_Pprog != value) { _Pprog = value; NotifyPropertyChanged();} } }
	private string _Proddes = null!;
	public required string Proddes { get => _Proddes; set { if (_Proddes != value) { _Proddes = value; NotifyPropertyChanged();} } }
	private string _Percorso = null!;
	public required string Percorso { get => _Percorso; set { if (_Percorso != value) { _Percorso = value; NotifyPropertyChanged();} } }
	private string _Gennum = null!;
	public required string Gennum { get => _Gennum; set { if (_Gennum != value) { _Gennum = value; NotifyPropertyChanged();} } }
	private int _UpgDE;
	public int UpgDE { get => _UpgDE; set { if (_UpgDE != value) { _UpgDE = value; NotifyPropertyChanged();} } }
	private int _Build;
	public int Build { get => _Build; set { if (_Build != value) { _Build = value; NotifyPropertyChanged();} } }
	private string _Cartsercli = null!;
	public required string Cartsercli { get => _Cartsercli; set { if (_Cartsercli != value) { _Cartsercli = value; NotifyPropertyChanged();} } }
	private string _UtConn = null!;
	public required string UtConn { get => _UtConn; set { if (_UtConn != value) { _UtConn = value; NotifyPropertyChanged();} } }
	private string _pswCon = null!;
	public required string pswCon { get => _pswCon; set { if (_pswCon != value) { _pswCon = value; NotifyPropertyChanged();} } }
	private string _Pubserver = null!;
	public required string Pubserver { get => _Pubserver; set { if (_Pubserver != value) { _Pubserver = value; NotifyPropertyChanged();} } }
	private string _Utserpub = null!;
	public required string Utserpub { get => _Utserpub; set { if (_Utserpub != value) { _Utserpub = value; NotifyPropertyChanged();} } }
	private string _pswserpub = null!;
	public required string pswserpub { get => _pswserpub; set { if (_pswserpub != value) { _pswserpub = value; NotifyPropertyChanged();} } }
	private string _Indconrem = null!;
	public required string Indconrem { get => _Indconrem; set { if (_Indconrem != value) { _Indconrem = value; NotifyPropertyChanged();} } }
	private string _TipConRem = null!;
	public required string TipConRem { get => _TipConRem; set { if (_TipConRem != value) { _TipConRem = value; NotifyPropertyChanged();} } }
	private string _Pnote = null!;
	public required string Pnote { get => _Pnote; set { if (_Pnote != value) { _Pnote = value; NotifyPropertyChanged();} } }
	private int _Prigamax;
	public int Prigamax { get => _Prigamax; set { if (_Prigamax != value) { _Prigamax = value; NotifyPropertyChanged();} } }
}