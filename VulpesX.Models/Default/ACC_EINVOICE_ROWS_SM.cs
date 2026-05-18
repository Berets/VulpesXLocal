namespace VulpesX.Models.Default;
 
public partial class ACC_EINVOICE_ROWS_SM : Base 
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
	private int _Progsc;
	public int Progsc { get => _Progsc; set { if (_Progsc != value) { _Progsc = value; NotifyPropertyChanged();} } }
	private string? _sctipo;
	public string? sctipo { get => _sctipo; set { if (_sctipo != value) { _sctipo = value; NotifyPropertyChanged();} } }
	private decimal? _scimpo;
	public decimal? scimpo { get => _scimpo; set { if (_scimpo != value) { _scimpo = value; NotifyPropertyChanged();} } }
	private decimal? _scperc;
	public decimal? scperc { get => _scperc; set { if (_scperc != value) { _scperc = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
	private long _id;
	public long id { get => _id; set { if (_id != value) { _id = value; NotifyPropertyChanged();} } }
}