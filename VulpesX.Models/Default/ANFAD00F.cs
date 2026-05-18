namespace VulpesX.Models.Default;
 
public partial class ANFAD00F : Base 
{
	private string _AFSOCI = null!;
	public required string AFSOCI { get => _AFSOCI; set { if (_AFSOCI != value) { _AFSOCI = value; NotifyPropertyChanged();} } }
	private int _AFANNO;
	public int AFANNO { get => _AFANNO; set { if (_AFANNO != value) { _AFANNO = value; NotifyPropertyChanged();} } }
	private int _AFNUOR;
	public int AFNUOR { get => _AFNUOR; set { if (_AFNUOR != value) { _AFNUOR = value; NotifyPropertyChanged();} } }
	private int _AFDRIGA;
	public int AFDRIGA { get => _AFDRIGA; set { if (_AFDRIGA != value) { _AFDRIGA = value; NotifyPropertyChanged();} } }
	private string _AFDCODA = null!;
	public required string AFDCODA { get => _AFDCODA; set { if (_AFDCODA != value) { _AFDCODA = value; NotifyPropertyChanged();} } }
	private decimal _AFDQTAV;
	public decimal AFDQTAV { get => _AFDQTAV; set { if (_AFDQTAV != value) { _AFDQTAV = value; NotifyPropertyChanged();} } }
	private decimal? _AFDREDD;
	public decimal? AFDREDD { get => _AFDREDD; set { if (_AFDREDD != value) { _AFDREDD = value; NotifyPropertyChanged();} } }
	private string? _AFDTRED;
	public string? AFDTRED { get => _AFDTRED; set { if (_AFDTRED != value) { _AFDTRED = value; NotifyPropertyChanged();} } }
	private decimal _AFDCOST;
	public decimal AFDCOST { get => _AFDCOST; set { if (_AFDCOST != value) { _AFDCOST = value; NotifyPropertyChanged();} } }
	private decimal _AFDPREZ;
	public decimal AFDPREZ { get => _AFDPREZ; set { if (_AFDPREZ != value) { _AFDPREZ = value; NotifyPropertyChanged();} } }
	private string? _AFDNOTE;
	public string? AFDNOTE { get => _AFDNOTE; set { if (_AFDNOTE != value) { _AFDNOTE = value; NotifyPropertyChanged();} } }
	private bool _AFDSHOW;
	public bool AFDSHOW { get => _AFDSHOW; set { if (_AFDSHOW != value) { _AFDSHOW = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
	private string _AFDUNIM = null!;
	public required string AFDUNIM { get => _AFDUNIM; set { if (_AFDUNIM != value) { _AFDUNIM = value; NotifyPropertyChanged();} } }
	private string? _AFDTMAR;
	public string? AFDTMAR { get => _AFDTMAR; set { if (_AFDTMAR != value) { _AFDTMAR = value; NotifyPropertyChanged();} } }
}