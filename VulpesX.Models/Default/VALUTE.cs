namespace VulpesX.Models.Default;
 
public partial class VALUTE : Base 
{
	private string _VALCOD = null!;
	public required string VALCOD { get => _VALCOD; set { if (_VALCOD != value) { _VALCOD = value; NotifyPropertyChanged();} } }
	private string _VALDIV = null!;
	public required string VALDIV { get => _VALDIV; set { if (_VALDIV != value) { _VALDIV = value; NotifyPropertyChanged();} } }
	private string? _VALDES;
	public string? VALDES { get => _VALDES; set { if (_VALDES != value) { _VALDES = value; NotifyPropertyChanged();} } }
	private string? _VALTIP;
	public string? VALTIP { get => _VALTIP; set { if (_VALTIP != value) { _VALTIP = value; NotifyPropertyChanged();} } }
	private decimal? _VALCAM;
	public decimal? VALCAM { get => _VALCAM; set { if (_VALCAM != value) { _VALCAM = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}