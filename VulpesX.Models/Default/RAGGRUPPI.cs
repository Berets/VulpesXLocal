namespace VulpesX.Models.Default;
 
public partial class RAGGRUPPI : Base 
{
	private string _R1GRUP = null!;
	public required string R1GRUP { get => _R1GRUP; set { if (_R1GRUP != value) { _R1GRUP = value; NotifyPropertyChanged();} } }
	private string? _r1chco;
	public string? r1chco { get => _r1chco; set { if (_r1chco != value) { _r1chco = value; NotifyPropertyChanged();} } }
	private string? _R1TICO;
	public string? R1TICO { get => _R1TICO; set { if (_R1TICO != value) { _R1TICO = value; NotifyPropertyChanged();} } }
	private string? _R1DES1;
	public string? R1DES1 { get => _R1DES1; set { if (_R1DES1 != value) { _R1DES1 = value; NotifyPropertyChanged();} } }
	private string? _R1DES2;
	public string? R1DES2 { get => _R1DES2; set { if (_R1DES2 != value) { _R1DES2 = value; NotifyPropertyChanged();} } }
	private string? _R1OBCP;
	public string? R1OBCP { get => _R1OBCP; set { if (_R1OBCP != value) { _R1OBCP = value; NotifyPropertyChanged();} } }
}