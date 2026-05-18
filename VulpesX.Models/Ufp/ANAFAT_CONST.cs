namespace VulpesX.Models.Ufp;
using VulpesX.Shared;
public partial class ANAFAT_CONST : Base 
{
	private string _afsoc = null!;
	public required string afsoc { get => _afsoc; set { if (_afsoc != value) { _afsoc = value; NotifyPropertyChanged();} } }
	private DateTime _afdata;
	public DateTime afdata { get => _afdata; set { if (_afdata != value) { _afdata = value; NotifyPropertyChanged();} } }
	private int _afver;
	public int afver { get => _afver; set { if (_afver != value) { _afver = value; NotifyPropertyChanged();} } }
	private decimal? _afproductionpre;
	public decimal? afproductionpre { get => _afproductionpre; set { if (_afproductionpre != value) { _afproductionpre = value; NotifyPropertyChanged();} } }
	private decimal? _afproductionaut;
	public decimal? afproductionaut { get => _afproductionaut; set { if (_afproductionaut != value) { _afproductionaut = value; NotifyPropertyChanged();} } }
	private decimal? _afcomplexitysta;
	public decimal? afcomplexitysta { get => _afcomplexitysta; set { if (_afcomplexitysta != value) { _afcomplexitysta = value; NotifyPropertyChanged();} } }
	private decimal? _afcomplexitymed;
	public decimal? afcomplexitymed { get => _afcomplexitymed; set { if (_afcomplexitymed != value) { _afcomplexitymed = value; NotifyPropertyChanged();} } }
	private decimal? _afcomplexitycom;
	public decimal? afcomplexitycom { get => _afcomplexitycom; set { if (_afcomplexitycom != value) { _afcomplexitycom = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
	private string? _addedUserID;
	public string? addedUserID { get => _addedUserID; set { if (_addedUserID != value) { _addedUserID = value; NotifyPropertyChanged();} } }
	private string? _updateUserID;
	public string? updateUserID { get => _updateUserID; set { if (_updateUserID != value) { _updateUserID = value; NotifyPropertyChanged();} } }
	private string? _canceledUserID;
	public string? canceledUserID { get => _canceledUserID; set { if (_canceledUserID != value) { _canceledUserID = value; NotifyPropertyChanged();} } }
	private DateTime? _added;
	public DateTime? added { get => _added; set { if (_added != value) { _added = value; NotifyPropertyChanged();} } }
	private DateTime? _updated;
	public DateTime? updated { get => _updated; set { if (_updated != value) { _updated = value; NotifyPropertyChanged();} } }
	private DateTime? _canceled;
	public DateTime? canceled { get => _canceled; set { if (_canceled != value) { _canceled = value; NotifyPropertyChanged();} } }
	private decimal? _afcliico;
	public decimal? afcliico { get => _afcliico; set { if (_afcliico != value) { _afcliico = value; NotifyPropertyChanged();} } }
	private decimal? _afclidir;
	public decimal? afclidir { get => _afclidir; set { if (_afclidir != value) { _afclidir = value; NotifyPropertyChanged();} } }
	private decimal? _afproico;
	public decimal? afproico { get => _afproico; set { if (_afproico != value) { _afproico = value; NotifyPropertyChanged();} } }
	private decimal? _afprodir;
	public decimal? afprodir { get => _afprodir; set { if (_afprodir != value) { _afprodir = value; NotifyPropertyChanged();} } }
}