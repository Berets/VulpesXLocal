namespace VulpesX.Models.Default;
 
public partial class cr_history : Base 
{
	private string _company_id = null!;
	public required string company_id { get => _company_id; set { if (_company_id != value) { _company_id = value; NotifyPropertyChanged();} } }
	private int _year;
	public int year { get => _year; set { if (_year != value) { _year = value; NotifyPropertyChanged();} } }
	private int _customer_id;
	public int customer_id { get => _customer_id; set { if (_customer_id != value) { _customer_id = value; NotifyPropertyChanged();} } }
	private string? _customer_description;
	public string? customer_description { get => _customer_description; set { if (_customer_description != value) { _customer_description = value; NotifyPropertyChanged();} } }
	private string? _province;
	public string? province { get => _province; set { if (_province != value) { _province = value; NotifyPropertyChanged();} } }
	private int? _point_obtain;
	public int? point_obtain { get => _point_obtain; set { if (_point_obtain != value) { _point_obtain = value; NotifyPropertyChanged();} } }
	private int? _operation;
	public int? operation { get => _operation; set { if (_operation != value) { _operation = value; NotifyPropertyChanged();} } }
	private int? _total_days;
	public int? total_days { get => _total_days; set { if (_total_days != value) { _total_days = value; NotifyPropertyChanged();} } }
	private decimal? _medium_operation;
	public decimal? medium_operation { get => _medium_operation; set { if (_medium_operation != value) { _medium_operation = value; NotifyPropertyChanged();} } }
	private int? _unsolved;
	public int? unsolved { get => _unsolved; set { if (_unsolved != value) { _unsolved = value; NotifyPropertyChanged();} } }
	private string? _rating_sign;
	public string? rating_sign { get => _rating_sign; set { if (_rating_sign != value) { _rating_sign = value; NotifyPropertyChanged();} } }
	private decimal? _medium_payment;
	public decimal? medium_payment { get => _medium_payment; set { if (_medium_payment != value) { _medium_payment = value; NotifyPropertyChanged();} } }
	private int? _max_payment;
	public int? max_payment { get => _max_payment; set { if (_max_payment != value) { _max_payment = value; NotifyPropertyChanged();} } }
	private decimal? _invoices_import;
	public decimal? invoices_import { get => _invoices_import; set { if (_invoices_import != value) { _invoices_import = value; NotifyPropertyChanged();} } }
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