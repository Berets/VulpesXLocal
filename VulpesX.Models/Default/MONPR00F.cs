namespace VulpesX.Models.Default;
 
public partial class MONPR00F : Base 
{
	private string _MPSOCB = null!;
	public required string MPSOCB { get => _MPSOCB; set { if (_MPSOCB != value) { _MPSOCB = value; NotifyPropertyChanged();} } }
	private int _MPANNP;
	public int MPANNP { get => _MPANNP; set { if (_MPANNP != value) { _MPANNP = value; NotifyPropertyChanged();} } }
	private int _MPORDP;
	public int MPORDP { get => _MPORDP; set { if (_MPORDP != value) { _MPORDP = value; NotifyPropertyChanged();} } }
	private int _MPNULP;
	public int MPNULP { get => _MPNULP; set { if (_MPNULP != value) { _MPNULP = value; NotifyPropertyChanged();} } }
	private int _MPNSEQ;
	public int MPNSEQ { get => _MPNSEQ; set { if (_MPNSEQ != value) { _MPNSEQ = value; NotifyPropertyChanged();} } }
	private int _MPNPRO;
	public int MPNPRO { get => _MPNPRO; set { if (_MPNPRO != value) { _MPNPRO = value; NotifyPropertyChanged();} } }
	private string? _MPPOSC;
	public string? MPPOSC { get => _MPPOSC; set { if (_MPPOSC != value) { _MPPOSC = value; NotifyPropertyChanged();} } }
	private string? _MPCOD1;
	public string? MPCOD1 { get => _MPCOD1; set { if (_MPCOD1 != value) { _MPCOD1 = value; NotifyPropertyChanged();} } }
	private string? _MPCOD2;
	public string? MPCOD2 { get => _MPCOD2; set { if (_MPCOD2 != value) { _MPCOD2 = value; NotifyPropertyChanged();} } }
	private string? _MPCAUS;
	public string? MPCAUS { get => _MPCAUS; set { if (_MPCAUS != value) { _MPCAUS = value; NotifyPropertyChanged();} } }
	private int? _MPNDIF;
	public int? MPNDIF { get => _MPNDIF; set { if (_MPNDIF != value) { _MPNDIF = value; NotifyPropertyChanged();} } }
	private int? _MPNCDI;
	public int? MPNCDI { get => _MPNCDI; set { if (_MPNCDI != value) { _MPNCDI = value; NotifyPropertyChanged();} } }
	private string? _MPNOTE;
	public string? MPNOTE { get => _MPNOTE; set { if (_MPNOTE != value) { _MPNOTE = value; NotifyPropertyChanged();} } }
	private DateTime? _MPDATP;
	public DateTime? MPDATP { get => _MPDATP; set { if (_MPDATP != value) { _MPDATP = value; NotifyPropertyChanged();} } }
	private string? _MPSOCR;
	public string? MPSOCR { get => _MPSOCR; set { if (_MPSOCR != value) { _MPSOCR = value; NotifyPropertyChanged();} } }
	private string? _MPREPA;
	public string? MPREPA { get => _MPREPA; set { if (_MPREPA != value) { _MPREPA = value; NotifyPropertyChanged();} } }
	private string? _MPSOCF;
	public string? MPSOCF { get => _MPSOCF; set { if (_MPSOCF != value) { _MPSOCF = value; NotifyPropertyChanged();} } }
	private string? _MPFASE;
	public string? MPFASE { get => _MPFASE; set { if (_MPFASE != value) { _MPFASE = value; NotifyPropertyChanged();} } }
	private int? _MPFORN;
	public int? MPFORN { get => _MPFORN; set { if (_MPFORN != value) { _MPFORN = value; NotifyPropertyChanged();} } }
	private string? _MPsoc1;
	public string? MPsoc1 { get => _MPsoc1; set { if (_MPsoc1 != value) { _MPsoc1 = value; NotifyPropertyChanged();} } }
	private string? _Mpsoc2;
	public string? Mpsoc2 { get => _Mpsoc2; set { if (_Mpsoc2 != value) { _Mpsoc2 = value; NotifyPropertyChanged();} } }
	private string? _mpsoc3;
	public string? mpsoc3 { get => _mpsoc3; set { if (_mpsoc3 != value) { _mpsoc3 = value; NotifyPropertyChanged();} } }
}