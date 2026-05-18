namespace VulpesX.Models.Default;
 
public partial class PDCCONTI : Base 
{
	private string _P1GRUP = null!;
	public required string P1GRUP { get => _P1GRUP; set { if (_P1GRUP != value) { _P1GRUP = value; NotifyPropertyChanged();} } }
	private string _P2CONT = null!;
	public required string P2CONT { get => _P2CONT; set { if (_P2CONT != value) { _P2CONT = value; NotifyPropertyChanged();} } }
	private string? _P2DES1;
	public string? P2DES1 { get => _P2DES1; set { if (_P2DES1 != value) { _P2DES1 = value; NotifyPropertyChanged();} } }
	private string? _P2DES2;
	public string? P2DES2 { get => _P2DES2; set { if (_P2DES2 != value) { _P2DES2 = value; NotifyPropertyChanged();} } }
	private string? _p2flcf;
	public string? p2flcf { get => _p2flcf; set { if (_p2flcf != value) { _p2flcf = value; NotifyPropertyChanged();} } }
	private string? _p2sett;
	public string? p2sett { get => _p2sett; set { if (_p2sett != value) { _p2sett = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}