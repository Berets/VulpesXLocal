namespace VulpesX.Models.Default;
 
public partial class RBINT00F : Base 
{
	private string _INTSOC = null!;
	public required string INTSOC { get => _INTSOC; set { if (_INTSOC != value) { _INTSOC = value; NotifyPropertyChanged();} } }
	private int _INTABI;
	public int INTABI { get => _INTABI; set { if (_INTABI != value) { _INTABI = value; NotifyPropertyChanged();} } }
	private int _INTCAB;
	public int INTCAB { get => _INTCAB; set { if (_INTCAB != value) { _INTCAB = value; NotifyPropertyChanged();} } }
	private string _INTCON = null!;
	public required string INTCON { get => _INTCON; set { if (_INTCON != value) { _INTCON = value; NotifyPropertyChanged();} } }
	private DateTime _INTDAT;
	public DateTime INTDAT { get => _INTDAT; set { if (_INTDAT != value) { _INTDAT = value; NotifyPropertyChanged();} } }
	private string? _INTATT;
	public string? INTATT { get => _INTATT; set { if (_INTATT != value) { _INTATT = value; NotifyPropertyChanged();} } }
	private int? _INTAZI;
	public int? INTAZI { get => _INTAZI; set { if (_INTAZI != value) { _INTAZI = value; NotifyPropertyChanged();} } }
	private string? _INTUTI;
	public string? INTUTI { get => _INTUTI; set { if (_INTUTI != value) { _INTUTI = value; NotifyPropertyChanged();} } }
	private string? _INTUTV;
	public string? INTUTV { get => _INTUTV; set { if (_INTUTV != value) { _INTUTV = value; NotifyPropertyChanged();} } }
	private string? _INTERM;
	public string? INTERM { get => _INTERM; set { if (_INTERM != value) { _INTERM = value; NotifyPropertyChanged();} } }
	private string? _INTORI;
	public string? INTORI { get => _INTORI; set { if (_INTORI != value) { _INTORI = value; NotifyPropertyChanged();} } }
	private string? _INTORV;
	public string? INTORV { get => _INTORV; set { if (_INTORV != value) { _INTORV = value; NotifyPropertyChanged();} } }
	private decimal? _INTCRE;
	public decimal? INTCRE { get => _INTCRE; set { if (_INTCRE != value) { _INTCRE = value; NotifyPropertyChanged();} } }
	private decimal? _INTDEB;
	public decimal? INTDEB { get => _INTDEB; set { if (_INTDEB != value) { _INTDEB = value; NotifyPropertyChanged();} } }
	private decimal? _INTSCO;
	public decimal? INTSCO { get => _INTSCO; set { if (_INTSCO != value) { _INTSCO = value; NotifyPropertyChanged();} } }
	private decimal? _INTFID;
	public decimal? INTFID { get => _INTFID; set { if (_INTFID != value) { _INTFID = value; NotifyPropertyChanged();} } }
	private decimal? _INTFI2;
	public decimal? INTFI2 { get => _INTFI2; set { if (_INTFI2 != value) { _INTFI2 = value; NotifyPropertyChanged();} } }
	private decimal? _INTFI3;
	public decimal? INTFI3 { get => _INTFI3; set { if (_INTFI3 != value) { _INTFI3 = value; NotifyPropertyChanged();} } }
	private decimal? _INTSPE;
	public decimal? INTSPE { get => _INTSPE; set { if (_INTSPE != value) { _INTSPE = value; NotifyPropertyChanged();} } }
	private decimal? _INTSPB;
	public decimal? INTSPB { get => _INTSPB; set { if (_INTSPB != value) { _INTSPB = value; NotifyPropertyChanged();} } }
	private string? _INTELA;
	public string? INTELA { get => _INTELA; set { if (_INTELA != value) { _INTELA = value; NotifyPropertyChanged();} } }
	private string? _INTTCC;
	public string? INTTCC { get => _INTTCC; set { if (_INTTCC != value) { _INTTCC = value; NotifyPropertyChanged();} } }
	private string? _INTTCD;
	public string? INTTCD { get => _INTTCD; set { if (_INTTCD != value) { _INTTCD = value; NotifyPropertyChanged();} } }
	private decimal? _INTFOND;
	public decimal? INTFOND { get => _INTFOND; set { if (_INTFOND != value) { _INTFOND = value; NotifyPropertyChanged();} } }
}