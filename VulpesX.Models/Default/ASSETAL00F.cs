namespace VulpesX.Models.Default;
 
public partial class ASSETAL00F : Base 
{
	private string _company_id = null!;
	public required string company_id { get => _company_id; set { if (_company_id != value) { _company_id = value; NotifyPropertyChanged();} } }
	private string _id = null!;
	public required string id { get => _id; set { if (_id != value) { _id = value; NotifyPropertyChanged();} } }
	private Guid _document_id;
	public Guid document_id { get => _document_id; set { if (_document_id != value) { _document_id = value; NotifyPropertyChanged();} } }
	private string _document_name = null!;
	public required string document_name { get => _document_name; set { if (_document_name != value) { _document_name = value; NotifyPropertyChanged();} } }
	private long? _document_size;
	public long? document_size { get => _document_size; set { if (_document_size != value) { _document_size = value; NotifyPropertyChanged();} } }
	private DateTime _added;
	public DateTime added { get => _added; set { if (_added != value) { _added = value; NotifyPropertyChanged();} } }
	private string _add_user = null!;
	public required string add_user { get => _add_user; set { if (_add_user != value) { _add_user = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
	private string? _note;
	public string? note { get => _note; set { if (_note != value) { _note = value; NotifyPropertyChanged();} } }
}