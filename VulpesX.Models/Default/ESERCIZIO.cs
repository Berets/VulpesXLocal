namespace VulpesX.Models.Default;
 
public partial class ESERCIZIO : Base 
{
	private string _esesoc = null!;
	public required string esesoc { get => _esesoc; set { if (_esesoc != value) { _esesoc = value; NotifyPropertyChanged();} } }
	private int _eseann;
	public int eseann { get => _eseann; set { if (_eseann != value) { _eseann = value; NotifyPropertyChanged();} } }
	private int? _eseulr;
	public int? eseulr { get => _eseulr; set { if (_eseulr != value) { _eseulr = value; NotifyPropertyChanged();} } }
	private decimal? _esedar;
	public decimal? esedar { get => _esedar; set { if (_esedar != value) { _esedar = value; NotifyPropertyChanged();} } }
	private decimal? _eseave;
	public decimal? eseave { get => _eseave; set { if (_eseave != value) { _eseave = value; NotifyPropertyChanged();} } }
	private string? _eseest;
	public string? eseest { get => _eseest; set { if (_eseest != value) { _eseest = value; NotifyPropertyChanged();} } }
	private string? _eseist;
	public string? eseist { get => _eseist; set { if (_eseist != value) { _eseist = value; NotifyPropertyChanged();} } }
	private int? _esereg;
	public int? esereg { get => _esereg; set { if (_esereg != value) { _esereg = value; NotifyPropertyChanged();} } }
	private int? _eserpr;
	public int? eserpr { get => _eserpr; set { if (_eserpr != value) { _eserpr = value; NotifyPropertyChanged();} } }
	private DateTime? _eseusg;
	public DateTime? eseusg { get => _eseusg; set { if (_eseusg != value) { _eseusg = value; NotifyPropertyChanged();} } }
	private DateTime? _eseusm;
	public DateTime? eseusm { get => _eseusm; set { if (_eseusm != value) { _eseusm = value; NotifyPropertyChanged();} } }
	private DateTime? _eselvi;
	public DateTime? eselvi { get => _eselvi; set { if (_eselvi != value) { _eselvi = value; NotifyPropertyChanged();} } }
	private DateTime? _eselai;
	public DateTime? eselai { get => _eselai; set { if (_eselai != value) { _eselai = value; NotifyPropertyChanged();} } }
	private DateTime? _eselso;
	public DateTime? eselso { get => _eselso; set { if (_eselso != value) { _eselso = value; NotifyPropertyChanged();} } }
	private DateTime? _eselve;
	public DateTime? eselve { get => _eselve; set { if (_eselve != value) { _eselve = value; NotifyPropertyChanged();} } }
	private DateTime? _eselae;
	public DateTime? eselae { get => _eselae; set { if (_eselae != value) { _eselae = value; NotifyPropertyChanged();} } }
	private DateTime? _eseuch;
	public DateTime? eseuch { get => _eseuch; set { if (_eseuch != value) { _eseuch = value; NotifyPropertyChanged();} } }
	private string? _eseflg;
	public string? eseflg { get => _eseflg; set { if (_eseflg != value) { _eseflg = value; NotifyPropertyChanged();} } }
	private int? _eseini;
	public int? eseini { get => _eseini; set { if (_eseini != value) { _eseini = value; NotifyPropertyChanged();} } }
	private int? _esepag;
	public int? esepag { get => _esepag; set { if (_esepag != value) { _esepag = value; NotifyPropertyChanged();} } }
	private string? _eseagg;
	public string? eseagg { get => _eseagg; set { if (_eseagg != value) { _eseagg = value; NotifyPropertyChanged();} } }
	private string? _esecom;
	public string? esecom { get => _esecom; set { if (_esecom != value) { _esecom = value; NotifyPropertyChanged();} } }
	private string? _esesco;
	public string? esesco { get => _esesco; set { if (_esesco != value) { _esesco = value; NotifyPropertyChanged();} } }
	private decimal? _esepli;
	public decimal? esepli { get => _esepli; set { if (_esepli != value) { _esepli = value; NotifyPropertyChanged();} } }
	private decimal? _esepre;
	public decimal? esepre { get => _esepre; set { if (_esepre != value) { _esepre = value; NotifyPropertyChanged();} } }
	private DateTime? _esedap;
	public DateTime? esedap { get => _esedap; set { if (_esedap != value) { _esedap = value; NotifyPropertyChanged();} } }
	private string? _eseliq;
	public string? eseliq { get => _eseliq; set { if (_eseliq != value) { _eseliq = value; NotifyPropertyChanged();} } }
	private string? _eseivaven;
	public string? eseivaven { get => _eseivaven; set { if (_eseivaven != value) { _eseivaven = value; NotifyPropertyChanged();} } }
	private DateTime? _esedtfniva;
	public DateTime? esedtfniva { get => _esedtfniva; set { if (_esedtfniva != value) { _esedtfniva = value; NotifyPropertyChanged();} } }
	private DateTime? _esedtiniva;
	public DateTime? esedtiniva { get => _esedtiniva; set { if (_esedtiniva != value) { _esedtiniva = value; NotifyPropertyChanged();} } }
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
}