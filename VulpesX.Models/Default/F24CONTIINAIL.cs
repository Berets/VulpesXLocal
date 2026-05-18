namespace VulpesX.Models.Default;
 
public partial class F24CONTIINAIL : Base 
{
	private string _TF24Soc2 = null!;
	public required string TF24Soc2 { get => _TF24Soc2; set { if (_TF24Soc2 != value) { _TF24Soc2 = value; NotifyPropertyChanged();} } }
	private string _TF24cod2 = null!;
	public required string TF24cod2 { get => _TF24cod2; set { if (_TF24cod2 != value) { _TF24cod2 = value; NotifyPropertyChanged();} } }
	private string _TF24desc2 = null!;
	public required string TF24desc2 { get => _TF24desc2; set { if (_TF24desc2 != value) { _TF24desc2 = value; NotifyPropertyChanged();} } }
	private string? _TF24grp2;
	public string? TF24grp2 { get => _TF24grp2; set { if (_TF24grp2 != value) { _TF24grp2 = value; NotifyPropertyChanged();} } }
	private string? _TF24cto2;
	public string? TF24cto2 { get => _TF24cto2; set { if (_TF24cto2 != value) { _TF24cto2 = value; NotifyPropertyChanged();} } }
	private string? _TF24sotc2;
	public string? TF24sotc2 { get => _TF24sotc2; set { if (_TF24sotc2 != value) { _TF24sotc2 = value; NotifyPropertyChanged();} } }
	private string? _tf24descagg;
	public string? tf24descagg { get => _tf24descagg; set { if (_tf24descagg != value) { _tf24descagg = value; NotifyPropertyChanged();} } }
}