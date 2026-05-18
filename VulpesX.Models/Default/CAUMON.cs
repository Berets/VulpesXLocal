namespace VulpesX.Models.Default;
 
public partial class CAUMON : Base 
{
	private string _camsoc = null!;
	public required string camsoc { get => _camsoc; set { if (_camsoc != value) { _camsoc = value; NotifyPropertyChanged();} } }
	private string _camcod = null!;
	public required string camcod { get => _camcod; set { if (_camcod != value) { _camcod = value; NotifyPropertyChanged();} } }
	private string? _camde1;
	public string? camde1 { get => _camde1; set { if (_camde1 != value) { _camde1 = value; NotifyPropertyChanged();} } }
	private string? _camtip;
	public string? camtip { get => _camtip; set { if (_camtip != value) { _camtip = value; NotifyPropertyChanged();} } }
	private string? _camso1;
	public string? camso1 { get => _camso1; set { if (_camso1 != value) { _camso1 = value; NotifyPropertyChanged();} } }
}