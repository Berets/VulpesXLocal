namespace VulpesX.Models.Default;
 
public partial class FCAST00F : Base 
{
	private string _PVSOCI = null!;
	public required string PVSOCI { get => _PVSOCI; set { if (_PVSOCI != value) { _PVSOCI = value; NotifyPropertyChanged();} } }
	private string _PVCOAR = null!;
	public required string PVCOAR { get => _PVCOAR; set { if (_PVCOAR != value) { _PVCOAR = value; NotifyPropertyChanged();} } }
	private int _PVCOCL;
	public int PVCOCL { get => _PVCOCL; set { if (_PVCOCL != value) { _PVCOCL = value; NotifyPropertyChanged();} } }
	private int _PVANNO;
	public int PVANNO { get => _PVANNO; set { if (_PVANNO != value) { _PVANNO = value; NotifyPropertyChanged();} } }
	private int _PVMESE;
	public int PVMESE { get => _PVMESE; set { if (_PVMESE != value) { _PVMESE = value; NotifyPropertyChanged();} } }
	private decimal? _PVQSIM;
	public decimal? PVQSIM { get => _PVQSIM; set { if (_PVQSIM != value) { _PVQSIM = value; NotifyPropertyChanged();} } }
	private string? _PVTIPO;
	public string? PVTIPO { get => _PVTIPO; set { if (_PVTIPO != value) { _PVTIPO = value; NotifyPropertyChanged();} } }
	private string? _PVFIL1;
	public string? PVFIL1 { get => _PVFIL1; set { if (_PVFIL1 != value) { _PVFIL1 = value; NotifyPropertyChanged();} } }
}