namespace VulpesX.Models.Default;
 
public partial class TAPUD00F : Base 
{
	private string _tusoci = null!;
	public required string tusoci { get => _tusoci; set { if (_tusoci != value) { _tusoci = value; NotifyPropertyChanged();} } }
	private string _tupvoc = null!;
	public required string tupvoc { get => _tupvoc; set { if (_tupvoc != value) { _tupvoc = value; NotifyPropertyChanged();} } }
	private string? _tupdes;
	public string? tupdes { get => _tupdes; set { if (_tupdes != value) { _tupdes = value; NotifyPropertyChanged();} } }
}