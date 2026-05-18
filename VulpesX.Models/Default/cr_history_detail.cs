namespace VulpesX.Models.Default;
 
public partial class cr_history_detail : Base 
{
	private string _company_id = null!;
	public required string company_id { get => _company_id; set { if (_company_id != value) { _company_id = value; NotifyPropertyChanged();} } }
	private int _year;
	public int year { get => _year; set { if (_year != value) { _year = value; NotifyPropertyChanged();} } }
	private string _ref_number = null!;
	public required string ref_number { get => _ref_number; set { if (_ref_number != value) { _ref_number = value; NotifyPropertyChanged();} } }
	private DateTime _ref_date;
	public DateTime ref_date { get => _ref_date; set { if (_ref_date != value) { _ref_date = value; NotifyPropertyChanged();} } }
	private int _customer_id;
	public int customer_id { get => _customer_id; set { if (_customer_id != value) { _customer_id = value; NotifyPropertyChanged();} } }
	private DateTime _expire;
	public DateTime expire { get => _expire; set { if (_expire != value) { _expire = value; NotifyPropertyChanged();} } }
	private DateTime? _last_date;
	public DateTime? last_date { get => _last_date; set { if (_last_date != value) { _last_date = value; NotifyPropertyChanged();} } }
	private decimal? _amount;
	public decimal? amount { get => _amount; set { if (_amount != value) { _amount = value; NotifyPropertyChanged();} } }
	private DateTime? _LogAdded;
	public DateTime? LogAdded { get => _LogAdded; set { if (_LogAdded != value) { _LogAdded = value; NotifyPropertyChanged();} } }
	private DateTime? _LogUpdated;
	public DateTime? LogUpdated { get => _LogUpdated; set { if (_LogUpdated != value) { _LogUpdated = value; NotifyPropertyChanged();} } }
	private DateTime? _LogCanceled;
	public DateTime? LogCanceled { get => _LogCanceled; set { if (_LogCanceled != value) { _LogCanceled = value; NotifyPropertyChanged();} } }
	private string? _LogAddedUserID;
	public string? LogAddedUserID { get => _LogAddedUserID; set { if (_LogAddedUserID != value) { _LogAddedUserID = value; NotifyPropertyChanged();} } }
	private string? _LogUpdatedUserID;
	public string? LogUpdatedUserID { get => _LogUpdatedUserID; set { if (_LogUpdatedUserID != value) { _LogUpdatedUserID = value; NotifyPropertyChanged();} } }
	private string? _LogCanceledUserID;
	public string? LogCanceledUserID { get => _LogCanceledUserID; set { if (_LogCanceledUserID != value) { _LogCanceledUserID = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}