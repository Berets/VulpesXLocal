namespace VulpesX.Models.Default;
 
public partial class CFP001P : Base 
{
	private string _cfpsoc = null!;
	public required string cfpsoc { get => _cfpsoc; set { if (_cfpsoc != value) { _cfpsoc = value; NotifyPropertyChanged();} } }
	private string? _cfpdoc;
	public string? cfpdoc { get => _cfpdoc; set { if (_cfpdoc != value) { _cfpdoc = value; NotifyPropertyChanged();} } }
	private string? _cfpdof;
	public string? cfpdof { get => _cfpdof; set { if (_cfpdof != value) { _cfpdof = value; NotifyPropertyChanged();} } }
}