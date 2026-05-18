namespace VulpesX.Models.Default;
 
public partial class TRNCOMDIP : Base 
{
	private string _tcsoc = null!;
	public required string tcsoc { get => _tcsoc; set { if (_tcsoc != value) { _tcsoc = value; NotifyPropertyChanged();} } }
	private int _tcann;
	public int tcann { get => _tcann; set { if (_tcann != value) { _tcann = value; NotifyPropertyChanged();} } }
	private string _tccom = null!;
	public required string tccom { get => _tccom; set { if (_tccom != value) { _tccom = value; NotifyPropertyChanged();} } }
	private string _tcmat = null!;
	public required string tcmat { get => _tcmat; set { if (_tcmat != value) { _tcmat = value; NotifyPropertyChanged();} } }
	private int? _tcpro;
	public int? tcpro { get => _tcpro; set { if (_tcpro != value) { _tcpro = value; NotifyPropertyChanged();} } }
	private decimal? _tcfatt;
	public decimal? tcfatt { get => _tcfatt; set { if (_tcfatt != value) { _tcfatt = value; NotifyPropertyChanged();} } }
	private decimal? _tccos;
	public decimal? tccos { get => _tccos; set { if (_tccos != value) { _tccos = value; NotifyPropertyChanged();} } }
	private decimal? _tcore;
	public decimal? tcore { get => _tcore; set { if (_tcore != value) { _tcore = value; NotifyPropertyChanged();} } }
}