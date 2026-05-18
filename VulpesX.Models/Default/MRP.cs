namespace VulpesX.Models.Default;
 
public partial class MRP : Base 
{
	private DateTime _mrpdat;
	public DateTime mrpdat { get => _mrpdat; set { if (_mrpdat != value) { _mrpdat = value; NotifyPropertyChanged();} } }
	private int _mrpseq;
	public int mrpseq { get => _mrpseq; set { if (_mrpseq != value) { _mrpseq = value; NotifyPropertyChanged();} } }
	private string _mrpart = null!;
	public required string mrpart { get => _mrpart; set { if (_mrpart != value) { _mrpart = value; NotifyPropertyChanged();} } }
	private decimal? _mrpqta;
	public decimal? mrpqta { get => _mrpqta; set { if (_mrpqta != value) { _mrpqta = value; NotifyPropertyChanged();} } }
	private string? _mrpsoc;
	public string? mrpsoc { get => _mrpsoc; set { if (_mrpsoc != value) { _mrpsoc = value; NotifyPropertyChanged();} } }
	private int? _mrpanp;
	public int? mrpanp { get => _mrpanp; set { if (_mrpanp != value) { _mrpanp = value; NotifyPropertyChanged();} } }
	private int? _mrporp;
	public int? mrporp { get => _mrporp; set { if (_mrporp != value) { _mrporp = value; NotifyPropertyChanged();} } }
	private string? _mrpsoo;
	public string? mrpsoo { get => _mrpsoo; set { if (_mrpsoo != value) { _mrpsoo = value; NotifyPropertyChanged();} } }
	private int? _mrpano;
	public int? mrpano { get => _mrpano; set { if (_mrpano != value) { _mrpano = value; NotifyPropertyChanged();} } }
	private int? _mrpord;
	public int? mrpord { get => _mrpord; set { if (_mrpord != value) { _mrpord = value; NotifyPropertyChanged();} } }
	private int? _mrprig;
	public int? mrprig { get => _mrprig; set { if (_mrprig != value) { _mrprig = value; NotifyPropertyChanged();} } }
	private decimal? _mrpqtu;
	public decimal? mrpqtu { get => _mrpqtu; set { if (_mrpqtu != value) { _mrpqtu = value; NotifyPropertyChanged();} } }
	private DateTime? _mrpdae;
	public DateTime? mrpdae { get => _mrpdae; set { if (_mrpdae != value) { _mrpdae = value; NotifyPropertyChanged();} } }
	private string? _mrparp;
	public string? mrparp { get => _mrparp; set { if (_mrparp != value) { _mrparp = value; NotifyPropertyChanged();} } }
	private string? _mrptip;
	public string? mrptip { get => _mrptip; set { if (_mrptip != value) { _mrptip = value; NotifyPropertyChanged();} } }
	private decimal? _mrpqt1;
	public decimal? mrpqt1 { get => _mrpqt1; set { if (_mrpqt1 != value) { _mrpqt1 = value; NotifyPropertyChanged();} } }
	private decimal? _mrpqt2;
	public decimal? mrpqt2 { get => _mrpqt2; set { if (_mrpqt2 != value) { _mrpqt2 = value; NotifyPropertyChanged();} } }
}