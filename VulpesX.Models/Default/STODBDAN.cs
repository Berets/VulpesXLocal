namespace VulpesX.Models.Default;
 
public partial class STODBDAN : Base 
{
	private string _sdbpso = null!;
	public required string sdbpso { get => _sdbpso; set { if (_sdbpso != value) { _sdbpso = value; NotifyPropertyChanged();} } }
	private string _Sdbpco = null!;
	public required string Sdbpco { get => _Sdbpco; set { if (_Sdbpco != value) { _Sdbpco = value; NotifyPropertyChanged();} } }
	private int _sdbdpr;
	public int sdbdpr { get => _sdbdpr; set { if (_sdbdpr != value) { _sdbdpr = value; NotifyPropertyChanged();} } }
	private string? _Sdbdco;
	public string? Sdbdco { get => _Sdbdco; set { if (_Sdbdco != value) { _Sdbdco = value; NotifyPropertyChanged();} } }
	private decimal? _sdbdqt;
	public decimal? sdbdqt { get => _sdbdqt; set { if (_sdbdqt != value) { _sdbdqt = value; NotifyPropertyChanged();} } }
	private string? _straco;
	public string? straco { get => _straco; set { if (_straco != value) { _straco = value; NotifyPropertyChanged();} } }
	private string? _strade;
	public string? strade { get => _strade; set { if (_strade != value) { _strade = value; NotifyPropertyChanged();} } }
	private decimal? _sdbdpe;
	public decimal? sdbdpe { get => _sdbdpe; set { if (_sdbdpe != value) { _sdbdpe = value; NotifyPropertyChanged();} } }
	private string? _Sdbdat;
	public string? Sdbdat { get => _Sdbdat; set { if (_Sdbdat != value) { _Sdbdat = value; NotifyPropertyChanged();} } }
	private string? _sdbdut;
	public string? sdbdut { get => _sdbdut; set { if (_sdbdut != value) { _sdbdut = value; NotifyPropertyChanged();} } }
	private string? _sdbduv;
	public string? sdbduv { get => _sdbduv; set { if (_sdbduv != value) { _sdbduv = value; NotifyPropertyChanged();} } }
	private string? _Sdbdor;
	public string? Sdbdor { get => _Sdbdor; set { if (_Sdbdor != value) { _Sdbdor = value; NotifyPropertyChanged();} } }
	private string? _sdbdov;
	public string? sdbdov { get => _sdbdov; set { if (_sdbdov != value) { _sdbdov = value; NotifyPropertyChanged();} } }
	private string? _sdbdte;
	public string? sdbdte { get => _sdbdte; set { if (_sdbdte != value) { _sdbdte = value; NotifyPropertyChanged();} } }
	private DateTime? _sdbdai;
	public DateTime? sdbdai { get => _sdbdai; set { if (_sdbdai != value) { _sdbdai = value; NotifyPropertyChanged();} } }
	private DateTime? _sdbddf;
	public DateTime? sdbddf { get => _sdbddf; set { if (_sdbddf != value) { _sdbddf = value; NotifyPropertyChanged();} } }
	private string? _sdbdnd;
	public string? sdbdnd { get => _sdbdnd; set { if (_sdbdnd != value) { _sdbdnd = value; NotifyPropertyChanged();} } }
	private DateTime? _sdbdau;
	public DateTime? sdbdau { get => _sdbdau; set { if (_sdbdau != value) { _sdbdau = value; NotifyPropertyChanged();} } }
	private DateTime? _sdbdda;
	public DateTime? sdbdda { get => _sdbdda; set { if (_sdbdda != value) { _sdbdda = value; NotifyPropertyChanged();} } }
	private string? _sdbdfl;
	public string? sdbdfl { get => _sdbdfl; set { if (_sdbdfl != value) { _sdbdfl = value; NotifyPropertyChanged();} } }
}