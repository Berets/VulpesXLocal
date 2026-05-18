namespace VulpesX.Models.Default;
 
public partial class MITESTATA : Base 
{
	private string _MISOCI = null!;
	public required string MISOCI { get => _MISOCI; set { if (_MISOCI != value) { _MISOCI = value; NotifyPropertyChanged();} } }
	private int _MIANNO;
	public int MIANNO { get => _MIANNO; set { if (_MIANNO != value) { _MIANNO = value; NotifyPropertyChanged();} } }
	private int _MINUME;
	public int MINUME { get => _MINUME; set { if (_MINUME != value) { _MINUME = value; NotifyPropertyChanged();} } }
	private DateTime _MIDATA;
	public DateTime MIDATA { get => _MIDATA; set { if (_MIDATA != value) { _MIDATA = value; NotifyPropertyChanged();} } }
	private string _MiGRUP = null!;
	public required string MiGRUP { get => _MiGRUP; set { if (_MiGRUP != value) { _MiGRUP = value; NotifyPropertyChanged();} } }
	private string _MiCONT = null!;
	public required string MiCONT { get => _MiCONT; set { if (_MiCONT != value) { _MiCONT = value; NotifyPropertyChanged();} } }
	private string _MiSOTT = null!;
	public required string MiSOTT { get => _MiSOTT; set { if (_MiSOTT != value) { _MiSOTT = value; NotifyPropertyChanged();} } }
	private string _MiSBAN = null!;
	public required string MiSBAN { get => _MiSBAN; set { if (_MiSBAN != value) { _MiSBAN = value; NotifyPropertyChanged();} } }
	private int _MiABI;
	public int MiABI { get => _MiABI; set { if (_MiABI != value) { _MiABI = value; NotifyPropertyChanged();} } }
	private int _MiCAB;
	public int MiCAB { get => _MiCAB; set { if (_MiCAB != value) { _MiCAB = value; NotifyPropertyChanged();} } }
	private string _MiCCOR = null!;
	public required string MiCCOR { get => _MiCCOR; set { if (_MiCCOR != value) { _MiCCOR = value; NotifyPropertyChanged();} } }
	private int _MiNRCL;
	public int MiNRCL { get => _MiNRCL; set { if (_MiNRCL != value) { _MiNRCL = value; NotifyPropertyChanged();} } }
	private decimal _MiEUIM;
	public decimal MiEUIM { get => _MiEUIM; set { if (_MiEUIM != value) { _MiEUIM = value; NotifyPropertyChanged();} } }
	private string _MiCOVA = null!;
	public required string MiCOVA { get => _MiCOVA; set { if (_MiCOVA != value) { _MiCOVA = value; NotifyPropertyChanged();} } }
	private string _MiVALU = null!;
	public required string MiVALU { get => _MiVALU; set { if (_MiVALU != value) { _MiVALU = value; NotifyPropertyChanged();} } }
	private decimal _MiVAIM;
	public decimal MiVAIM { get => _MiVAIM; set { if (_MiVAIM != value) { _MiVAIM = value; NotifyPropertyChanged();} } }
	private string _MiFLST = null!;
	public required string MiFLST { get => _MiFLST; set { if (_MiFLST != value) { _MiFLST = value; NotifyPropertyChanged();} } }
	private string _MiFLCO = null!;
	public required string MiFLCO { get => _MiFLCO; set { if (_MiFLCO != value) { _MiFLCO = value; NotifyPropertyChanged();} } }
	private decimal _mispese;
	public decimal mispese { get => _mispese; set { if (_mispese != value) { _mispese = value; NotifyPropertyChanged();} } }
	private string? _miccbon;
	public string? miccbon { get => _miccbon; set { if (_miccbon != value) { _miccbon = value; NotifyPropertyChanged();} } }
	private int? _micabbon;
	public int? micabbon { get => _micabbon; set { if (_micabbon != value) { _micabbon = value; NotifyPropertyChanged();} } }
	private int? _miabibon;
	public int? miabibon { get => _miabibon; set { if (_miabibon != value) { _miabibon = value; NotifyPropertyChanged();} } }
}