namespace VulpesX.Models.Default;
 
public partial class PCSTA00F : Base 
{
	private string _pcssoc = null!;
	public required string pcssoc { get => _pcssoc; set { if (_pcssoc != value) { _pcssoc = value; NotifyPropertyChanged();} } }
	private int _pcssta;
	public int pcssta { get => _pcssta; set { if (_pcssta != value) { _pcssta = value; NotifyPropertyChanged();} } }
	private string? _pcscod;
	public string? pcscod { get => _pcscod; set { if (_pcscod != value) { _pcscod = value; NotifyPropertyChanged();} } }
	private string? _pcsdes;
	public string? pcsdes { get => _pcsdes; set { if (_pcsdes != value) { _pcsdes = value; NotifyPropertyChanged();} } }
	private string? _pcstip;
	public string? pcstip { get => _pcstip; set { if (_pcstip != value) { _pcstip = value; NotifyPropertyChanged();} } }
	private string? _pcstqt;
	public string? pcstqt { get => _pcstqt; set { if (_pcstqt != value) { _pcstqt = value; NotifyPropertyChanged();} } }
	private string? _pcstva;
	public string? pcstva { get => _pcstva; set { if (_pcstva != value) { _pcstva = value; NotifyPropertyChanged();} } }
	private string? _pcsleg;
	public string? pcsleg { get => _pcsleg; set { if (_pcsleg != value) { _pcsleg = value; NotifyPropertyChanged();} } }
	private string? _pcsatt;
	public string? pcsatt { get => _pcsatt; set { if (_pcsatt != value) { _pcsatt = value; NotifyPropertyChanged();} } }
	private string? _pcsori;
	public string? pcsori { get => _pcsori; set { if (_pcsori != value) { _pcsori = value; NotifyPropertyChanged();} } }
	private string? _pcsorv;
	public string? pcsorv { get => _pcsorv; set { if (_pcsorv != value) { _pcsorv = value; NotifyPropertyChanged();} } }
	private string? _pcster;
	public string? pcster { get => _pcster; set { if (_pcster != value) { _pcster = value; NotifyPropertyChanged();} } }
	private string? _pcsuti;
	public string? pcsuti { get => _pcsuti; set { if (_pcsuti != value) { _pcsuti = value; NotifyPropertyChanged();} } }
	private string? _pcsutv;
	public string? pcsutv { get => _pcsutv; set { if (_pcsutv != value) { _pcsutv = value; NotifyPropertyChanged();} } }
}