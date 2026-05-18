namespace VulpesX.Models.Default;
 
public partial class DBDAN00 : Base 
{
	private string _dbpsoc = null!;
	public required string dbpsoc { get => _dbpsoc; set { if (_dbpsoc != value) { _dbpsoc = value; NotifyPropertyChanged();} } }
	private string _dbpcod = null!;
	public required string dbpcod { get => _dbpcod; set { if (_dbpcod != value) { _dbpcod = value; NotifyPropertyChanged();} } }
	private int _dbdprg;
	public int dbdprg { get => _dbdprg; set { if (_dbdprg != value) { _dbdprg = value; NotifyPropertyChanged();} } }
	private string? _dbdcod;
	public string? dbdcod { get => _dbdcod; set { if (_dbdcod != value) { _dbdcod = value; NotifyPropertyChanged();} } }
	private decimal? _dbdqta;
	public decimal? dbdqta { get => _dbdqta; set { if (_dbdqta != value) { _dbdqta = value; NotifyPropertyChanged();} } }
	private string? _tracod;
	public string? tracod { get => _tracod; set { if (_tracod != value) { _tracod = value; NotifyPropertyChanged();} } }
	private string? _trades;
	public string? trades { get => _trades; set { if (_trades != value) { _trades = value; NotifyPropertyChanged();} } }
	private decimal? _dbdpes;
	public decimal? dbdpes { get => _dbdpes; set { if (_dbdpes != value) { _dbdpes = value; NotifyPropertyChanged();} } }
	private string? _dbdatt;
	public string? dbdatt { get => _dbdatt; set { if (_dbdatt != value) { _dbdatt = value; NotifyPropertyChanged();} } }
	private string? _dbduti;
	public string? dbduti { get => _dbduti; set { if (_dbduti != value) { _dbduti = value; NotifyPropertyChanged();} } }
	private string? _dbdutv;
	public string? dbdutv { get => _dbdutv; set { if (_dbdutv != value) { _dbdutv = value; NotifyPropertyChanged();} } }
	private string? _dbdori;
	public string? dbdori { get => _dbdori; set { if (_dbdori != value) { _dbdori = value; NotifyPropertyChanged();} } }
	private string? _dbdorv;
	public string? dbdorv { get => _dbdorv; set { if (_dbdorv != value) { _dbdorv = value; NotifyPropertyChanged();} } }
	private string? _dbdter;
	public string? dbdter { get => _dbdter; set { if (_dbdter != value) { _dbdter = value; NotifyPropertyChanged();} } }
	private DateTime? _dbddai;
	public DateTime? dbddai { get => _dbddai; set { if (_dbddai != value) { _dbddai = value; NotifyPropertyChanged();} } }
	private DateTime? _dbddaf;
	public DateTime? dbddaf { get => _dbddaf; set { if (_dbddaf != value) { _dbddaf = value; NotifyPropertyChanged();} } }
	private string? _dbdndo;
	public string? dbdndo { get => _dbdndo; set { if (_dbdndo != value) { _dbdndo = value; NotifyPropertyChanged();} } }
	private DateTime? _dbddau;
	public DateTime? dbddau { get => _dbddau; set { if (_dbddau != value) { _dbddau = value; NotifyPropertyChanged();} } }
}