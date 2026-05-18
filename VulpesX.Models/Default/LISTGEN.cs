namespace VulpesX.Models.Default;
 
public partial class LISTGEN : Base 
{
	private string _lisoci = null!;
	public required string lisoci { get => _lisoci; set { if (_lisoci != value) { _lisoci = value; NotifyPropertyChanged();} } }
	private string _liarti = null!;
	public required string liarti { get => _liarti; set { if (_liarti != value) { _liarti = value; NotifyPropertyChanged();} } }
	private DateTime _lidatv;
	public DateTime lidatv { get => _lidatv; set { if (_lidatv != value) { _lidatv = value; NotifyPropertyChanged();} } }
	private string _litipo = null!;
	public required string litipo { get => _litipo; set { if (_litipo != value) { _litipo = value; NotifyPropertyChanged();} } }
	private decimal? _liprez;
	public decimal? liprez { get => _liprez; set { if (_liprez != value) { _liprez = value; NotifyPropertyChanged();} } }
	private decimal? _lisco1;
	public decimal? lisco1 { get => _lisco1; set { if (_lisco1 != value) { _lisco1 = value; NotifyPropertyChanged();} } }
	private decimal? _lisco2;
	public decimal? lisco2 { get => _lisco2; set { if (_lisco2 != value) { _lisco2 = value; NotifyPropertyChanged();} } }
	private decimal? _lisco3;
	public decimal? lisco3 { get => _lisco3; set { if (_lisco3 != value) { _lisco3 = value; NotifyPropertyChanged();} } }
	private string? _litsc1;
	public string? litsc1 { get => _litsc1; set { if (_litsc1 != value) { _litsc1 = value; NotifyPropertyChanged();} } }
	private string? _litsc2;
	public string? litsc2 { get => _litsc2; set { if (_litsc2 != value) { _litsc2 = value; NotifyPropertyChanged();} } }
	private string? _litsc3;
	public string? litsc3 { get => _litsc3; set { if (_litsc3 != value) { _litsc3 = value; NotifyPropertyChanged();} } }
	private decimal? _limagg;
	public decimal? limagg { get => _limagg; set { if (_limagg != value) { _limagg = value; NotifyPropertyChanged();} } }
	private string? _litmag;
	public string? litmag { get => _litmag; set { if (_litmag != value) { _litmag = value; NotifyPropertyChanged();} } }
	private decimal? _liperp;
	public decimal? liperp { get => _liperp; set { if (_liperp != value) { _liperp = value; NotifyPropertyChanged();} } }
	private decimal? _liprov;
	public decimal? liprov { get => _liprov; set { if (_liprov != value) { _liprov = value; NotifyPropertyChanged();} } }
}