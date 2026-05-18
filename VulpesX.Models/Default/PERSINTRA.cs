namespace VulpesX.Models.Default;
 
public partial class PERSINTRA : Base 
{
	private string _PINTKEY = null!;
	public required string PINTKEY { get => _PINTKEY; set { if (_PINTKEY != value) { _PINTKEY = value; NotifyPropertyChanged();} } }
	private string? _PINTTRV;
	public string? PINTTRV { get => _PINTTRV; set { if (_PINTTRV != value) { _PINTTRV = value; NotifyPropertyChanged();} } }
	private string? _PINTNOV;
	public string? PINTNOV { get => _PINTNOV; set { if (_PINTNOV != value) { _PINTNOV = value; NotifyPropertyChanged();} } }
	private string? _PINTCOV;
	public string? PINTCOV { get => _PINTCOV; set { if (_PINTCOV != value) { _PINTCOV = value; NotifyPropertyChanged();} } }
	private int? _PINTTSV;
	public int? PINTTSV { get => _PINTTSV; set { if (_PINTTSV != value) { _PINTTSV = value; NotifyPropertyChanged();} } }
	private string? _PINTPRV;
	public string? PINTPRV { get => _PINTPRV; set { if (_PINTPRV != value) { _PINTPRV = value; NotifyPropertyChanged();} } }
	private string? _PINTTRA;
	public string? PINTTRA { get => _PINTTRA; set { if (_PINTTRA != value) { _PINTTRA = value; NotifyPropertyChanged();} } }
	private string? _PINTNOA;
	public string? PINTNOA { get => _PINTNOA; set { if (_PINTNOA != value) { _PINTNOA = value; NotifyPropertyChanged();} } }
	private string? _PINTCOA;
	public string? PINTCOA { get => _PINTCOA; set { if (_PINTCOA != value) { _PINTCOA = value; NotifyPropertyChanged();} } }
	private int? _PINTTSA;
	public int? PINTTSA { get => _PINTTSA; set { if (_PINTTSA != value) { _PINTTSA = value; NotifyPropertyChanged();} } }
	private string? _PINTPRA;
	public string? PINTPRA { get => _PINTPRA; set { if (_PINTPRA != value) { _PINTPRA = value; NotifyPropertyChanged();} } }
}