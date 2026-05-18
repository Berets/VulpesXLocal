namespace VulpesX.Models.Default;
 
public partial class PERFLUFIN1 : Base 
{
	private string _Flusoc = null!;
	public required string Flusoc { get => _Flusoc; set { if (_Flusoc != value) { _Flusoc = value; NotifyPropertyChanged();} } }
	private int _flurig;
	public int flurig { get => _flurig; set { if (_flurig != value) { _flurig = value; NotifyPropertyChanged();} } }
	private string? _fluclfo;
	public string? fluclfo { get => _fluclfo; set { if (_fluclfo != value) { _fluclfo = value; NotifyPropertyChanged();} } }
	private int? _Flucod;
	public int? Flucod { get => _Flucod; set { if (_Flucod != value) { _Flucod = value; NotifyPropertyChanged();} } }
}