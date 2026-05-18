namespace VulpesX.Models.Default;
 
public partial class RFFTB00F : Base 
{
	private int _FOCLIF;
	public int FOCLIF { get => _FOCLIF; set { if (_FOCLIF != value) { _FOCLIF = value; NotifyPropertyChanged();} } }
	private int _rffrig;
	public int rffrig { get => _rffrig; set { if (_rffrig != value) { _rffrig = value; NotifyPropertyChanged();} } }
	private string _rffcgn = null!;
	public required string rffcgn { get => _rffcgn; set { if (_rffcgn != value) { _rffcgn = value; NotifyPropertyChanged();} } }
	private string _rffnom = null!;
	public required string rffnom { get => _rffnom; set { if (_rffnom != value) { _rffnom = value; NotifyPropertyChanged();} } }
	private string _rffqal = null!;
	public required string rffqal { get => _rffqal; set { if (_rffqal != value) { _rffqal = value; NotifyPropertyChanged();} } }
	private string? _rfftel;
	public string? rfftel { get => _rfftel; set { if (_rfftel != value) { _rfftel = value; NotifyPropertyChanged();} } }
	private string? _rfffax;
	public string? rfffax { get => _rfffax; set { if (_rfffax != value) { _rfffax = value; NotifyPropertyChanged();} } }
	private string? _rffcel;
	public string? rffcel { get => _rffcel; set { if (_rffcel != value) { _rffcel = value; NotifyPropertyChanged();} } }
	private string? _rffmai;
	public string? rffmai { get => _rffmai; set { if (_rffmai != value) { _rffmai = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
	private bool _rffsendbuy;
	public bool rffsendbuy { get => _rffsendbuy; set { if (_rffsendbuy != value) { _rffsendbuy = value; NotifyPropertyChanged();} } }
}