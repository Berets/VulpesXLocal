namespace VulpesX.Models.Default;
 
public partial class TIPCAUMA : Base 
{
	private string _TiMaco = null!;
	public required string TiMaco { get => _TiMaco; set { if (_TiMaco != value) { _TiMaco = value; NotifyPropertyChanged();} } }
	private string? _TiMade;
	public string? TiMade { get => _TiMade; set { if (_TiMade != value) { _TiMade = value; NotifyPropertyChanged();} } }
}