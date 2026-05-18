namespace VulpesX.Models.Default;
 
public partial class store_stocks_lots : Base 
{
	private string _company_id = null!;
	public required string company_id { get => _company_id; set { if (_company_id != value) { _company_id = value; NotifyPropertyChanged();} } }
	private string _store_id = null!;
	public required string store_id { get => _store_id; set { if (_store_id != value) { _store_id = value; NotifyPropertyChanged();} } }
	private string _product_id = null!;
	public required string product_id { get => _product_id; set { if (_product_id != value) { _product_id = value; NotifyPropertyChanged();} } }
	private string _lot = null!;
	public required string lot { get => _lot; set { if (_lot != value) { _lot = value; NotifyPropertyChanged();} } }
	private DateTime? _expire;
	public DateTime? expire { get => _expire; set { if (_expire != value) { _expire = value; NotifyPropertyChanged();} } }
	private decimal? _quantity_stock;
	public decimal? quantity_stock { get => _quantity_stock; set { if (_quantity_stock != value) { _quantity_stock = value; NotifyPropertyChanged();} } }
	private decimal? _quantity_production;
	public decimal? quantity_production { get => _quantity_production; set { if (_quantity_production != value) { _quantity_production = value; NotifyPropertyChanged();} } }
	private decimal? _quantity_ordered;
	public decimal? quantity_ordered { get => _quantity_ordered; set { if (_quantity_ordered != value) { _quantity_ordered = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
	private string? _goods_location;
	public string? goods_location { get => _goods_location; set { if (_goods_location != value) { _goods_location = value; NotifyPropertyChanged();} } }
	private string? _supplier_lot;
	public string? supplier_lot { get => _supplier_lot; set { if (_supplier_lot != value) { _supplier_lot = value; NotifyPropertyChanged();} } }
}