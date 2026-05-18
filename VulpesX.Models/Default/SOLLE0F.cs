namespace VulpesX.Models.Default;
 
public partial class SOLLE0F : Base 
{
	private string _Sollsoc = null!;
	public required string Sollsoc { get => _Sollsoc; set { if (_Sollsoc != value) { _Sollsoc = value; NotifyPropertyChanged();} } }
	private int _Sollcli;
	public int Sollcli { get => _Sollcli; set { if (_Sollcli != value) { _Sollcli = value; NotifyPropertyChanged();} } }
	private DateTime _sollann;
	public DateTime sollann { get => _sollann; set { if (_sollann != value) { _sollann = value; NotifyPropertyChanged();} } }
	private DateTime _solldat;
	public DateTime solldat { get => _solldat; set { if (_solldat != value) { _solldat = value; NotifyPropertyChanged();} } }
	private string _sollrif = null!;
	public required string sollrif { get => _sollrif; set { if (_sollrif != value) { _sollrif = value; NotifyPropertyChanged();} } }
	private int _soltip;
	public int soltip { get => _soltip; set { if (_soltip != value) { _soltip = value; NotifyPropertyChanged();} } }
	private DateTime _sollscad;
	public DateTime sollscad { get => _sollscad; set { if (_sollscad != value) { _sollscad = value; NotifyPropertyChanged();} } }
	private int _sollrig;
	public int sollrig { get => _sollrig; set { if (_sollrig != value) { _sollrig = value; NotifyPropertyChanged();} } }
	private string? _solsot;
	public string? solsot { get => _solsot; set { if (_solsot != value) { _solsot = value; NotifyPropertyChanged();} } }
	private string? _solcot;
	public string? solcot { get => _solcot; set { if (_solcot != value) { _solcot = value; NotifyPropertyChanged();} } }
	private string? _soltit;
	public string? soltit { get => _soltit; set { if (_soltit != value) { _soltit = value; NotifyPropertyChanged();} } }
	private string? _solltes;
	public string? solltes { get => _solltes; set { if (_solltes != value) { _solltes = value; NotifyPropertyChanged();} } }
	private decimal? _sollimpo;
	public decimal? sollimpo { get => _sollimpo; set { if (_sollimpo != value) { _sollimpo = value; NotifyPropertyChanged();} } }
	private int? _sollgra;
	public int? sollgra { get => _sollgra; set { if (_sollgra != value) { _sollgra = value; NotifyPropertyChanged();} } }
	private string? _sollflg;
	public string? sollflg { get => _sollflg; set { if (_sollflg != value) { _sollflg = value; NotifyPropertyChanged();} } }
	private string? _sollfile;
	public string? sollfile { get => _sollfile; set { if (_sollfile != value) { _sollfile = value; NotifyPropertyChanged();} } }
	private string? _sollsegn;
	public string? sollsegn { get => _sollsegn; set { if (_sollsegn != value) { _sollsegn = value; NotifyPropertyChanged();} } }
	private string? _sollcau;
	public string? sollcau { get => _sollcau; set { if (_sollcau != value) { _sollcau = value; NotifyPropertyChanged();} } }
	private string? _sollcaude;
	public string? sollcaude { get => _sollcaude; set { if (_sollcaude != value) { _sollcaude = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}