namespace VulpesX.Models.Default;
 
public partial class LEGACAUSALI : Base 
{
	private string _comapn = null!;
	public required string comapn { get => _comapn; set { if (_comapn != value) { _comapn = value; NotifyPropertyChanged();} } }
	private string _comcpn = null!;
	public required string comcpn { get => _comcpn; set { if (_comcpn != value) { _comcpn = value; NotifyPropertyChanged();} } }
	private string _comtpr = null!;
	public required string comtpr { get => _comtpr; set { if (_comtpr != value) { _comtpr = value; NotifyPropertyChanged();} } }
	private string? _comaco;
	public string? comaco { get => _comaco; set { if (_comaco != value) { _comaco = value; NotifyPropertyChanged();} } }
	private string? _comcco;
	public string? comcco { get => _comcco; set { if (_comcco != value) { _comcco = value; NotifyPropertyChanged();} } }
}