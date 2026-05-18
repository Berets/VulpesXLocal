namespace VulpesX.Models.Default;
 
public partial class TPROTAUT : Base 
{
	private string _socprot = null!;
	public required string socprot { get => _socprot; set { if (_socprot != value) { _socprot = value; NotifyPropertyChanged();} } }
	private string _annprot = null!;
	public required string annprot { get => _annprot; set { if (_annprot != value) { _annprot = value; NotifyPropertyChanged();} } }
	private string _numprot = null!;
	public required string numprot { get => _numprot; set { if (_numprot != value) { _numprot = value; NotifyPropertyChanged();} } }
	private string? _flgprot;
	public string? flgprot { get => _flgprot; set { if (_flgprot != value) { _flgprot = value; NotifyPropertyChanged();} } }
}