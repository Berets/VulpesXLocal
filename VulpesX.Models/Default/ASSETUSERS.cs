namespace VulpesX.Models.Default;
 
public partial class ASSETUSERS : Base 
{
	private string _company_id = null!;
	public required string company_id { get => _company_id; set { if (_company_id != value) { _company_id = value; NotifyPropertyChanged();} } }
	private string _id = null!;
	public required string id { get => _id; set { if (_id != value) { _id = value; NotifyPropertyChanged();} } }
	private string _user_id = null!;
	public required string user_id { get => _user_id; set { if (_user_id != value) { _user_id = value; NotifyPropertyChanged();} } }
	private string _user_id_added = null!;
	public required string user_id_added { get => _user_id_added; set { if (_user_id_added != value) { _user_id_added = value; NotifyPropertyChanged();} } }
	private DateTime _added;
	public DateTime added { get => _added; set { if (_added != value) { _added = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}