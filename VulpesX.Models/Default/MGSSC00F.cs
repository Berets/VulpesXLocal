namespace VulpesX.Models.Default;
 
public partial class MGSSC00F : Base 
{
	private string _ARTCOD = null!;
	public required string ARTCOD { get => _ARTCOD; set { if (_ARTCOD != value) { _ARTCOD = value; NotifyPropertyChanged();} } }
	private int _sscprg;
	public int sscprg { get => _sscprg; set { if (_sscprg != value) { _sscprg = value; NotifyPropertyChanged();} } }
	private string? _ssccod;
	public string? ssccod { get => _ssccod; set { if (_ssccod != value) { _ssccod = value; NotifyPropertyChanged();} } }
	private decimal? _sscqta;
	public decimal? sscqta { get => _sscqta; set { if (_sscqta != value) { _sscqta = value; NotifyPropertyChanged();} } }
	private DateTime? _sscdai;
	public DateTime? sscdai { get => _sscdai; set { if (_sscdai != value) { _sscdai = value; NotifyPropertyChanged();} } }
	private DateTime? _sscdaf;
	public DateTime? sscdaf { get => _sscdaf; set { if (_sscdaf != value) { _sscdaf = value; NotifyPropertyChanged();} } }
	private string? _sscndo;
	public string? sscndo { get => _sscndo; set { if (_sscndo != value) { _sscndo = value; NotifyPropertyChanged();} } }
	private DateTime? _sscddo;
	public DateTime? sscddo { get => _sscddo; set { if (_sscddo != value) { _sscddo = value; NotifyPropertyChanged();} } }
	private int? _sscpri;
	public int? sscpri { get => _sscpri; set { if (_sscpri != value) { _sscpri = value; NotifyPropertyChanged();} } }
	private string? _sscdes;
	public string? sscdes { get => _sscdes; set { if (_sscdes != value) { _sscdes = value; NotifyPropertyChanged();} } }
}