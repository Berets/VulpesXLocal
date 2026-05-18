namespace VulpesX.Models.Default;
 
public partial class AZIENDA_LINGUA : Base 
{
	private string _AZCode = null!;
	public required string AZCode { get => _AZCode; set { if (_AZCode != value) { _AZCode = value; NotifyPropertyChanged();} } }
	private string _lincod = null!;
	public required string lincod { get => _lincod; set { if (_lincod != value) { _lincod = value; NotifyPropertyChanged();} } }
	private string? _azoffgtex;
	public string? azoffgtex { get => _azoffgtex; set { if (_azoffgtex != value) { _azoffgtex = value; NotifyPropertyChanged();} } }
	private string? _azoffogg;
	public string? azoffogg { get => _azoffogg; set { if (_azoffogg != value) { _azoffogg = value; NotifyPropertyChanged();} } }
	private string? _azofftex;
	public string? azofftex { get => _azofftex; set { if (_azofftex != value) { _azofftex = value; NotifyPropertyChanged();} } }
	private string? _azordgtex;
	public string? azordgtex { get => _azordgtex; set { if (_azordgtex != value) { _azordgtex = value; NotifyPropertyChanged();} } }
	private string? _azordogg;
	public string? azordogg { get => _azordogg; set { if (_azordogg != value) { _azordogg = value; NotifyPropertyChanged();} } }
	private string? _azordtex;
	public string? azordtex { get => _azordtex; set { if (_azordtex != value) { _azordtex = value; NotifyPropertyChanged();} } }
	private string? _azddtgtex;
	public string? azddtgtex { get => _azddtgtex; set { if (_azddtgtex != value) { _azddtgtex = value; NotifyPropertyChanged();} } }
	private string? _azddtogg;
	public string? azddtogg { get => _azddtogg; set { if (_azddtogg != value) { _azddtogg = value; NotifyPropertyChanged();} } }
	private string? _azddttex;
	public string? azddttex { get => _azddttex; set { if (_azddttex != value) { _azddttex = value; NotifyPropertyChanged();} } }
	private string? _azinvgtex;
	public string? azinvgtex { get => _azinvgtex; set { if (_azinvgtex != value) { _azinvgtex = value; NotifyPropertyChanged();} } }
	private string? _azinvogg;
	public string? azinvogg { get => _azinvogg; set { if (_azinvogg != value) { _azinvogg = value; NotifyPropertyChanged();} } }
	private string? _azinvtex;
	public string? azinvtex { get => _azinvtex; set { if (_azinvtex != value) { _azinvtex = value; NotifyPropertyChanged();} } }
	private string? _azacqgtex;
	public string? azacqgtex { get => _azacqgtex; set { if (_azacqgtex != value) { _azacqgtex = value; NotifyPropertyChanged();} } }
	private string? _azbuyogg;
	public string? azbuyogg { get => _azbuyogg; set { if (_azbuyogg != value) { _azbuyogg = value; NotifyPropertyChanged();} } }
	private string? _azbuytex;
	public string? azbuytex { get => _azbuytex; set { if (_azbuytex != value) { _azbuytex = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}