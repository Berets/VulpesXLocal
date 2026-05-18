namespace VulpesX.Models.Default;
 
public partial class PERSONALE : Base 
{
	private string _prssoc = null!;
	public required string prssoc { get => _prssoc; set { if (_prssoc != value) { _prssoc = value; NotifyPropertyChanged();} } }
	private string _prsid1 = null!;
	public required string prsid1 { get => _prsid1; set { if (_prsid1 != value) { _prsid1 = value; NotifyPropertyChanged();} } }
	private string _prsid2 = null!;
	public required string prsid2 { get => _prsid2; set { if (_prsid2 != value) { _prsid2 = value; NotifyPropertyChanged();} } }
	private string _prsid3 = null!;
	public required string prsid3 { get => _prsid3; set { if (_prsid3 != value) { _prsid3 = value; NotifyPropertyChanged();} } }
	private string? _prscog;
	public string? prscog { get => _prscog; set { if (_prscog != value) { _prscog = value; NotifyPropertyChanged();} } }
	private string? _prsnom;
	public string? prsnom { get => _prsnom; set { if (_prsnom != value) { _prsnom = value; NotifyPropertyChanged();} } }
	private int? _prsit;
	public int? prsit { get => _prsit; set { if (_prsit != value) { _prsit = value; NotifyPropertyChanged();} } }
	private string? _prscin;
	public string? prscin { get => _prscin; set { if (_prscin != value) { _prscin = value; NotifyPropertyChanged();} } }
	private int? _prsabi;
	public int? prsabi { get => _prsabi; set { if (_prsabi != value) { _prsabi = value; NotifyPropertyChanged();} } }
	private int? _prscab;
	public int? prscab { get => _prscab; set { if (_prscab != value) { _prscab = value; NotifyPropertyChanged();} } }
	private string? _prscc;
	public string? prscc { get => _prscc; set { if (_prscc != value) { _prscc = value; NotifyPropertyChanged();} } }
	private int? _prsaba;
	public int? prsaba { get => _prsaba; set { if (_prsaba != value) { _prsaba = value; NotifyPropertyChanged();} } }
	private int? _prscaa;
	public int? prscaa { get => _prscaa; set { if (_prscaa != value) { _prscaa = value; NotifyPropertyChanged();} } }
	private decimal? _prsacc;
	public decimal? prsacc { get => _prsacc; set { if (_prsacc != value) { _prsacc = value; NotifyPropertyChanged();} } }
	private decimal? _prssti;
	public decimal? prssti { get => _prssti; set { if (_prssti != value) { _prssti = value; NotifyPropertyChanged();} } }
	private string? _prscfi;
	public string? prscfi { get => _prscfi; set { if (_prscfi != value) { _prscfi = value; NotifyPropertyChanged();} } }
	private string? _prsind;
	public string? prsind { get => _prsind; set { if (_prsind != value) { _prsind = value; NotifyPropertyChanged();} } }
	private int? _prscap;
	public int? prscap { get => _prscap; set { if (_prscap != value) { _prscap = value; NotifyPropertyChanged();} } }
	private string? _prsloc;
	public string? prsloc { get => _prsloc; set { if (_prsloc != value) { _prsloc = value; NotifyPropertyChanged();} } }
	private string? _prspro;
	public string? prspro { get => _prspro; set { if (_prspro != value) { _prspro = value; NotifyPropertyChanged();} } }
	private string? _prcodiban;
	public string? prcodiban { get => _prcodiban; set { if (_prcodiban != value) { _prcodiban = value; NotifyPropertyChanged();} } }
	private string? _isocod;
	public string? isocod { get => _isocod; set { if (_isocod != value) { _isocod = value; NotifyPropertyChanged();} } }
	private string? _prsotto;
	public string? prsotto { get => _prsotto; set { if (_prsotto != value) { _prsotto = value; NotifyPropertyChanged();} } }
	private string? _prcont;
	public string? prcont { get => _prcont; set { if (_prcont != value) { _prcont = value; NotifyPropertyChanged();} } }
	private string? _prgrup;
	public string? prgrup { get => _prgrup; set { if (_prgrup != value) { _prgrup = value; NotifyPropertyChanged();} } }
	private string? _prstdicod;
	public string? prstdicod { get => _prstdicod; set { if (_prstdicod != value) { _prstdicod = value; NotifyPropertyChanged();} } }
	private string? _prsgrup;
	public string? prsgrup { get => _prsgrup; set { if (_prsgrup != value) { _prsgrup = value; NotifyPropertyChanged();} } }
	private string? _prscon;
	public string? prscon { get => _prscon; set { if (_prscon != value) { _prscon = value; NotifyPropertyChanged();} } }
	private string? _prssott;
	public string? prssott { get => _prssott; set { if (_prssott != value) { _prssott = value; NotifyPropertyChanged();} } }
}