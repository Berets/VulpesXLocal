namespace VulpesX.Models.Default;
 
public partial class DBPAN00 : Base 
{
	private string _dbpsoc = null!;
	public required string dbpsoc { get => _dbpsoc; set { if (_dbpsoc != value) { _dbpsoc = value; NotifyPropertyChanged();} } }
	private string _dbpcod = null!;
	public required string dbpcod { get => _dbpcod; set { if (_dbpcod != value) { _dbpcod = value; NotifyPropertyChanged();} } }
	private decimal? _dbpcos;
	public decimal? dbpcos { get => _dbpcos; set { if (_dbpcos != value) { _dbpcos = value; NotifyPropertyChanged();} } }
	private decimal? _dbpcoe;
	public decimal? dbpcoe { get => _dbpcoe; set { if (_dbpcoe != value) { _dbpcoe = value; NotifyPropertyChanged();} } }
	private string? _dbpatt;
	public string? dbpatt { get => _dbpatt; set { if (_dbpatt != value) { _dbpatt = value; NotifyPropertyChanged();} } }
	private string? _dbputi;
	public string? dbputi { get => _dbputi; set { if (_dbputi != value) { _dbputi = value; NotifyPropertyChanged();} } }
	private string? _dbputv;
	public string? dbputv { get => _dbputv; set { if (_dbputv != value) { _dbputv = value; NotifyPropertyChanged();} } }
	private string? _dbpori;
	public string? dbpori { get => _dbpori; set { if (_dbpori != value) { _dbpori = value; NotifyPropertyChanged();} } }
	private string? _dbporv;
	public string? dbporv { get => _dbporv; set { if (_dbporv != value) { _dbporv = value; NotifyPropertyChanged();} } }
	private string? _dbpter;
	public string? dbpter { get => _dbpter; set { if (_dbpter != value) { _dbpter = value; NotifyPropertyChanged();} } }
}