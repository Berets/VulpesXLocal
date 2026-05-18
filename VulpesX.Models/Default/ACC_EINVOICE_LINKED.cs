namespace VulpesX.Models.Default;
 
public partial class ACC_EINVOICE_LINKED : Base 
{
	private string _fattsoc = null!;
	public required string fattsoc { get => _fattsoc; set { if (_fattsoc != value) { _fattsoc = value; NotifyPropertyChanged();} } }
	private string _fattnum = null!;
	public required string fattnum { get => _fattnum; set { if (_fattnum != value) { _fattnum = value; NotifyPropertyChanged();} } }
	private DateTime _fattdata;
	public DateTime fattdata { get => _fattdata; set { if (_fattdata != value) { _fattdata = value; NotifyPropertyChanged();} } }
	private string _fattpiva = null!;
	public required string fattpiva { get => _fattpiva; set { if (_fattpiva != value) { _fattpiva = value; NotifyPropertyChanged();} } }
	private int _fattcollriga;
	public int fattcollriga { get => _fattcollriga; set { if (_fattcollriga != value) { _fattcollriga = value; NotifyPropertyChanged();} } }
	private string? _fattcollnumitem;
	public string? fattcollnumitem { get => _fattcollnumitem; set { if (_fattcollnumitem != value) { _fattcollnumitem = value; NotifyPropertyChanged();} } }
	private string? _fattcolliddocumento;
	public string? fattcolliddocumento { get => _fattcolliddocumento; set { if (_fattcolliddocumento != value) { _fattcolliddocumento = value; NotifyPropertyChanged();} } }
	private DateTime? _fattcolldatadoc;
	public DateTime? fattcolldatadoc { get => _fattcolldatadoc; set { if (_fattcolldatadoc != value) { _fattcolldatadoc = value; NotifyPropertyChanged();} } }
	private int? _fattcollriferiga;
	public int? fattcollriferiga { get => _fattcollriferiga; set { if (_fattcollriferiga != value) { _fattcollriferiga = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
	private long _id;
	public long id { get => _id; set { if (_id != value) { _id = value; NotifyPropertyChanged();} } }
}