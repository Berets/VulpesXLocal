namespace VulpesX.Models.Default;
 
public partial class PNTESTATA : Base 
{
	private string _N1SOCI = null!;
	public required string N1SOCI { get => _N1SOCI; set { if (_N1SOCI != value) { _N1SOCI = value; NotifyPropertyChanged();} } }
	private int _N1ANNO;
	public int N1ANNO { get => _N1ANNO; set { if (_N1ANNO != value) { _N1ANNO = value; NotifyPropertyChanged();} } }
	private int _N1REGI;
	public int N1REGI { get => _N1REGI; set { if (_N1REGI != value) { _N1REGI = value; NotifyPropertyChanged();} } }
	private string? _pncaus;
	public string? pncaus { get => _pncaus; set { if (_pncaus != value) { _pncaus = value; NotifyPropertyChanged();} } }
	private DateTime? _N1DARE;
	public DateTime? N1DARE { get => _N1DARE; set { if (_N1DARE != value) { _N1DARE = value; NotifyPropertyChanged();} } }
	private string? _pnvcod;
	public string? pnvcod { get => _pnvcod; set { if (_pnvcod != value) { _pnvcod = value; NotifyPropertyChanged();} } }
	private string? _pnvdiv;
	public string? pnvdiv { get => _pnvdiv; set { if (_pnvdiv != value) { _pnvdiv = value; NotifyPropertyChanged();} } }
	private string? _N1FL01;
	public string? N1FL01 { get => _N1FL01; set { if (_N1FL01 != value) { _N1FL01 = value; NotifyPropertyChanged();} } }
	private string? _N1docn;
	public string? N1docn { get => _N1docn; set { if (_N1docn != value) { _N1docn = value; NotifyPropertyChanged();} } }
	private DateTime? _N1docd;
	public DateTime? N1docd { get => _N1docd; set { if (_N1docd != value) { _N1docd = value; NotifyPropertyChanged();} } }
	private string? _N1rifn;
	public string? N1rifn { get => _N1rifn; set { if (_N1rifn != value) { _N1rifn = value; NotifyPropertyChanged();} } }
	private DateTime? _N1rifd;
	public DateTime? N1rifd { get => _N1rifd; set { if (_N1rifd != value) { _N1rifd = value; NotifyPropertyChanged();} } }
	private string? _N1FLCF;
	public string? N1FLCF { get => _N1FLCF; set { if (_N1FLCF != value) { _N1FLCF = value; NotifyPropertyChanged();} } }
	private int? _N1CLFO;
	public int? N1CLFO { get => _N1CLFO; set { if (_N1CLFO != value) { _N1CLFO = value; NotifyPropertyChanged();} } }
	private string? _N1VADO;
	public string? N1VADO { get => _N1VADO; set { if (_N1VADO != value) { _N1VADO = value; NotifyPropertyChanged();} } }
	private string? _N1DIDO;
	public string? N1DIDO { get => _N1DIDO; set { if (_N1DIDO != value) { _N1DIDO = value; NotifyPropertyChanged();} } }
	private DateTime? _N1DADI;
	public DateTime? N1DADI { get => _N1DADI; set { if (_N1DADI != value) { _N1DADI = value; NotifyPropertyChanged();} } }
	private decimal? _N1CADO;
	public decimal? N1CADO { get => _N1CADO; set { if (_N1CADO != value) { _N1CADO = value; NotifyPropertyChanged();} } }
	private decimal? _N1IMDO;
	public decimal? N1IMDO { get => _N1IMDO; set { if (_N1IMDO != value) { _N1IMDO = value; NotifyPropertyChanged();} } }
	private int? _n1mrii;
	public int? n1mrii { get => _n1mrii; set { if (_n1mrii != value) { _n1mrii = value; NotifyPropertyChanged();} } }
	private string? _N1TmpPN;
	public string? N1TmpPN { get => _N1TmpPN; set { if (_N1TmpPN != value) { _N1TmpPN = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
	private DateTime? _added;
	public DateTime? added { get => _added; set { if (_added != value) { _added = value; NotifyPropertyChanged();} } }
	private DateTime? _updated;
	public DateTime? updated { get => _updated; set { if (_updated != value) { _updated = value; NotifyPropertyChanged();} } }
	private DateTime? _canceled;
	public DateTime? canceled { get => _canceled; set { if (_canceled != value) { _canceled = value; NotifyPropertyChanged();} } }
	private string? _addedUserID;
	public string? addedUserID { get => _addedUserID; set { if (_addedUserID != value) { _addedUserID = value; NotifyPropertyChanged();} } }
	private string? _updatedUserID;
	public string? updatedUserID { get => _updatedUserID; set { if (_updatedUserID != value) { _updatedUserID = value; NotifyPropertyChanged();} } }
	private string? _canceledUserID;
	public string? canceledUserID { get => _canceledUserID; set { if (_canceledUserID != value) { _canceledUserID = value; NotifyPropertyChanged();} } }
	private string? _canceledNote;
	public string? canceledNote { get => _canceledNote; set { if (_canceledNote != value) { _canceledNote = value; NotifyPropertyChanged();} } }
	private int? _N1AUAN;
	public int? N1AUAN { get => _N1AUAN; set { if (_N1AUAN != value) { _N1AUAN = value; NotifyPropertyChanged();} } }
	private int? _N1AUNU;
	public int? N1AUNU { get => _N1AUNU; set { if (_N1AUNU != value) { _N1AUNU = value; NotifyPropertyChanged();} } }
	private DateTime? _N1AUGE;
	public DateTime? N1AUGE { get => _N1AUGE; set { if (_N1AUGE != value) { _N1AUGE = value; NotifyPropertyChanged();} } }
}