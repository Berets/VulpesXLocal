namespace VulpesX.Models.Default;
 
public partial class PERSO00F : Base 
{
	private string _pesoci = null!;
	public required string pesoci { get => _pesoci; set { if (_pesoci != value) { _pesoci = value; NotifyPropertyChanged();} } }
	private int _peanno;
	public int peanno { get => _peanno; set { if (_peanno != value) { _peanno = value; NotifyPropertyChanged();} } }
	private DateTime? _pedati;
	public DateTime? pedati { get => _pedati; set { if (_pedati != value) { _pedati = value; NotifyPropertyChanged();} } }
	private DateTime? _pedatf;
	public DateTime? pedatf { get => _pedatf; set { if (_pedatf != value) { _pedatf = value; NotifyPropertyChanged();} } }
	private string? _pemeto;
	public string? pemeto { get => _pemeto; set { if (_pemeto != value) { _pemeto = value; NotifyPropertyChanged();} } }
	private string? _petemp;
	public string? petemp { get => _petemp; set { if (_petemp != value) { _petemp = value; NotifyPropertyChanged();} } }
	private string? _peresi;
	public string? peresi { get => _peresi; set { if (_peresi != value) { _peresi = value; NotifyPropertyChanged();} } }
	private string? _petipo;
	public string? petipo { get => _petipo; set { if (_petipo != value) { _petipo = value; NotifyPropertyChanged();} } }
	private string? _pecoll;
	public string? pecoll { get => _pecoll; set { if (_pecoll != value) { _pecoll = value; NotifyPropertyChanged();} } }
	private string? _peobbl;
	public string? peobbl { get => _peobbl; set { if (_peobbl != value) { _peobbl = value; NotifyPropertyChanged();} } }
	private string? _pesocb;
	public string? pesocb { get => _pesocb; set { if (_pesocb != value) { _pesocb = value; NotifyPropertyChanged();} } }
	private int? _penugg;
	public int? penugg { get => _penugg; set { if (_penugg != value) { _penugg = value; NotifyPropertyChanged();} } }
	private DateTime? _pedaci;
	public DateTime? pedaci { get => _pedaci; set { if (_pedaci != value) { _pedaci = value; NotifyPropertyChanged();} } }
	private DateTime? _pedacf;
	public DateTime? pedacf { get => _pedacf; set { if (_pedacf != value) { _pedacf = value; NotifyPropertyChanged();} } }
	private string? _peastf;
	public string? peastf { get => _peastf; set { if (_peastf != value) { _peastf = value; NotifyPropertyChanged();} } }
	private string? _peastc;
	public string? peastc { get => _peastc; set { if (_peastc != value) { _peastc = value; NotifyPropertyChanged();} } }
}