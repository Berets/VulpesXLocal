namespace VulpesX.Models.Default;
 
public partial class LIPTA00F : Base 
{
	private string _lipcod = null!;
	public required string lipcod { get => _lipcod; set { if (_lipcod != value) { _lipcod = value; NotifyPropertyChanged();} } }
	private string? _lipdes;
	public string? lipdes { get => _lipdes; set { if (_lipdes != value) { _lipdes = value; NotifyPropertyChanged();} } }
	private string? _lipatt;
	public string? lipatt { get => _lipatt; set { if (_lipatt != value) { _lipatt = value; NotifyPropertyChanged();} } }
	private string? _liputi;
	public string? liputi { get => _liputi; set { if (_liputi != value) { _liputi = value; NotifyPropertyChanged();} } }
	private string? _liputv;
	public string? liputv { get => _liputv; set { if (_liputv != value) { _liputv = value; NotifyPropertyChanged();} } }
	private string? _lipori;
	public string? lipori { get => _lipori; set { if (_lipori != value) { _lipori = value; NotifyPropertyChanged();} } }
	private string? _liporv;
	public string? liporv { get => _liporv; set { if (_liporv != value) { _liporv = value; NotifyPropertyChanged();} } }
	private string? _lipter;
	public string? lipter { get => _lipter; set { if (_lipter != value) { _lipter = value; NotifyPropertyChanged();} } }
	private string? _lipnom;
	public string? lipnom { get => _lipnom; set { if (_lipnom != value) { _lipnom = value; NotifyPropertyChanged();} } }
	private string? _liplip;
	public string? liplip { get => _liplip; set { if (_liplip != value) { _liplip = value; NotifyPropertyChanged();} } }
}