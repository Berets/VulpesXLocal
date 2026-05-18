namespace VulpesX.Models.Default;
 
public partial class ACC_LIPE_ELABS : Base 
{
	private string _company_id = null!;
	public required string company_id { get => _company_id; set { if (_company_id != value) { _company_id = value; NotifyPropertyChanged();} } }
	private int _elab_year;
	public int elab_year { get => _elab_year; set { if (_elab_year != value) { _elab_year = value; NotifyPropertyChanged();} } }
	private int _elab_period;
	public int elab_period { get => _elab_period; set { if (_elab_period != value) { _elab_period = value; NotifyPropertyChanged();} } }
	private string _code_id = null!;
	public required string code_id { get => _code_id; set { if (_code_id != value) { _code_id = value; NotifyPropertyChanged();} } }
	private string _elab_type = null!;
	public required string elab_type { get => _elab_type; set { if (_elab_type != value) { _elab_type = value; NotifyPropertyChanged();} } }
	private decimal _debit;
	public decimal debit { get => _debit; set { if (_debit != value) { _debit = value; NotifyPropertyChanged();} } }
	private decimal _credit;
	public decimal credit { get => _credit; set { if (_credit != value) { _credit = value; NotifyPropertyChanged();} } }
	private DateTime? _exported;
	public DateTime? exported { get => _exported; set { if (_exported != value) { _exported = value; NotifyPropertyChanged();} } }
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