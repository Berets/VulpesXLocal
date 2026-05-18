namespace VulpesX.Models.Default;
 
public partial class TCECO00F : Base 
{
	private string _cecodc = null!;
	public required string cecodc { get => _cecodc; set { if (_cecodc != value) { _cecodc = value; NotifyPropertyChanged();} } }
	private string? _cedesc;
	public string? cedesc { get => _cedesc; set { if (_cedesc != value) { _cedesc = value; NotifyPropertyChanged();} } }
	private string? _cetipo;
	public string? cetipo { get => _cetipo; set { if (_cetipo != value) { _cetipo = value; NotifyPropertyChanged();} } }
	private decimal? _ceorla;
	public decimal? ceorla { get => _ceorla; set { if (_ceorla != value) { _ceorla = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
	private string? _cesoci;
	public string? cesoci { get => _cesoci; set { if (_cesoci != value) { _cesoci = value; NotifyPropertyChanged();} } }
}