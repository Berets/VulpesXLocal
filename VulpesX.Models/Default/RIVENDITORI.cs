namespace VulpesX.Models.Default;
 
public partial class RIVENDITORI : Base 
{
	private string _rivcod = null!;
	public required string rivcod { get => _rivcod; set { if (_rivcod != value) { _rivcod = value; NotifyPropertyChanged();} } }
	private string _rivdes = null!;
	public required string rivdes { get => _rivdes; set { if (_rivdes != value) { _rivdes = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}