namespace VulpesX.Models.Default;
 
public partial class ImpoRiba : Base 
{
	private string _IRSOCI = null!;
	public required string IRSOCI { get => _IRSOCI; set { if (_IRSOCI != value) { _IRSOCI = value; NotifyPropertyChanged();} } }
	private int _IRANNO;
	public int IRANNO { get => _IRANNO; set { if (_IRANNO != value) { _IRANNO = value; NotifyPropertyChanged();} } }
	private int _IRNUME;
	public int IRNUME { get => _IRNUME; set { if (_IRNUME != value) { _IRNUME = value; NotifyPropertyChanged();} } }
	private int _IRRIGA;
	public int IRRIGA { get => _IRRIGA; set { if (_IRRIGA != value) { _IRRIGA = value; NotifyPropertyChanged();} } }
	private int? _IRABI;
	public int? IRABI { get => _IRABI; set { if (_IRABI != value) { _IRABI = value; NotifyPropertyChanged();} } }
	private int? _IRCAB;
	public int? IRCAB { get => _IRCAB; set { if (_IRCAB != value) { _IRCAB = value; NotifyPropertyChanged();} } }
	private int? _IRFORN;
	public int? IRFORN { get => _IRFORN; set { if (_IRFORN != value) { _IRFORN = value; NotifyPropertyChanged();} } }
	private string? _IRDESFOR;
	public string? IRDESFOR { get => _IRDESFOR; set { if (_IRDESFOR != value) { _IRDESFOR = value; NotifyPropertyChanged();} } }
	private DateTime? _IRSCAD;
	public DateTime? IRSCAD { get => _IRSCAD; set { if (_IRSCAD != value) { _IRSCAD = value; NotifyPropertyChanged();} } }
	private decimal? _IRIMPO;
	public decimal? IRIMPO { get => _IRIMPO; set { if (_IRIMPO != value) { _IRIMPO = value; NotifyPropertyChanged();} } }
	private string? _IRDESC;
	public string? IRDESC { get => _IRDESC; set { if (_IRDESC != value) { _IRDESC = value; NotifyPropertyChanged();} } }
	private string? _IRPAG;
	public string? IRPAG { get => _IRPAG; set { if (_IRPAG != value) { _IRPAG = value; NotifyPropertyChanged();} } }
	private string? _IRNUMEF;
	public string? IRNUMEF { get => _IRNUMEF; set { if (_IRNUMEF != value) { _IRNUMEF = value; NotifyPropertyChanged();} } }
}