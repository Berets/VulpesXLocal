namespace VulpesX.Models.Default;
 
public partial class ACC_EINVOICE_DDT : Base 
{
	private string _fattsoc = null!;
	public required string fattsoc { get => _fattsoc; set { if (_fattsoc != value) { _fattsoc = value; NotifyPropertyChanged();} } }
	private string _fattnum = null!;
	public required string fattnum { get => _fattnum; set { if (_fattnum != value) { _fattnum = value; NotifyPropertyChanged();} } }
	private DateTime _fattdata;
	public DateTime fattdata { get => _fattdata; set { if (_fattdata != value) { _fattdata = value; NotifyPropertyChanged();} } }
	private string _fattpiva = null!;
	public required string fattpiva { get => _fattpiva; set { if (_fattpiva != value) { _fattpiva = value; NotifyPropertyChanged();} } }
	private int _ddtriga;
	public int ddtriga { get => _ddtriga; set { if (_ddtriga != value) { _ddtriga = value; NotifyPropertyChanged();} } }
	private DateTime? _ddtdata;
	public DateTime? ddtdata { get => _ddtdata; set { if (_ddtdata != value) { _ddtdata = value; NotifyPropertyChanged();} } }
	private string? _ddtnum;
	public string? ddtnum { get => _ddtnum; set { if (_ddtnum != value) { _ddtnum = value; NotifyPropertyChanged();} } }
	private int? _ddtriferiga;
	public int? ddtriferiga { get => _ddtriferiga; set { if (_ddtriferiga != value) { _ddtriferiga = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
	private long _id;
	public long id { get => _id; set { if (_id != value) { _id = value; NotifyPropertyChanged();} } }
}