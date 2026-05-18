namespace VulpesX.Models.Default;
 
public partial class PDCGRUPPI : Base 
{
	private string _P1GRUP = null!;
	public required string P1GRUP { get => _P1GRUP; set { if (_P1GRUP != value) { _P1GRUP = value; NotifyPropertyChanged();} } }
	private string? _p1chco;
	public string? p1chco { get => _p1chco; set { if (_p1chco != value) { _p1chco = value; NotifyPropertyChanged();} } }
	private string? _P1TICO;
	public string? P1TICO { get => _P1TICO; set { if (_P1TICO != value) { _P1TICO = value; NotifyPropertyChanged();} } }
	private string? _P1DES1;
	public string? P1DES1 { get => _P1DES1; set { if (_P1DES1 != value) { _P1DES1 = value; NotifyPropertyChanged();} } }
	private string? _P1DES2;
	public string? P1DES2 { get => _P1DES2; set { if (_P1DES2 != value) { _P1DES2 = value; NotifyPropertyChanged();} } }
	private string? _P1OBCP;
	public string? P1OBCP { get => _P1OBCP; set { if (_P1OBCP != value) { _P1OBCP = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}