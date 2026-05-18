namespace VulpesX.Models.Default;
 
public partial class TABDIPE : Base 
{
	private string _tdicod = null!;
	public required string tdicod { get => _tdicod; set { if (_tdicod != value) { _tdicod = value; NotifyPropertyChanged();} } }
	private string? _tdinom;
	public string? tdinom { get => _tdinom; set { if (_tdinom != value) { _tdinom = value; NotifyPropertyChanged();} } }
	private string? _tdigru;
	public string? tdigru { get => _tdigru; set { if (_tdigru != value) { _tdigru = value; NotifyPropertyChanged();} } }
	private string? _tdicon;
	public string? tdicon { get => _tdicon; set { if (_tdicon != value) { _tdicon = value; NotifyPropertyChanged();} } }
	private string? _tdisot;
	public string? tdisot { get => _tdisot; set { if (_tdisot != value) { _tdisot = value; NotifyPropertyChanged();} } }
	private string? _tdmatr;
	public string? tdmatr { get => _tdmatr; set { if (_tdmatr != value) { _tdmatr = value; NotifyPropertyChanged();} } }
	private string? _tdiammcod;
	public string? tdiammcod { get => _tdiammcod; set { if (_tdiammcod != value) { _tdiammcod = value; NotifyPropertyChanged();} } }
	private string? _tdiammdes;
	public string? tdiammdes { get => _tdiammdes; set { if (_tdiammdes != value) { _tdiammdes = value; NotifyPropertyChanged();} } }
	private string? _isocod;
	public string? isocod { get => _isocod; set { if (_isocod != value) { _isocod = value; NotifyPropertyChanged();} } }
	private string? _tdicodiban;
	public string? tdicodiban { get => _tdicodiban; set { if (_tdicodiban != value) { _tdicodiban = value; NotifyPropertyChanged();} } }
	private string? _tdipro;
	public string? tdipro { get => _tdipro; set { if (_tdipro != value) { _tdipro = value; NotifyPropertyChanged();} } }
	private string? _tdiloc;
	public string? tdiloc { get => _tdiloc; set { if (_tdiloc != value) { _tdiloc = value; NotifyPropertyChanged();} } }
	private int? _tdicap;
	public int? tdicap { get => _tdicap; set { if (_tdicap != value) { _tdicap = value; NotifyPropertyChanged();} } }
	private string? _tdiind;
	public string? tdiind { get => _tdiind; set { if (_tdiind != value) { _tdiind = value; NotifyPropertyChanged();} } }
	private string? _tdicfi;
	public string? tdicfi { get => _tdicfi; set { if (_tdicfi != value) { _tdicfi = value; NotifyPropertyChanged();} } }
	private int? _tdicaa;
	public int? tdicaa { get => _tdicaa; set { if (_tdicaa != value) { _tdicaa = value; NotifyPropertyChanged();} } }
	private int? _tdiaba;
	public int? tdiaba { get => _tdiaba; set { if (_tdiaba != value) { _tdiaba = value; NotifyPropertyChanged();} } }
	private string? _tdicc;
	public string? tdicc { get => _tdicc; set { if (_tdicc != value) { _tdicc = value; NotifyPropertyChanged();} } }
	private int? _tdicab;
	public int? tdicab { get => _tdicab; set { if (_tdicab != value) { _tdicab = value; NotifyPropertyChanged();} } }
	private int? _tdiabi;
	public int? tdiabi { get => _tdiabi; set { if (_tdiabi != value) { _tdiabi = value; NotifyPropertyChanged();} } }
	private string? _tdicin;
	public string? tdicin { get => _tdicin; set { if (_tdicin != value) { _tdicin = value; NotifyPropertyChanged();} } }
	private int? _tdisit;
	public int? tdisit { get => _tdisit; set { if (_tdisit != value) { _tdisit = value; NotifyPropertyChanged();} } }
	private string? _tdisoc;
	public string? tdisoc { get => _tdisoc; set { if (_tdisoc != value) { _tdisoc = value; NotifyPropertyChanged();} } }
}