namespace VulpesX.Models.Default;
 
public partial class ACC_EINVOICE_VAT : Base 
{
	private string _fattsoc = null!;
	public required string fattsoc { get => _fattsoc; set { if (_fattsoc != value) { _fattsoc = value; NotifyPropertyChanged();} } }
	private string _fattnum = null!;
	public required string fattnum { get => _fattnum; set { if (_fattnum != value) { _fattnum = value; NotifyPropertyChanged();} } }
	private DateTime _fattdata;
	public DateTime fattdata { get => _fattdata; set { if (_fattdata != value) { _fattdata = value; NotifyPropertyChanged();} } }
	private string _fattpiva = null!;
	public required string fattpiva { get => _fattpiva; set { if (_fattpiva != value) { _fattpiva = value; NotifyPropertyChanged();} } }
	private int _fattprog;
	public int fattprog { get => _fattprog; set { if (_fattprog != value) { _fattprog = value; NotifyPropertyChanged();} } }
	private string? _Fattaliq;
	public string? Fattaliq { get => _Fattaliq; set { if (_Fattaliq != value) { _Fattaliq = value; NotifyPropertyChanged();} } }
	private decimal? _fattimpodett;
	public decimal? fattimpodett { get => _fattimpodett; set { if (_fattimpodett != value) { _fattimpodett = value; NotifyPropertyChanged();} } }
	private decimal? _fattimpostadett;
	public decimal? fattimpostadett { get => _fattimpostadett; set { if (_fattimpostadett != value) { _fattimpostadett = value; NotifyPropertyChanged();} } }
	private string? _fattesi;
	public string? fattesi { get => _fattesi; set { if (_fattesi != value) { _fattesi = value; NotifyPropertyChanged();} } }
	private string? _fattrifenorm;
	public string? fattrifenorm { get => _fattrifenorm; set { if (_fattrifenorm != value) { _fattrifenorm = value; NotifyPropertyChanged();} } }
	private string? _fattnatu;
	public string? fattnatu { get => _fattnatu; set { if (_fattnatu != value) { _fattnatu = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
	private long _id;
	public long id { get => _id; set { if (_id != value) { _id = value; NotifyPropertyChanged();} } }
	private decimal? _fattarrotondamento;
	public decimal? fattarrotondamento { get => _fattarrotondamento; set { if (_fattarrotondamento != value) { _fattarrotondamento = value; NotifyPropertyChanged();} } }
}