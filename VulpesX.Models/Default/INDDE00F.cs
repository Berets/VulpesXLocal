namespace VulpesX.Models.Default;
 
public partial class INDDE00F : Base 
{
	private string _ANASOC = null!;
	public required string ANASOC { get => _ANASOC; set { if (_ANASOC != value) { _ANASOC = value; NotifyPropertyChanged();} } }
	private int _ANACOD;
	public int ANACOD { get => _ANACOD; set { if (_ANACOD != value) { _ANACOD = value; NotifyPropertyChanged();} } }
	private int _ADAPOS;
	public int ADAPOS { get => _ADAPOS; set { if (_ADAPOS != value) { _ADAPOS = value; NotifyPropertyChanged();} } }
	private string? _ADATIP;
	public string? ADATIP { get => _ADATIP; set { if (_ADATIP != value) { _ADATIP = value; NotifyPropertyChanged();} } }
	private string? _ADATIB;
	public string? ADATIB { get => _ADATIB; set { if (_ADATIB != value) { _ADATIB = value; NotifyPropertyChanged();} } }
	private int? _ADACOB;
	public int? ADACOB { get => _ADACOB; set { if (_ADACOB != value) { _ADACOB = value; NotifyPropertyChanged();} } }
	private string? _ADAOPE;
	public string? ADAOPE { get => _ADAOPE; set { if (_ADAOPE != value) { _ADAOPE = value; NotifyPropertyChanged();} } }
	private decimal? _ANACOS;
	public decimal? ANACOS { get => _ANACOS; set { if (_ANACOS != value) { _ANACOS = value; NotifyPropertyChanged();} } }
	private string? _adaso1;
	public string? adaso1 { get => _adaso1; set { if (_adaso1 != value) { _adaso1 = value; NotifyPropertyChanged();} } }
	private int? _ADAFILCOD;
	public int? ADAFILCOD { get => _ADAFILCOD; set { if (_ADAFILCOD != value) { _ADAFILCOD = value; NotifyPropertyChanged();} } }
	private int? _ADAANNO;
	public int? ADAANNO { get => _ADAANNO; set { if (_ADAANNO != value) { _ADAANNO = value; NotifyPropertyChanged();} } }
}