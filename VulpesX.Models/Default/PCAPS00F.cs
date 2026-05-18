namespace VulpesX.Models.Default;
 
public partial class PCAPS00F : Base 
{
	private string _VSSOCI = null!;
	public required string VSSOCI { get => _VSSOCI; set { if (_VSSOCI != value) { _VSSOCI = value; NotifyPropertyChanged();} } }
	private int _VSVOCE;
	public int VSVOCE { get => _VSVOCE; set { if (_VSVOCE != value) { _VSVOCE = value; NotifyPropertyChanged();} } }
	private string? _VSTIPO;
	public string? VSTIPO { get => _VSTIPO; set { if (_VSTIPO != value) { _VSTIPO = value; NotifyPropertyChanged();} } }
	private string? _VSDESC;
	public string? VSDESC { get => _VSDESC; set { if (_VSDESC != value) { _VSDESC = value; NotifyPropertyChanged();} } }
	private int? _VSTO01;
	public int? VSTO01 { get => _VSTO01; set { if (_VSTO01 != value) { _VSTO01 = value; NotifyPropertyChanged();} } }
	private string? _VSSE01;
	public string? VSSE01 { get => _VSSE01; set { if (_VSSE01 != value) { _VSSE01 = value; NotifyPropertyChanged();} } }
	private int? _VSTO02;
	public int? VSTO02 { get => _VSTO02; set { if (_VSTO02 != value) { _VSTO02 = value; NotifyPropertyChanged();} } }
	private string? _VSSE02;
	public string? VSSE02 { get => _VSSE02; set { if (_VSSE02 != value) { _VSSE02 = value; NotifyPropertyChanged();} } }
	private int? _VSTO03;
	public int? VSTO03 { get => _VSTO03; set { if (_VSTO03 != value) { _VSTO03 = value; NotifyPropertyChanged();} } }
	private string? _VSSE03;
	public string? VSSE03 { get => _VSSE03; set { if (_VSSE03 != value) { _VSSE03 = value; NotifyPropertyChanged();} } }
}