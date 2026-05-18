namespace VulpesX.Models.Default;
 
public partial class SIM001P : Base 
{
	private string _SIMSOC = null!;
	public required string SIMSOC { get => _SIMSOC; set { if (_SIMSOC != value) { _SIMSOC = value; NotifyPropertyChanged();} } }
	private string _SIMTIP = null!;
	public required string SIMTIP { get => _SIMTIP; set { if (_SIMTIP != value) { _SIMTIP = value; NotifyPropertyChanged();} } }
	private byte[]? _SIMIMM;
	public byte[]? SIMIMM { get => _SIMIMM; set { if (_SIMIMM != value) { _SIMIMM = value; NotifyPropertyChanged();} } }
	private string? _SIMFILNOM;
	public string? SIMFILNOM { get => _SIMFILNOM; set { if (_SIMFILNOM != value) { _SIMFILNOM = value; NotifyPropertyChanged();} } }
	private string? _SIMFILEST;
	public string? SIMFILEST { get => _SIMFILEST; set { if (_SIMFILEST != value) { _SIMFILEST = value; NotifyPropertyChanged();} } }
}