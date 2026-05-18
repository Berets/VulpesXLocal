namespace VulpesX.Models.Default;
 
public partial class ACC_ASSETS_RATES : Base 
{
	private string _ammsoc = null!;
	public required string ammsoc { get => _ammsoc; set { if (_ammsoc != value) { _ammsoc = value; NotifyPropertyChanged();} } }
	private string _tgrupp = null!;
	public required string tgrupp { get => _tgrupp; set { if (_tgrupp != value) { _tgrupp = value; NotifyPropertyChanged();} } }
	private string _gconto = null!;
	public required string gconto { get => _gconto; set { if (_gconto != value) { _gconto = value; NotifyPropertyChanged();} } }
	private string _tsotco = null!;
	public required string tsotco { get => _tsotco; set { if (_tsotco != value) { _tsotco = value; NotifyPropertyChanged();} } }
	private int _janno;
	public int janno { get => _janno; set { if (_janno != value) { _janno = value; NotifyPropertyChanged();} } }
	private decimal? _tpep1;
	public decimal? tpep1 { get => _tpep1; set { if (_tpep1 != value) { _tpep1 = value; NotifyPropertyChanged();} } }
	private decimal? _tpep2;
	public decimal? tpep2 { get => _tpep2; set { if (_tpep2 != value) { _tpep2 = value; NotifyPropertyChanged();} } }
	private int? _tmaxam;
	public int? tmaxam { get => _tmaxam; set { if (_tmaxam != value) { _tmaxam = value; NotifyPropertyChanged();} } }
	private int? _triv;
	public int? triv { get => _triv; set { if (_triv != value) { _triv = value; NotifyPropertyChanged();} } }
	private decimal? _trepai;
	public decimal? trepai { get => _trepai; set { if (_trepai != value) { _trepai = value; NotifyPropertyChanged();} } }
	private bool _tancb;
	public bool tancb { get => _tancb; set { if (_tancb != value) { _tancb = value; NotifyPropertyChanged();} } }
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