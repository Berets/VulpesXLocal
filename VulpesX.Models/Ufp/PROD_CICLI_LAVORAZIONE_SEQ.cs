namespace VulpesX.Models.Ufp;
using VulpesX.Shared;
public partial class PROD_CICLI_LAVORAZIONE_SEQ : Base 
{
	private string _cicsoc = null!;
	public required string cicsoc { get => _cicsoc; set { if (_cicsoc != value) { _cicsoc = value; NotifyPropertyChanged();} } }
	private string _cicart = null!;
	public required string cicart { get => _cicart; set { if (_cicart != value) { _cicart = value; NotifyPropertyChanged();} } }
	private int _cicseq;
	public int cicseq { get => _cicseq; set { if (_cicseq != value) { _cicseq = value; NotifyPropertyChanged();} } }
	private string? _ciccre;
	public string? ciccre { get => _ciccre; set { if (_ciccre != value) { _ciccre = value; NotifyPropertyChanged();} } }
	private string? _cicfas;
	public string? cicfas { get => _cicfas; set { if (_cicfas != value) { _cicfas = value; NotifyPropertyChanged();} } }
	private decimal? _cictmm;
	public decimal? cictmm { get => _cictmm; set { if (_cictmm != value) { _cictmm = value; NotifyPropertyChanged();} } }
	private decimal? _cictma;
	public decimal? cictma { get => _cictma; set { if (_cictma != value) { _cictma = value; NotifyPropertyChanged();} } }
	private decimal? _cictat;
	public decimal? cictat { get => _cictat; set { if (_cictat != value) { _cictat = value; NotifyPropertyChanged();} } }
	private string? _cictfa;
	public string? cictfa { get => _cictfa; set { if (_cictfa != value) { _cictfa = value; NotifyPropertyChanged();} } }
	private int? _cicumq;
	public int? cicumq { get => _cicumq; set { if (_cicumq != value) { _cicumq = value; NotifyPropertyChanged();} } }
	private decimal? _cictmp;
	public decimal? cictmp { get => _cictmp; set { if (_cictmp != value) { _cictmp = value; NotifyPropertyChanged();} } }
	private decimal? _cictms;
	public decimal? cictms { get => _cictms; set { if (_cictms != value) { _cictms = value; NotifyPropertyChanged();} } }
	private string? _cicdes;
	public string? cicdes { get => _cicdes; set { if (_cicdes != value) { _cicdes = value; NotifyPropertyChanged();} } }
	private string? _repso1;
	public string? repso1 { get => _repso1; set { if (_repso1 != value) { _repso1 = value; NotifyPropertyChanged();} } }
	private string? _lavso1;
	public string? lavso1 { get => _lavso1; set { if (_lavso1 != value) { _lavso1 = value; NotifyPropertyChanged();} } }
	private int? _cicmdh;
	public int? cicmdh { get => _cicmdh; set { if (_cicmdh != value) { _cicmdh = value; NotifyPropertyChanged();} } }
	private int? _cicmdm;
	public int? cicmdm { get => _cicmdm; set { if (_cicmdm != value) { _cicmdm = value; NotifyPropertyChanged();} } }
	private int? _cicmds;
	public int? cicmds { get => _cicmds; set { if (_cicmds != value) { _cicmds = value; NotifyPropertyChanged();} } }
	private int? _cicmah;
	public int? cicmah { get => _cicmah; set { if (_cicmah != value) { _cicmah = value; NotifyPropertyChanged();} } }
	private int? _cicmam;
	public int? cicmam { get => _cicmam; set { if (_cicmam != value) { _cicmam = value; NotifyPropertyChanged();} } }
	private int? _cicmas;
	public int? cicmas { get => _cicmas; set { if (_cicmas != value) { _cicmas = value; NotifyPropertyChanged();} } }
	private int? _cicath;
	public int? cicath { get => _cicath; set { if (_cicath != value) { _cicath = value; NotifyPropertyChanged();} } }
	private int? _cicatm;
	public int? cicatm { get => _cicatm; set { if (_cicatm != value) { _cicatm = value; NotifyPropertyChanged();} } }
	private int? _cicats;
	public int? cicats { get => _cicats; set { if (_cicats != value) { _cicats = value; NotifyPropertyChanged();} } }
	private string? _macso1;
	public string? macso1 { get => _macso1; set { if (_macso1 != value) { _macso1 = value; NotifyPropertyChanged();} } }
	private string? _cicmac;
	public string? cicmac { get => _cicmac; set { if (_cicmac != value) { _cicmac = value; NotifyPropertyChanged();} } }
	private string? _cictype;
	public string? cictype { get => _cictype; set { if (_cictype != value) { _cictype = value; NotifyPropertyChanged();} } }
}