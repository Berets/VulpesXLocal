namespace VulpesX.Models.Default;
 
public partial class LISTINO : Base 
{
	private string _lissoc = null!;
	public required string lissoc { get => _lissoc; set { if (_lissoc != value) { _lissoc = value; NotifyPropertyChanged();} } }
	private int _listcli;
	public int listcli { get => _listcli; set { if (_listcli != value) { _listcli = value; NotifyPropertyChanged();} } }
	private string _listart = null!;
	public required string listart { get => _listart; set { if (_listart != value) { _listart = value; NotifyPropertyChanged();} } }
	private DateTime _lisdata;
	public DateTime lisdata { get => _lisdata; set { if (_lisdata != value) { _lisdata = value; NotifyPropertyChanged();} } }
	private string _listip = null!;
	public required string listip { get => _listip; set { if (_listip != value) { _listip = value; NotifyPropertyChanged();} } }
	private decimal? _liscon1;
	public decimal? liscon1 { get => _liscon1; set { if (_liscon1 != value) { _liscon1 = value; NotifyPropertyChanged();} } }
	private decimal? _liscon2;
	public decimal? liscon2 { get => _liscon2; set { if (_liscon2 != value) { _liscon2 = value; NotifyPropertyChanged();} } }
	private string? _listsc1;
	public string? listsc1 { get => _listsc1; set { if (_listsc1 != value) { _listsc1 = value; NotifyPropertyChanged();} } }
	private string? _listsc2;
	public string? listsc2 { get => _listsc2; set { if (_listsc2 != value) { _listsc2 = value; NotifyPropertyChanged();} } }
	private decimal? _liscon3;
	public decimal? liscon3 { get => _liscon3; set { if (_liscon3 != value) { _liscon3 = value; NotifyPropertyChanged();} } }
	private string? _listsc3;
	public string? listsc3 { get => _listsc3; set { if (_listsc3 != value) { _listsc3 = value; NotifyPropertyChanged();} } }
	private decimal? _lismag;
	public decimal? lismag { get => _lismag; set { if (_lismag != value) { _lismag = value; NotifyPropertyChanged();} } }
	private string? _listma;
	public string? listma { get => _listma; set { if (_listma != value) { _listma = value; NotifyPropertyChanged();} } }
	private decimal? _lispro;
	public decimal? lispro { get => _lispro; set { if (_lispro != value) { _lispro = value; NotifyPropertyChanged();} } }
	private decimal? _lisper;
	public decimal? lisper { get => _lisper; set { if (_lisper != value) { _lisper = value; NotifyPropertyChanged();} } }
	private decimal? _lispre;
	public decimal? lispre { get => _lispre; set { if (_lispre != value) { _lispre = value; NotifyPropertyChanged();} } }
	private decimal? _lisfed;
	public decimal? lisfed { get => _lisfed; set { if (_lisfed != value) { _lisfed = value; NotifyPropertyChanged();} } }
	private string? _lisfet;
	public string? lisfet { get => _lisfet; set { if (_lisfet != value) { _lisfet = value; NotifyPropertyChanged();} } }
}