namespace VulpesX.Models.Default;
 
public partial class PCATA00F : Base 
{
	private string _pctsoc = null!;
	public required string pctsoc { get => _pctsoc; set { if (_pctsoc != value) { _pctsoc = value; NotifyPropertyChanged();} } }
	private int _pctann;
	public int pctann { get => _pctann; set { if (_pctann != value) { _pctann = value; NotifyPropertyChanged();} } }
	private string? _pcttip;
	public string? pcttip { get => _pcttip; set { if (_pcttip != value) { _pcttip = value; NotifyPropertyChanged();} } }
	private int? _pctnum;
	public int? pctnum { get => _pctnum; set { if (_pctnum != value) { _pctnum = value; NotifyPropertyChanged();} } }
	private DateTime? _pctdat;
	public DateTime? pctdat { get => _pctdat; set { if (_pctdat != value) { _pctdat = value; NotifyPropertyChanged();} } }
	private string? _pctatt;
	public string? pctatt { get => _pctatt; set { if (_pctatt != value) { _pctatt = value; NotifyPropertyChanged();} } }
	private string? _pctori;
	public string? pctori { get => _pctori; set { if (_pctori != value) { _pctori = value; NotifyPropertyChanged();} } }
	private string? _pctorv;
	public string? pctorv { get => _pctorv; set { if (_pctorv != value) { _pctorv = value; NotifyPropertyChanged();} } }
	private string? _pctter;
	public string? pctter { get => _pctter; set { if (_pctter != value) { _pctter = value; NotifyPropertyChanged();} } }
	private string? _pctuti;
	public string? pctuti { get => _pctuti; set { if (_pctuti != value) { _pctuti = value; NotifyPropertyChanged();} } }
	private string? _pctutv;
	public string? pctutv { get => _pctutv; set { if (_pctutv != value) { _pctutv = value; NotifyPropertyChanged();} } }
}