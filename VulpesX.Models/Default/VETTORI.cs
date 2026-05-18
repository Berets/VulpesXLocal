namespace VulpesX.Models.Default;
 
public partial class VETTORI : Base 
{
	private int _vetcod;
	public int vetcod { get => _vetcod; set { if (_vetcod != value) { _vetcod = value; NotifyPropertyChanged();} } }
	private string _vetdes = null!;
	public required string vetdes { get => _vetdes; set { if (_vetdes != value) { _vetdes = value; NotifyPropertyChanged();} } }
	private string? _vetind;
	public string? vetind { get => _vetind; set { if (_vetind != value) { _vetind = value; NotifyPropertyChanged();} } }
	private string? _vetloc;
	public string? vetloc { get => _vetloc; set { if (_vetloc != value) { _vetloc = value; NotifyPropertyChanged();} } }
	private string? _vetpro;
	public string? vetpro { get => _vetpro; set { if (_vetpro != value) { _vetpro = value; NotifyPropertyChanged();} } }
	private int? _vetcap;
	public int? vetcap { get => _vetcap; set { if (_vetcap != value) { _vetcap = value; NotifyPropertyChanged();} } }
	private int? _vetfor;
	public int? vetfor { get => _vetfor; set { if (_vetfor != value) { _vetfor = value; NotifyPropertyChanged();} } }
	private string? _vetcalb;
	public string? vetcalb { get => _vetcalb; set { if (_vetcalb != value) { _vetcalb = value; NotifyPropertyChanged();} } }
	private int? _vetpiva;
	public int? vetpiva { get => _vetpiva; set { if (_vetpiva != value) { _vetpiva = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}