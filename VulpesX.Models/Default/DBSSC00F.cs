namespace VulpesX.Models.Default;
 
public partial class DBSSC00F : Base 
{
	private string _dbpsoc = null!;
	public required string dbpsoc { get => _dbpsoc; set { if (_dbpsoc != value) { _dbpsoc = value; NotifyPropertyChanged();} } }
	private string _dbpcod = null!;
	public required string dbpcod { get => _dbpcod; set { if (_dbpcod != value) { _dbpcod = value; NotifyPropertyChanged();} } }
	private int _dbdprg;
	public int dbdprg { get => _dbdprg; set { if (_dbdprg != value) { _dbdprg = value; NotifyPropertyChanged();} } }
	private int _dssprg;
	public int dssprg { get => _dssprg; set { if (_dssprg != value) { _dssprg = value; NotifyPropertyChanged();} } }
	private string? _dsscod;
	public string? dsscod { get => _dsscod; set { if (_dsscod != value) { _dsscod = value; NotifyPropertyChanged();} } }
	private decimal? _dssqta;
	public decimal? dssqta { get => _dssqta; set { if (_dssqta != value) { _dssqta = value; NotifyPropertyChanged();} } }
	private DateTime? _dssdai;
	public DateTime? dssdai { get => _dssdai; set { if (_dssdai != value) { _dssdai = value; NotifyPropertyChanged();} } }
	private DateTime? _dssdaf;
	public DateTime? dssdaf { get => _dssdaf; set { if (_dssdaf != value) { _dssdaf = value; NotifyPropertyChanged();} } }
	private string? _dssndo;
	public string? dssndo { get => _dssndo; set { if (_dssndo != value) { _dssndo = value; NotifyPropertyChanged();} } }
	private DateTime? _dssddo;
	public DateTime? dssddo { get => _dssddo; set { if (_dssddo != value) { _dssddo = value; NotifyPropertyChanged();} } }
	private int? _dsspri;
	public int? dsspri { get => _dsspri; set { if (_dsspri != value) { _dsspri = value; NotifyPropertyChanged();} } }
	private string? _dssdes;
	public string? dssdes { get => _dssdes; set { if (_dssdes != value) { _dssdes = value; NotifyPropertyChanged();} } }
}