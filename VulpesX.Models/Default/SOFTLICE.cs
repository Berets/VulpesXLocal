namespace VulpesX.Models.Default;
 
public partial class SOFTLICE : Base 
{
	private string _SLsoc = null!;
	public required string SLsoc { get => _SLsoc; set { if (_SLsoc != value) { _SLsoc = value; NotifyPropertyChanged();} } }
	private int _SLRiga;
	public int SLRiga { get => _SLRiga; set { if (_SLRiga != value) { _SLRiga = value; NotifyPropertyChanged();} } }
	private string? _SLkeyc;
	public string? SLkeyc { get => _SLkeyc; set { if (_SLkeyc != value) { _SLkeyc = value; NotifyPropertyChanged();} } }
	private string? _SLCri;
	public string? SLCri { get => _SLCri; set { if (_SLCri != value) { _SLCri = value; NotifyPropertyChanged();} } }
}