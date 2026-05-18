namespace VulpesX.Models.Default;
 
public partial class CAUCOMMESSA : Base 
{
	private string _ccosoc = null!;
	public required string ccosoc { get => _ccosoc; set { if (_ccosoc != value) { _ccosoc = value; NotifyPropertyChanged();} } }
	private string _ccocau = null!;
	public required string ccocau { get => _ccocau; set { if (_ccocau != value) { _ccocau = value; NotifyPropertyChanged();} } }
	private string? _ccodes;
	public string? ccodes { get => _ccodes; set { if (_ccodes != value) { _ccodes = value; NotifyPropertyChanged();} } }
	private string? _ccomov;
	public string? ccomov { get => _ccomov; set { if (_ccomov != value) { _ccomov = value; NotifyPropertyChanged();} } }
	private string? _ccoseg;
	public string? ccoseg { get => _ccoseg; set { if (_ccoseg != value) { _ccoseg = value; NotifyPropertyChanged();} } }
	private string? _ccocon;
	public string? ccocon { get => _ccocon; set { if (_ccocon != value) { _ccocon = value; NotifyPropertyChanged();} } }
	private string? _ccotca;
	public string? ccotca { get => _ccotca; set { if (_ccotca != value) { _ccotca = value; NotifyPropertyChanged();} } }
}