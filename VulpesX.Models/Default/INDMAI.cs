namespace VulpesX.Models.Default;
 
public partial class INDMAI : Base 
{
	private string _indmsoc = null!;
	public required string indmsoc { get => _indmsoc; set { if (_indmsoc != value) { _indmsoc = value; NotifyPropertyChanged();} } }
	private string _indmcod = null!;
	public required string indmcod { get => _indmcod; set { if (_indmcod != value) { _indmcod = value; NotifyPropertyChanged();} } }
	private int _indrig;
	public int indrig { get => _indrig; set { if (_indrig != value) { _indrig = value; NotifyPropertyChanged();} } }
	private string? _indmema;
	public string? indmema { get => _indmema; set { if (_indmema != value) { _indmema = value; NotifyPropertyChanged();} } }
	private string? _indnom;
	public string? indnom { get => _indnom; set { if (_indnom != value) { _indnom = value; NotifyPropertyChanged();} } }
	private string? _indatt;
	public string? indatt { get => _indatt; set { if (_indatt != value) { _indatt = value; NotifyPropertyChanged();} } }
}