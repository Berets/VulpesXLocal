namespace VulpesX.Models.Default;
 
public partial class CAUCONT : Base 
{
	private string _caucod = null!;
	public required string caucod { get => _caucod; set { if (_caucod != value) { _caucod = value; NotifyPropertyChanged();} } }
	private string _caudes = null!;
	public required string caudes { get => _caudes; set { if (_caudes != value) { _caudes = value; NotifyPropertyChanged();} } }
	private string? _caugen;
	public string? caugen { get => _caugen; set { if (_caugen != value) { _caugen = value; NotifyPropertyChanged();} } }
	private string? _cauiva;
	public string? cauiva { get => _cauiva; set { if (_cauiva != value) { _cauiva = value; NotifyPropertyChanged();} } }
	private string? _caucli;
	public string? caucli { get => _caucli; set { if (_caucli != value) { _caucli = value; NotifyPropertyChanged();} } }
	private string? _caufor;
	public string? caufor { get => _caufor; set { if (_caufor != value) { _caufor = value; NotifyPropertyChanged();} } }
	private string? _cauass;
	public string? cauass { get => _cauass; set { if (_cauass != value) { _cauass = value; NotifyPropertyChanged();} } }
	private string? _cauali;
	public string? cauali { get => _cauali; set { if (_cauali != value) { _cauali = value; NotifyPropertyChanged();} } }
	private string? _cauliv;
	public string? cauliv { get => _cauliv; set { if (_cauliv != value) { _cauliv = value; NotifyPropertyChanged();} } }
	private string? _causeg;
	public string? causeg { get => _causeg; set { if (_causeg != value) { _causeg = value; NotifyPropertyChanged();} } }
	private string? _causol;
	public string? causol { get => _causol; set { if (_causol != value) { _causol = value; NotifyPropertyChanged();} } }
	private string? _caucol;
	public string? caucol { get => _caucol; set { if (_caucol != value) { _caucol = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
	private bool _cauzer;
	public bool cauzer { get => _cauzer; set { if (_cauzer != value) { _cauzer = value; NotifyPropertyChanged();} } }
	private string? _cauceco;
	public string? cauceco { get => _cauceco; set { if (_cauceco != value) { _cauceco = value; NotifyPropertyChanged();} } }
	private DateTime? _added;
	public DateTime? added { get => _added; set { if (_added != value) { _added = value; NotifyPropertyChanged();} } }
	private DateTime? _updated;
	public DateTime? updated { get => _updated; set { if (_updated != value) { _updated = value; NotifyPropertyChanged();} } }
	private DateTime? _canceled;
	public DateTime? canceled { get => _canceled; set { if (_canceled != value) { _canceled = value; NotifyPropertyChanged();} } }
	private string? _addedUserID;
	public string? addedUserID { get => _addedUserID; set { if (_addedUserID != value) { _addedUserID = value; NotifyPropertyChanged();} } }
	private string? _updatedUserID;
	public string? updatedUserID { get => _updatedUserID; set { if (_updatedUserID != value) { _updatedUserID = value; NotifyPropertyChanged();} } }
	private string? _canceledUserID;
	public string? canceledUserID { get => _canceledUserID; set { if (_canceledUserID != value) { _canceledUserID = value; NotifyPropertyChanged();} } }
	private string? _canceledNote;
	public string? canceledNote { get => _canceledNote; set { if (_canceledNote != value) { _canceledNote = value; NotifyPropertyChanged();} } }
}