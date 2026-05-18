namespace VulpesX.Models.Default;
 
public partial class TTABMOD : Base 
{
	private string _codmod = null!;
	public required string codmod { get => _codmod; set { if (_codmod != value) { _codmod = value; NotifyPropertyChanged();} } }
	private string? _descmodulo;
	public string? descmodulo { get => _descmodulo; set { if (_descmodulo != value) { _descmodulo = value; NotifyPropertyChanged();} } }
}