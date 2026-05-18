namespace VulpesX.Models.Default;
 
public partial class TAB_STATES : Base 
{
	private string _cappro = null!;
	public required string cappro { get => _cappro; set { if (_cappro != value) { _cappro = value; NotifyPropertyChanged();} } }
	private string _capdpr = null!;
	public required string capdpr { get => _capdpr; set { if (_capdpr != value) { _capdpr = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}