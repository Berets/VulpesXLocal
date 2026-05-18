namespace VulpesX.Models.Default;
 
public partial class CCTES00 : Base 
{
	private string _ccsoci = null!;
	public required string ccsoci { get => _ccsoci; set { if (_ccsoci != value) { _ccsoci = value; NotifyPropertyChanged();} } }
	private string _cctmod = null!;
	public required string cctmod { get => _cctmod; set { if (_cctmod != value) { _cctmod = value; NotifyPropertyChanged();} } }
	private string? _cctdes;
	public string? cctdes { get => _cctdes; set { if (_cctdes != value) { _cctdes = value; NotifyPropertyChanged();} } }
	private int? _cctrig;
	public int? cctrig { get => _cctrig; set { if (_cctrig != value) { _cctrig = value; NotifyPropertyChanged();} } }
	private int? _cctncd;
	public int? cctncd { get => _cctncd; set { if (_cctncd != value) { _cctncd = value; NotifyPropertyChanged();} } }
}