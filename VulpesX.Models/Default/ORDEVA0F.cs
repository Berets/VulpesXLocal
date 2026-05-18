namespace VulpesX.Models.Default;
 
public partial class ORDEVA0F : Base 
{
	private string _ODUTEN = null!;
	public required string ODUTEN { get => _ODUTEN; set { if (_ODUTEN != value) { _ODUTEN = value; NotifyPropertyChanged();} } }
	private int _ODCOCL;
	public int ODCOCL { get => _ODCOCL; set { if (_ODCOCL != value) { _ODCOCL = value; NotifyPropertyChanged();} } }
	private string _otsoci = null!;
	public required string otsoci { get => _otsoci; set { if (_otsoci != value) { _otsoci = value; NotifyPropertyChanged();} } }
	private int _OTANNO;
	public int OTANNO { get => _OTANNO; set { if (_OTANNO != value) { _OTANNO = value; NotifyPropertyChanged();} } }
	private int _OTNUOR;
	public int OTNUOR { get => _OTNUOR; set { if (_OTNUOR != value) { _OTNUOR = value; NotifyPropertyChanged();} } }
	private int _ODRIGA;
	public int ODRIGA { get => _ODRIGA; set { if (_ODRIGA != value) { _ODRIGA = value; NotifyPropertyChanged();} } }
	private int _ODCASS;
	public int ODCASS { get => _ODCASS; set { if (_ODCASS != value) { _ODCASS = value; NotifyPropertyChanged();} } }
	private decimal? _ODQTSP;
	public decimal? ODQTSP { get => _ODQTSP; set { if (_ODQTSP != value) { _ODQTSP = value; NotifyPropertyChanged();} } }
	private string? _ODFLGS;
	public string? ODFLGS { get => _ODFLGS; set { if (_ODFLGS != value) { _ODFLGS = value; NotifyPropertyChanged();} } }
	private string? _odpaga;
	public string? odpaga { get => _odpaga; set { if (_odpaga != value) { _odpaga = value; NotifyPropertyChanged();} } }
	private int? _oddest;
	public int? oddest { get => _oddest; set { if (_oddest != value) { _oddest = value; NotifyPropertyChanged();} } }
	private string? _odcoag;
	public string? odcoag { get => _odcoag; set { if (_odcoag != value) { _odcoag = value; NotifyPropertyChanged();} } }
	private string? _odcaus;
	public string? odcaus { get => _odcaus; set { if (_odcaus != value) { _odcaus = value; NotifyPropertyChanged();} } }
	private int? _odcorr;
	public int? odcorr { get => _odcorr; set { if (_odcorr != value) { _odcorr = value; NotifyPropertyChanged();} } }
	private decimal? _odprag;
	public decimal? odprag { get => _odprag; set { if (_odprag != value) { _odprag = value; NotifyPropertyChanged();} } }
	private decimal? _odptra;
	public decimal? odptra { get => _odptra; set { if (_odptra != value) { _odptra = value; NotifyPropertyChanged();} } }
	private decimal? _odpimb;
	public decimal? odpimb { get => _odpimb; set { if (_odpimb != value) { _odpimb = value; NotifyPropertyChanged();} } }
	private string? _odasfi;
	public string? odasfi { get => _odasfi; set { if (_odasfi != value) { _odasfi = value; NotifyPropertyChanged();} } }
	private string? _odpcar;
	public string? odpcar { get => _odpcar; set { if (_odpcar != value) { _odpcar = value; NotifyPropertyChanged();} } }
	private string? _odlott;
	public string? odlott { get => _odlott; set { if (_odlott != value) { _odlott = value; NotifyPropertyChanged();} } }
}