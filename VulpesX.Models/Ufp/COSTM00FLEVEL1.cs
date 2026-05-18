namespace VulpesX.Models.Ufp;
using VulpesX.Shared;
public partial class COSTM00FLEVEL1 : Base 
{
	private string _cmsoci = null!;
	public required string cmsoci { get => _cmsoci; set { if (_cmsoci != value) { _cmsoci = value; NotifyPropertyChanged();} } }
	private string _CMARTI = null!;
	public required string CMARTI { get => _CMARTI; set { if (_CMARTI != value) { _CMARTI = value; NotifyPropertyChanged();} } }
	private int _CMANNO;
	public int CMANNO { get => _CMANNO; set { if (_CMANNO != value) { _CMANNO = value; NotifyPropertyChanged();} } }
	private int _CMMESE;
	public int CMMESE { get => _CMMESE; set { if (_CMMESE != value) { _CMMESE = value; NotifyPropertyChanged();} } }
	private int _CMPROG;
	public int CMPROG { get => _CMPROG; set { if (_CMPROG != value) { _CMPROG = value; NotifyPropertyChanged();} } }
	private int _CMANMA;
	public int CMANMA { get => _CMANMA; set { if (_CMANMA != value) { _CMANMA = value; NotifyPropertyChanged();} } }
	private int _CMNUMA;
	public int CMNUMA { get => _CMNUMA; set { if (_CMNUMA != value) { _CMNUMA = value; NotifyPropertyChanged();} } }
	private string _CMTIPO = null!;
	public required string CMTIPO { get => _CMTIPO; set { if (_CMTIPO != value) { _CMTIPO = value; NotifyPropertyChanged();} } }
	private decimal _CMQTA;
	public decimal CMQTA { get => _CMQTA; set { if (_CMQTA != value) { _CMQTA = value; NotifyPropertyChanged();} } }
	private decimal _CMPREZ;
	public decimal CMPREZ { get => _CMPREZ; set { if (_CMPREZ != value) { _CMPREZ = value; NotifyPropertyChanged();} } }
	private decimal _CMSCO1;
	public decimal CMSCO1 { get => _CMSCO1; set { if (_CMSCO1 != value) { _CMSCO1 = value; NotifyPropertyChanged();} } }
	private decimal _CMSCO2;
	public decimal CMSCO2 { get => _CMSCO2; set { if (_CMSCO2 != value) { _CMSCO2 = value; NotifyPropertyChanged();} } }
	private decimal _CMSCO3;
	public decimal CMSCO3 { get => _CMSCO3; set { if (_CMSCO3 != value) { _CMSCO3 = value; NotifyPropertyChanged();} } }
}