namespace VulpesX.Models.Ufp;
using VulpesX.Shared;
public partial class ANAFAT_ROW : Base 
{
	private string _afsoc = null!;
	public required string afsoc { get => _afsoc; set { if (_afsoc != value) { _afsoc = value; NotifyPropertyChanged();} } }
	private int _afyear;
	public int afyear { get => _afyear; set { if (_afyear != value) { _afyear = value; NotifyPropertyChanged();} } }
	private long _afid;
	public long afid { get => _afid; set { if (_afid != value) { _afid = value; NotifyPropertyChanged();} } }
    private DateTime _afdata;
    public DateTime afdata { get => _afdata; set { if (_afdata != value) { _afdata = value; NotifyPropertyChanged(); } } }
    private DateTime _afconstdata;
	public DateTime afconstdata { get => _afconstdata; set { if (_afconstdata != value) { _afconstdata = value; NotifyPropertyChanged();} } }
	private int _afconstver;
	public int afconstver { get => _afconstver; set { if (_afconstver != value) { _afconstver = value; NotifyPropertyChanged();} } }
    private int? _afcli;
    public int? afcli { get => _afcli; set { if (_afcli != value) { _afcli = value; NotifyPropertyChanged(); } } }
    private string? _afclides;
    public string? afclides { get => _afclides; set { if (_afclides != value) { _afclides = value; NotifyPropertyChanged(); } } }
    private string? _afartid;
	public string? afartid { get => _afartid; set { if (_afartid != value) { _afartid = value; NotifyPropertyChanged();} } }
	private string? _afmatid;
	public string? afmatid { get => _afmatid; set { if (_afmatid != value) { _afmatid = value; NotifyPropertyChanged();} } }
	private int? _afmatforid;
	public int? afmatforid { get => _afmatforid; set { if (_afmatforid != value) { _afmatforid = value; NotifyPropertyChanged();} } }
	private DateTime? _afmatfordata;
	public DateTime? afmatfordata { get => _afmatfordata; set { if (_afmatfordata != value) { _afmatfordata = value; NotifyPropertyChanged();} } }
	private decimal? _afmatpre;
	public decimal? afmatpre { get => _afmatpre; set { if (_afmatpre != value) { _afmatpre = value; NotifyPropertyChanged();} } }
	private string? _afcicid;
	public string? afcicid { get => _afcicid; set { if (_afcicid != value) { _afcicid = value; NotifyPropertyChanged();} } }
	private string? _afcicseq;
	public string? afcicseq { get => _afcicseq; set { if (_afcicseq != value) { _afcicseq = value; NotifyPropertyChanged();} } }
	private decimal? _afcicmin;
	public decimal? afcicmin { get => _afcicmin; set { if (_afcicmin != value) { _afcicmin = value; NotifyPropertyChanged();} } }
	private decimal? _afcicpre;
	public decimal? afcicpre { get => _afcicpre; set { if (_afcicpre != value) { _afcicpre = value; NotifyPropertyChanged();} } }
	private decimal? _afextpre;
	public decimal? afextpre { get => _afextpre; set { if (_afextpre != value) { _afextpre = value; NotifyPropertyChanged();} } }
	private string? _afvarnote;
	public string? afvarnote { get => _afvarnote; set { if (_afvarnote != value) { _afvarnote = value; NotifyPropertyChanged();} } }
	private decimal? _afvarpre;
	public decimal? afvarpre { get => _afvarpre; set { if (_afvarpre != value) { _afvarpre = value; NotifyPropertyChanged();} } }
	private string? _afcustomertype;
	public string? afcustomertype { get => _afcustomertype; set { if (_afcustomertype != value) { _afcustomertype = value; NotifyPropertyChanged();} } }
	private string? _afproductiontype;
	public string? afproductiontype { get => _afproductiontype; set { if (_afproductiontype != value) { _afproductiontype = value; NotifyPropertyChanged();} } }
	private string? _afcomplexitytype;
	public string? afcomplexitytype { get => _afcomplexitytype; set { if (_afcomplexitytype != value) { _afcomplexitytype = value; NotifyPropertyChanged();} } }
    private string? _afmatsearch;
    public string? afmatsearch { get => _afmatsearch; set { if (_afmatsearch != value) { _afmatsearch = value; NotifyPropertyChanged(); } } }
    private string? _afcicsearch;
    public string? afcicsearch { get => _afcicsearch; set { if (_afcicsearch != value) { _afcicsearch = value; NotifyPropertyChanged(); } } }
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
}