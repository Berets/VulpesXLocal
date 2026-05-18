namespace VulpesX.Models.Default;
 
public partial class TPERSSPESO2017 : Base 
{
	private string _socspeso = null!;
	public required string socspeso { get => _socspeso; set { if (_socspeso != value) { _socspeso = value; NotifyPropertyChanged();} } }
}