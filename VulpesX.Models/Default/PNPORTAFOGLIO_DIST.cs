namespace VulpesX.Models.Default;
 
public partial class PNPORTAFOGLIO_DIST : Base 
{
	private string _company_id = null!;
	public required string company_id { get => _company_id; set { if (_company_id != value) { _company_id = value; NotifyPropertyChanged();} } }
	private long _id;
	public long id { get => _id; set { if (_id != value) { _id = value; NotifyPropertyChanged();} } }
	private decimal _amount;
	public decimal amount { get => _amount; set { if (_amount != value) { _amount = value; NotifyPropertyChanged();} } }
	private int? _abi;
	public int? abi { get => _abi; set { if (_abi != value) { _abi = value; NotifyPropertyChanged();} } }
	private int? _cab;
	public int? cab { get => _cab; set { if (_cab != value) { _cab = value; NotifyPropertyChanged();} } }
	private string? _account;
	public string? account { get => _account; set { if (_account != value) { _account = value; NotifyPropertyChanged();} } }
	private DateTime? _extraction_date;
	public DateTime? extraction_date { get => _extraction_date; set { if (_extraction_date != value) { _extraction_date = value; NotifyPropertyChanged();} } }
	private DateTime? _accounting_date;
	public DateTime? accounting_date { get => _accounting_date; set { if (_accounting_date != value) { _accounting_date = value; NotifyPropertyChanged();} } }
	private DateTime? _added;
	public DateTime? added { get => _added; set { if (_added != value) { _added = value; NotifyPropertyChanged();} } }
	private string _addedUserID = null!;
	public required string addedUserID { get => _addedUserID; set { if (_addedUserID != value) { _addedUserID = value; NotifyPropertyChanged();} } }
	private DateTime? _updated;
	public DateTime? updated { get => _updated; set { if (_updated != value) { _updated = value; NotifyPropertyChanged();} } }
	private string? _updatedUserID;
	public string? updatedUserID { get => _updatedUserID; set { if (_updatedUserID != value) { _updatedUserID = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}