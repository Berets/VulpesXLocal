namespace VulpesX.Models.Default;
 
public partial class F24CONTIAE : Base 
{
	private string _TF24soc3 = null!;
	public required string TF24soc3 { get => _TF24soc3; set { if (_TF24soc3 != value) { _TF24soc3 = value; NotifyPropertyChanged();} } }
	private string _TF24cod3 = null!;
	public required string TF24cod3 { get => _TF24cod3; set { if (_TF24cod3 != value) { _TF24cod3 = value; NotifyPropertyChanged();} } }
	private string _TF24desc3 = null!;
	public required string TF24desc3 { get => _TF24desc3; set { if (_TF24desc3 != value) { _TF24desc3 = value; NotifyPropertyChanged();} } }
	private string _TF24grp3 = null!;
	public required string TF24grp3 { get => _TF24grp3; set { if (_TF24grp3 != value) { _TF24grp3 = value; NotifyPropertyChanged();} } }
	private string _TF24cto3 = null!;
	public required string TF24cto3 { get => _TF24cto3; set { if (_TF24cto3 != value) { _TF24cto3 = value; NotifyPropertyChanged();} } }
	private string _TF24sotc3 = null!;
	public required string TF24sotc3 { get => _TF24sotc3; set { if (_TF24sotc3 != value) { _TF24sotc3 = value; NotifyPropertyChanged();} } }
	private string? _tf24descagg1;
	public string? tf24descagg1 { get => _tf24descagg1; set { if (_tf24descagg1 != value) { _tf24descagg1 = value; NotifyPropertyChanged();} } }
}