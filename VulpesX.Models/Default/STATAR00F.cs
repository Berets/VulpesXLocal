namespace VulpesX.Models.Default;
 
public partial class STATAR00F : Base 
{
	private string _kasoci = null!;
	public required string kasoci { get => _kasoci; set { if (_kasoci != value) { _kasoci = value; NotifyPropertyChanged();} } }
	private string _kaarti = null!;
	public required string kaarti { get => _kaarti; set { if (_kaarti != value) { _kaarti = value; NotifyPropertyChanged();} } }
	private int _kaanno;
	public int kaanno { get => _kaanno; set { if (_kaanno != value) { _kaanno = value; NotifyPropertyChanged();} } }
	private int _kamese;
	public int kamese { get => _kamese; set { if (_kamese != value) { _kamese = value; NotifyPropertyChanged();} } }
	private string? _kaarea;
	public string? kaarea { get => _kaarea; set { if (_kaarea != value) { _kaarea = value; NotifyPropertyChanged();} } }
	private string? _kazona;
	public string? kazona { get => _kazona; set { if (_kazona != value) { _kazona = value; NotifyPropertyChanged();} } }
	private string? _kacatm;
	public string? kacatm { get => _kacatm; set { if (_kacatm != value) { _kacatm = value; NotifyPropertyChanged();} } }
	private decimal? _kaqtaa;
	public decimal? kaqtaa { get => _kaqtaa; set { if (_kaqtaa != value) { _kaqtaa = value; NotifyPropertyChanged();} } }
	private decimal? _kavalut;
	public decimal? kavalut { get => _kavalut; set { if (_kavalut != value) { _kavalut = value; NotifyPropertyChanged();} } }
	private decimal? _kasco1;
	public decimal? kasco1 { get => _kasco1; set { if (_kasco1 != value) { _kasco1 = value; NotifyPropertyChanged();} } }
	private int? _kasco2;
	public int? kasco2 { get => _kasco2; set { if (_kasco2 != value) { _kasco2 = value; NotifyPropertyChanged();} } }
	private decimal? _kasco3;
	public decimal? kasco3 { get => _kasco3; set { if (_kasco3 != value) { _kasco3 = value; NotifyPropertyChanged();} } }
	private decimal? _kamagg;
	public decimal? kamagg { get => _kamagg; set { if (_kamagg != value) { _kamagg = value; NotifyPropertyChanged();} } }
	private decimal? _kamarg;
	public decimal? kamarg { get => _kamarg; set { if (_kamarg != value) { _kamarg = value; NotifyPropertyChanged();} } }
	private decimal? _kavaln;
	public decimal? kavaln { get => _kavaln; set { if (_kavaln != value) { _kavaln = value; NotifyPropertyChanged();} } }
	private decimal? _kaprme;
	public decimal? kaprme { get => _kaprme; set { if (_kaprme != value) { _kaprme = value; NotifyPropertyChanged();} } }
	private decimal? _kascom;
	public decimal? kascom { get => _kascom; set { if (_kascom != value) { _kascom = value; NotifyPropertyChanged();} } }
	private decimal? _kacost;
	public decimal? kacost { get => _kacost; set { if (_kacost != value) { _kacost = value; NotifyPropertyChanged();} } }
	private decimal? _kaspes;
	public decimal? kaspes { get => _kaspes; set { if (_kaspes != value) { _kaspes = value; NotifyPropertyChanged();} } }
	private decimal? _kapera;
	public decimal? kapera { get => _kapera; set { if (_kapera != value) { _kapera = value; NotifyPropertyChanged();} } }
}