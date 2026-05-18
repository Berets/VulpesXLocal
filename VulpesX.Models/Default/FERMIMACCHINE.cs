namespace VulpesX.Models.Default;
 
public partial class FERMIMACCHINE : Base 
{
	private string _macsoc = null!;
	public required string macsoc { get => _macsoc; set { if (_macsoc != value) { _macsoc = value; NotifyPropertyChanged();} } }
	private string _maccod = null!;
	public required string maccod { get => _maccod; set { if (_maccod != value) { _maccod = value; NotifyPropertyChanged();} } }
	private DateTime _macdaf;
	public DateTime macdaf { get => _macdaf; set { if (_macdaf != value) { _macdaf = value; NotifyPropertyChanged();} } }
	private string? _cfmcod;
	public string? cfmcod { get => _cfmcod; set { if (_cfmcod != value) { _cfmcod = value; NotifyPropertyChanged();} } }
	private string? _macdfm;
	public string? macdfm { get => _macdfm; set { if (_macdfm != value) { _macdfm = value; NotifyPropertyChanged();} } }
	private decimal? _macofm;
	public decimal? macofm { get => _macofm; set { if (_macofm != value) { _macofm = value; NotifyPropertyChanged();} } }
	private decimal? _mactdi;
	public decimal? mactdi { get => _mactdi; set { if (_mactdi != value) { _mactdi = value; NotifyPropertyChanged();} } }
	private int? _macnua;
	public int? macnua { get => _macnua; set { if (_macnua != value) { _macnua = value; NotifyPropertyChanged();} } }
	private int? _macnma;
	public int? macnma { get => _macnma; set { if (_macnma != value) { _macnma = value; NotifyPropertyChanged();} } }
}