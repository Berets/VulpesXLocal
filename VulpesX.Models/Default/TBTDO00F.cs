namespace VulpesX.Models.Default;
 
public partial class TBTDO00F : Base 
{
	private string _ttdcod = null!;
	public required string ttdcod { get => _ttdcod; set { if (_ttdcod != value) { _ttdcod = value; NotifyPropertyChanged();} } }
	private string? _ttddes;
	public string? ttddes { get => _ttddes; set { if (_ttddes != value) { _ttddes = value; NotifyPropertyChanged();} } }
}