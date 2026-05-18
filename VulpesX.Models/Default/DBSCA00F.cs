namespace VulpesX.Models.Default;
 
public partial class DBSCA00F : Base 
{
	private string _DBSUTE = null!;
	public required string DBSUTE { get => _DBSUTE; set { if (_DBSUTE != value) { _DBSUTE = value; NotifyPropertyChanged();} } }
	private string _DBSCOP = null!;
	public required string DBSCOP { get => _DBSCOP; set { if (_DBSCOP != value) { _DBSCOP = value; NotifyPropertyChanged();} } }
	private int _DBSPRG;
	public int DBSPRG { get => _DBSPRG; set { if (_DBSPRG != value) { _DBSPRG = value; NotifyPropertyChanged();} } }
	private string _DBSCOD = null!;
	public required string DBSCOD { get => _DBSCOD; set { if (_DBSCOD != value) { _DBSCOD = value; NotifyPropertyChanged();} } }
	private decimal? _DBSQTA;
	public decimal? DBSQTA { get => _DBSQTA; set { if (_DBSQTA != value) { _DBSQTA = value; NotifyPropertyChanged();} } }
	private int? _DBSLIV;
	public int? DBSLIV { get => _DBSLIV; set { if (_DBSLIV != value) { _DBSLIV = value; NotifyPropertyChanged();} } }
	private decimal? _DBSPES;
	public decimal? DBSPES { get => _DBSPES; set { if (_DBSPES != value) { _DBSPES = value; NotifyPropertyChanged();} } }
	private int? _DBSPRO;
	public int? DBSPRO { get => _DBSPRO; set { if (_DBSPRO != value) { _DBSPRO = value; NotifyPropertyChanged();} } }
	private decimal? _DBSGIA;
	public decimal? DBSGIA { get => _DBSGIA; set { if (_DBSGIA != value) { _DBSGIA = value; NotifyPropertyChanged();} } }
	private decimal? _DBSIMP;
	public decimal? DBSIMP { get => _DBSIMP; set { if (_DBSIMP != value) { _DBSIMP = value; NotifyPropertyChanged();} } }
	private decimal? _DBSENT;
	public decimal? DBSENT { get => _DBSENT; set { if (_DBSENT != value) { _DBSENT = value; NotifyPropertyChanged();} } }
	private DateTime? _DBSDAT;
	public DateTime? DBSDAT { get => _DBSDAT; set { if (_DBSDAT != value) { _DBSDAT = value; NotifyPropertyChanged();} } }
}