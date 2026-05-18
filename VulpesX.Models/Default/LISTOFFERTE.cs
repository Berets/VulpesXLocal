namespace VulpesX.Models.Default;
 
public partial class LISTOFFERTE : Base 
{
	private string _liosoc = null!;
	public required string liosoc { get => _liosoc; set { if (_liosoc != value) { _liosoc = value; NotifyPropertyChanged();} } }
	private int _liocli;
	public int liocli { get => _liocli; set { if (_liocli != value) { _liocli = value; NotifyPropertyChanged();} } }
	private string _lioart = null!;
	public required string lioart { get => _lioart; set { if (_lioart != value) { _lioart = value; NotifyPropertyChanged();} } }
	private DateTime _liodad;
	public DateTime liodad { get => _liodad; set { if (_liodad != value) { _liodad = value; NotifyPropertyChanged();} } }
	private DateTime _lioadd;
	public DateTime lioadd { get => _lioadd; set { if (_lioadd != value) { _lioadd = value; NotifyPropertyChanged();} } }
	private string _liotip = null!;
	public required string liotip { get => _liotip; set { if (_liotip != value) { _liotip = value; NotifyPropertyChanged();} } }
	private decimal? _liosc1;
	public decimal? liosc1 { get => _liosc1; set { if (_liosc1 != value) { _liosc1 = value; NotifyPropertyChanged();} } }
	private string? _liots1;
	public string? liots1 { get => _liots1; set { if (_liots1 != value) { _liots1 = value; NotifyPropertyChanged();} } }
	private decimal? _liosc2;
	public decimal? liosc2 { get => _liosc2; set { if (_liosc2 != value) { _liosc2 = value; NotifyPropertyChanged();} } }
	private string? _liots2;
	public string? liots2 { get => _liots2; set { if (_liots2 != value) { _liots2 = value; NotifyPropertyChanged();} } }
	private decimal? _liosc3;
	public decimal? liosc3 { get => _liosc3; set { if (_liosc3 != value) { _liosc3 = value; NotifyPropertyChanged();} } }
	private string? _liots3;
	public string? liots3 { get => _liots3; set { if (_liots3 != value) { _liots3 = value; NotifyPropertyChanged();} } }
	private decimal? _liomag;
	public decimal? liomag { get => _liomag; set { if (_liomag != value) { _liomag = value; NotifyPropertyChanged();} } }
	private string? _liotma;
	public string? liotma { get => _liotma; set { if (_liotma != value) { _liotma = value; NotifyPropertyChanged();} } }
	private decimal? _liopro;
	public decimal? liopro { get => _liopro; set { if (_liopro != value) { _liopro = value; NotifyPropertyChanged();} } }
	private decimal? _lioper;
	public decimal? lioper { get => _lioper; set { if (_lioper != value) { _lioper = value; NotifyPropertyChanged();} } }
	private decimal? _liopre;
	public decimal? liopre { get => _liopre; set { if (_liopre != value) { _liopre = value; NotifyPropertyChanged();} } }
	private decimal? _liofed;
	public decimal? liofed { get => _liofed; set { if (_liofed != value) { _liofed = value; NotifyPropertyChanged();} } }
	private string? _liofet;
	public string? liofet { get => _liofet; set { if (_liofet != value) { _liofet = value; NotifyPropertyChanged();} } }
}