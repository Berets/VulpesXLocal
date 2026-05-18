namespace VulpesX.Models.Default;
 
public partial class TEMPO00F : Base 
{
	private string _tosoci = null!;
	public required string tosoci { get => _tosoci; set { if (_tosoci != value) { _tosoci = value; NotifyPropertyChanged();} } }
	private string _TOCOPA = null!;
	public required string TOCOPA { get => _TOCOPA; set { if (_TOCOPA != value) { _TOCOPA = value; NotifyPropertyChanged();} } }
	private int _toannp;
	public int toannp { get => _toannp; set { if (_toannp != value) { _toannp = value; NotifyPropertyChanged();} } }
	private int _TOORDP;
	public int TOORDP { get => _TOORDP; set { if (_TOORDP != value) { _TOORDP = value; NotifyPropertyChanged();} } }
	private int _TONSEQ;
	public int TONSEQ { get => _TONSEQ; set { if (_TONSEQ != value) { _TONSEQ = value; NotifyPropertyChanged();} } }
	private string _TOREPA = null!;
	public required string TOREPA { get => _TOREPA; set { if (_TOREPA != value) { _TOREPA = value; NotifyPropertyChanged();} } }
	private string _TOFASE = null!;
	public required string TOFASE { get => _TOFASE; set { if (_TOFASE != value) { _TOFASE = value; NotifyPropertyChanged();} } }
	private string _TOMACC = null!;
	public required string TOMACC { get => _TOMACC; set { if (_TOMACC != value) { _TOMACC = value; NotifyPropertyChanged();} } }
	private string? _TOATTI;
	public string? TOATTI { get => _TOATTI; set { if (_TOATTI != value) { _TOATTI = value; NotifyPropertyChanged();} } }
	private decimal? _TOTEMP;
	public decimal? TOTEMP { get => _TOTEMP; set { if (_TOTEMP != value) { _TOTEMP = value; NotifyPropertyChanged();} } }
	private decimal? _TOTEMT;
	public decimal? TOTEMT { get => _TOTEMT; set { if (_TOTEMT != value) { _TOTEMT = value; NotifyPropertyChanged();} } }
	private decimal? _TOTQTA;
	public decimal? TOTQTA { get => _TOTQTA; set { if (_TOTQTA != value) { _TOTQTA = value; NotifyPropertyChanged();} } }
	private int? _TOTTUH;
	public int? TOTTUH { get => _TOTTUH; set { if (_TOTTUH != value) { _TOTTUH = value; NotifyPropertyChanged();} } }
	private int? _TOTTUM;
	public int? TOTTUM { get => _TOTTUM; set { if (_TOTTUM != value) { _TOTTUM = value; NotifyPropertyChanged();} } }
	private int? _TOTTUS;
	public int? TOTTUS { get => _TOTTUS; set { if (_TOTTUS != value) { _TOTTUS = value; NotifyPropertyChanged();} } }
	private int? _TOTTTH;
	public int? TOTTTH { get => _TOTTTH; set { if (_TOTTTH != value) { _TOTTTH = value; NotifyPropertyChanged();} } }
	private int? _TOTTTM;
	public int? TOTTTM { get => _TOTTTM; set { if (_TOTTTM != value) { _TOTTTM = value; NotifyPropertyChanged();} } }
	private int? _TOTTTS;
	public int? TOTTTS { get => _TOTTTS; set { if (_TOTTTS != value) { _TOTTTS = value; NotifyPropertyChanged();} } }
}