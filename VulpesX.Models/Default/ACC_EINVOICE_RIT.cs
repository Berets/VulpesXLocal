namespace VulpesX.Models.Default;
 
public partial class ACC_EINVOICE_RIT : Base 
{
	private string _fattsoc = null!;
	public required string fattsoc { get => _fattsoc; set { if (_fattsoc != value) { _fattsoc = value; NotifyPropertyChanged();} } }
	private string _fattnum = null!;
	public required string fattnum { get => _fattnum; set { if (_fattnum != value) { _fattnum = value; NotifyPropertyChanged();} } }
	private DateTime _fattdata;
	public DateTime fattdata { get => _fattdata; set { if (_fattdata != value) { _fattdata = value; NotifyPropertyChanged();} } }
	private string _fattpiva = null!;
	public required string fattpiva { get => _fattpiva; set { if (_fattpiva != value) { _fattpiva = value; NotifyPropertyChanged();} } }
	private int _progrit;
	public int progrit { get => _progrit; set { if (_progrit != value) { _progrit = value; NotifyPropertyChanged();} } }
	private string? _tiporit;
	public string? tiporit { get => _tiporit; set { if (_tiporit != value) { _tiporit = value; NotifyPropertyChanged();} } }
	private decimal? _importo;
	public decimal? importo { get => _importo; set { if (_importo != value) { _importo = value; NotifyPropertyChanged();} } }
	private decimal? _aliquota;
	public decimal? aliquota { get => _aliquota; set { if (_aliquota != value) { _aliquota = value; NotifyPropertyChanged();} } }
	private string? _causalepagamento;
	public string? causalepagamento { get => _causalepagamento; set { if (_causalepagamento != value) { _causalepagamento = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
	private long _id;
	public long id { get => _id; set { if (_id != value) { _id = value; NotifyPropertyChanged();} } }
}