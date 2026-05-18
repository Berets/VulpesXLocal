namespace VulpesX.Models.Default;
 
public partial class TABDIPE1 : Base 
{
	private string _tdicod = null!;
	public required string tdicod { get => _tdicod; set { if (_tdicod != value) { _tdicod = value; NotifyPropertyChanged();} } }
	private string _tdinuc = null!;
	public required string tdinuc { get => _tdinuc; set { if (_tdinuc != value) { _tdinuc = value; NotifyPropertyChanged();} } }
	private string? _tdides;
	public string? tdides { get => _tdides; set { if (_tdides != value) { _tdides = value; NotifyPropertyChanged();} } }
	private string? _tdengr;
	public string? tdengr { get => _tdengr; set { if (_tdengr != value) { _tdengr = value; NotifyPropertyChanged();} } }
	private string? _tdenco;
	public string? tdenco { get => _tdenco; set { if (_tdenco != value) { _tdenco = value; NotifyPropertyChanged();} } }
	private string? _tdenso;
	public string? tdenso { get => _tdenso; set { if (_tdenso != value) { _tdenso = value; NotifyPropertyChanged();} } }
}