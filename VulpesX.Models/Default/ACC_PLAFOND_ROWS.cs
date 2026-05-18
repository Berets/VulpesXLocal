namespace VulpesX.Models.Default;
 
public partial class ACC_PLAFOND_ROWS : Base 
{
	private string _Cliasoc = null!;
	public required string Cliasoc { get => _Cliasoc; set { if (_Cliasoc != value) { _Cliasoc = value; NotifyPropertyChanged();} } }
	private int _Cliacod;
	public int Cliacod { get => _Cliacod; set { if (_Cliacod != value) { _Cliacod = value; NotifyPropertyChanged();} } }
	private int _cliannosol;
	public int cliannosol { get => _cliannosol; set { if (_cliannosol != value) { _cliannosol = value; NotifyPropertyChanged();} } }
	private int _cliprog;
	public int cliprog { get => _cliprog; set { if (_cliprog != value) { _cliprog = value; NotifyPropertyChanged();} } }
	private string _clinumfat = null!;
	public required string clinumfat { get => _clinumfat; set { if (_clinumfat != value) { _clinumfat = value; NotifyPropertyChanged();} } }
	private decimal? _cliimpimpo;
	public decimal? cliimpimpo { get => _cliimpimpo; set { if (_cliimpimpo != value) { _cliimpimpo = value; NotifyPropertyChanged();} } }
	private decimal? _cliimpplaf;
	public decimal? cliimpplaf { get => _cliimpplaf; set { if (_cliimpplaf != value) { _cliimpplaf = value; NotifyPropertyChanged();} } }
	private DateTime? _clidatfatt;
	public DateTime? clidatfatt { get => _clidatfatt; set { if (_clidatfatt != value) { _clidatfatt = value; NotifyPropertyChanged();} } }
}