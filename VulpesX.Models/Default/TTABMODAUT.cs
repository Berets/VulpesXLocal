namespace VulpesX.Models.Default;
 
public partial class TTABMODAUT : Base 
{
	private string _socmodaut = null!;
	public required string socmodaut { get => _socmodaut; set { if (_socmodaut != value) { _socmodaut = value; NotifyPropertyChanged();} } }
	private string _codmodaut = null!;
	public required string codmodaut { get => _codmodaut; set { if (_codmodaut != value) { _codmodaut = value; NotifyPropertyChanged();} } }
	private DateTime? _datmodaut;
	public DateTime? datmodaut { get => _datmodaut; set { if (_datmodaut != value) { _datmodaut = value; NotifyPropertyChanged();} } }
}