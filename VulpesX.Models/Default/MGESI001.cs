namespace VulpesX.Models.Default;
 
public partial class MGESI001 : Base 
{
	private string _escsoc = null!;
	public required string escsoc { get => _escsoc; set { if (_escsoc != value) { _escsoc = value; NotifyPropertyChanged();} } }
	private string _CODMAE = null!;
	public required string CODMAE { get => _CODMAE; set { if (_CODMAE != value) { _CODMAE = value; NotifyPropertyChanged();} } }
	private string _CODARE = null!;
	public required string CODARE { get => _CODARE; set { if (_CODARE != value) { _CODARE = value; NotifyPropertyChanged();} } }
	private string _ESECOL = null!;
	public required string ESECOL { get => _ESECOL; set { if (_ESECOL != value) { _ESECOL = value; NotifyPropertyChanged();} } }
	private decimal? _ESEQTA;
	public decimal? ESEQTA { get => _ESEQTA; set { if (_ESEQTA != value) { _ESEQTA = value; NotifyPropertyChanged();} } }
	private DateTime? _ESEDAI;
	public DateTime? ESEDAI { get => _ESEDAI; set { if (_ESEDAI != value) { _ESEDAI = value; NotifyPropertyChanged();} } }
	private DateTime? _ESEDAU;
	public DateTime? ESEDAU { get => _ESEDAU; set { if (_ESEDAU != value) { _ESEDAU = value; NotifyPropertyChanged();} } }
	private decimal? _eseqts;
	public decimal? eseqts { get => _eseqts; set { if (_eseqts != value) { _eseqts = value; NotifyPropertyChanged();} } }
	private DateTime? _esedsc;
	public DateTime? esedsc { get => _esedsc; set { if (_esedsc != value) { _esedsc = value; NotifyPropertyChanged();} } }
}