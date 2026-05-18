namespace VulpesX.Models.Default;
 
public partial class ACC_EINVOICE_ROWS : Base 
{
	private string _fattsoc = null!;
	public required string fattsoc { get => _fattsoc; set { if (_fattsoc != value) { _fattsoc = value; NotifyPropertyChanged();} } }
	private string _fattnum = null!;
	public required string fattnum { get => _fattnum; set { if (_fattnum != value) { _fattnum = value; NotifyPropertyChanged();} } }
	private DateTime _fattdata;
	public DateTime fattdata { get => _fattdata; set { if (_fattdata != value) { _fattdata = value; NotifyPropertyChanged();} } }
	private string _fattpiva = null!;
	public required string fattpiva { get => _fattpiva; set { if (_fattpiva != value) { _fattpiva = value; NotifyPropertyChanged();} } }
	private int _fattriga;
	public int fattriga { get => _fattriga; set { if (_fattriga != value) { _fattriga = value; NotifyPropertyChanged();} } }
	private string? _fattartdes;
	public string? fattartdes { get => _fattartdes; set { if (_fattartdes != value) { _fattartdes = value; NotifyPropertyChanged();} } }
	private decimal? _fattprz;
	public decimal? fattprz { get => _fattprz; set { if (_fattprz != value) { _fattprz = value; NotifyPropertyChanged();} } }
	private decimal? _fattqta;
	public decimal? fattqta { get => _fattqta; set { if (_fattqta != value) { _fattqta = value; NotifyPropertyChanged();} } }
	private decimal? _fatttotriga;
	public decimal? fatttotriga { get => _fatttotriga; set { if (_fatttotriga != value) { _fatttotriga = value; NotifyPropertyChanged();} } }
	private string? _fattgrup;
	public string? fattgrup { get => _fattgrup; set { if (_fattgrup != value) { _fattgrup = value; NotifyPropertyChanged();} } }
	private string? _fattcont;
	public string? fattcont { get => _fattcont; set { if (_fattcont != value) { _fattcont = value; NotifyPropertyChanged();} } }
	private string? _fattsott;
	public string? fattsott { get => _fattsott; set { if (_fattsott != value) { _fattsott = value; NotifyPropertyChanged();} } }
	private string? _fattumi;
	public string? fattumi { get => _fattumi; set { if (_fattumi != value) { _fattumi = value; NotifyPropertyChanged();} } }
	private string? _fattaliriga;
	public string? fattaliriga { get => _fattaliriga; set { if (_fattaliriga != value) { _fattaliriga = value; NotifyPropertyChanged();} } }
	private string? _fattnatu;
	public string? fattnatu { get => _fattnatu; set { if (_fattnatu != value) { _fattnatu = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
	private long _id;
	public long id { get => _id; set { if (_id != value) { _id = value; NotifyPropertyChanged();} } }
}