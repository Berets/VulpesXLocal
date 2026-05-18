namespace VulpesX.Models.Default;
 
public partial class MODTB00F : Base 
{
	private string _modinf = null!;
	public required string modinf { get => _modinf; set { if (_modinf != value) { _modinf = value; NotifyPropertyChanged();} } }
	private string? _moddes;
	public string? moddes { get => _moddes; set { if (_moddes != value) { _moddes = value; NotifyPropertyChanged();} } }
	private string? _modpth;
	public string? modpth { get => _modpth; set { if (_modpth != value) { _modpth = value; NotifyPropertyChanged();} } }
}