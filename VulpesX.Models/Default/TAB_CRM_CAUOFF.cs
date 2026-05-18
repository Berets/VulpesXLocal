namespace VulpesX.Models.Default;
 
public partial class TAB_CRM_CAUOFF : Base 
{
	private string _offcod = null!;
	public required string offcod { get => _offcod; set { if (_offcod != value) { _offcod = value; NotifyPropertyChanged();} } }
	private string? _offede;
	public string? offede { get => _offede; set { if (_offede != value) { _offede = value; NotifyPropertyChanged();} } }
	private string? _offord;
	public string? offord { get => _offord; set { if (_offord != value) { _offord = value; NotifyPropertyChanged();} } }
	private string? _offtxt;
	public string? offtxt { get => _offtxt; set { if (_offtxt != value) { _offtxt = value; NotifyPropertyChanged();} } }
	private string? _offte1;
	public string? offte1 { get => _offte1; set { if (_offte1 != value) { _offte1 = value; NotifyPropertyChanged();} } }
	private int? _offsca;
	public int? offsca { get => _offsca; set { if (_offsca != value) { _offsca = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
	private string _offsoc = null!;
	public required string offsoc { get => _offsoc; set { if (_offsoc != value) { _offsoc = value; NotifyPropertyChanged();} } }
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