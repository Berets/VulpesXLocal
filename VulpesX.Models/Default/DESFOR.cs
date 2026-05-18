namespace VulpesX.Models.Default;
 
public partial class DESFOR : Base 
{
	private int _fornicod;
	public int fornicod { get => _fornicod; set { if (_fornicod != value) { _fornicod = value; NotifyPropertyChanged();} } }
	private int _fodesti;
	public int fodesti { get => _fodesti; set { if (_fodesti != value) { _fodesti = value; NotifyPropertyChanged();} } }
	private string? _foragso;
	public string? foragso { get => _foragso; set { if (_foragso != value) { _foragso = value; NotifyPropertyChanged();} } }
	private string? _fodein;
	public string? fodein { get => _fodein; set { if (_fodein != value) { _fodein = value; NotifyPropertyChanged();} } }
	private int? _fodecap;
	public int? fodecap { get => _fodecap; set { if (_fodecap != value) { _fodecap = value; NotifyPropertyChanged();} } }
	private string? _fodeloc;
	public string? fodeloc { get => _fodeloc; set { if (_fodeloc != value) { _fodeloc = value; NotifyPropertyChanged();} } }
	private string? _fodepro;
	public string? fodepro { get => _fodepro; set { if (_fodepro != value) { _fodepro = value; NotifyPropertyChanged();} } }
	private string? _foperco;
	public string? foperco { get => _foperco; set { if (_foperco != value) { _foperco = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}