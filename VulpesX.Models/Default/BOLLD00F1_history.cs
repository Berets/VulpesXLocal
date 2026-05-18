namespace VulpesX.Models.Default;
 
public partial class BOLLD00F1_history : Base 
{
	private string _bolsoc = null!;
	public required string bolsoc { get => _bolsoc; set { if (_bolsoc != value) { _bolsoc = value; NotifyPropertyChanged();} } }
	private int _BTANNO;
	public int BTANNO { get => _BTANNO; set { if (_BTANNO != value) { _BTANNO = value; NotifyPropertyChanged();} } }
	private int _BTBOLL;
	public int BTBOLL { get => _BTBOLL; set { if (_BTBOLL != value) { _BTBOLL = value; NotifyPropertyChanged();} } }
	private int _revision;
	public int revision { get => _revision; set { if (_revision != value) { _revision = value; NotifyPropertyChanged();} } }
	private int _BORIGB;
	public int BORIGB { get => _BORIGB; set { if (_BORIGB != value) { _BORIGB = value; NotifyPropertyChanged();} } }
	private int _boposc;
	public int boposc { get => _boposc; set { if (_boposc != value) { _boposc = value; NotifyPropertyChanged();} } }
	private string _bolott = null!;
	public required string bolott { get => _bolott; set { if (_bolott != value) { _bolott = value; NotifyPropertyChanged();} } }
	private decimal _boqtlo;
	public decimal boqtlo { get => _boqtlo; set { if (_boqtlo != value) { _boqtlo = value; NotifyPropertyChanged();} } }
	private string _store_id = null!;
	public required string store_id { get => _store_id; set { if (_store_id != value) { _store_id = value; NotifyPropertyChanged();} } }
	private string _product_id = null!;
	public required string product_id { get => _product_id; set { if (_product_id != value) { _product_id = value; NotifyPropertyChanged();} } }
}