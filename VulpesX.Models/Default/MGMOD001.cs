namespace VulpesX.Models.Default;
 
public partial class MGMOD001 : Base 
{
	private string _mmsoci = null!;
	public required string mmsoci { get => _mmsoci; set { if (_mmsoci != value) { _mmsoci = value; NotifyPropertyChanged();} } }
	private int _MMESCO;
	public int MMESCO { get => _MMESCO; set { if (_MMESCO != value) { _MMESCO = value; NotifyPropertyChanged();} } }
	private int _MMNURE;
	public int MMNURE { get => _MMNURE; set { if (_MMNURE != value) { _MMNURE = value; NotifyPropertyChanged();} } }
	private int _MMPODO;
	public int MMPODO { get => _MMPODO; set { if (_MMPODO != value) { _MMPODO = value; NotifyPropertyChanged();} } }
	private int _MMPOSC;
	public int MMPOSC { get => _MMPOSC; set { if (_MMPOSC != value) { _MMPOSC = value; NotifyPropertyChanged();} } }
	private string _MMCOLA = null!;
	public required string MMCOLA { get => _MMCOLA; set { if (_MMCOLA != value) { _MMCOLA = value; NotifyPropertyChanged();} } }
	private decimal? _MMQTC;
	public decimal? MMQTC { get => _MMQTC; set { if (_MMQTC != value) { _MMQTC = value; NotifyPropertyChanged();} } }
	private decimal? _mmqts;
	public decimal? mmqts { get => _mmqts; set { if (_mmqts != value) { _mmqts = value; NotifyPropertyChanged();} } }
	private DateTime? _mmdasc;
	public DateTime? mmdasc { get => _mmdasc; set { if (_mmdasc != value) { _mmdasc = value; NotifyPropertyChanged();} } }
}