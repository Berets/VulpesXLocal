namespace VulpesX.Models.Default;
 
public partial class MRPORD : Base 
{
	private string _mroart = null!;
	public required string mroart { get => _mroart; set { if (_mroart != value) { _mroart = value; NotifyPropertyChanged();} } }
	private DateTime _mrodat;
	public DateTime mrodat { get => _mrodat; set { if (_mrodat != value) { _mrodat = value; NotifyPropertyChanged();} } }
	private int _mroseq;
	public int mroseq { get => _mroseq; set { if (_mroseq != value) { _mroseq = value; NotifyPropertyChanged();} } }
	private decimal? _mroqta;
	public decimal? mroqta { get => _mroqta; set { if (_mroqta != value) { _mroqta = value; NotifyPropertyChanged();} } }
	private string? _mrosoc;
	public string? mrosoc { get => _mrosoc; set { if (_mrosoc != value) { _mrosoc = value; NotifyPropertyChanged();} } }
	private int? _mroanp;
	public int? mroanp { get => _mroanp; set { if (_mroanp != value) { _mroanp = value; NotifyPropertyChanged();} } }
	private int? _mroorp;
	public int? mroorp { get => _mroorp; set { if (_mroorp != value) { _mroorp = value; NotifyPropertyChanged();} } }
	private string? _mrosoo;
	public string? mrosoo { get => _mrosoo; set { if (_mrosoo != value) { _mrosoo = value; NotifyPropertyChanged();} } }
	private int? _mroano;
	public int? mroano { get => _mroano; set { if (_mroano != value) { _mroano = value; NotifyPropertyChanged();} } }
	private int? _mroord;
	public int? mroord { get => _mroord; set { if (_mroord != value) { _mroord = value; NotifyPropertyChanged();} } }
	private int? _mrorig;
	public int? mrorig { get => _mrorig; set { if (_mrorig != value) { _mrorig = value; NotifyPropertyChanged();} } }
	private decimal? _mroqtu;
	public decimal? mroqtu { get => _mroqtu; set { if (_mroqtu != value) { _mroqtu = value; NotifyPropertyChanged();} } }
}