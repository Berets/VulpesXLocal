namespace VulpesX.Models.Default;
 
public partial class ACC_EINVOICE_SM : Base 
{
	private string _fattsoc = null!;
	public required string fattsoc { get => _fattsoc; set { if (_fattsoc != value) { _fattsoc = value; NotifyPropertyChanged();} } }
	private string _fattnum = null!;
	public required string fattnum { get => _fattnum; set { if (_fattnum != value) { _fattnum = value; NotifyPropertyChanged();} } }
	private DateTime _fattdata;
	public DateTime fattdata { get => _fattdata; set { if (_fattdata != value) { _fattdata = value; NotifyPropertyChanged();} } }
	private string _fattpiva = null!;
	public required string fattpiva { get => _fattpiva; set { if (_fattpiva != value) { _fattpiva = value; NotifyPropertyChanged();} } }
	private int _sctprog;
	public int sctprog { get => _sctprog; set { if (_sctprog != value) { _sctprog = value; NotifyPropertyChanged();} } }
	private string? _scttipo;
	public string? scttipo { get => _scttipo; set { if (_scttipo != value) { _scttipo = value; NotifyPropertyChanged();} } }
	private decimal? _sctimpo;
	public decimal? sctimpo { get => _sctimpo; set { if (_sctimpo != value) { _sctimpo = value; NotifyPropertyChanged();} } }
	private decimal? _sctperc;
	public decimal? sctperc { get => _sctperc; set { if (_sctperc != value) { _sctperc = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
	private long _id;
	public long id { get => _id; set { if (_id != value) { _id = value; NotifyPropertyChanged();} } }
}