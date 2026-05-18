namespace VulpesX.Models.Default;
 
public partial class BUDGETSV : Base 
{
	private string _BGSOCI = null!;
	public required string BGSOCI { get => _BGSOCI; set { if (_BGSOCI != value) { _BGSOCI = value; NotifyPropertyChanged();} } }
	private int _BGANNO;
	public int BGANNO { get => _BGANNO; set { if (_BGANNO != value) { _BGANNO = value; NotifyPropertyChanged();} } }
	private int _BGMESE;
	public int BGMESE { get => _BGMESE; set { if (_BGMESE != value) { _BGMESE = value; NotifyPropertyChanged();} } }
	private string _BGTIAR = null!;
	public required string BGTIAR { get => _BGTIAR; set { if (_BGTIAR != value) { _BGTIAR = value; NotifyPropertyChanged();} } }
	private string _BGART = null!;
	public required string BGART { get => _BGART; set { if (_BGART != value) { _BGART = value; NotifyPropertyChanged();} } }
	private int _BGCLIE;
	public int BGCLIE { get => _BGCLIE; set { if (_BGCLIE != value) { _BGCLIE = value; NotifyPropertyChanged();} } }
	private string _bgclsc = null!;
	public required string bgclsc { get => _bgclsc; set { if (_bgclsc != value) { _bgclsc = value; NotifyPropertyChanged();} } }
	private decimal? _BGIMPB;
	public decimal? BGIMPB { get => _BGIMPB; set { if (_BGIMPB != value) { _BGIMPB = value; NotifyPropertyChanged();} } }
	private decimal? _BGQTAB;
	public decimal? BGQTAB { get => _BGQTAB; set { if (_BGQTAB != value) { _BGQTAB = value; NotifyPropertyChanged();} } }
	private decimal? _BGIMPP;
	public decimal? BGIMPP { get => _BGIMPP; set { if (_BGIMPP != value) { _BGIMPP = value; NotifyPropertyChanged();} } }
	private decimal? _BGQTAP;
	public decimal? BGQTAP { get => _BGQTAP; set { if (_BGQTAP != value) { _BGQTAP = value; NotifyPropertyChanged();} } }
	private decimal? _BGIMPC;
	public decimal? BGIMPC { get => _BGIMPC; set { if (_BGIMPC != value) { _BGIMPC = value; NotifyPropertyChanged();} } }
	private decimal? _BGQTAC;
	public decimal? BGQTAC { get => _BGQTAC; set { if (_BGQTAC != value) { _BGQTAC = value; NotifyPropertyChanged();} } }
	private decimal? _BGimpo;
	public decimal? BGimpo { get => _BGimpo; set { if (_BGimpo != value) { _BGimpo = value; NotifyPropertyChanged();} } }
}