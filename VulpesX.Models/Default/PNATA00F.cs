namespace VulpesX.Models.Default;
 
public partial class PNATA00F : Base 
{
	private string _pnasoc = null!;
	public required string pnasoc { get => _pnasoc; set { if (_pnasoc != value) { _pnasoc = value; NotifyPropertyChanged();} } }
	private int _pnaann;
	public int pnaann { get => _pnaann; set { if (_pnaann != value) { _pnaann = value; NotifyPropertyChanged();} } }
	private int _pnanum;
	public int pnanum { get => _pnanum; set { if (_pnanum != value) { _pnanum = value; NotifyPropertyChanged();} } }
	private DateTime? _pnadat;
	public DateTime? pnadat { get => _pnadat; set { if (_pnadat != value) { _pnadat = value; NotifyPropertyChanged();} } }
	private DateTime? _pnaddo;
	public DateTime? pnaddo { get => _pnaddo; set { if (_pnaddo != value) { _pnaddo = value; NotifyPropertyChanged();} } }
	private int? _pnadoc;
	public int? pnadoc { get => _pnadoc; set { if (_pnadoc != value) { _pnadoc = value; NotifyPropertyChanged();} } }
	private string? _pnacau;
	public string? pnacau { get => _pnacau; set { if (_pnacau != value) { _pnacau = value; NotifyPropertyChanged();} } }
	private string? _pnagru;
	public string? pnagru { get => _pnagru; set { if (_pnagru != value) { _pnagru = value; NotifyPropertyChanged();} } }
	private string? _pnacon;
	public string? pnacon { get => _pnacon; set { if (_pnacon != value) { _pnacon = value; NotifyPropertyChanged();} } }
	private string? _pnasot;
	public string? pnasot { get => _pnasot; set { if (_pnasot != value) { _pnasot = value; NotifyPropertyChanged();} } }
	private decimal? _pnaimp;
	public decimal? pnaimp { get => _pnaimp; set { if (_pnaimp != value) { _pnaimp = value; NotifyPropertyChanged();} } }
	private decimal? _pnaime;
	public decimal? pnaime { get => _pnaime; set { if (_pnaime != value) { _pnaime = value; NotifyPropertyChanged();} } }
	private decimal? _pnaqta;
	public decimal? pnaqta { get => _pnaqta; set { if (_pnaqta != value) { _pnaqta = value; NotifyPropertyChanged();} } }
	private string? _pnaseg;
	public string? pnaseg { get => _pnaseg; set { if (_pnaseg != value) { _pnaseg = value; NotifyPropertyChanged();} } }
	private string? _pnacos;
	public string? pnacos { get => _pnacos; set { if (_pnacos != value) { _pnacos = value; NotifyPropertyChanged();} } }
	private string? _pnaspe;
	public string? pnaspe { get => _pnaspe; set { if (_pnaspe != value) { _pnaspe = value; NotifyPropertyChanged();} } }
	private string? _pnades;
	public string? pnades { get => _pnades; set { if (_pnades != value) { _pnades = value; NotifyPropertyChanged();} } }
	private int? _pnaapn;
	public int? pnaapn { get => _pnaapn; set { if (_pnaapn != value) { _pnaapn = value; NotifyPropertyChanged();} } }
	private int? _pnanpn;
	public int? pnanpn { get => _pnanpn; set { if (_pnanpn != value) { _pnanpn = value; NotifyPropertyChanged();} } }
	private int? _pnarpn;
	public int? pnarpn { get => _pnarpn; set { if (_pnarpn != value) { _pnarpn = value; NotifyPropertyChanged();} } }
	private int? _pnacod;
	public int? pnacod { get => _pnacod; set { if (_pnacod != value) { _pnacod = value; NotifyPropertyChanged();} } }
	private string? _pnaatt;
	public string? pnaatt { get => _pnaatt; set { if (_pnaatt != value) { _pnaatt = value; NotifyPropertyChanged();} } }
	private string? _pnaori;
	public string? pnaori { get => _pnaori; set { if (_pnaori != value) { _pnaori = value; NotifyPropertyChanged();} } }
	private string? _pnaorv;
	public string? pnaorv { get => _pnaorv; set { if (_pnaorv != value) { _pnaorv = value; NotifyPropertyChanged();} } }
	private string? _pnauti;
	public string? pnauti { get => _pnauti; set { if (_pnauti != value) { _pnauti = value; NotifyPropertyChanged();} } }
	private string? _pnautv;
	public string? pnautv { get => _pnautv; set { if (_pnautv != value) { _pnautv = value; NotifyPropertyChanged();} } }
	private string? _pnater;
	public string? pnater { get => _pnater; set { if (_pnater != value) { _pnater = value; NotifyPropertyChanged();} } }
	private int? _PLFLAG;
	public int? PLFLAG { get => _PLFLAG; set { if (_PLFLAG != value) { _PLFLAG = value; NotifyPropertyChanged();} } }
}