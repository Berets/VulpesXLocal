namespace VulpesX.Models.Default;
 
public partial class CAUSVEN : Base 
{
	private string _cauven = null!;
	public required string cauven { get => _cauven; set { if (_cauven != value) { _cauven = value; NotifyPropertyChanged();} } }
	private string? _caudev;
	public string? caudev { get => _caudev; set { if (_caudev != value) { _caudev = value; NotifyPropertyChanged();} } }
	private string? _cauflv;
	public string? cauflv { get => _cauflv; set { if (_cauflv != value) { _cauflv = value; NotifyPropertyChanged();} } }
	private string? _caugrv;
	public string? caugrv { get => _caugrv; set { if (_caugrv != value) { _caugrv = value; NotifyPropertyChanged();} } }
	private string? _caucov;
	public string? caucov { get => _caucov; set { if (_caucov != value) { _caucov = value; NotifyPropertyChanged();} } }
	private string? _causov;
	public string? causov { get => _causov; set { if (_causov != value) { _causov = value; NotifyPropertyChanged();} } }
	private string? _caumav;
	public string? caumav { get => _caumav; set { if (_caumav != value) { _caumav = value; NotifyPropertyChanged();} } }
	private string? _caucof;
	public string? caucof { get => _caucof; set { if (_caucof != value) { _caucof = value; NotifyPropertyChanged();} } }
}