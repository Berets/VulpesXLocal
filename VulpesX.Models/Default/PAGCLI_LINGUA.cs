namespace VulpesX.Models.Default;
 
public partial class PAGCLI_LINGUA : Base 
{
	private string _pclcod = null!;
	public required string pclcod { get => _pclcod; set { if (_pclcod != value) { _pclcod = value; NotifyPropertyChanged();} } }
	private string _lincod = null!;
	public required string lincod { get => _lincod; set { if (_lincod != value) { _lincod = value; NotifyPropertyChanged();} } }
	private string? _pcldes;
	public string? pcldes { get => _pcldes; set { if (_pcldes != value) { _pcldes = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}