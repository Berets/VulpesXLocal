namespace VulpesX.Models.Default;
 
public partial class ANDEFRES : Base 
{
	private int _CLIENT;
	public int CLIENT { get => _CLIENT; set { if (_CLIENT != value) { _CLIENT = value; NotifyPropertyChanged();} } }
	private int _clirig;
	public int clirig { get => _clirig; set { if (_clirig != value) { _clirig = value; NotifyPropertyChanged();} } }
	private string _clirco = null!;
	public required string clirco { get => _clirco; set { if (_clirco != value) { _clirco = value; NotifyPropertyChanged();} } }
	private string _clirte = null!;
	public required string clirte { get => _clirte; set { if (_clirte != value) { _clirte = value; NotifyPropertyChanged();} } }
	private string? _climco;
	public string? climco { get => _climco; set { if (_climco != value) { _climco = value; NotifyPropertyChanged();} } }
	private string? _clmte;
	public string? clmte { get => _clmte; set { if (_clmte != value) { _clmte = value; NotifyPropertyChanged();} } }
	private string? _rfctel;
	public string? rfctel { get => _rfctel; set { if (_rfctel != value) { _rfctel = value; NotifyPropertyChanged();} } }
	private string? _rfcfax;
	public string? rfcfax { get => _rfcfax; set { if (_rfcfax != value) { _rfcfax = value; NotifyPropertyChanged();} } }
	private string? _rfccel;
	public string? rfccel { get => _rfccel; set { if (_rfccel != value) { _rfccel = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
	private bool _clirifamm;
	public bool clirifamm { get => _clirifamm; set { if (_clirifamm != value) { _clirifamm = value; NotifyPropertyChanged();} } }
	private bool _clisendoff;
	public bool clisendoff { get => _clisendoff; set { if (_clisendoff != value) { _clisendoff = value; NotifyPropertyChanged();} } }
	private bool _clisendord;
	public bool clisendord { get => _clisendord; set { if (_clisendord != value) { _clisendord = value; NotifyPropertyChanged();} } }
	private bool _clisendddt;
	public bool clisendddt { get => _clisendddt; set { if (_clisendddt != value) { _clisendddt = value; NotifyPropertyChanged();} } }
	private bool _clisendinv;
	public bool clisendinv { get => _clisendinv; set { if (_clisendinv != value) { _clisendinv = value; NotifyPropertyChanged();} } }
}