namespace VulpesX.Models.Default;
 
public partial class ANFAT00F : Base 
{
	private string _AFSOCI = null!;
	public required string AFSOCI { get => _AFSOCI; set { if (_AFSOCI != value) { _AFSOCI = value; NotifyPropertyChanged();} } }
	private int _AFANNO;
	public int AFANNO { get => _AFANNO; set { if (_AFANNO != value) { _AFANNO = value; NotifyPropertyChanged();} } }
	private int _AFNUOR;
	public int AFNUOR { get => _AFNUOR; set { if (_AFNUOR != value) { _AFNUOR = value; NotifyPropertyChanged();} } }
	private DateTime? _AFDAOR;
	public DateTime? AFDAOR { get => _AFDAOR; set { if (_AFDAOR != value) { _AFDAOR = value; NotifyPropertyChanged();} } }
	private int? _AFCOCL;
	public int? AFCOCL { get => _AFCOCL; set { if (_AFCOCL != value) { _AFCOCL = value; NotifyPropertyChanged();} } }
	private int? _AFDEST;
	public int? AFDEST { get => _AFDEST; set { if (_AFDEST != value) { _AFDEST = value; NotifyPropertyChanged();} } }
	private string? _AFRASO;
	public string? AFRASO { get => _AFRASO; set { if (_AFRASO != value) { _AFRASO = value; NotifyPropertyChanged();} } }
	private string? _AFINDI;
	public string? AFINDI { get => _AFINDI; set { if (_AFINDI != value) { _AFINDI = value; NotifyPropertyChanged();} } }
	private string? _AFPIVA;
	public string? AFPIVA { get => _AFPIVA; set { if (_AFPIVA != value) { _AFPIVA = value; NotifyPropertyChanged();} } }
	private string? _AFCOFI;
	public string? AFCOFI { get => _AFCOFI; set { if (_AFCOFI != value) { _AFCOFI = value; NotifyPropertyChanged();} } }
	private string? _AFLOCA;
	public string? AFLOCA { get => _AFLOCA; set { if (_AFLOCA != value) { _AFLOCA = value; NotifyPropertyChanged();} } }
	private string? _AFPROV;
	public string? AFPROV { get => _AFPROV; set { if (_AFPROV != value) { _AFPROV = value; NotifyPropertyChanged();} } }
	private string? _AFTELE;
	public string? AFTELE { get => _AFTELE; set { if (_AFTELE != value) { _AFTELE = value; NotifyPropertyChanged();} } }
	private string? _AFNFAX;
	public string? AFNFAX { get => _AFNFAX; set { if (_AFNFAX != value) { _AFNFAX = value; NotifyPropertyChanged();} } }
	private string? _AFMAIL;
	public string? AFMAIL { get => _AFMAIL; set { if (_AFMAIL != value) { _AFMAIL = value; NotifyPropertyChanged();} } }
	private string? _AFCELL;
	public string? AFCELL { get => _AFCELL; set { if (_AFCELL != value) { _AFCELL = value; NotifyPropertyChanged();} } }
	private int? _AFCAPP;
	public int? AFCAPP { get => _AFCAPP; set { if (_AFCAPP != value) { _AFCAPP = value; NotifyPropertyChanged();} } }
	private string? _AFCIDI;
	public string? AFCIDI { get => _AFCIDI; set { if (_AFCIDI != value) { _AFCIDI = value; NotifyPropertyChanged();} } }
	private string? _AFDIVI;
	public string? AFDIVI { get => _AFDIVI; set { if (_AFDIVI != value) { _AFDIVI = value; NotifyPropertyChanged();} } }
	private string? _AFRIFE;
	public string? AFRIFE { get => _AFRIFE; set { if (_AFRIFE != value) { _AFRIFE = value; NotifyPropertyChanged();} } }
	private string _AFOGGE = null!;
	public required string AFOGGE { get => _AFOGGE; set { if (_AFOGGE != value) { _AFOGGE = value; NotifyPropertyChanged();} } }
	private string? _AFSTAT;
	public string? AFSTAT { get => _AFSTAT; set { if (_AFSTAT != value) { _AFSTAT = value; NotifyPropertyChanged();} } }
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
	private string? _AFNOTET;
	public string? AFNOTET { get => _AFNOTET; set { if (_AFNOTET != value) { _AFNOTET = value; NotifyPropertyChanged();} } }
	private string? _AFNOTEP;
	public string? AFNOTEP { get => _AFNOTEP; set { if (_AFNOTEP != value) { _AFNOTEP = value; NotifyPropertyChanged();} } }
	private bool _AFSHOWT;
	public bool AFSHOWT { get => _AFSHOWT; set { if (_AFSHOWT != value) { _AFSHOWT = value; NotifyPropertyChanged();} } }
	private bool _AFSHOWP;
	public bool AFSHOWP { get => _AFSHOWP; set { if (_AFSHOWP != value) { _AFSHOWP = value; NotifyPropertyChanged();} } }
	private string? _AFLING;
	public string? AFLING { get => _AFLING; set { if (_AFLING != value) { _AFLING = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}