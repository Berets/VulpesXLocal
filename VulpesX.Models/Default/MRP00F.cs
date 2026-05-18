namespace VulpesX.Models.Default;
 
public partial class MRP00F : Base 
{
	private string _RPSOCI = null!;
	public required string RPSOCI { get => _RPSOCI; set { if (_RPSOCI != value) { _RPSOCI = value; NotifyPropertyChanged();} } }
	private string _RPCOAR = null!;
	public required string RPCOAR { get => _RPCOAR; set { if (_RPCOAR != value) { _RPCOAR = value; NotifyPropertyChanged();} } }
	private DateTime _RPDACO;
	public DateTime RPDACO { get => _RPDACO; set { if (_RPDACO != value) { _RPDACO = value; NotifyPropertyChanged();} } }
	private string? _RPTIPP;
	public string? RPTIPP { get => _RPTIPP; set { if (_RPTIPP != value) { _RPTIPP = value; NotifyPropertyChanged();} } }
	private decimal? _RPQLOR;
	public decimal? RPQLOR { get => _RPQLOR; set { if (_RPQLOR != value) { _RPQLOR = value; NotifyPropertyChanged();} } }
	private decimal? _RPQPUR;
	public decimal? RPQPUR { get => _RPQPUR; set { if (_RPQPUR != value) { _RPQPUR = value; NotifyPropertyChanged();} } }
	private decimal? _RPQSCO;
	public decimal? RPQSCO { get => _RPQSCO; set { if (_RPQSCO != value) { _RPQSCO = value; NotifyPropertyChanged();} } }
	private int? _RPTEAP;
	public int? RPTEAP { get => _RPTEAP; set { if (_RPTEAP != value) { _RPTEAP = value; NotifyPropertyChanged();} } }
	private decimal? _RPCAMA;
	public decimal? RPCAMA { get => _RPCAMA; set { if (_RPCAMA != value) { _RPCAMA = value; NotifyPropertyChanged();} } }
	private decimal? _RPCAMB;
	public decimal? RPCAMB { get => _RPCAMB; set { if (_RPCAMB != value) { _RPCAMB = value; NotifyPropertyChanged();} } }
	private decimal? _RPCAMC;
	public decimal? RPCAMC { get => _RPCAMC; set { if (_RPCAMC != value) { _RPCAMC = value; NotifyPropertyChanged();} } }
	private decimal? _RPCAMD;
	public decimal? RPCAMD { get => _RPCAMD; set { if (_RPCAMD != value) { _RPCAMD = value; NotifyPropertyChanged();} } }
	private decimal? _RPCAME;
	public decimal? RPCAME { get => _RPCAME; set { if (_RPCAME != value) { _RPCAME = value; NotifyPropertyChanged();} } }
	private decimal? _RPCAMF;
	public decimal? RPCAMF { get => _RPCAMF; set { if (_RPCAMF != value) { _RPCAMF = value; NotifyPropertyChanged();} } }
	private decimal? _RPCAMG;
	public decimal? RPCAMG { get => _RPCAMG; set { if (_RPCAMG != value) { _RPCAMG = value; NotifyPropertyChanged();} } }
	private decimal? _RPCAMH;
	public decimal? RPCAMH { get => _RPCAMH; set { if (_RPCAMH != value) { _RPCAMH = value; NotifyPropertyChanged();} } }
	private string? _RPMABA;
	public string? RPMABA { get => _RPMABA; set { if (_RPMABA != value) { _RPMABA = value; NotifyPropertyChanged();} } }
	private string? _RPPOLI;
	public string? RPPOLI { get => _RPPOLI; set { if (_RPPOLI != value) { _RPPOLI = value; NotifyPropertyChanged();} } }
	private int? _RPNUSE;
	public int? RPNUSE { get => _RPNUSE; set { if (_RPNUSE != value) { _RPNUSE = value; NotifyPropertyChanged();} } }
	private decimal? _RPQEDI;
	public decimal? RPQEDI { get => _RPQEDI; set { if (_RPQEDI != value) { _RPQEDI = value; NotifyPropertyChanged();} } }
	private DateTime? _RPDAOG;
	public DateTime? RPDAOG { get => _RPDAOG; set { if (_RPDAOG != value) { _RPDAOG = value; NotifyPropertyChanged();} } }
	private DateTime? _RPDAIM;
	public DateTime? RPDAIM { get => _RPDAIM; set { if (_RPDAIM != value) { _RPDAIM = value; NotifyPropertyChanged();} } }
	private int? _RPTMPS;
	public int? RPTMPS { get => _RPTMPS; set { if (_RPTMPS != value) { _RPTMPS = value; NotifyPropertyChanged();} } }
	private int? _RPGGOR;
	public int? RPGGOR { get => _RPGGOR; set { if (_RPGGOR != value) { _RPGGOR = value; NotifyPropertyChanged();} } }
	private int? _RPGORC;
	public int? RPGORC { get => _RPGORC; set { if (_RPGORC != value) { _RPGORC = value; NotifyPropertyChanged();} } }
	private DateTime? _RPDALI;
	public DateTime? RPDALI { get => _RPDALI; set { if (_RPDALI != value) { _RPDALI = value; NotifyPropertyChanged();} } }
	private int? _RPULSE;
	public int? RPULSE { get => _RPULSE; set { if (_RPULSE != value) { _RPULSE = value; NotifyPropertyChanged();} } }
	private int? _RPNERR;
	public int? RPNERR { get => _RPNERR; set { if (_RPNERR != value) { _RPNERR = value; NotifyPropertyChanged();} } }
	private decimal? _RPTEAV;
	public decimal? RPTEAV { get => _RPTEAV; set { if (_RPTEAV != value) { _RPTEAV = value; NotifyPropertyChanged();} } }
	private decimal? _RPOTGP;
	public decimal? RPOTGP { get => _RPOTGP; set { if (_RPOTGP != value) { _RPOTGP = value; NotifyPropertyChanged();} } }
	private int? _RPTECO;
	public int? RPTECO { get => _RPTECO; set { if (_RPTECO != value) { _RPTECO = value; NotifyPropertyChanged();} } }
	private string? _RPFILL;
	public string? RPFILL { get => _RPFILL; set { if (_RPFILL != value) { _RPFILL = value; NotifyPropertyChanged();} } }
	private string? _RPFLAG;
	public string? RPFLAG { get => _RPFLAG; set { if (_RPFLAG != value) { _RPFLAG = value; NotifyPropertyChanged();} } }
}