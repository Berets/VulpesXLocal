namespace VulpesX.Models.Default;
 
public partial class ANFADD00F : Base 
{
	private string _AFSOCI = null!;
	public required string AFSOCI { get => _AFSOCI; set { if (_AFSOCI != value) { _AFSOCI = value; NotifyPropertyChanged();} } }
	private int _AFANNO;
	public int AFANNO { get => _AFANNO; set { if (_AFANNO != value) { _AFANNO = value; NotifyPropertyChanged();} } }
	private int _AFNUOR;
	public int AFNUOR { get => _AFNUOR; set { if (_AFNUOR != value) { _AFNUOR = value; NotifyPropertyChanged();} } }
	private int _AFDRIGA;
	public int AFDRIGA { get => _AFDRIGA; set { if (_AFDRIGA != value) { _AFDRIGA = value; NotifyPropertyChanged();} } }
	private int _AFDDPROG;
	public int AFDDPROG { get => _AFDDPROG; set { if (_AFDDPROG != value) { _AFDDPROG = value; NotifyPropertyChanged();} } }
	private string _AFDDTIPO = null!;
	public required string AFDDTIPO { get => _AFDDTIPO; set { if (_AFDDTIPO != value) { _AFDDTIPO = value; NotifyPropertyChanged();} } }
	private string? _AFDDDESC;
	public string? AFDDDESC { get => _AFDDDESC; set { if (_AFDDDESC != value) { _AFDDDESC = value; NotifyPropertyChanged();} } }
	private string? _AFDDCODA;
	public string? AFDDCODA { get => _AFDDCODA; set { if (_AFDDCODA != value) { _AFDDCODA = value; NotifyPropertyChanged();} } }
	private decimal _AFDDQTAV;
	public decimal AFDDQTAV { get => _AFDDQTAV; set { if (_AFDDQTAV != value) { _AFDDQTAV = value; NotifyPropertyChanged();} } }
	private decimal _AFDDCOST;
	public decimal AFDDCOST { get => _AFDDCOST; set { if (_AFDDCOST != value) { _AFDDCOST = value; NotifyPropertyChanged();} } }
	private string? _AFDDNOTE;
	public string? AFDDNOTE { get => _AFDDNOTE; set { if (_AFDDNOTE != value) { _AFDDNOTE = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}