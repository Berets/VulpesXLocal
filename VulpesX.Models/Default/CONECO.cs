namespace VulpesX.Models.Default;
 
public partial class CONECO : Base 
{
	private string _ecosoc = null!;
	public required string ecosoc { get => _ecosoc; set { if (_ecosoc != value) { _ecosoc = value; NotifyPropertyChanged();} } }
	private int _ecocod;
	public int ecocod { get => _ecocod; set { if (_ecocod != value) { _ecocod = value; NotifyPropertyChanged();} } }
	private string? _ecodes;
	public string? ecodes { get => _ecodes; set { if (_ecodes != value) { _ecodes = value; NotifyPropertyChanged();} } }
	private int? _ecotac;
	public int? ecotac { get => _ecotac; set { if (_ecotac != value) { _ecotac = value; NotifyPropertyChanged();} } }
	private decimal? _ecoprl;
	public decimal? ecoprl { get => _ecoprl; set { if (_ecoprl != value) { _ecoprl = value; NotifyPropertyChanged();} } }
	private decimal? _ecopre;
	public decimal? ecopre { get => _ecopre; set { if (_ecopre != value) { _ecopre = value; NotifyPropertyChanged();} } }
	private decimal? _ecopel;
	public decimal? ecopel { get => _ecopel; set { if (_ecopel != value) { _ecopel = value; NotifyPropertyChanged();} } }
	private decimal? _ecopee;
	public decimal? ecopee { get => _ecopee; set { if (_ecopee != value) { _ecopee = value; NotifyPropertyChanged();} } }
	private decimal? _ecofil;
	public decimal? ecofil { get => _ecofil; set { if (_ecofil != value) { _ecofil = value; NotifyPropertyChanged();} } }
	private decimal? _ecofie;
	public decimal? ecofie { get => _ecofie; set { if (_ecofie != value) { _ecofie = value; NotifyPropertyChanged();} } }
	private string? _ecotot;
	public string? ecotot { get => _ecotot; set { if (_ecotot != value) { _ecotot = value; NotifyPropertyChanged();} } }
	private decimal? _ecop01;
	public decimal? ecop01 { get => _ecop01; set { if (_ecop01 != value) { _ecop01 = value; NotifyPropertyChanged();} } }
	private decimal? _ecop02;
	public decimal? ecop02 { get => _ecop02; set { if (_ecop02 != value) { _ecop02 = value; NotifyPropertyChanged();} } }
	private decimal? _ecop03;
	public decimal? ecop03 { get => _ecop03; set { if (_ecop03 != value) { _ecop03 = value; NotifyPropertyChanged();} } }
	private int? _ecotas;
	public int? ecotas { get => _ecotas; set { if (_ecotas != value) { _ecotas = value; NotifyPropertyChanged();} } }
	private decimal? _ecobul;
	public decimal? ecobul { get => _ecobul; set { if (_ecobul != value) { _ecobul = value; NotifyPropertyChanged();} } }
	private decimal? _ecobue;
	public decimal? ecobue { get => _ecobue; set { if (_ecobue != value) { _ecobue = value; NotifyPropertyChanged();} } }
	private decimal? _ecop04;
	public decimal? ecop04 { get => _ecop04; set { if (_ecop04 != value) { _ecop04 = value; NotifyPropertyChanged();} } }
	private decimal? _ecop05;
	public decimal? ecop05 { get => _ecop05; set { if (_ecop05 != value) { _ecop05 = value; NotifyPropertyChanged();} } }
}