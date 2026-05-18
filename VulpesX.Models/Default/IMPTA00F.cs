namespace VulpesX.Models.Default;
 
public partial class IMPTA00F : Base 
{
	private string _tasocb = null!;
	public required string tasocb { get => _tasocb; set { if (_tasocb != value) { _tasocb = value; NotifyPropertyChanged();} } }
	private int _taanno;
	public int taanno { get => _taanno; set { if (_taanno != value) { _taanno = value; NotifyPropertyChanged();} } }
	private decimal? _tairap;
	public decimal? tairap { get => _tairap; set { if (_tairap != value) { _tairap = value; NotifyPropertyChanged();} } }
	private decimal? _tairpe;
	public decimal? tairpe { get => _tairpe; set { if (_tairpe != value) { _tairpe = value; NotifyPropertyChanged();} } }
	private decimal? _taiira;
	public decimal? taiira { get => _taiira; set { if (_taiira != value) { _taiira = value; NotifyPropertyChanged();} } }
	private decimal? _taeira;
	public decimal? taeira { get => _taeira; set { if (_taeira != value) { _taeira = value; NotifyPropertyChanged();} } }
	private decimal? _taipra;
	public decimal? taipra { get => _taipra; set { if (_taipra != value) { _taipra = value; NotifyPropertyChanged();} } }
	private decimal? _taepra;
	public decimal? taepra { get => _taepra; set { if (_taepra != value) { _taepra = value; NotifyPropertyChanged();} } }
	private decimal? _taiirp;
	public decimal? taiirp { get => _taiirp; set { if (_taiirp != value) { _taiirp = value; NotifyPropertyChanged();} } }
	private decimal? _taeirp;
	public decimal? taeirp { get => _taeirp; set { if (_taeirp != value) { _taeirp = value; NotifyPropertyChanged();} } }
	private decimal? _taipep;
	public decimal? taipep { get => _taipep; set { if (_taipep != value) { _taipep = value; NotifyPropertyChanged();} } }
	private decimal? _taepep;
	public decimal? taepep { get => _taepep; set { if (_taepep != value) { _taepep = value; NotifyPropertyChanged();} } }
	private string? _tatiso;
	public string? tatiso { get => _tatiso; set { if (_tatiso != value) { _tatiso = value; NotifyPropertyChanged();} } }
	private int? _tataso;
	public int? tataso { get => _tataso; set { if (_tataso != value) { _tataso = value; NotifyPropertyChanged();} } }
	private string? _tatis1;
	public string? tatis1 { get => _tatis1; set { if (_tatis1 != value) { _tatis1 = value; NotifyPropertyChanged();} } }
	private int? _tatas1;
	public int? tatas1 { get => _tatas1; set { if (_tatas1 != value) { _tatas1 = value; NotifyPropertyChanged();} } }
}