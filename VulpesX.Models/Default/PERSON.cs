namespace VulpesX.Models.Default;
 
public partial class PERSON : Base 
{
	private string _perkey = null!;
	public required string perkey { get => _perkey; set { if (_perkey != value) { _perkey = value; NotifyPropertyChanged();} } }
	private string? _pertip;
	public string? pertip { get => _pertip; set { if (_pertip != value) { _pertip = value; NotifyPropertyChanged();} } }
	private string? _perusr;
	public string? perusr { get => _perusr; set { if (_perusr != value) { _perusr = value; NotifyPropertyChanged();} } }
}