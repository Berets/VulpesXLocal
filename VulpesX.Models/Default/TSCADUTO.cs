namespace VulpesX.Models.Default;
 
public partial class TSCADUTO : Base 
{
	private int _SRANNO;
	public int SRANNO { get => _SRANNO; set { if (_SRANNO != value) { _SRANNO = value; NotifyPropertyChanged();} } }
	private int _SRNUREG;
	public int SRNUREG { get => _SRNUREG; set { if (_SRNUREG != value) { _SRNUREG = value; NotifyPropertyChanged();} } }
	private int _SRNURIG;
	public int SRNURIG { get => _SRNURIG; set { if (_SRNURIG != value) { _SRNURIG = value; NotifyPropertyChanged();} } }
	private DateTime _SRDARE;
	public DateTime SRDARE { get => _SRDARE; set { if (_SRDARE != value) { _SRDARE = value; NotifyPropertyChanged();} } }
	private string _SRCAUS = null!;
	public required string SRCAUS { get => _SRCAUS; set { if (_SRCAUS != value) { _SRCAUS = value; NotifyPropertyChanged();} } }
	private string _SRDECA = null!;
	public required string SRDECA { get => _SRDECA; set { if (_SRDECA != value) { _SRDECA = value; NotifyPropertyChanged();} } }
	private int _SRCOCL;
	public int SRCOCL { get => _SRCOCL; set { if (_SRCOCL != value) { _SRCOCL = value; NotifyPropertyChanged();} } }
	private string _SRDECL = null!;
	public required string SRDECL { get => _SRDECL; set { if (_SRDECL != value) { _SRDECL = value; NotifyPropertyChanged();} } }
	private string _SRNURI = null!;
	public required string SRNURI { get => _SRNURI; set { if (_SRNURI != value) { _SRNURI = value; NotifyPropertyChanged();} } }
	private DateTime _SRDARI;
	public DateTime SRDARI { get => _SRDARI; set { if (_SRDARI != value) { _SRDARI = value; NotifyPropertyChanged();} } }
	private string _SRNUDO = null!;
	public required string SRNUDO { get => _SRNUDO; set { if (_SRNUDO != value) { _SRNUDO = value; NotifyPropertyChanged();} } }
	private DateTime _SRDADO;
	public DateTime SRDADO { get => _SRDADO; set { if (_SRDADO != value) { _SRDADO = value; NotifyPropertyChanged();} } }
	private string _SRSEGN = null!;
	public required string SRSEGN { get => _SRSEGN; set { if (_SRSEGN != value) { _SRSEGN = value; NotifyPropertyChanged();} } }
	private DateTime _SRDASC;
	public DateTime SRDASC { get => _SRDASC; set { if (_SRDASC != value) { _SRDASC = value; NotifyPropertyChanged();} } }
	private decimal _SRIMPO;
	public decimal SRIMPO { get => _SRIMPO; set { if (_SRIMPO != value) { _SRIMPO = value; NotifyPropertyChanged();} } }
	private string _SRCOPA = null!;
	public required string SRCOPA { get => _SRCOPA; set { if (_SRCOPA != value) { _SRCOPA = value; NotifyPropertyChanged();} } }
	private string _SRDEPA = null!;
	public required string SRDEPA { get => _SRDEPA; set { if (_SRDEPA != value) { _SRDEPA = value; NotifyPropertyChanged();} } }
	private int _SRNURA;
	public int SRNURA { get => _SRNURA; set { if (_SRNURA != value) { _SRNURA = value; NotifyPropertyChanged();} } }
	private string _SRPARE = null!;
	public required string SRPARE { get => _SRPARE; set { if (_SRPARE != value) { _SRPARE = value; NotifyPropertyChanged();} } }
}