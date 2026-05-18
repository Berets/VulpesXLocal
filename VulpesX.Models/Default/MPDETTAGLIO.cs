namespace VulpesX.Models.Default;
 
public partial class MPDETTAGLIO : Base 
{
	private string _MPSOCI = null!;
	public required string MPSOCI { get => _MPSOCI; set { if (_MPSOCI != value) { _MPSOCI = value; NotifyPropertyChanged();} } }
	private int _MPANNO;
	public int MPANNO { get => _MPANNO; set { if (_MPANNO != value) { _MPANNO = value; NotifyPropertyChanged();} } }
	private int _MPNUME;
	public int MPNUME { get => _MPNUME; set { if (_MPNUME != value) { _MPNUME = value; NotifyPropertyChanged();} } }
	private int _MPPOSI;
	public int MPPOSI { get => _MPPOSI; set { if (_MPPOSI != value) { _MPPOSI = value; NotifyPropertyChanged();} } }
	private string? _M3SOCI;
	public string? M3SOCI { get => _M3SOCI; set { if (_M3SOCI != value) { _M3SOCI = value; NotifyPropertyChanged();} } }
	private int? _M3ANNO;
	public int? M3ANNO { get => _M3ANNO; set { if (_M3ANNO != value) { _M3ANNO = value; NotifyPropertyChanged();} } }
	private int? _M3REGI;
	public int? M3REGI { get => _M3REGI; set { if (_M3REGI != value) { _M3REGI = value; NotifyPropertyChanged();} } }
	private int? _M3RIGA;
	public int? M3RIGA { get => _M3RIGA; set { if (_M3RIGA != value) { _M3RIGA = value; NotifyPropertyChanged();} } }
	private DateTime? _M3DARI;
	public DateTime? M3DARI { get => _M3DARI; set { if (_M3DARI != value) { _M3DARI = value; NotifyPropertyChanged();} } }
	private string? _M3RIFE;
	public string? M3RIFE { get => _M3RIFE; set { if (_M3RIFE != value) { _M3RIFE = value; NotifyPropertyChanged();} } }
	private string? _M3DOCU;
	public string? M3DOCU { get => _M3DOCU; set { if (_M3DOCU != value) { _M3DOCU = value; NotifyPropertyChanged();} } }
	private DateTime? _M3DARE;
	public DateTime? M3DARE { get => _M3DARE; set { if (_M3DARE != value) { _M3DARE = value; NotifyPropertyChanged();} } }
	private DateTime? _M3DADO;
	public DateTime? M3DADO { get => _M3DADO; set { if (_M3DADO != value) { _M3DADO = value; NotifyPropertyChanged();} } }
	private string? _M3CAUS;
	public string? M3CAUS { get => _M3CAUS; set { if (_M3CAUS != value) { _M3CAUS = value; NotifyPropertyChanged();} } }
	private string? _M3GRUP;
	public string? M3GRUP { get => _M3GRUP; set { if (_M3GRUP != value) { _M3GRUP = value; NotifyPropertyChanged();} } }
	private string? _M3CONT;
	public string? M3CONT { get => _M3CONT; set { if (_M3CONT != value) { _M3CONT = value; NotifyPropertyChanged();} } }
	private string? _M3SSOC;
	public string? M3SSOC { get => _M3SSOC; set { if (_M3SSOC != value) { _M3SSOC = value; NotifyPropertyChanged();} } }
	private int? _M3SOTT;
	public int? M3SOTT { get => _M3SOTT; set { if (_M3SOTT != value) { _M3SOTT = value; NotifyPropertyChanged();} } }
	private decimal? _M3IMPO;
	public decimal? M3IMPO { get => _M3IMPO; set { if (_M3IMPO != value) { _M3IMPO = value; NotifyPropertyChanged();} } }
	private string? _M3DESC;
	public string? M3DESC { get => _M3DESC; set { if (_M3DESC != value) { _M3DESC = value; NotifyPropertyChanged();} } }
	private string? _M3SEGN;
	public string? M3SEGN { get => _M3SEGN; set { if (_M3SEGN != value) { _M3SEGN = value; NotifyPropertyChanged();} } }
	private int? _M3RATA;
	public int? M3RATA { get => _M3RATA; set { if (_M3RATA != value) { _M3RATA = value; NotifyPropertyChanged();} } }
	private DateTime? _M3SCAD;
	public DateTime? M3SCAD { get => _M3SCAD; set { if (_M3SCAD != value) { _M3SCAD = value; NotifyPropertyChanged();} } }
	private string? _M3PAGA;
	public string? M3PAGA { get => _M3PAGA; set { if (_M3PAGA != value) { _M3PAGA = value; NotifyPropertyChanged();} } }
	private string? _M3PARE;
	public string? M3PARE { get => _M3PARE; set { if (_M3PARE != value) { _M3PARE = value; NotifyPropertyChanged();} } }
	private string? _M3PRAT;
	public string? M3PRAT { get => _M3PRAT; set { if (_M3PRAT != value) { _M3PRAT = value; NotifyPropertyChanged();} } }
	private string? _M3DEST;
	public string? M3DEST { get => _M3DEST; set { if (_M3DEST != value) { _M3DEST = value; NotifyPropertyChanged();} } }
	private int? _M3PAXI;
	public int? M3PAXI { get => _M3PAXI; set { if (_M3PAXI != value) { _M3PAXI = value; NotifyPropertyChanged();} } }
	private DateTime? _M3INIZ;
	public DateTime? M3INIZ { get => _M3INIZ; set { if (_M3INIZ != value) { _M3INIZ = value; NotifyPropertyChanged();} } }
	private decimal? _M3CAMB;
	public decimal? M3CAMB { get => _M3CAMB; set { if (_M3CAMB != value) { _M3CAMB = value; NotifyPropertyChanged();} } }
	private decimal? _M3VALU;
	public decimal? M3VALU { get => _M3VALU; set { if (_M3VALU != value) { _M3VALU = value; NotifyPropertyChanged();} } }
	private string? _M3DIVI;
	public string? M3DIVI { get => _M3DIVI; set { if (_M3DIVI != value) { _M3DIVI = value; NotifyPropertyChanged();} } }
	private string? _M3vcod;
	public string? M3vcod { get => _M3vcod; set { if (_M3vcod != value) { _M3vcod = value; NotifyPropertyChanged();} } }
	private string? _M3FLPA;
	public string? M3FLPA { get => _M3FLPA; set { if (_M3FLPA != value) { _M3FLPA = value; NotifyPropertyChanged();} } }
	private DateTime? _M3DVAL;
	public DateTime? M3DVAL { get => _M3DVAL; set { if (_M3DVAL != value) { _M3DVAL = value; NotifyPropertyChanged();} } }
	private int? _M3ABIF;
	public int? M3ABIF { get => _M3ABIF; set { if (_M3ABIF != value) { _M3ABIF = value; NotifyPropertyChanged();} } }
	private int? _M3CABF;
	public int? M3CABF { get => _M3CABF; set { if (_M3CABF != value) { _M3CABF = value; NotifyPropertyChanged();} } }
	private decimal? _M3IMEU;
	public decimal? M3IMEU { get => _M3IMEU; set { if (_M3IMEU != value) { _M3IMEU = value; NotifyPropertyChanged();} } }
	private string? _M3TIDO;
	public string? M3TIDO { get => _M3TIDO; set { if (_M3TIDO != value) { _M3TIDO = value; NotifyPropertyChanged();} } }
	private int? _M3RIOR;
	public int? M3RIOR { get => _M3RIOR; set { if (_M3RIOR != value) { _M3RIOR = value; NotifyPropertyChanged();} } }
	private string? _M3FL01;
	public string? M3FL01 { get => _M3FL01; set { if (_M3FL01 != value) { _M3FL01 = value; NotifyPropertyChanged();} } }
	private string? _M3FLCO;
	public string? M3FLCO { get => _M3FLCO; set { if (_M3FLCO != value) { _M3FLCO = value; NotifyPropertyChanged();} } }
	private string? _M3FLES;
	public string? M3FLES { get => _M3FLES; set { if (_M3FLES != value) { _M3FLES = value; NotifyPropertyChanged();} } }
	private string? _M3RSOC;
	public string? M3RSOC { get => _M3RSOC; set { if (_M3RSOC != value) { _M3RSOC = value; NotifyPropertyChanged();} } }
	private int? _M3RANN;
	public int? M3RANN { get => _M3RANN; set { if (_M3RANN != value) { _M3RANN = value; NotifyPropertyChanged();} } }
	private int? _M3RREG;
	public int? M3RREG { get => _M3RREG; set { if (_M3RREG != value) { _M3RREG = value; NotifyPropertyChanged();} } }
	private int? _M3RRIG;
	public int? M3RRIG { get => _M3RRIG; set { if (_M3RRIG != value) { _M3RRIG = value; NotifyPropertyChanged();} } }
	private decimal? _M3IMAB;
	public decimal? M3IMAB { get => _M3IMAB; set { if (_M3IMAB != value) { _M3IMAB = value; NotifyPropertyChanged();} } }
	private decimal? _M3EUAB;
	public decimal? M3EUAB { get => _M3EUAB; set { if (_M3EUAB != value) { _M3EUAB = value; NotifyPropertyChanged();} } }
	private string? _M3SEGA;
	public string? M3SEGA { get => _M3SEGA; set { if (_M3SEGA != value) { _M3SEGA = value; NotifyPropertyChanged();} } }
	private decimal? _M3VAAB;
	public decimal? M3VAAB { get => _M3VAAB; set { if (_M3VAAB != value) { _M3VAAB = value; NotifyPropertyChanged();} } }
	private string? _m3numef;
	public string? m3numef { get => _m3numef; set { if (_m3numef != value) { _m3numef = value; NotifyPropertyChanged();} } }
    private byte[]? _rv;
    public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged(); } } }
}