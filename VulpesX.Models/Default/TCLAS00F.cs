namespace VulpesX.Models.Default;
 
public partial class TCLAS00F : Base 
{
	private string _cmcodm = null!;
	public required string cmcodm { get => _cmcodm; set { if (_cmcodm != value) { _cmcodm = value; NotifyPropertyChanged();} } }
	private string? _cmdesc;
	public string? cmdesc { get => _cmdesc; set { if (_cmdesc != value) { _cmdesc = value; NotifyPropertyChanged();} } }
}