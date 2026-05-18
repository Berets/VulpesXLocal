namespace VulpesX.Models.Default;
 
public partial class tab_articolo_costi : Base 
{
	private string _cid = null!;
	public required string cid { get => _cid; set { if (_cid != value) { _cid = value; NotifyPropertyChanged();} } }
	private string _product_id = null!;
	public required string product_id { get => _product_id; set { if (_product_id != value) { _product_id = value; NotifyPropertyChanged();} } }
	private int _year;
	public int year { get => _year; set { if (_year != value) { _year = value; NotifyPropertyChanged();} } }
	private int _month;
	public int month { get => _month; set { if (_month != value) { _month = value; NotifyPropertyChanged();} } }
	private decimal _total_load;
	public decimal total_load { get => _total_load; set { if (_total_load != value) { _total_load = value; NotifyPropertyChanged();} } }
	private decimal _total_value;
	public decimal total_value { get => _total_value; set { if (_total_value != value) { _total_value = value; NotifyPropertyChanged();} } }
	private decimal _last_cost;
	public decimal last_cost { get => _last_cost; set { if (_last_cost != value) { _last_cost = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
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