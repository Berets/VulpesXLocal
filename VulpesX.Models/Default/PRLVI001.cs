namespace VulpesX.Models.Default;
 
public partial class PRLVI001 : Base 
{
	private string _OPSOCI = null!;
	public required string OPSOCI { get => _OPSOCI; set { if (_OPSOCI != value) { _OPSOCI = value; NotifyPropertyChanged();} } }
	private int _OPANNP;
	public int OPANNP { get => _OPANNP; set { if (_OPANNP != value) { _OPANNP = value; NotifyPropertyChanged();} } }
	private int _OPNUOP;
	public int OPNUOP { get => _OPNUOP; set { if (_OPNUOP != value) { _OPNUOP = value; NotifyPropertyChanged();} } }
	private string _PLLCOP = null!;
	public required string PLLCOP { get => _PLLCOP; set { if (_PLLCOP != value) { _PLLCOP = value; NotifyPropertyChanged();} } }
	private string _PLLMAG = null!;
	public required string PLLMAG { get => _PLLMAG; set { if (_PLLMAG != value) { _PLLMAG = value; NotifyPropertyChanged();} } }
	private string _PLNLOT = null!;
	public required string PLNLOT { get => _PLNLOT; set { if (_PLNLOT != value) { _PLNLOT = value; NotifyPropertyChanged();} } }
	private decimal? _PLLQPR;
	public decimal? PLLQPR { get => _PLLQPR; set { if (_PLLQPR != value) { _PLLQPR = value; NotifyPropertyChanged();} } }
	private decimal? _PLLQPS;
	public decimal? PLLQPS { get => _PLLQPS; set { if (_PLLQPS != value) { _PLLQPS = value; NotifyPropertyChanged();} } }
	private DateTime? _PLLDUP;
	public DateTime? PLLDUP { get => _PLLDUP; set { if (_PLLDUP != value) { _PLLDUP = value; NotifyPropertyChanged();} } }
}