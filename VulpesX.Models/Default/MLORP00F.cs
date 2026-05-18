namespace VulpesX.Models.Default;
 
public partial class MLORP00F : Base 
{
	private string _LPSOCI = null!;
	public required string LPSOCI { get => _LPSOCI; set { if (_LPSOCI != value) { _LPSOCI = value; NotifyPropertyChanged();} } }
	private int _LPANNP;
	public int LPANNP { get => _LPANNP; set { if (_LPANNP != value) { _LPANNP = value; NotifyPropertyChanged();} } }
	private int _LPNUOP;
	public int LPNUOP { get => _LPNUOP; set { if (_LPNUOP != value) { _LPNUOP = value; NotifyPropertyChanged();} } }
	private int _LPNULP;
	public int LPNULP { get => _LPNULP; set { if (_LPNULP != value) { _LPNULP = value; NotifyPropertyChanged();} } }
	private decimal? _LPQPRO;
	public decimal? LPQPRO { get => _LPQPRO; set { if (_LPQPRO != value) { _LPQPRO = value; NotifyPropertyChanged();} } }
	private decimal? _LPQCON;
	public decimal? LPQCON { get => _LPQCON; set { if (_LPQCON != value) { _LPQCON = value; NotifyPropertyChanged();} } }
	private string? _LPFLEV;
	public string? LPFLEV { get => _LPFLEV; set { if (_LPFLEV != value) { _LPFLEV = value; NotifyPropertyChanged();} } }
}