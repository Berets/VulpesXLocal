namespace VulpesX.Models.Default;
 
public partial class SUPPLIER_GROUPS : Base 
{
	private string _ccfsoc = null!;
	public required string ccfsoc { get => _ccfsoc; set { if (_ccfsoc != value) { _ccfsoc = value; NotifyPropertyChanged();} } }
	private int _cfcode;
	public int cfcode { get => _cfcode; set { if (_cfcode != value) { _cfcode = value; NotifyPropertyChanged();} } }
	private int _cfprog;
	public int cfprog { get => _cfprog; set { if (_cfprog != value) { _cfprog = value; NotifyPropertyChanged();} } }
	private string _cfgrup = null!;
	public required string cfgrup { get => _cfgrup; set { if (_cfgrup != value) { _cfgrup = value; NotifyPropertyChanged();} } }
	private string _cfcont = null!;
	public required string cfcont { get => _cfcont; set { if (_cfcont != value) { _cfcont = value; NotifyPropertyChanged();} } }
	private string _cfsott = null!;
	public required string cfsott { get => _cfsott; set { if (_cfsott != value) { _cfsott = value; NotifyPropertyChanged();} } }
	private string? _cfsegn;
	public string? cfsegn { get => _cfsegn; set { if (_cfsegn != value) { _cfsegn = value; NotifyPropertyChanged();} } }
	private string? _cfcaus;
	public string? cfcaus { get => _cfcaus; set { if (_cfcaus != value) { _cfcaus = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}