namespace VulpesX.Models.Default;
 
public partial class ASSED00F : Base 
{
	private string _company_id = null!;
	public required string company_id { get => _company_id; set { if (_company_id != value) { _company_id = value; NotifyPropertyChanged();} } }
	private string _id = null!;
	public required string id { get => _id; set { if (_id != value) { _id = value; NotifyPropertyChanged();} } }
	private int _position;
	public int position { get => _position; set { if (_position != value) { _position = value; NotifyPropertyChanged();} } }
	private string? _description;
	public string? description { get => _description; set { if (_description != value) { _description = value; NotifyPropertyChanged();} } }
	private string? _product_id;
	public string? product_id { get => _product_id; set { if (_product_id != value) { _product_id = value; NotifyPropertyChanged();} } }
	private decimal? _quantity;
	public decimal? quantity { get => _quantity; set { if (_quantity != value) { _quantity = value; NotifyPropertyChanged();} } }
	private string? _note;
	public string? note { get => _note; set { if (_note != value) { _note = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
	private int? _step;
	public int? step { get => _step; set { if (_step != value) { _step = value; NotifyPropertyChanged();} } }
}