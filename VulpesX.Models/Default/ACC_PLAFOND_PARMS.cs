namespace VulpesX.Models.Default;
 
public partial class ACC_PLAFOND_PARMS : Base 
{
	private string _company_id = null!;
	public required string company_id { get => _company_id; set { if (_company_id != value) { _company_id = value; NotifyPropertyChanged();} } }
	private decimal _limit_amount;
	public decimal limit_amount { get => _limit_amount; set { if (_limit_amount != value) { _limit_amount = value; NotifyPropertyChanged();} } }
	private decimal _stamp_amount;
	public decimal stamp_amount { get => _stamp_amount; set { if (_stamp_amount != value) { _stamp_amount = value; NotifyPropertyChanged();} } }
	private string _rate_code = null!;
	public required string rate_code { get => _rate_code; set { if (_rate_code != value) { _rate_code = value; NotifyPropertyChanged();} } }
	private string _rate_value = null!;
	public required string rate_value { get => _rate_value; set { if (_rate_value != value) { _rate_value = value; NotifyPropertyChanged();} } }
	private string? _group_id;
	public string? group_id { get => _group_id; set { if (_group_id != value) { _group_id = value; NotifyPropertyChanged();} } }
	private string? _account_id;
	public string? account_id { get => _account_id; set { if (_account_id != value) { _account_id = value; NotifyPropertyChanged();} } }
	private string? _subaccount_id;
	public string? subaccount_id { get => _subaccount_id; set { if (_subaccount_id != value) { _subaccount_id = value; NotifyPropertyChanged();} } }
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
	private string? _product_id;
	public string? product_id { get => _product_id; set { if (_product_id != value) { _product_id = value; NotifyPropertyChanged();} } }
}