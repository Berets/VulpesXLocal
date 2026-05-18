namespace VulpesX.Models.Default;
 
public partial class LOGTB00F : Base 
{
	private string _AZCode = null!;
	public required string AZCode { get => _AZCode; set { if (_AZCode != value) { _AZCode = value; NotifyPropertyChanged();} } }
	private string _infcod = null!;
	public required string infcod { get => _infcod; set { if (_infcod != value) { _infcod = value; NotifyPropertyChanged();} } }
	private string _inftip = null!;
	public required string inftip { get => _inftip; set { if (_inftip != value) { _inftip = value; NotifyPropertyChanged();} } }
	private DateTime _logora;
	public DateTime logora { get => _logora; set { if (_logora != value) { _logora = value; NotifyPropertyChanged();} } }
	private string? _logdoc;
	public string? logdoc { get => _logdoc; set { if (_logdoc != value) { _logdoc = value; NotifyPropertyChanged();} } }
	private string? _logema;
	public string? logema { get => _logema; set { if (_logema != value) { _logema = value; NotifyPropertyChanged();} } }
	private string? _logmod;
	public string? logmod { get => _logmod; set { if (_logmod != value) { _logmod = value; NotifyPropertyChanged();} } }
	private string? _logcon;
	public string? logcon { get => _logcon; set { if (_logcon != value) { _logcon = value; NotifyPropertyChanged();} } }
	private string? _logute;
	public string? logute { get => _logute; set { if (_logute != value) { _logute = value; NotifyPropertyChanged();} } }
	private string? _logcau;
	public string? logcau { get => _logcau; set { if (_logcau != value) { _logcau = value; NotifyPropertyChanged();} } }
	private DateTime? _logric;
	public DateTime? logric { get => _logric; set { if (_logric != value) { _logric = value; NotifyPropertyChanged();} } }
}