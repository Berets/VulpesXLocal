namespace VulpesX.Models.Default;
 
public partial class MPS00F : Base 
{
	private string _PSSOCI = null!;
	public required string PSSOCI { get => _PSSOCI; set { if (_PSSOCI != value) { _PSSOCI = value; NotifyPropertyChanged();} } }
	private string _PSCOAR = null!;
	public required string PSCOAR { get => _PSCOAR; set { if (_PSCOAR != value) { _PSCOAR = value; NotifyPropertyChanged();} } }
	private DateTime _PSDACO;
	public DateTime PSDACO { get => _PSDACO; set { if (_PSDACO != value) { _PSDACO = value; NotifyPropertyChanged();} } }
	private string? _PSTIPP;
	public string? PSTIPP { get => _PSTIPP; set { if (_PSTIPP != value) { _PSTIPP = value; NotifyPropertyChanged();} } }
	private decimal? _PSQLOR;
	public decimal? PSQLOR { get => _PSQLOR; set { if (_PSQLOR != value) { _PSQLOR = value; NotifyPropertyChanged();} } }
	private decimal? _PSQPUR;
	public decimal? PSQPUR { get => _PSQPUR; set { if (_PSQPUR != value) { _PSQPUR = value; NotifyPropertyChanged();} } }
	private decimal? _PSQSCO;
	public decimal? PSQSCO { get => _PSQSCO; set { if (_PSQSCO != value) { _PSQSCO = value; NotifyPropertyChanged();} } }
	private int? _PSTEAP;
	public int? PSTEAP { get => _PSTEAP; set { if (_PSTEAP != value) { _PSTEAP = value; NotifyPropertyChanged();} } }
	private decimal? _PSCAMA;
	public decimal? PSCAMA { get => _PSCAMA; set { if (_PSCAMA != value) { _PSCAMA = value; NotifyPropertyChanged();} } }
	private decimal? _PSCAMB;
	public decimal? PSCAMB { get => _PSCAMB; set { if (_PSCAMB != value) { _PSCAMB = value; NotifyPropertyChanged();} } }
	private decimal? _PSCAMC;
	public decimal? PSCAMC { get => _PSCAMC; set { if (_PSCAMC != value) { _PSCAMC = value; NotifyPropertyChanged();} } }
	private decimal? _PSCAMD;
	public decimal? PSCAMD { get => _PSCAMD; set { if (_PSCAMD != value) { _PSCAMD = value; NotifyPropertyChanged();} } }
	private decimal? _PSCAME;
	public decimal? PSCAME { get => _PSCAME; set { if (_PSCAME != value) { _PSCAME = value; NotifyPropertyChanged();} } }
	private decimal? _PSCAMF;
	public decimal? PSCAMF { get => _PSCAMF; set { if (_PSCAMF != value) { _PSCAMF = value; NotifyPropertyChanged();} } }
	private decimal? _PSCAMG;
	public decimal? PSCAMG { get => _PSCAMG; set { if (_PSCAMG != value) { _PSCAMG = value; NotifyPropertyChanged();} } }
	private decimal? _PSCAMH;
	public decimal? PSCAMH { get => _PSCAMH; set { if (_PSCAMH != value) { _PSCAMH = value; NotifyPropertyChanged();} } }
	private string? _PSMABA;
	public string? PSMABA { get => _PSMABA; set { if (_PSMABA != value) { _PSMABA = value; NotifyPropertyChanged();} } }
	private string? _PSPOLI;
	public string? PSPOLI { get => _PSPOLI; set { if (_PSPOLI != value) { _PSPOLI = value; NotifyPropertyChanged();} } }
	private int? _PSNUSE;
	public int? PSNUSE { get => _PSNUSE; set { if (_PSNUSE != value) { _PSNUSE = value; NotifyPropertyChanged();} } }
	private decimal? _PSQEDI;
	public decimal? PSQEDI { get => _PSQEDI; set { if (_PSQEDI != value) { _PSQEDI = value; NotifyPropertyChanged();} } }
	private DateTime? _PSDAOG;
	public DateTime? PSDAOG { get => _PSDAOG; set { if (_PSDAOG != value) { _PSDAOG = value; NotifyPropertyChanged();} } }
	private DateTime? _PSDAIM;
	public DateTime? PSDAIM { get => _PSDAIM; set { if (_PSDAIM != value) { _PSDAIM = value; NotifyPropertyChanged();} } }
	private int? _PSTMPS;
	public int? PSTMPS { get => _PSTMPS; set { if (_PSTMPS != value) { _PSTMPS = value; NotifyPropertyChanged();} } }
	private int? _PSGGOR;
	public int? PSGGOR { get => _PSGGOR; set { if (_PSGGOR != value) { _PSGGOR = value; NotifyPropertyChanged();} } }
	private int? _PSGORC;
	public int? PSGORC { get => _PSGORC; set { if (_PSGORC != value) { _PSGORC = value; NotifyPropertyChanged();} } }
	private DateTime? _PSDALI;
	public DateTime? PSDALI { get => _PSDALI; set { if (_PSDALI != value) { _PSDALI = value; NotifyPropertyChanged();} } }
	private int? _PSULSE;
	public int? PSULSE { get => _PSULSE; set { if (_PSULSE != value) { _PSULSE = value; NotifyPropertyChanged();} } }
	private int? _PSNERR;
	public int? PSNERR { get => _PSNERR; set { if (_PSNERR != value) { _PSNERR = value; NotifyPropertyChanged();} } }
	private decimal? _PSTEAV;
	public decimal? PSTEAV { get => _PSTEAV; set { if (_PSTEAV != value) { _PSTEAV = value; NotifyPropertyChanged();} } }
	private decimal? _PSOTGP;
	public decimal? PSOTGP { get => _PSOTGP; set { if (_PSOTGP != value) { _PSOTGP = value; NotifyPropertyChanged();} } }
	private int? _PSTECO;
	public int? PSTECO { get => _PSTECO; set { if (_PSTECO != value) { _PSTECO = value; NotifyPropertyChanged();} } }
	private string? _PSFILL;
	public string? PSFILL { get => _PSFILL; set { if (_PSFILL != value) { _PSFILL = value; NotifyPropertyChanged();} } }
	private string? _PSFLAG;
	public string? PSFLAG { get => _PSFLAG; set { if (_PSFLAG != value) { _PSFLAG = value; NotifyPropertyChanged();} } }
}