namespace VulpesX.Models.Default;
 
public partial class PERSAUTOF : Base 
{
	private string _PAuSoc = null!;
	public required string PAuSoc { get => _PAuSoc; set { if (_PAuSoc != value) { _PAuSoc = value; NotifyPropertyChanged();} } }
	private int? _PAuCli;
	public int? PAuCli { get => _PAuCli; set { if (_PAuCli != value) { _PAuCli = value; NotifyPropertyChanged();} } }
	private string? _PAuAli;
	public string? PAuAli { get => _PAuAli; set { if (_PAuAli != value) { _PAuAli = value; NotifyPropertyChanged();} } }
	private string? _PAuAss;
	public string? PAuAss { get => _PAuAss; set { if (_PAuAss != value) { _PAuAss = value; NotifyPropertyChanged();} } }
	private string? _pauart;
	public string? pauart { get => _pauart; set { if (_pauart != value) { _pauart = value; NotifyPropertyChanged();} } }
	private string? _pauliv;
	public string? pauliv { get => _pauliv; set { if (_pauliv != value) { _pauliv = value; NotifyPropertyChanged();} } }
	private string? _pcaugir;
	public string? pcaugir { get => _pcaugir; set { if (_pcaugir != value) { _pcaugir = value; NotifyPropertyChanged();} } }
}