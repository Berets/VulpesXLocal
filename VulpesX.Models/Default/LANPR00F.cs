namespace VulpesX.Models.Default;
 
public partial class LANPR00F : Base 
{
	private string _PRSOCB = null!;
	public required string PRSOCB { get => _PRSOCB; set { if (_PRSOCB != value) { _PRSOCB = value; NotifyPropertyChanged();} } }
	private int _PRANNP;
	public int PRANNP { get => _PRANNP; set { if (_PRANNP != value) { _PRANNP = value; NotifyPropertyChanged();} } }
	private int _PRORDP;
	public int PRORDP { get => _PRORDP; set { if (_PRORDP != value) { _PRORDP = value; NotifyPropertyChanged();} } }
	private int _PRNSEQ;
	public int PRNSEQ { get => _PRNSEQ; set { if (_PRNSEQ != value) { _PRNSEQ = value; NotifyPropertyChanged();} } }
	private string? _PRATTI;
	public string? PRATTI { get => _PRATTI; set { if (_PRATTI != value) { _PRATTI = value; NotifyPropertyChanged();} } }
	private string? _PRREPA;
	public string? PRREPA { get => _PRREPA; set { if (_PRREPA != value) { _PRREPA = value; NotifyPropertyChanged();} } }
	private string? _PRFASE;
	public string? PRFASE { get => _PRFASE; set { if (_PRFASE != value) { _PRFASE = value; NotifyPropertyChanged();} } }
	private DateTime? _PRDAIP;
	public DateTime? PRDAIP { get => _PRDAIP; set { if (_PRDAIP != value) { _PRDAIP = value; NotifyPropertyChanged();} } }
	private DateTime? _PRDAFP;
	public DateTime? PRDAFP { get => _PRDAFP; set { if (_PRDAFP != value) { _PRDAFP = value; NotifyPropertyChanged();} } }
	private decimal? _PRTETP;
	public decimal? PRTETP { get => _PRTETP; set { if (_PRTETP != value) { _PRTETP = value; NotifyPropertyChanged();} } }
	private decimal? _PRTETE;
	public decimal? PRTETE { get => _PRTETE; set { if (_PRTETE != value) { _PRTETE = value; NotifyPropertyChanged();} } }
	private string? _PRTIFA;
	public string? PRTIFA { get => _PRTIFA; set { if (_PRTIFA != value) { _PRTIFA = value; NotifyPropertyChanged();} } }
	private string? _prsoc1;
	public string? prsoc1 { get => _prsoc1; set { if (_prsoc1 != value) { _prsoc1 = value; NotifyPropertyChanged();} } }
	private string? _prsoc2;
	public string? prsoc2 { get => _prsoc2; set { if (_prsoc2 != value) { _prsoc2 = value; NotifyPropertyChanged();} } }
	private string? _prsoc3;
	public string? prsoc3 { get => _prsoc3; set { if (_prsoc3 != value) { _prsoc3 = value; NotifyPropertyChanged();} } }
	private string? _prmacc;
	public string? prmacc { get => _prmacc; set { if (_prmacc != value) { _prmacc = value; NotifyPropertyChanged();} } }
}