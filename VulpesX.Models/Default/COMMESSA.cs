namespace VulpesX.Models.Default;
 
public partial class COMMESSA : Base 
{
	private string _comsoc = null!;
	public required string comsoc { get => _comsoc; set { if (_comsoc != value) { _comsoc = value; NotifyPropertyChanged();} } }
	private string _comnum = null!;
	public required string comnum { get => _comnum; set { if (_comnum != value) { _comnum = value; NotifyPropertyChanged();} } }
	private int? _comann;
	public int? comann { get => _comann; set { if (_comann != value) { _comann = value; NotifyPropertyChanged();} } }
	private DateTime? _comdip;
	public DateTime? comdip { get => _comdip; set { if (_comdip != value) { _comdip = value; NotifyPropertyChanged();} } }
	private DateTime? _comdie;
	public DateTime? comdie { get => _comdie; set { if (_comdie != value) { _comdie = value; NotifyPropertyChanged();} } }
	private DateTime? _comdfp;
	public DateTime? comdfp { get => _comdfp; set { if (_comdfp != value) { _comdfp = value; NotifyPropertyChanged();} } }
	private DateTime? _comdfe;
	public DateTime? comdfe { get => _comdfe; set { if (_comdfe != value) { _comdfe = value; NotifyPropertyChanged();} } }
	private string? _comcdc;
	public string? comcdc { get => _comcdc; set { if (_comcdc != value) { _comcdc = value; NotifyPropertyChanged();} } }
	private string? _comlip;
	public string? comlip { get => _comlip; set { if (_comlip != value) { _comlip = value; NotifyPropertyChanged();} } }
	private string? _comres;
	public string? comres { get => _comres; set { if (_comres != value) { _comres = value; NotifyPropertyChanged();} } }
	private decimal? _comven;
	public decimal? comven { get => _comven; set { if (_comven != value) { _comven = value; NotifyPropertyChanged();} } }
	private decimal? _commat;
	public decimal? commat { get => _commat; set { if (_commat != value) { _commat = value; NotifyPropertyChanged();} } }
	private int? _comore;
	public int? comore { get => _comore; set { if (_comore != value) { _comore = value; NotifyPropertyChanged();} } }
	private decimal? _comtar;
	public decimal? comtar { get => _comtar; set { if (_comtar != value) { _comtar = value; NotifyPropertyChanged();} } }
	private decimal? _comman;
	public decimal? comman { get => _comman; set { if (_comman != value) { _comman = value; NotifyPropertyChanged();} } }
	private string? _comnot;
	public string? comnot { get => _comnot; set { if (_comnot != value) { _comnot = value; NotifyPropertyChanged();} } }
	private string? _comsta;
	public string? comsta { get => _comsta; set { if (_comsta != value) { _comsta = value; NotifyPropertyChanged();} } }
	private string? _commac;
	public string? commac { get => _commac; set { if (_commac != value) { _commac = value; NotifyPropertyChanged();} } }
	private string? _comso1;
	public string? comso1 { get => _comso1; set { if (_comso1 != value) { _comso1 = value; NotifyPropertyChanged();} } }
}