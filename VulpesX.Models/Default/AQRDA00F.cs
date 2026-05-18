namespace VulpesX.Models.Default;
 
public partial class AQRDA00F : Base 
{
	private string _rdasoc = null!;
	public required string rdasoc { get => _rdasoc; set { if (_rdasoc != value) { _rdasoc = value; NotifyPropertyChanged();} } }
	private int _RDANNO;
	public int RDANNO { get => _RDANNO; set { if (_RDANNO != value) { _RDANNO = value; NotifyPropertyChanged();} } }
	private int _RDANOP;
	public int RDANOP { get => _RDANOP; set { if (_RDANOP != value) { _RDANOP = value; NotifyPropertyChanged();} } }
	private int _RDANUP;
	public int RDANUP { get => _RDANUP; set { if (_RDANUP != value) { _RDANUP = value; NotifyPropertyChanged();} } }
	private DateTime? _RDADOP;
	public DateTime? RDADOP { get => _RDADOP; set { if (_RDADOP != value) { _RDADOP = value; NotifyPropertyChanged();} } }
	private int? _RDANOD;
	public int? RDANOD { get => _RDANOD; set { if (_RDANOD != value) { _RDANOD = value; NotifyPropertyChanged();} } }
	private DateTime? _RDADOD;
	public DateTime? RDADOD { get => _RDADOD; set { if (_RDADOD != value) { _RDADOD = value; NotifyPropertyChanged();} } }
	private string? _RDALIQ;
	public string? RDALIQ { get => _RDALIQ; set { if (_RDALIQ != value) { _RDALIQ = value; NotifyPropertyChanged();} } }
	private string? _RDASSO;
	public string? RDASSO { get => _RDASSO; set { if (_RDASSO != value) { _RDASSO = value; NotifyPropertyChanged();} } }
	private decimal? _RDASC1;
	public decimal? RDASC1 { get => _RDASC1; set { if (_RDASC1 != value) { _RDASC1 = value; NotifyPropertyChanged();} } }
	private decimal? _RDASC2;
	public decimal? RDASC2 { get => _RDASC2; set { if (_RDASC2 != value) { _RDASC2 = value; NotifyPropertyChanged();} } }
	private decimal? _RDAMAG;
	public decimal? RDAMAG { get => _RDAMAG; set { if (_RDAMAG != value) { _RDAMAG = value; NotifyPropertyChanged();} } }
	private decimal? _RDASC3;
	public decimal? RDASC3 { get => _RDASC3; set { if (_RDASC3 != value) { _RDASC3 = value; NotifyPropertyChanged();} } }
	private string? _RDATS1;
	public string? RDATS1 { get => _RDATS1; set { if (_RDATS1 != value) { _RDATS1 = value; NotifyPropertyChanged();} } }
	private string? _RDATS2;
	public string? RDATS2 { get => _RDATS2; set { if (_RDATS2 != value) { _RDATS2 = value; NotifyPropertyChanged();} } }
	private string? _RDATPM;
	public string? RDATPM { get => _RDATPM; set { if (_RDATPM != value) { _RDATPM = value; NotifyPropertyChanged();} } }
	private string? _RDATS3;
	public string? RDATS3 { get => _RDATS3; set { if (_RDATS3 != value) { _RDATS3 = value; NotifyPropertyChanged();} } }
	private DateTime? _RDADAS;
	public DateTime? RDADAS { get => _RDADAS; set { if (_RDADAS != value) { _RDADAS = value; NotifyPropertyChanged();} } }
	private string? _RDACCR;
	public string? RDACCR { get => _RDACCR; set { if (_RDACCR != value) { _RDACCR = value; NotifyPropertyChanged();} } }
	private string? _RDATIR;
	public string? RDATIR { get => _RDATIR; set { if (_RDATIR != value) { _RDATIR = value; NotifyPropertyChanged();} } }
	private string? _RDANRI;
	public string? RDANRI { get => _RDANRI; set { if (_RDANRI != value) { _RDANRI = value; NotifyPropertyChanged();} } }
	private DateTime? _RDADCR;
	public DateTime? RDADCR { get => _RDADCR; set { if (_RDADCR != value) { _RDADCR = value; NotifyPropertyChanged();} } }
	private string? _RDANO1;
	public string? RDANO1 { get => _RDANO1; set { if (_RDANO1 != value) { _RDANO1 = value; NotifyPropertyChanged();} } }
	private string? _RDACCG;
	public string? RDACCG { get => _RDACCG; set { if (_RDACCG != value) { _RDACCG = value; NotifyPropertyChanged();} } }
	private string? _RDANOG;
	public string? RDANOG { get => _RDANOG; set { if (_RDANOG != value) { _RDANOG = value; NotifyPropertyChanged();} } }
	private DateTime _RDADAA;
	public DateTime RDADAA { get => _RDADAA; set { if (_RDADAA != value) { _RDADAA = value; NotifyPropertyChanged();} } }
	private string? _RDACDC;
	public string? RDACDC { get => _RDACDC; set { if (_RDACDC != value) { _RDACDC = value; NotifyPropertyChanged();} } }
	private string? _RDANUC;
	public string? RDANUC { get => _RDANUC; set { if (_RDANUC != value) { _RDANUC = value; NotifyPropertyChanged();} } }
	private string? _RDATIC;
	public string? RDATIC { get => _RDATIC; set { if (_RDATIC != value) { _RDATIC = value; NotifyPropertyChanged();} } }
	private string? _RDAREC;
	public string? RDAREC { get => _RDAREC; set { if (_RDAREC != value) { _RDAREC = value; NotifyPropertyChanged();} } }
	private string? _RDAZON;
	public string? RDAZON { get => _RDAZON; set { if (_RDAZON != value) { _RDAZON = value; NotifyPropertyChanged();} } }
	private string? _RDACOR;
	public string? RDACOR { get => _RDACOR; set { if (_RDACOR != value) { _RDACOR = value; NotifyPropertyChanged();} } }
	private string? _RDATAR;
	public string? RDATAR { get => _RDATAR; set { if (_RDATAR != value) { _RDATAR = value; NotifyPropertyChanged();} } }
	private string? _RDACAR;
	public string? RDACAR { get => _RDACAR; set { if (_RDACAR != value) { _RDACAR = value; NotifyPropertyChanged();} } }
	private string? _RDANAR;
	public string? RDANAR { get => _RDANAR; set { if (_RDANAR != value) { _RDANAR = value; NotifyPropertyChanged();} } }
	private string? _RDAUNM;
	public string? RDAUNM { get => _RDAUNM; set { if (_RDAUNM != value) { _RDAUNM = value; NotifyPropertyChanged();} } }
	private string? _RDAGRC;
	public string? RDAGRC { get => _RDAGRC; set { if (_RDAGRC != value) { _RDAGRC = value; NotifyPropertyChanged();} } }
	private string? _RDACOC;
	public string? RDACOC { get => _RDACOC; set { if (_RDACOC != value) { _RDACOC = value; NotifyPropertyChanged();} } }
	private string? _RDASOT;
	public string? RDASOT { get => _RDASOT; set { if (_RDASOT != value) { _RDASOT = value; NotifyPropertyChanged();} } }
	private string? _RDAGRM;
	public string? RDAGRM { get => _RDAGRM; set { if (_RDAGRM != value) { _RDAGRM = value; NotifyPropertyChanged();} } }
	private string? _RDASOM;
	public string? RDASOM { get => _RDASOM; set { if (_RDASOM != value) { _RDASOM = value; NotifyPropertyChanged();} } }
	private string? _RDACAI;
	public string? RDACAI { get => _RDACAI; set { if (_RDACAI != value) { _RDACAI = value; NotifyPropertyChanged();} } }
	private string? _RDARMS;
	public string? RDARMS { get => _RDARMS; set { if (_RDARMS != value) { _RDARMS = value; NotifyPropertyChanged();} } }
	private DateTime? _RDATEC;
	public DateTime? RDATEC { get => _RDATEC; set { if (_RDATEC != value) { _RDATEC = value; NotifyPropertyChanged();} } }
	private string? _RDACOM;
	public string? RDACOM { get => _RDACOM; set { if (_RDACOM != value) { _RDACOM = value; NotifyPropertyChanged();} } }
	private string? _RDALTC;
	public string? RDALTC { get => _RDALTC; set { if (_RDALTC != value) { _RDALTC = value; NotifyPropertyChanged();} } }
	private decimal? _RDATOQ;
	public decimal? RDATOQ { get => _RDATOQ; set { if (_RDATOQ != value) { _RDATOQ = value; NotifyPropertyChanged();} } }
	private decimal? _RDATOP;
	public decimal? RDATOP { get => _RDATOP; set { if (_RDATOP != value) { _RDATOP = value; NotifyPropertyChanged();} } }
	private decimal? _RDATOD;
	public decimal? RDATOD { get => _RDATOD; set { if (_RDATOD != value) { _RDATOD = value; NotifyPropertyChanged();} } }
	private string? _RDADIC;
	public string? RDADIC { get => _RDADIC; set { if (_RDADIC != value) { _RDADIC = value; NotifyPropertyChanged();} } }
	private int? _RDACOL;
	public int? RDACOL { get => _RDACOL; set { if (_RDACOL != value) { _RDACOL = value; NotifyPropertyChanged();} } }
	private string? _RDAMOC;
	public string? RDAMOC { get => _RDAMOC; set { if (_RDAMOC != value) { _RDAMOC = value; NotifyPropertyChanged();} } }
	private string? _RDAMOS;
	public string? RDAMOS { get => _RDAMOS; set { if (_RDAMOS != value) { _RDAMOS = value; NotifyPropertyChanged();} } }
	private string? _RDACOI;
	public string? RDACOI { get => _RDACOI; set { if (_RDACOI != value) { _RDACOI = value; NotifyPropertyChanged();} } }
	private int? _RDACCO;
	public int? RDACCO { get => _RDACCO; set { if (_RDACCO != value) { _RDACCO = value; NotifyPropertyChanged();} } }
	private string _RDAFLE = null!;
	public required string RDAFLE { get => _RDAFLE; set { if (_RDAFLE != value) { _RDAFLE = value; NotifyPropertyChanged();} } }
	private int? _RDACDF;
	public int? RDACDF { get => _RDACDF; set { if (_RDACDF != value) { _RDACDF = value; NotifyPropertyChanged();} } }
	private string? _RDANUA;
	public string? RDANUA { get => _RDANUA; set { if (_RDANUA != value) { _RDANUA = value; NotifyPropertyChanged();} } }
	private DateTime? _RDAURI;
	public DateTime? RDAURI { get => _RDAURI; set { if (_RDAURI != value) { _RDAURI = value; NotifyPropertyChanged();} } }
	private string? _RDAFLB;
	public string? RDAFLB { get => _RDAFLB; set { if (_RDAFLB != value) { _RDAFLB = value; NotifyPropertyChanged();} } }
	private string? _RDATIF;
	public string? RDATIF { get => _RDATIF; set { if (_RDATIF != value) { _RDATIF = value; NotifyPropertyChanged();} } }
	private decimal? _RDAQTO;
	public decimal? RDAQTO { get => _RDAQTO; set { if (_RDAQTO != value) { _RDAQTO = value; NotifyPropertyChanged();} } }
	private string? _RDANOA;
	public string? RDANOA { get => _RDANOA; set { if (_RDANOA != value) { _RDANOA = value; NotifyPropertyChanged();} } }
	private string? _RDANAC;
	public string? RDANAC { get => _RDANAC; set { if (_RDANAC != value) { _RDANAC = value; NotifyPropertyChanged();} } }
	private string? _RDANOB;
	public string? RDANOB { get => _RDANOB; set { if (_RDANOB != value) { _RDANOB = value; NotifyPropertyChanged();} } }
	private string? _RDANON;
	public string? RDANON { get => _RDANON; set { if (_RDANON != value) { _RDANON = value; NotifyPropertyChanged();} } }
	private int? _RDANUV;
	public int? RDANUV { get => _RDANUV; set { if (_RDANUV != value) { _RDANUV = value; NotifyPropertyChanged();} } }
	private DateTime? _RDADAR;
	public DateTime? RDADAR { get => _RDADAR; set { if (_RDADAR != value) { _RDADAR = value; NotifyPropertyChanged();} } }
	private string? _RDAMOR;
	public string? RDAMOR { get => _RDAMOR; set { if (_RDAMOR != value) { _RDAMOR = value; NotifyPropertyChanged();} } }
	private DateTime? _RDADRG;
	public DateTime? RDADRG { get => _RDADRG; set { if (_RDADRG != value) { _RDADRG = value; NotifyPropertyChanged();} } }
	private string? _RDAMRG;
	public string? RDAMRG { get => _RDAMRG; set { if (_RDAMRG != value) { _RDAMRG = value; NotifyPropertyChanged();} } }
	private string _RDAFLD = null!;
	public required string RDAFLD { get => _RDAFLD; set { if (_RDAFLD != value) { _RDAFLD = value; NotifyPropertyChanged();} } }
	private string _RDAFEV = null!;
	public required string RDAFEV { get => _RDAFEV; set { if (_RDAFEV != value) { _RDAFEV = value; NotifyPropertyChanged();} } }
	private DateTime? _RDATCC;
	public DateTime? RDATCC { get => _RDATCC; set { if (_RDATCC != value) { _RDATCC = value; NotifyPropertyChanged();} } }
	private decimal? _RDAIMP;
	public decimal? RDAIMP { get => _RDAIMP; set { if (_RDAIMP != value) { _RDAIMP = value; NotifyPropertyChanged();} } }
	private string? _RDAACQ;
	public string? RDAACQ { get => _RDAACQ; set { if (_RDAACQ != value) { _RDAACQ = value; NotifyPropertyChanged();} } }
	private string? _RDASTA;
	public string? RDASTA { get => _RDASTA; set { if (_RDASTA != value) { _RDASTA = value; NotifyPropertyChanged();} } }
	private string? _RDASAL;
	public string? RDASAL { get => _RDASAL; set { if (_RDASAL != value) { _RDASAL = value; NotifyPropertyChanged();} } }
	private string? _RDAFAT;
	public string? RDAFAT { get => _RDAFAT; set { if (_RDAFAT != value) { _RDAFAT = value; NotifyPropertyChanged();} } }
	private string? _RDAFG1;
	public string? RDAFG1 { get => _RDAFG1; set { if (_RDAFG1 != value) { _RDAFG1 = value; NotifyPropertyChanged();} } }
	private string? _RDAFG2;
	public string? RDAFG2 { get => _RDAFG2; set { if (_RDAFG2 != value) { _RDAFG2 = value; NotifyPropertyChanged();} } }
	private string? _RDAFG3;
	public string? RDAFG3 { get => _RDAFG3; set { if (_RDAFG3 != value) { _RDAFG3 = value; NotifyPropertyChanged();} } }
	private int? _RDAFOR;
	public int? RDAFOR { get => _RDAFOR; set { if (_RDAFOR != value) { _RDAFOR = value; NotifyPropertyChanged();} } }
	private string? _RDAFLA;
	public string? RDAFLA { get => _RDAFLA; set { if (_RDAFLA != value) { _RDAFLA = value; NotifyPropertyChanged();} } }
	private string? _rdatip;
	public string? rdatip { get => _rdatip; set { if (_rdatip != value) { _rdatip = value; NotifyPropertyChanged();} } }
	private string? _RDANO2;
	public string? RDANO2 { get => _RDANO2; set { if (_RDANO2 != value) { _RDANO2 = value; NotifyPropertyChanged();} } }
	private string? _RDANO3;
	public string? RDANO3 { get => _RDANO3; set { if (_RDANO3 != value) { _RDANO3 = value; NotifyPropertyChanged();} } }
	private string? _RDANOT;
	public string? RDANOT { get => _RDANOT; set { if (_RDANOT != value) { _RDANOT = value; NotifyPropertyChanged();} } }
	private string? _RDASFA;
	public string? RDASFA { get => _RDASFA; set { if (_RDASFA != value) { _RDASFA = value; NotifyPropertyChanged();} } }
	private string? _RDATifo;
	public string? RDATifo { get => _RDATifo; set { if (_RDATifo != value) { _RDATifo = value; NotifyPropertyChanged();} } }
	private string? _RDARA1;
	public string? RDARA1 { get => _RDARA1; set { if (_RDARA1 != value) { _RDARA1 = value; NotifyPropertyChanged();} } }
	private string? _RDARA2;
	public string? RDARA2 { get => _RDARA2; set { if (_RDARA2 != value) { _RDARA2 = value; NotifyPropertyChanged();} } }
	private string? _RDAIND;
	public string? RDAIND { get => _RDAIND; set { if (_RDAIND != value) { _RDAIND = value; NotifyPropertyChanged();} } }
	private int? _RDACAP;
	public int? RDACAP { get => _RDACAP; set { if (_RDACAP != value) { _RDACAP = value; NotifyPropertyChanged();} } }
	private string? _RDALOC;
	public string? RDALOC { get => _RDALOC; set { if (_RDALOC != value) { _RDALOC = value; NotifyPropertyChanged();} } }
	private string? _RDAPRO;
	public string? RDAPRO { get => _RDAPRO; set { if (_RDAPRO != value) { _RDAPRO = value; NotifyPropertyChanged();} } }
	private string? _RDACES;
	public string? RDACES { get => _RDACES; set { if (_RDACES != value) { _RDACES = value; NotifyPropertyChanged();} } }
	private DateTime? _RDADAO;
	public DateTime? RDADAO { get => _RDADAO; set { if (_RDADAO != value) { _RDADAO = value; NotifyPropertyChanged();} } }
	private int? _RDANUO;
	public int? RDANUO { get => _RDANUO; set { if (_RDANUO != value) { _RDANUO = value; NotifyPropertyChanged();} } }
	private decimal? _RDATOT;
	public decimal? RDATOT { get => _RDATOT; set { if (_RDATOT != value) { _RDATOT = value; NotifyPropertyChanged();} } }
	private string? _RDASON;
	public string? RDASON { get => _RDASON; set { if (_RDASON != value) { _RDASON = value; NotifyPropertyChanged();} } }
}