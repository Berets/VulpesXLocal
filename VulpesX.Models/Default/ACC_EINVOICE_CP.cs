namespace VulpesX.Models.Default;
 
public partial class ACC_EINVOICE_CP : Base 
{
	private string _fattsoc = null!;
	public required string fattsoc { get => _fattsoc; set { if (_fattsoc != value) { _fattsoc = value; NotifyPropertyChanged();} } }
	private string _fattnum = null!;
	public required string fattnum { get => _fattnum; set { if (_fattnum != value) { _fattnum = value; NotifyPropertyChanged();} } }
	private DateTime _fattdata;
	public DateTime fattdata { get => _fattdata; set { if (_fattdata != value) { _fattdata = value; NotifyPropertyChanged();} } }
	private string _fattpiva = null!;
	public required string fattpiva { get => _fattpiva; set { if (_fattpiva != value) { _fattpiva = value; NotifyPropertyChanged();} } }
	private int _progcassa;
	public int progcassa { get => _progcassa; set { if (_progcassa != value) { _progcassa = value; NotifyPropertyChanged();} } }
	private string? _tipocassa;
	public string? tipocassa { get => _tipocassa; set { if (_tipocassa != value) { _tipocassa = value; NotifyPropertyChanged();} } }
	private string? _alicassa;
	public string? alicassa { get => _alicassa; set { if (_alicassa != value) { _alicassa = value; NotifyPropertyChanged();} } }
	private decimal? _impcontricassa;
	public decimal? impcontricassa { get => _impcontricassa; set { if (_impcontricassa != value) { _impcontricassa = value; NotifyPropertyChanged();} } }
	private decimal? _impocassa;
	public decimal? impocassa { get => _impocassa; set { if (_impocassa != value) { _impocassa = value; NotifyPropertyChanged();} } }
	private string? _aliivacassa;
	public string? aliivacassa { get => _aliivacassa; set { if (_aliivacassa != value) { _aliivacassa = value; NotifyPropertyChanged();} } }
	private string? _ritecassa;
	public string? ritecassa { get => _ritecassa; set { if (_ritecassa != value) { _ritecassa = value; NotifyPropertyChanged();} } }
	private string? _natcassa;
	public string? natcassa { get => _natcassa; set { if (_natcassa != value) { _natcassa = value; NotifyPropertyChanged();} } }
	private string? _rifammicassa;
	public string? rifammicassa { get => _rifammicassa; set { if (_rifammicassa != value) { _rifammicassa = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
	private long _id;
	public long id { get => _id; set { if (_id != value) { _id = value; NotifyPropertyChanged();} } }
}