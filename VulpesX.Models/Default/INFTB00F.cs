namespace VulpesX.Models.Default;
 
public partial class INFTB00F : Base 
{
	private string _AZCode = null!;
	public required string AZCode { get => _AZCode; set { if (_AZCode != value) { _AZCode = value; NotifyPropertyChanged();} } }
	private string _inftip = null!;
	public required string inftip { get => _inftip; set { if (_inftip != value) { _inftip = value; NotifyPropertyChanged();} } }
	private string _infcod = null!;
	public required string infcod { get => _infcod; set { if (_infcod != value) { _infcod = value; NotifyPropertyChanged();} } }
	private string? _infrag;
	public string? infrag { get => _infrag; set { if (_infrag != value) { _infrag = value; NotifyPropertyChanged();} } }
	private string? _infagg;
	public string? infagg { get => _infagg; set { if (_infagg != value) { _infagg = value; NotifyPropertyChanged();} } }
	private string? _infind;
	public string? infind { get => _infind; set { if (_infind != value) { _infind = value; NotifyPropertyChanged();} } }
	private string? _infloc;
	public string? infloc { get => _infloc; set { if (_infloc != value) { _infloc = value; NotifyPropertyChanged();} } }
	private int? _infcap;
	public int? infcap { get => _infcap; set { if (_infcap != value) { _infcap = value; NotifyPropertyChanged();} } }
	private string? _infpro;
	public string? infpro { get => _infpro; set { if (_infpro != value) { _infpro = value; NotifyPropertyChanged();} } }
	private string? _infnaz;
	public string? infnaz { get => _infnaz; set { if (_infnaz != value) { _infnaz = value; NotifyPropertyChanged();} } }
	private string? _infiva;
	public string? infiva { get => _infiva; set { if (_infiva != value) { _infiva = value; NotifyPropertyChanged();} } }
	private string? _inffis;
	public string? inffis { get => _inffis; set { if (_inffis != value) { _inffis = value; NotifyPropertyChanged();} } }
	private string? _infcat;
	public string? infcat { get => _infcat; set { if (_infcat != value) { _infcat = value; NotifyPropertyChanged();} } }
	private string? _infema;
	public string? infema { get => _infema; set { if (_infema != value) { _infema = value; NotifyPropertyChanged();} } }
	private string? _inftel;
	public string? inftel { get => _inftel; set { if (_inftel != value) { _inftel = value; NotifyPropertyChanged();} } }
	private string? _inffax;
	public string? inffax { get => _inffax; set { if (_inffax != value) { _inffax = value; NotifyPropertyChanged();} } }
	private string? _infdoc;
	public string? infdoc { get => _infdoc; set { if (_infdoc != value) { _infdoc = value; NotifyPropertyChanged();} } }
	private string? _infpth;
	public string? infpth { get => _infpth; set { if (_infpth != value) { _infpth = value; NotifyPropertyChanged();} } }
	private DateTime? _infdat;
	public DateTime? infdat { get => _infdat; set { if (_infdat != value) { _infdat = value; NotifyPropertyChanged();} } }
	private int? _inftpo;
	public int? inftpo { get => _inftpo; set { if (_inftpo != value) { _inftpo = value; NotifyPropertyChanged();} } }
	private string? _infcon;
	public string? infcon { get => _infcon; set { if (_infcon != value) { _infcon = value; NotifyPropertyChanged();} } }
	private string? _infmod;
	public string? infmod { get => _infmod; set { if (_infmod != value) { _infmod = value; NotifyPropertyChanged();} } }
	private DateTime? _infric;
	public DateTime? infric { get => _infric; set { if (_infric != value) { _infric = value; NotifyPropertyChanged();} } }
	private string? _infute;
	public string? infute { get => _infute; set { if (_infute != value) { _infute = value; NotifyPropertyChanged();} } }
	private string? _infinv;
	public string? infinv { get => _infinv; set { if (_infinv != value) { _infinv = value; NotifyPropertyChanged();} } }
}