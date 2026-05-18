namespace VulpesX.Models.Default;
 
public partial class TPERSSPESO2017LEVEL1 : Base 
{
	private string _socspeso = null!;
	public required string socspeso { get => _socspeso; set { if (_socspeso != value) { _socspeso = value; NotifyPropertyChanged();} } }
	private string _spcauven = null!;
	public required string spcauven { get => _spcauven; set { if (_spcauven != value) { _spcauven = value; NotifyPropertyChanged();} } }
	private string? _spcauvendes;
	public string? spcauvendes { get => _spcauvendes; set { if (_spcauvendes != value) { _spcauvendes = value; NotifyPropertyChanged();} } }
}