namespace VulpesX.Models.Default;
 
public partial class MCDETTAGLIO : Base 
{
	private string _MCSOCI = null!;
	public required string MCSOCI { get => _MCSOCI; set { if (_MCSOCI != value) { _MCSOCI = value; NotifyPropertyChanged();} } }
	private int _MCANNO;
	public int MCANNO { get => _MCANNO; set { if (_MCANNO != value) { _MCANNO = value; NotifyPropertyChanged();} } }
	private int _MCNUME;
	public int MCNUME { get => _MCNUME; set { if (_MCNUME != value) { _MCNUME = value; NotifyPropertyChanged();} } }
	private int _MCPOSI;
	public int MCPOSI { get => _MCPOSI; set { if (_MCPOSI != value) { _MCPOSI = value; NotifyPropertyChanged();} } }
	private string? _M2SOCI;
	public string? M2SOCI { get => _M2SOCI; set { if (_M2SOCI != value) { _M2SOCI = value; NotifyPropertyChanged();} } }
	private int? _M2ANNO;
	public int? M2ANNO { get => _M2ANNO; set { if (_M2ANNO != value) { _M2ANNO = value; NotifyPropertyChanged();} } }
	private int? _M2REGI;
	public int? M2REGI { get => _M2REGI; set { if (_M2REGI != value) { _M2REGI = value; NotifyPropertyChanged();} } }
	private int? _M2RIGA;
	public int? M2RIGA { get => _M2RIGA; set { if (_M2RIGA != value) { _M2RIGA = value; NotifyPropertyChanged();} } }
	private DateTime? _M2DARI;
	public DateTime? M2DARI { get => _M2DARI; set { if (_M2DARI != value) { _M2DARI = value; NotifyPropertyChanged();} } }
	private string? _M2RIFE;
	public string? M2RIFE { get => _M2RIFE; set { if (_M2RIFE != value) { _M2RIFE = value; NotifyPropertyChanged();} } }
	private string? _M2DOCU;
	public string? M2DOCU { get => _M2DOCU; set { if (_M2DOCU != value) { _M2DOCU = value; NotifyPropertyChanged();} } }
	private DateTime? _M2DARE;
	public DateTime? M2DARE { get => _M2DARE; set { if (_M2DARE != value) { _M2DARE = value; NotifyPropertyChanged();} } }
	private DateTime? _M2DADO;
	public DateTime? M2DADO { get => _M2DADO; set { if (_M2DADO != value) { _M2DADO = value; NotifyPropertyChanged();} } }
	private string? _M2CAUS;
	public string? M2CAUS { get => _M2CAUS; set { if (_M2CAUS != value) { _M2CAUS = value; NotifyPropertyChanged();} } }
	private string? _M2GRUP;
	public string? M2GRUP { get => _M2GRUP; set { if (_M2GRUP != value) { _M2GRUP = value; NotifyPropertyChanged();} } }
	private string? _M2CONT;
	public string? M2CONT { get => _M2CONT; set { if (_M2CONT != value) { _M2CONT = value; NotifyPropertyChanged();} } }
	private string? _M2SSOC;
	public string? M2SSOC { get => _M2SSOC; set { if (_M2SSOC != value) { _M2SSOC = value; NotifyPropertyChanged();} } }
	private int? _M2SOTT;
	public int? M2SOTT { get => _M2SOTT; set { if (_M2SOTT != value) { _M2SOTT = value; NotifyPropertyChanged();} } }
	private decimal? _M2IMPO;
	public decimal? M2IMPO { get => _M2IMPO; set { if (_M2IMPO != value) { _M2IMPO = value; NotifyPropertyChanged();} } }
	private string? _M2DESC;
	public string? M2DESC { get => _M2DESC; set { if (_M2DESC != value) { _M2DESC = value; NotifyPropertyChanged();} } }
	private string? _M2SEGN;
	public string? M2SEGN { get => _M2SEGN; set { if (_M2SEGN != value) { _M2SEGN = value; NotifyPropertyChanged();} } }
	private int? _M2RATA;
	public int? M2RATA { get => _M2RATA; set { if (_M2RATA != value) { _M2RATA = value; NotifyPropertyChanged();} } }
	private DateTime? _M2SCAD;
	public DateTime? M2SCAD { get => _M2SCAD; set { if (_M2SCAD != value) { _M2SCAD = value; NotifyPropertyChanged();} } }
	private string? _M2PAGA;
	public string? M2PAGA { get => _M2PAGA; set { if (_M2PAGA != value) { _M2PAGA = value; NotifyPropertyChanged();} } }
	private string? _M2PARE;
	public string? M2PARE { get => _M2PARE; set { if (_M2PARE != value) { _M2PARE = value; NotifyPropertyChanged();} } }
	private string? _M2PRAT;
	public string? M2PRAT { get => _M2PRAT; set { if (_M2PRAT != value) { _M2PRAT = value; NotifyPropertyChanged();} } }
	private string? _M2DEST;
	public string? M2DEST { get => _M2DEST; set { if (_M2DEST != value) { _M2DEST = value; NotifyPropertyChanged();} } }
	private int? _M2PAXI;
	public int? M2PAXI { get => _M2PAXI; set { if (_M2PAXI != value) { _M2PAXI = value; NotifyPropertyChanged();} } }
	private DateTime? _M2INIZ;
	public DateTime? M2INIZ { get => _M2INIZ; set { if (_M2INIZ != value) { _M2INIZ = value; NotifyPropertyChanged();} } }
	private decimal? _M2CAMB;
	public decimal? M2CAMB { get => _M2CAMB; set { if (_M2CAMB != value) { _M2CAMB = value; NotifyPropertyChanged();} } }
	private decimal? _M2VALU;
	public decimal? M2VALU { get => _M2VALU; set { if (_M2VALU != value) { _M2VALU = value; NotifyPropertyChanged();} } }
	private string? _M2DIVI;
	public string? M2DIVI { get => _M2DIVI; set { if (_M2DIVI != value) { _M2DIVI = value; NotifyPropertyChanged();} } }
	private string? _M2vcod;
	public string? M2vcod { get => _M2vcod; set { if (_M2vcod != value) { _M2vcod = value; NotifyPropertyChanged();} } }
	private string? _M2FLIN;
	public string? M2FLIN { get => _M2FLIN; set { if (_M2FLIN != value) { _M2FLIN = value; NotifyPropertyChanged();} } }
	private DateTime? _M2DVAL;
	public DateTime? M2DVAL { get => _M2DVAL; set { if (_M2DVAL != value) { _M2DVAL = value; NotifyPropertyChanged();} } }
	private decimal? _M2IMEU;
	public decimal? M2IMEU { get => _M2IMEU; set { if (_M2IMEU != value) { _M2IMEU = value; NotifyPropertyChanged();} } }
	private string? _M2TIDO;
	public string? M2TIDO { get => _M2TIDO; set { if (_M2TIDO != value) { _M2TIDO = value; NotifyPropertyChanged();} } }
	private int? _M2RIOR;
	public int? M2RIOR { get => _M2RIOR; set { if (_M2RIOR != value) { _M2RIOR = value; NotifyPropertyChanged();} } }
	private string? _M2FL01;
	public string? M2FL01 { get => _M2FL01; set { if (_M2FL01 != value) { _M2FL01 = value; NotifyPropertyChanged();} } }
	private string? _M2FLCO;
	public string? M2FLCO { get => _M2FLCO; set { if (_M2FLCO != value) { _M2FLCO = value; NotifyPropertyChanged();} } }
	private string? _M2RSOC;
	public string? M2RSOC { get => _M2RSOC; set { if (_M2RSOC != value) { _M2RSOC = value; NotifyPropertyChanged();} } }
	private int? _M2RANN;
	public int? M2RANN { get => _M2RANN; set { if (_M2RANN != value) { _M2RANN = value; NotifyPropertyChanged();} } }
	private int? _M2RREG;
	public int? M2RREG { get => _M2RREG; set { if (_M2RREG != value) { _M2RREG = value; NotifyPropertyChanged();} } }
	private int? _M2RRIG;
	public int? M2RRIG { get => _M2RRIG; set { if (_M2RRIG != value) { _M2RRIG = value; NotifyPropertyChanged();} } }
	private decimal? _M2IMAB;
	public decimal? M2IMAB { get => _M2IMAB; set { if (_M2IMAB != value) { _M2IMAB = value; NotifyPropertyChanged();} } }
	private decimal? _M2EUAB;
	public decimal? M2EUAB { get => _M2EUAB; set { if (_M2EUAB != value) { _M2EUAB = value; NotifyPropertyChanged();} } }
	private string? _M2SEGA;
	public string? M2SEGA { get => _M2SEGA; set { if (_M2SEGA != value) { _M2SEGA = value; NotifyPropertyChanged();} } }
	private string? _M2TICS;
	public string? M2TICS { get => _M2TICS; set { if (_M2TICS != value) { _M2TICS = value; NotifyPropertyChanged();} } }
}