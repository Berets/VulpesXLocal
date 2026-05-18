namespace VulpesX.Models.Default;
 
public partial class ACC_ASSETS_DEP_HISTORY : Base 
{
	private string _bhsoci = null!;
	public required string bhsoci { get => _bhsoci; set { if (_bhsoci != value) { _bhsoci = value; NotifyPropertyChanged();} } }
	private int _bhanco;
	public int bhanco { get => _bhanco; set { if (_bhanco != value) { _bhanco = value; NotifyPropertyChanged();} } }
	private int _bhann4;
	public int bhann4 { get => _bhann4; set { if (_bhann4 != value) { _bhann4 = value; NotifyPropertyChanged();} } }
	private string _bhgrup = null!;
	public required string bhgrup { get => _bhgrup; set { if (_bhgrup != value) { _bhgrup = value; NotifyPropertyChanged();} } }
	private string _bhcont = null!;
	public required string bhcont { get => _bhcont; set { if (_bhcont != value) { _bhcont = value; NotifyPropertyChanged();} } }
	private string _bhsotc = null!;
	public required string bhsotc { get => _bhsotc; set { if (_bhsotc != value) { _bhsotc = value; NotifyPropertyChanged();} } }
	private int _bhinv2;
	public int bhinv2 { get => _bhinv2; set { if (_bhinv2 != value) { _bhinv2 = value; NotifyPropertyChanged();} } }
	private int _bhinv;
	public int bhinv { get => _bhinv; set { if (_bhinv != value) { _bhinv = value; NotifyPropertyChanged();} } }
	private decimal? _bhval;
	public decimal? bhval { get => _bhval; set { if (_bhval != value) { _bhval = value; NotifyPropertyChanged();} } }
	private decimal? _bhpea;
	public decimal? bhpea { get => _bhpea; set { if (_bhpea != value) { _bhpea = value; NotifyPropertyChanged();} } }
	private decimal? _bhfoa;
	public decimal? bhfoa { get => _bhfoa; set { if (_bhfoa != value) { _bhfoa = value; NotifyPropertyChanged();} } }
	private decimal? _bhanqo;
	public decimal? bhanqo { get => _bhanqo; set { if (_bhanqo != value) { _bhanqo = value; NotifyPropertyChanged();} } }
	private decimal? _bhper;
	public decimal? bhper { get => _bhper; set { if (_bhper != value) { _bhper = value; NotifyPropertyChanged();} } }
	private decimal? _bhanna;
	public decimal? bhanna { get => _bhanna; set { if (_bhanna != value) { _bhanna = value; NotifyPropertyChanged();} } }
	private decimal? _bhpac;
	public decimal? bhpac { get => _bhpac; set { if (_bhpac != value) { _bhpac = value; NotifyPropertyChanged();} } }
	private decimal? _bhapp;
	public decimal? bhapp { get => _bhapp; set { if (_bhapp != value) { _bhapp = value; NotifyPropertyChanged();} } }
	private decimal? _bhpcc;
	public decimal? bhpcc { get => _bhpcc; set { if (_bhpcc != value) { _bhpcc = value; NotifyPropertyChanged();} } }
	private DateTime? _bhdaip;
	public DateTime? bhdaip { get => _bhdaip; set { if (_bhdaip != value) { _bhdaip = value; NotifyPropertyChanged();} } }
	private DateTime? _bhdafp;
	public DateTime? bhdafp { get => _bhdafp; set { if (_bhdafp != value) { _bhdafp = value; NotifyPropertyChanged();} } }
	private decimal? _bhanne;
	public decimal? bhanne { get => _bhanne; set { if (_bhanne != value) { _bhanne = value; NotifyPropertyChanged();} } }
	private decimal? _bhnnae;
	public decimal? bhnnae { get => _bhnnae; set { if (_bhnnae != value) { _bhnnae = value; NotifyPropertyChanged();} } }
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