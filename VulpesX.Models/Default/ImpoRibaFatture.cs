namespace VulpesX.Models.Default;
 
public partial class ImpoRibaFatture : Base 
{
	private string _IRSOCI = null!;
	public required string IRSOCI { get => _IRSOCI; set { if (_IRSOCI != value) { _IRSOCI = value; NotifyPropertyChanged();} } }
	private int _IRANNO;
	public int IRANNO { get => _IRANNO; set { if (_IRANNO != value) { _IRANNO = value; NotifyPropertyChanged();} } }
	private int _IRNUME;
	public int IRNUME { get => _IRNUME; set { if (_IRNUME != value) { _IRNUME = value; NotifyPropertyChanged();} } }
	private int _IRRIGA;
	public int IRRIGA { get => _IRRIGA; set { if (_IRRIGA != value) { _IRRIGA = value; NotifyPropertyChanged();} } }
	private int _IRPROG;
	public int IRPROG { get => _IRPROG; set { if (_IRPROG != value) { _IRPROG = value; NotifyPropertyChanged();} } }
	private DateTime? _IRDARI;
	public DateTime? IRDARI { get => _IRDARI; set { if (_IRDARI != value) { _IRDARI = value; NotifyPropertyChanged();} } }
	private string? _IRNURI;
	public string? IRNURI { get => _IRNURI; set { if (_IRNURI != value) { _IRNURI = value; NotifyPropertyChanged();} } }
	private decimal? _IRIMPF;
	public decimal? IRIMPF { get => _IRIMPF; set { if (_IRIMPF != value) { _IRIMPF = value; NotifyPropertyChanged();} } }
	private string? _IRSOCC;
	public string? IRSOCC { get => _IRSOCC; set { if (_IRSOCC != value) { _IRSOCC = value; NotifyPropertyChanged();} } }
	private int? _IRANNOC;
	public int? IRANNOC { get => _IRANNOC; set { if (_IRANNOC != value) { _IRANNOC = value; NotifyPropertyChanged();} } }
	private int? _IRNUMEC;
	public int? IRNUMEC { get => _IRNUMEC; set { if (_IRNUMEC != value) { _IRNUMEC = value; NotifyPropertyChanged();} } }
	private int? _IRRIGAC;
	public int? IRRIGAC { get => _IRRIGAC; set { if (_IRRIGAC != value) { _IRRIGAC = value; NotifyPropertyChanged();} } }
}