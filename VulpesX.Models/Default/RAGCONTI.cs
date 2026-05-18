namespace VulpesX.Models.Default;
 
public partial class RAGCONTI : Base 
{
	private string _R1GRUP = null!;
	public required string R1GRUP { get => _R1GRUP; set { if (_R1GRUP != value) { _R1GRUP = value; NotifyPropertyChanged();} } }
	private string _R2CONT = null!;
	public required string R2CONT { get => _R2CONT; set { if (_R2CONT != value) { _R2CONT = value; NotifyPropertyChanged();} } }
	private string? _R2DES1;
	public string? R2DES1 { get => _R2DES1; set { if (_R2DES1 != value) { _R2DES1 = value; NotifyPropertyChanged();} } }
	private string? _R2DES2;
	public string? R2DES2 { get => _R2DES2; set { if (_R2DES2 != value) { _R2DES2 = value; NotifyPropertyChanged();} } }
	private string? _r2flcf;
	public string? r2flcf { get => _r2flcf; set { if (_r2flcf != value) { _r2flcf = value; NotifyPropertyChanged();} } }
}