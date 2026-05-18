namespace VulpesX.Models.Default;
 
public partial class PCBUD00F : Base 
{
	private string _pcbsoc = null!;
	public required string pcbsoc { get => _pcbsoc; set { if (_pcbsoc != value) { _pcbsoc = value; NotifyPropertyChanged();} } }
	private int _pcbann;
	public int pcbann { get => _pcbann; set { if (_pcbann != value) { _pcbann = value; NotifyPropertyChanged();} } }
	private int _pcbmes;
	public int pcbmes { get => _pcbmes; set { if (_pcbmes != value) { _pcbmes = value; NotifyPropertyChanged();} } }
	private string _pcbcos = null!;
	public required string pcbcos { get => _pcbcos; set { if (_pcbcos != value) { _pcbcos = value; NotifyPropertyChanged();} } }
	private string _pcbgru = null!;
	public required string pcbgru { get => _pcbgru; set { if (_pcbgru != value) { _pcbgru = value; NotifyPropertyChanged();} } }
	private string _pcbcon = null!;
	public required string pcbcon { get => _pcbcon; set { if (_pcbcon != value) { _pcbcon = value; NotifyPropertyChanged();} } }
	private string _pcbsot = null!;
	public required string pcbsot { get => _pcbsot; set { if (_pcbsot != value) { _pcbsot = value; NotifyPropertyChanged();} } }
	private int _pcbbud;
	public int pcbbud { get => _pcbbud; set { if (_pcbbud != value) { _pcbbud = value; NotifyPropertyChanged();} } }
	private decimal? _pcbcvl;
	public decimal? pcbcvl { get => _pcbcvl; set { if (_pcbcvl != value) { _pcbcvl = value; NotifyPropertyChanged();} } }
	private decimal? _pcbcve;
	public decimal? pcbcve { get => _pcbcve; set { if (_pcbcve != value) { _pcbcve = value; NotifyPropertyChanged();} } }
	private decimal? _pcbcqt;
	public decimal? pcbcqt { get => _pcbcqt; set { if (_pcbcqt != value) { _pcbcqt = value; NotifyPropertyChanged();} } }
	private decimal? _pcbpri;
	public decimal? pcbpri { get => _pcbpri; set { if (_pcbpri != value) { _pcbpri = value; NotifyPropertyChanged();} } }
	private string? _pcbfl1;
	public string? pcbfl1 { get => _pcbfl1; set { if (_pcbfl1 != value) { _pcbfl1 = value; NotifyPropertyChanged();} } }
	private string? _pcbfl2;
	public string? pcbfl2 { get => _pcbfl2; set { if (_pcbfl2 != value) { _pcbfl2 = value; NotifyPropertyChanged();} } }
	private string? _pcbori;
	public string? pcbori { get => _pcbori; set { if (_pcbori != value) { _pcbori = value; NotifyPropertyChanged();} } }
	private string? _pcborv;
	public string? pcborv { get => _pcborv; set { if (_pcborv != value) { _pcborv = value; NotifyPropertyChanged();} } }
	private string? _pcbter;
	public string? pcbter { get => _pcbter; set { if (_pcbter != value) { _pcbter = value; NotifyPropertyChanged();} } }
	private string? _pcbuti;
	public string? pcbuti { get => _pcbuti; set { if (_pcbuti != value) { _pcbuti = value; NotifyPropertyChanged();} } }
	private string? _pcbutv;
	public string? pcbutv { get => _pcbutv; set { if (_pcbutv != value) { _pcbutv = value; NotifyPropertyChanged();} } }
	private string? _pcbatt;
	public string? pcbatt { get => _pcbatt; set { if (_pcbatt != value) { _pcbatt = value; NotifyPropertyChanged();} } }
	private string? _pcbspe;
	public string? pcbspe { get => _pcbspe; set { if (_pcbspe != value) { _pcbspe = value; NotifyPropertyChanged();} } }
	private string? _pcbtbu;
	public string? pcbtbu { get => _pcbtbu; set { if (_pcbtbu != value) { _pcbtbu = value; NotifyPropertyChanged();} } }
}