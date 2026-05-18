namespace VulpesX.Models.Default;
 
public partial class TDIPENDENTILEVEL1 : Base 
{
	private string _dtdisoc = null!;
	public required string dtdisoc { get => _dtdisoc; set { if (_dtdisoc != value) { _dtdisoc = value; NotifyPropertyChanged();} } }
	private string _dtdicod = null!;
	public required string dtdicod { get => _dtdicod; set { if (_dtdicod != value) { _dtdicod = value; NotifyPropertyChanged();} } }
	private string _dtdinuc = null!;
	public required string dtdinuc { get => _dtdinuc; set { if (_dtdinuc != value) { _dtdinuc = value; NotifyPropertyChanged();} } }
	private string? _dtdides;
	public string? dtdides { get => _dtdides; set { if (_dtdides != value) { _dtdides = value; NotifyPropertyChanged();} } }
	private string? _dtdengr;
	public string? dtdengr { get => _dtdengr; set { if (_dtdengr != value) { _dtdengr = value; NotifyPropertyChanged();} } }
	private string? _dtdenco;
	public string? dtdenco { get => _dtdenco; set { if (_dtdenco != value) { _dtdenco = value; NotifyPropertyChanged();} } }
	private string? _dtdenso;
	public string? dtdenso { get => _dtdenso; set { if (_dtdenso != value) { _dtdenso = value; NotifyPropertyChanged();} } }
}