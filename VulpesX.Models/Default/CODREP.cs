namespace VulpesX.Models.Default;
 
public partial class CODREP : Base 
{
	private string _repsoc = null!;
	public required string repsoc { get => _repsoc; set { if (_repsoc != value) { _repsoc = value; NotifyPropertyChanged();} } }
	private string _repcod = null!;
	public required string repcod { get => _repcod; set { if (_repcod != value) { _repcod = value; NotifyPropertyChanged();} } }
	private string? _repdes;
	public string? repdes { get => _repdes; set { if (_repdes != value) { _repdes = value; NotifyPropertyChanged();} } }
	private int? _repren;
	public int? repren { get => _repren; set { if (_repren != value) { _repren = value; NotifyPropertyChanged();} } }
	private decimal? _repcin;
	public decimal? repcin { get => _repcin; set { if (_repcin != value) { _repcin = value; NotifyPropertyChanged();} } }
	private decimal? _reptst;
	public decimal? reptst { get => _reptst; set { if (_reptst != value) { _reptst = value; NotifyPropertyChanged();} } }
	private decimal? _reptpr;
	public decimal? reptpr { get => _reptpr; set { if (_reptpr != value) { _reptpr = value; NotifyPropertyChanged();} } }
	private int? _repntu;
	public int? repntu { get => _repntu; set { if (_repntu != value) { _repntu = value; NotifyPropertyChanged();} } }
	private int? _repnad;
	public int? repnad { get => _repnad; set { if (_repnad != value) { _repnad = value; NotifyPropertyChanged();} } }
	private int? _reposi;
	public int? reposi { get => _reposi; set { if (_reposi != value) { _reposi = value; NotifyPropertyChanged();} } }
	private int? _reposf;
	public int? reposf { get => _reposf; set { if (_reposf != value) { _reposf = value; NotifyPropertyChanged();} } }
	private decimal? _reptpa;
	public decimal? reptpa { get => _reptpa; set { if (_reptpa != value) { _reptpa = value; NotifyPropertyChanged();} } }
	private int? _reparr;
	public int? reparr { get => _reparr; set { if (_reparr != value) { _reparr = value; NotifyPropertyChanged();} } }
	private string? _repdis;
	public string? repdis { get => _repdis; set { if (_repdis != value) { _repdis = value; NotifyPropertyChanged();} } }
	private string? _repmat;
	public string? repmat { get => _repmat; set { if (_repmat != value) { _repmat = value; NotifyPropertyChanged();} } }
}