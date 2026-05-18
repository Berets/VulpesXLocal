namespace VulpesX.Models.Default;
 
public partial class ACC_EINVOICE_PO : Base 
{
	private string _fattsoc = null!;
	public required string fattsoc { get => _fattsoc; set { if (_fattsoc != value) { _fattsoc = value; NotifyPropertyChanged();} } }
	private string _fattnum = null!;
	public required string fattnum { get => _fattnum; set { if (_fattnum != value) { _fattnum = value; NotifyPropertyChanged();} } }
	private DateTime _fattdata;
	public DateTime fattdata { get => _fattdata; set { if (_fattdata != value) { _fattdata = value; NotifyPropertyChanged();} } }
	private string _fattpiva = null!;
	public required string fattpiva { get => _fattpiva; set { if (_fattpiva != value) { _fattpiva = value; NotifyPropertyChanged();} } }
	private int _riga;
	public int riga { get => _riga; set { if (_riga != value) { _riga = value; NotifyPropertyChanged();} } }
	private string? _numitem;
	public string? numitem { get => _numitem; set { if (_numitem != value) { _numitem = value; NotifyPropertyChanged();} } }
	private string? _iddocumento;
	public string? iddocumento { get => _iddocumento; set { if (_iddocumento != value) { _iddocumento = value; NotifyPropertyChanged();} } }
	private DateTime? _datadoc;
	public DateTime? datadoc { get => _datadoc; set { if (_datadoc != value) { _datadoc = value; NotifyPropertyChanged();} } }
	private int? _rigarife;
	public int? rigarife { get => _rigarife; set { if (_rigarife != value) { _rigarife = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
	private long _id;
	public long id { get => _id; set { if (_id != value) { _id = value; NotifyPropertyChanged();} } }
}