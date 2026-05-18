namespace VulpesX.Models.Default;
 
public partial class DISPREP : Base 
{
	private string _dissoc = null!;
	public required string dissoc { get => _dissoc; set { if (_dissoc != value) { _dissoc = value; NotifyPropertyChanged();} } }
	private string _disrep = null!;
	public required string disrep { get => _disrep; set { if (_disrep != value) { _disrep = value; NotifyPropertyChanged();} } }
	private DateTime _disdat;
	public DateTime disdat { get => _disdat; set { if (_disdat != value) { _disdat = value; NotifyPropertyChanged();} } }
	private decimal? _distmd;
	public decimal? distmd { get => _distmd; set { if (_distmd != value) { _distmd = value; NotifyPropertyChanged();} } }
	private decimal? _dislau;
	public decimal? dislau { get => _dislau; set { if (_dislau != value) { _dislau = value; NotifyPropertyChanged();} } }
	private int? _disnpt;
	public int? disnpt { get => _disnpt; set { if (_disnpt != value) { _disnpt = value; NotifyPropertyChanged();} } }
	private int? _disnst;
	public int? disnst { get => _disnst; set { if (_disnst != value) { _disnst = value; NotifyPropertyChanged();} } }
	private int? _disntt;
	public int? disntt { get => _disntt; set { if (_disntt != value) { _disntt = value; NotifyPropertyChanged();} } }
	private decimal? _distpt;
	public decimal? distpt { get => _distpt; set { if (_distpt != value) { _distpt = value; NotifyPropertyChanged();} } }
	private decimal? _distst;
	public decimal? distst { get => _distst; set { if (_distst != value) { _distst = value; NotifyPropertyChanged();} } }
	private decimal? _disttt;
	public decimal? disttt { get => _disttt; set { if (_disttt != value) { _disttt = value; NotifyPropertyChanged();} } }
	private string? _disso1;
	public string? disso1 { get => _disso1; set { if (_disso1 != value) { _disso1 = value; NotifyPropertyChanged();} } }
}