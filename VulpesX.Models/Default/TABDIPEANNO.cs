namespace VulpesX.Models.Default;
 
public partial class TABDIPEANNO : Base 
{
	private string _AmsTdiCod = null!;
	public required string AmsTdiCod { get => _AmsTdiCod; set { if (_AmsTdiCod != value) { _AmsTdiCod = value; NotifyPropertyChanged();} } }
	private int _AmsTdiAnn;
	public int AmsTdiAnn { get => _AmsTdiAnn; set { if (_AmsTdiAnn != value) { _AmsTdiAnn = value; NotifyPropertyChanged();} } }
	private int _AmsTdiMes;
	public int AmsTdiMes { get => _AmsTdiMes; set { if (_AmsTdiMes != value) { _AmsTdiMes = value; NotifyPropertyChanged();} } }
	private decimal? _AmsCosOcd;
	public decimal? AmsCosOcd { get => _AmsCosOcd; set { if (_AmsCosOcd != value) { _AmsCosOcd = value; NotifyPropertyChanged();} } }
	private int? _AmsNorMst;
	public int? AmsNorMst { get => _AmsNorMst; set { if (_AmsNorMst != value) { _AmsNorMst = value; NotifyPropertyChanged();} } }
	private decimal? _AmsImpMNs;
	public decimal? AmsImpMNs { get => _AmsImpMNs; set { if (_AmsImpMNs != value) { _AmsImpMNs = value; NotifyPropertyChanged();} } }
	private int? _AmsNorMse;
	public int? AmsNorMse { get => _AmsNorMse; set { if (_AmsNorMse != value) { _AmsNorMse = value; NotifyPropertyChanged();} } }
	private decimal? _AmsCosOms;
	public decimal? AmsCosOms { get => _AmsCosOms; set { if (_AmsCosOms != value) { _AmsCosOms = value; NotifyPropertyChanged();} } }
	private decimal? _AmsCodOef;
	public decimal? AmsCodOef { get => _AmsCodOef; set { if (_AmsCodOef != value) { _AmsCodOef = value; NotifyPropertyChanged();} } }
	private string? _AmsTdiCnm;
	public string? AmsTdiCnm { get => _AmsTdiCnm; set { if (_AmsTdiCnm != value) { _AmsTdiCnm = value; NotifyPropertyChanged();} } }
}