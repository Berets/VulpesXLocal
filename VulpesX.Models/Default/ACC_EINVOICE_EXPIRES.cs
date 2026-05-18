namespace VulpesX.Models.Default;
 
public partial class ACC_EINVOICE_EXPIRES : Base 
{
	private string _fattsoc = null!;
	public required string fattsoc { get => _fattsoc; set { if (_fattsoc != value) { _fattsoc = value; NotifyPropertyChanged();} } }
	private string _fattnum = null!;
	public required string fattnum { get => _fattnum; set { if (_fattnum != value) { _fattnum = value; NotifyPropertyChanged();} } }
	private DateTime _fattdata;
	public DateTime fattdata { get => _fattdata; set { if (_fattdata != value) { _fattdata = value; NotifyPropertyChanged();} } }
	private string _fattpiva = null!;
	public required string fattpiva { get => _fattpiva; set { if (_fattpiva != value) { _fattpiva = value; NotifyPropertyChanged();} } }
	private DateTime _fattdatascad;
	public DateTime fattdatascad { get => _fattdatascad; set { if (_fattdatascad != value) { _fattdatascad = value; NotifyPropertyChanged();} } }
	private decimal? _fattimpscad;
	public decimal? fattimpscad { get => _fattimpscad; set { if (_fattimpscad != value) { _fattimpscad = value; NotifyPropertyChanged();} } }
	private string? _fattipopag;
	public string? fattipopag { get => _fattipopag; set { if (_fattipopag != value) { _fattipopag = value; NotifyPropertyChanged();} } }
	private string? _fattiban;
	public string? fattiban { get => _fattiban; set { if (_fattiban != value) { _fattiban = value; NotifyPropertyChanged();} } }
	private string? _fattistu;
	public string? fattistu { get => _fattistu; set { if (_fattistu != value) { _fattistu = value; NotifyPropertyChanged();} } }
	private string? _fattcab;
	public string? fattcab { get => _fattcab; set { if (_fattcab != value) { _fattcab = value; NotifyPropertyChanged();} } }
	private string? _fattabi;
	public string? fattabi { get => _fattabi; set { if (_fattabi != value) { _fattabi = value; NotifyPropertyChanged();} } }
	private string? _fattcond;
	public string? fattcond { get => _fattcond; set { if (_fattcond != value) { _fattcond = value; NotifyPropertyChanged();} } }
	private string? _FATTTIPOLPAG;
	public string? FATTTIPOLPAG { get => _FATTTIPOLPAG; set { if (_FATTTIPOLPAG != value) { _FATTTIPOLPAG = value; NotifyPropertyChanged();} } }
	private string? _FATTCODPAGV;
	public string? FATTCODPAGV { get => _FATTCODPAGV; set { if (_FATTCODPAGV != value) { _FATTCODPAGV = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
	private long _id;
	public long id { get => _id; set { if (_id != value) { _id = value; NotifyPropertyChanged();} } }
}