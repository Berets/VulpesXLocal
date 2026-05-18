namespace VulpesX.Models.Default;
 
public partial class ISO : Base 
{
	private string _isocod = null!;
	public required string isocod { get => _isocod; set { if (_isocod != value) { _isocod = value; NotifyPropertyChanged();} } }
	private string _isodes = null!;
	public required string isodes { get => _isodes; set { if (_isodes != value) { _isodes = value; NotifyPropertyChanged();} } }
	private int? _isopiv;
	public int? isopiv { get => _isopiv; set { if (_isopiv != value) { _isopiv = value; NotifyPropertyChanged();} } }
	private string? _isolin;
	public string? isolin { get => _isolin; set { if (_isolin != value) { _isolin = value; NotifyPropertyChanged();} } }
	private string? _isointr;
	public string? isointr { get => _isointr; set { if (_isointr != value) { _isointr = value; NotifyPropertyChanged();} } }
	private string? _isocod3166;
	public string? isocod3166 { get => _isocod3166; set { if (_isocod3166 != value) { _isocod3166 = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}