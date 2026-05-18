namespace VulpesX.Models.Default;
 
public partial class PCNTA00F : Base 
{
	private string _pcasoc = null!;
	public required string pcasoc { get => _pcasoc; set { if (_pcasoc != value) { _pcasoc = value; NotifyPropertyChanged();} } }
	private int _pcaann;
	public int pcaann { get => _pcaann; set { if (_pcaann != value) { _pcaann = value; NotifyPropertyChanged();} } }
	private string _pcacos = null!;
	public required string pcacos { get => _pcacos; set { if (_pcacos != value) { _pcacos = value; NotifyPropertyChanged();} } }
	private string _pcagru = null!;
	public required string pcagru { get => _pcagru; set { if (_pcagru != value) { _pcagru = value; NotifyPropertyChanged();} } }
	private string _pcacon = null!;
	public required string pcacon { get => _pcacon; set { if (_pcacon != value) { _pcacon = value; NotifyPropertyChanged();} } }
	private string _pcasot = null!;
	public required string pcasot { get => _pcasot; set { if (_pcasot != value) { _pcasot = value; NotifyPropertyChanged();} } }
	private decimal? _pcapvl;
	public decimal? pcapvl { get => _pcapvl; set { if (_pcapvl != value) { _pcapvl = value; NotifyPropertyChanged();} } }
	private decimal? _pcapve;
	public decimal? pcapve { get => _pcapve; set { if (_pcapve != value) { _pcapve = value; NotifyPropertyChanged();} } }
	private decimal? _pcacvl;
	public decimal? pcacvl { get => _pcacvl; set { if (_pcacvl != value) { _pcacvl = value; NotifyPropertyChanged();} } }
	private decimal? _pcacve;
	public decimal? pcacve { get => _pcacve; set { if (_pcacve != value) { _pcacve = value; NotifyPropertyChanged();} } }
	private decimal? _pcapqt;
	public decimal? pcapqt { get => _pcapqt; set { if (_pcapqt != value) { _pcapqt = value; NotifyPropertyChanged();} } }
	private decimal? _pcacqt;
	public decimal? pcacqt { get => _pcacqt; set { if (_pcacqt != value) { _pcacqt = value; NotifyPropertyChanged();} } }
	private string? _pcabud;
	public string? pcabud { get => _pcabud; set { if (_pcabud != value) { _pcabud = value; NotifyPropertyChanged();} } }
	private int? _pcacod;
	public int? pcacod { get => _pcacod; set { if (_pcacod != value) { _pcacod = value; NotifyPropertyChanged();} } }
	private string? _pcaori;
	public string? pcaori { get => _pcaori; set { if (_pcaori != value) { _pcaori = value; NotifyPropertyChanged();} } }
	private string? _pcaorv;
	public string? pcaorv { get => _pcaorv; set { if (_pcaorv != value) { _pcaorv = value; NotifyPropertyChanged();} } }
	private string? _pcater;
	public string? pcater { get => _pcater; set { if (_pcater != value) { _pcater = value; NotifyPropertyChanged();} } }
	private string? _pcauti;
	public string? pcauti { get => _pcauti; set { if (_pcauti != value) { _pcauti = value; NotifyPropertyChanged();} } }
	private string? _pcautv;
	public string? pcautv { get => _pcautv; set { if (_pcautv != value) { _pcautv = value; NotifyPropertyChanged();} } }
	private string? _pcaatt;
	public string? pcaatt { get => _pcaatt; set { if (_pcaatt != value) { _pcaatt = value; NotifyPropertyChanged();} } }
	private string? _pcaspe;
	public string? pcaspe { get => _pcaspe; set { if (_pcaspe != value) { _pcaspe = value; NotifyPropertyChanged();} } }
	private int? _pcames;
	public int? pcames { get => _pcames; set { if (_pcames != value) { _pcames = value; NotifyPropertyChanged();} } }
	private string? _pcaso1;
	public string? pcaso1 { get => _pcaso1; set { if (_pcaso1 != value) { _pcaso1 = value; NotifyPropertyChanged();} } }
}