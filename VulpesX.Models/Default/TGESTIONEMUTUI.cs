namespace VulpesX.Models.Default;
 
public partial class TGESTIONEMUTUI : Base 
{
	private string _mutsoc = null!;
	public required string mutsoc { get => _mutsoc; set { if (_mutsoc != value) { _mutsoc = value; NotifyPropertyChanged();} } }
	private int _mutnum;
	public int mutnum { get => _mutnum; set { if (_mutnum != value) { _mutnum = value; NotifyPropertyChanged();} } }
	private int? _mutabi;
	public int? mutabi { get => _mutabi; set { if (_mutabi != value) { _mutabi = value; NotifyPropertyChanged();} } }
	private int? _mutcab;
	public int? mutcab { get => _mutcab; set { if (_mutcab != value) { _mutcab = value; NotifyPropertyChanged();} } }
	private string? _mutcont;
	public string? mutcont { get => _mutcont; set { if (_mutcont != value) { _mutcont = value; NotifyPropertyChanged();} } }
	private DateTime? _mutdat;
	public DateTime? mutdat { get => _mutdat; set { if (_mutdat != value) { _mutdat = value; NotifyPropertyChanged();} } }
	private decimal? _mutimp;
	public decimal? mutimp { get => _mutimp; set { if (_mutimp != value) { _mutimp = value; NotifyPropertyChanged();} } }
	private int? _mutrat;
	public int? mutrat { get => _mutrat; set { if (_mutrat != value) { _mutrat = value; NotifyPropertyChanged();} } }
	private string? _mutper;
	public string? mutper { get => _mutper; set { if (_mutper != value) { _mutper = value; NotifyPropertyChanged();} } }
	private decimal? _muttas;
	public decimal? muttas { get => _muttas; set { if (_muttas != value) { _muttas = value; NotifyPropertyChanged();} } }
	private string? _mutval;
	public string? mutval { get => _mutval; set { if (_mutval != value) { _mutval = value; NotifyPropertyChanged();} } }
	private int? _mutultr;
	public int? mutultr { get => _mutultr; set { if (_mutultr != value) { _mutultr = value; NotifyPropertyChanged();} } }
	private string? _mutgca;
	public string? mutgca { get => _mutgca; set { if (_mutgca != value) { _mutgca = value; NotifyPropertyChanged();} } }
	private string? _mutcca;
	public string? mutcca { get => _mutcca; set { if (_mutcca != value) { _mutcca = value; NotifyPropertyChanged();} } }
	private string? _mutsca;
	public string? mutsca { get => _mutsca; set { if (_mutsca != value) { _mutsca = value; NotifyPropertyChanged();} } }
	private string? _mutgin;
	public string? mutgin { get => _mutgin; set { if (_mutgin != value) { _mutgin = value; NotifyPropertyChanged();} } }
	private string? _mutcin;
	public string? mutcin { get => _mutcin; set { if (_mutcin != value) { _mutcin = value; NotifyPropertyChanged();} } }
	private string? _mutsin;
	public string? mutsin { get => _mutsin; set { if (_mutsin != value) { _mutsin = value; NotifyPropertyChanged();} } }
	private string? _mutgsb;
	public string? mutgsb { get => _mutgsb; set { if (_mutgsb != value) { _mutgsb = value; NotifyPropertyChanged();} } }
	private string? _mutcsb;
	public string? mutcsb { get => _mutcsb; set { if (_mutcsb != value) { _mutcsb = value; NotifyPropertyChanged();} } }
	private string? _mutssb;
	public string? mutssb { get => _mutssb; set { if (_mutssb != value) { _mutssb = value; NotifyPropertyChanged();} } }
	private string? _mutflg;
	public string? mutflg { get => _mutflg; set { if (_mutflg != value) { _mutflg = value; NotifyPropertyChanged();} } }
}