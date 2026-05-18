namespace VulpesX.Models.Default;
 
public partial class TARUD00F : Base 
{
	private string _tasoc = null!;
	public required string tasoc { get => _tasoc; set { if (_tasoc != value) { _tasoc = value; NotifyPropertyChanged();} } }
	private int _tarigr;
	public int tarigr { get => _tarigr; set { if (_tarigr != value) { _tarigr = value; NotifyPropertyChanged();} } }
	private string? _tatipo;
	public string? tatipo { get => _tatipo; set { if (_tatipo != value) { _tatipo = value; NotifyPropertyChanged();} } }
	private string? _tadesc;
	public string? tadesc { get => _tadesc; set { if (_tadesc != value) { _tadesc = value; NotifyPropertyChanged();} } }
	private int? _tato01;
	public int? tato01 { get => _tato01; set { if (_tato01 != value) { _tato01 = value; NotifyPropertyChanged();} } }
	private string? _tase01;
	public string? tase01 { get => _tase01; set { if (_tase01 != value) { _tase01 = value; NotifyPropertyChanged();} } }
	private int? _tato02;
	public int? tato02 { get => _tato02; set { if (_tato02 != value) { _tato02 = value; NotifyPropertyChanged();} } }
	private string? _tase02;
	public string? tase02 { get => _tase02; set { if (_tase02 != value) { _tase02 = value; NotifyPropertyChanged();} } }
	private int? _tato03;
	public int? tato03 { get => _tato03; set { if (_tato03 != value) { _tato03 = value; NotifyPropertyChanged();} } }
	private string? _tase03;
	public string? tase03 { get => _tase03; set { if (_tase03 != value) { _tase03 = value; NotifyPropertyChanged();} } }
	private int? _tato04;
	public int? tato04 { get => _tato04; set { if (_tato04 != value) { _tato04 = value; NotifyPropertyChanged();} } }
	private string? _tase04;
	public string? tase04 { get => _tase04; set { if (_tase04 != value) { _tase04 = value; NotifyPropertyChanged();} } }
	private int? _tato05;
	public int? tato05 { get => _tato05; set { if (_tato05 != value) { _tato05 = value; NotifyPropertyChanged();} } }
	private string? _tase05;
	public string? tase05 { get => _tase05; set { if (_tase05 != value) { _tase05 = value; NotifyPropertyChanged();} } }
}