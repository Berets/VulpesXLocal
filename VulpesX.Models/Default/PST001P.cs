namespace VulpesX.Models.Default;
 
public partial class PST001P : Base 
{
	private string _PSTSOC = null!;
	public required string PSTSOC { get => _PSTSOC; set { if (_PSTSOC != value) { _PSTSOC = value; NotifyPropertyChanged();} } }
	private string _PSTCAT = null!;
	public required string PSTCAT { get => _PSTCAT; set { if (_PSTCAT != value) { _PSTCAT = value; NotifyPropertyChanged();} } }
	private int _PSTANN;
	public int PSTANN { get => _PSTANN; set { if (_PSTANN != value) { _PSTANN = value; NotifyPropertyChanged();} } }
	private int _PSTRIG;
	public int PSTRIG { get => _PSTRIG; set { if (_PSTRIG != value) { _PSTRIG = value; NotifyPropertyChanged();} } }
	private decimal? _PSTIMP;
	public decimal? PSTIMP { get => _PSTIMP; set { if (_PSTIMP != value) { _PSTIMP = value; NotifyPropertyChanged();} } }
	private decimal? _PSTPAM;
	public decimal? PSTPAM { get => _PSTPAM; set { if (_PSTPAM != value) { _PSTPAM = value; NotifyPropertyChanged();} } }
	private decimal? _PSTIMA;
	public decimal? PSTIMA { get => _PSTIMA; set { if (_PSTIMA != value) { _PSTIMA = value; NotifyPropertyChanged();} } }
	private decimal? _PSTABB;
	public decimal? PSTABB { get => _PSTABB; set { if (_PSTABB != value) { _PSTABB = value; NotifyPropertyChanged();} } }
	private string? _PSTNOT;
	public string? PSTNOT { get => _PSTNOT; set { if (_PSTNOT != value) { _PSTNOT = value; NotifyPropertyChanged();} } }
}