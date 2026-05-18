namespace VulpesX.Models.Default;
 
public partial class BOLLDP0F : Base 
{
	private string _bolsoc = null!;
	public required string bolsoc { get => _bolsoc; set { if (_bolsoc != value) { _bolsoc = value; NotifyPropertyChanged();} } }
	private int _BTANNO;
	public int BTANNO { get => _BTANNO; set { if (_BTANNO != value) { _BTANNO = value; NotifyPropertyChanged();} } }
	private int _BTBOLL;
	public int BTBOLL { get => _BTBOLL; set { if (_BTBOLL != value) { _BTBOLL = value; NotifyPropertyChanged();} } }
	private int _BORIGB;
	public int BORIGB { get => _BORIGB; set { if (_BORIGB != value) { _BORIGB = value; NotifyPropertyChanged();} } }
	private int _bocass;
	public int bocass { get => _bocass; set { if (_bocass != value) { _bocass = value; NotifyPropertyChanged();} } }
	private int? _bopack;
	public int? bopack { get => _bopack; set { if (_bopack != value) { _bopack = value; NotifyPropertyChanged();} } }
	private decimal? _BOQTAP;
	public decimal? BOQTAP { get => _BOQTAP; set { if (_BOQTAP != value) { _BOQTAP = value; NotifyPropertyChanged();} } }
}