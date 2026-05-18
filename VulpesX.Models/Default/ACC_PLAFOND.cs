namespace VulpesX.Models.Default;
 
public partial class ACC_PLAFOND : Base 
{
	private string _Cliasoc = null!;
	public required string Cliasoc { get => _Cliasoc; set { if (_Cliasoc != value) { _Cliasoc = value; NotifyPropertyChanged();} } }
	private int _Cliacod;
	public int Cliacod { get => _Cliacod; set { if (_Cliacod != value) { _Cliacod = value; NotifyPropertyChanged();} } }
	private int _cliannosol;
	public int cliannosol { get => _cliannosol; set { if (_cliannosol != value) { _cliannosol = value; NotifyPropertyChanged();} } }
	private int _cliprog;
	public int cliprog { get => _cliprog; set { if (_cliprog != value) { _cliprog = value; NotifyPropertyChanged();} } }
	private DateTime? _clidatchi;
	public DateTime? clidatchi { get => _clidatchi; set { if (_clidatchi != value) { _clidatchi = value; NotifyPropertyChanged();} } }
	private decimal? _cliimpfattprog;
	public decimal? cliimpfattprog { get => _cliimpfattprog; set { if (_cliimpfattprog != value) { _cliimpfattprog = value; NotifyPropertyChanged();} } }
	private decimal? _cliimpesefino;
	public decimal? cliimpesefino { get => _cliimpesefino; set { if (_cliimpesefino != value) { _cliimpesefino = value; NotifyPropertyChanged();} } }
	private decimal? _cliimpfattprovv;
	public decimal? cliimpfattprovv { get => _cliimpfattprovv; set { if (_cliimpfattprovv != value) { _cliimpfattprovv = value; NotifyPropertyChanged();} } }
	private string? _clinumprotuffiva;
	public string? clinumprotuffiva { get => _clinumprotuffiva; set { if (_clinumprotuffiva != value) { _clinumprotuffiva = value; NotifyPropertyChanged();} } }
	private DateTime? _clidatuffiva;
	public DateTime? clidatuffiva { get => _clidatuffiva; set { if (_clidatuffiva != value) { _clidatuffiva = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
	private DateTime? _clistart;
	public DateTime? clistart { get => _clistart; set { if (_clistart != value) { _clistart = value; NotifyPropertyChanged();} } }
	private string? _clinote;
	public string? clinote { get => _clinote; set { if (_clinote != value) { _clinote = value; NotifyPropertyChanged();} } }
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