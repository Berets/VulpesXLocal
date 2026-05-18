namespace VulpesX.Models.Default;
 
public partial class INVPER : Base 
{
	private string _IPesoc = null!;
	public required string IPesoc { get => _IPesoc; set { if (_IPesoc != value) { _IPesoc = value; NotifyPropertyChanged();} } }
	private string? _IPeCas;
	public string? IPeCas { get => _IPeCas; set { if (_IPeCas != value) { _IPeCas = value; NotifyPropertyChanged();} } }
	private string? _IPeCac;
	public string? IPeCac { get => _IPeCac; set { if (_IPeCac != value) { _IPeCac = value; NotifyPropertyChanged();} } }
}