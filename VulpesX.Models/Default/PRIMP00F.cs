namespace VulpesX.Models.Default;
 
public partial class PRIMP00F : Base 
{
	private string _IMSOCI = null!;
	public required string IMSOCI { get => _IMSOCI; set { if (_IMSOCI != value) { _IMSOCI = value; NotifyPropertyChanged();} } }
	private int _IMANNP;
	public int IMANNP { get => _IMANNP; set { if (_IMANNP != value) { _IMANNP = value; NotifyPropertyChanged();} } }
	private int _IMORDP;
	public int IMORDP { get => _IMORDP; set { if (_IMORDP != value) { _IMORDP = value; NotifyPropertyChanged();} } }
	private string _IMCOPA = null!;
	public required string IMCOPA { get => _IMCOPA; set { if (_IMCOPA != value) { _IMCOPA = value; NotifyPropertyChanged();} } }
	private string? _IMATTI;
	public string? IMATTI { get => _IMATTI; set { if (_IMATTI != value) { _IMATTI = value; NotifyPropertyChanged();} } }
	private DateTime? _IMDAIP;
	public DateTime? IMDAIP { get => _IMDAIP; set { if (_IMDAIP != value) { _IMDAIP = value; NotifyPropertyChanged();} } }
	private string? _IMTIPP;
	public string? IMTIPP { get => _IMTIPP; set { if (_IMTIPP != value) { _IMTIPP = value; NotifyPropertyChanged();} } }
	private decimal? _IMQIMP;
	public decimal? IMQIMP { get => _IMQIMP; set { if (_IMQIMP != value) { _IMQIMP = value; NotifyPropertyChanged();} } }
	private string? _IMFLIE;
	public string? IMFLIE { get => _IMFLIE; set { if (_IMFLIE != value) { _IMFLIE = value; NotifyPropertyChanged();} } }
	private string? _IMMAGA;
	public string? IMMAGA { get => _IMMAGA; set { if (_IMMAGA != value) { _IMMAGA = value; NotifyPropertyChanged();} } }
	private decimal? _IMQSCA;
	public decimal? IMQSCA { get => _IMQSCA; set { if (_IMQSCA != value) { _IMQSCA = value; NotifyPropertyChanged();} } }
	private string? _IMFILL;
	public string? IMFILL { get => _IMFILL; set { if (_IMFILL != value) { _IMFILL = value; NotifyPropertyChanged();} } }
}