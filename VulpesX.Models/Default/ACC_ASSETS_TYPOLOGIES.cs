namespace VulpesX.Models.Default;
 
public partial class ACC_ASSETS_TYPOLOGIES : Base 
{
	private string _tsoci = null!;
	public required string tsoci { get => _tsoci; set { if (_tsoci != value) { _tsoci = value; NotifyPropertyChanged();} } }
	private string _tgrupp = null!;
	public required string tgrupp { get => _tgrupp; set { if (_tgrupp != value) { _tgrupp = value; NotifyPropertyChanged();} } }
	private string _gconto = null!;
	public required string gconto { get => _gconto; set { if (_gconto != value) { _gconto = value; NotifyPropertyChanged();} } }
	private string _tsotco = null!;
	public required string tsotco { get => _tsotco; set { if (_tsotco != value) { _tsotco = value; NotifyPropertyChanged();} } }
	private string? _jcateg;
	public string? jcateg { get => _jcateg; set { if (_jcateg != value) { _jcateg = value; NotifyPropertyChanged();} } }
	private int? _tnupro;
	public int? tnupro { get => _tnupro; set { if (_tnupro != value) { _tnupro = value; NotifyPropertyChanged();} } }
	private string? _grupp2;
	public string? grupp2 { get => _grupp2; set { if (_grupp2 != value) { _grupp2 = value; NotifyPropertyChanged();} } }
	private string? _conto2;
	public string? conto2 { get => _conto2; set { if (_conto2 != value) { _conto2 = value; NotifyPropertyChanged();} } }
	private string? _sotto2;
	public string? sotto2 { get => _sotto2; set { if (_sotto2 != value) { _sotto2 = value; NotifyPropertyChanged();} } }
	private string? _grupp1;
	public string? grupp1 { get => _grupp1; set { if (_grupp1 != value) { _grupp1 = value; NotifyPropertyChanged();} } }
	private string? _cont1;
	public string? cont1 { get => _cont1; set { if (_cont1 != value) { _cont1 = value; NotifyPropertyChanged();} } }
	private string? _sotto1;
	public string? sotto1 { get => _sotto1; set { if (_sotto1 != value) { _sotto1 = value; NotifyPropertyChanged();} } }
	private string? _segno1;
	public string? segno1 { get => _segno1; set { if (_segno1 != value) { _segno1 = value; NotifyPropertyChanged();} } }
	private string? _segno2;
	public string? segno2 { get => _segno2; set { if (_segno2 != value) { _segno2 = value; NotifyPropertyChanged();} } }
	private string? _grupp3;
	public string? grupp3 { get => _grupp3; set { if (_grupp3 != value) { _grupp3 = value; NotifyPropertyChanged();} } }
	private string? _conto3;
	public string? conto3 { get => _conto3; set { if (_conto3 != value) { _conto3 = value; NotifyPropertyChanged();} } }
	private string? _sotto3;
	public string? sotto3 { get => _sotto3; set { if (_sotto3 != value) { _sotto3 = value; NotifyPropertyChanged();} } }
	private string? _segno3;
	public string? segno3 { get => _segno3; set { if (_segno3 != value) { _segno3 = value; NotifyPropertyChanged();} } }
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
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}