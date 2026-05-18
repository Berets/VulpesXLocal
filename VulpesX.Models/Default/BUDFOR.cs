namespace VulpesX.Models.Default;
 
public partial class BUDFOR : Base 
{
	private string _BDFSOC = null!;
	public required string BDFSOC { get => _BDFSOC; set { if (_BDFSOC != value) { _BDFSOC = value; NotifyPropertyChanged();} } }
	private int _BDFANNO;
	public int BDFANNO { get => _BDFANNO; set { if (_BDFANNO != value) { _BDFANNO = value; NotifyPropertyChanged();} } }
	private int _BDFMESE;
	public int BDFMESE { get => _BDFMESE; set { if (_BDFMESE != value) { _BDFMESE = value; NotifyPropertyChanged();} } }
	private string _BDFTIAR = null!;
	public required string BDFTIAR { get => _BDFTIAR; set { if (_BDFTIAR != value) { _BDFTIAR = value; NotifyPropertyChanged();} } }
	private string _BDFART = null!;
	public required string BDFART { get => _BDFART; set { if (_BDFART != value) { _BDFART = value; NotifyPropertyChanged();} } }
	private int _BDFFOR;
	public int BDFFOR { get => _BDFFOR; set { if (_BDFFOR != value) { _BDFFOR = value; NotifyPropertyChanged();} } }
	private decimal _BDFIMPB;
	public decimal BDFIMPB { get => _BDFIMPB; set { if (_BDFIMPB != value) { _BDFIMPB = value; NotifyPropertyChanged();} } }
	private decimal _BDFQTAB;
	public decimal BDFQTAB { get => _BDFQTAB; set { if (_BDFQTAB != value) { _BDFQTAB = value; NotifyPropertyChanged();} } }
	private decimal _BDFIMPP;
	public decimal BDFIMPP { get => _BDFIMPP; set { if (_BDFIMPP != value) { _BDFIMPP = value; NotifyPropertyChanged();} } }
	private decimal _BDFQTAP;
	public decimal BDFQTAP { get => _BDFQTAP; set { if (_BDFQTAP != value) { _BDFQTAP = value; NotifyPropertyChanged();} } }
	private decimal _BDFIMPC;
	public decimal BDFIMPC { get => _BDFIMPC; set { if (_BDFIMPC != value) { _BDFIMPC = value; NotifyPropertyChanged();} } }
	private decimal _BDFQTAC;
	public decimal BDFQTAC { get => _BDFQTAC; set { if (_BDFQTAC != value) { _BDFQTAC = value; NotifyPropertyChanged();} } }
	private decimal _BDFIMPO;
	public decimal BDFIMPO { get => _BDFIMPO; set { if (_BDFIMPO != value) { _BDFIMPO = value; NotifyPropertyChanged();} } }
}