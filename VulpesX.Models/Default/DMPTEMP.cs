namespace VulpesX.Models.Default;
 
public partial class DMPTEMP : Base 
{
	private string _dmpsoc = null!;
	public required string dmpsoc { get => _dmpsoc; set { if (_dmpsoc != value) { _dmpsoc = value; NotifyPropertyChanged();} } }
	private int _dmpann;
	public int dmpann { get => _dmpann; set { if (_dmpann != value) { _dmpann = value; NotifyPropertyChanged();} } }
	private int _dmpnum;
	public int dmpnum { get => _dmpnum; set { if (_dmpnum != value) { _dmpnum = value; NotifyPropertyChanged();} } }
	private int _dmpfor;
	public int dmpfor { get => _dmpfor; set { if (_dmpfor != value) { _dmpfor = value; NotifyPropertyChanged();} } }
	private string? _dmprs1;
	public string? dmprs1 { get => _dmprs1; set { if (_dmprs1 != value) { _dmprs1 = value; NotifyPropertyChanged();} } }
	private string? _dmprs2;
	public string? dmprs2 { get => _dmprs2; set { if (_dmprs2 != value) { _dmprs2 = value; NotifyPropertyChanged();} } }
	private string? _dmpind;
	public string? dmpind { get => _dmpind; set { if (_dmpind != value) { _dmpind = value; NotifyPropertyChanged();} } }
	private string? _dmploc;
	public string? dmploc { get => _dmploc; set { if (_dmploc != value) { _dmploc = value; NotifyPropertyChanged();} } }
	private int? _dmpcap;
	public int? dmpcap { get => _dmpcap; set { if (_dmpcap != value) { _dmpcap = value; NotifyPropertyChanged();} } }
	private string? _dmppro;
	public string? dmppro { get => _dmppro; set { if (_dmppro != value) { _dmppro = value; NotifyPropertyChanged();} } }
	private string? _dmppiv;
	public string? dmppiv { get => _dmppiv; set { if (_dmppiv != value) { _dmppiv = value; NotifyPropertyChanged();} } }
	private string? _dmpcfi;
	public string? dmpcfi { get => _dmpcfi; set { if (_dmpcfi != value) { _dmpcfi = value; NotifyPropertyChanged();} } }
	private int? _dmpabi;
	public int? dmpabi { get => _dmpabi; set { if (_dmpabi != value) { _dmpabi = value; NotifyPropertyChanged();} } }
	private int? _dmpcab;
	public int? dmpcab { get => _dmpcab; set { if (_dmpcab != value) { _dmpcab = value; NotifyPropertyChanged();} } }
	private decimal? _dmpcam;
	public decimal? dmpcam { get => _dmpcam; set { if (_dmpcam != value) { _dmpcam = value; NotifyPropertyChanged();} } }
	private string? _dmpdiv;
	public string? dmpdiv { get => _dmpdiv; set { if (_dmpdiv != value) { _dmpdiv = value; NotifyPropertyChanged();} } }
	private decimal? _dmpimp;
	public decimal? dmpimp { get => _dmpimp; set { if (_dmpimp != value) { _dmpimp = value; NotifyPropertyChanged();} } }
	private decimal? _dmpeui;
	public decimal? dmpeui { get => _dmpeui; set { if (_dmpeui != value) { _dmpeui = value; NotifyPropertyChanged();} } }
	private string? _dmpseg;
	public string? dmpseg { get => _dmpseg; set { if (_dmpseg != value) { _dmpseg = value; NotifyPropertyChanged();} } }
	private int? _dmpabo;
	public int? dmpabo { get => _dmpabo; set { if (_dmpabo != value) { _dmpabo = value; NotifyPropertyChanged();} } }
	private int? _dmpcao;
	public int? dmpcao { get => _dmpcao; set { if (_dmpcao != value) { _dmpcao = value; NotifyPropertyChanged();} } }
	private string? _dmpcco;
	public string? dmpcco { get => _dmpcco; set { if (_dmpcco != value) { _dmpcco = value; NotifyPropertyChanged();} } }
	private DateTime? _dmpda1;
	public DateTime? dmpda1 { get => _dmpda1; set { if (_dmpda1 != value) { _dmpda1 = value; NotifyPropertyChanged();} } }
	private string? _dmpdo1;
	public string? dmpdo1 { get => _dmpdo1; set { if (_dmpdo1 != value) { _dmpdo1 = value; NotifyPropertyChanged();} } }
	private DateTime? _dmpda2;
	public DateTime? dmpda2 { get => _dmpda2; set { if (_dmpda2 != value) { _dmpda2 = value; NotifyPropertyChanged();} } }
	private string? _dmpdo2;
	public string? dmpdo2 { get => _dmpdo2; set { if (_dmpdo2 != value) { _dmpdo2 = value; NotifyPropertyChanged();} } }
	private DateTime? _dmpda3;
	public DateTime? dmpda3 { get => _dmpda3; set { if (_dmpda3 != value) { _dmpda3 = value; NotifyPropertyChanged();} } }
	private string? _dmpdo3;
	public string? dmpdo3 { get => _dmpdo3; set { if (_dmpdo3 != value) { _dmpdo3 = value; NotifyPropertyChanged();} } }
	private DateTime? _dmpda4;
	public DateTime? dmpda4 { get => _dmpda4; set { if (_dmpda4 != value) { _dmpda4 = value; NotifyPropertyChanged();} } }
	private string? _dmpdo4;
	public string? dmpdo4 { get => _dmpdo4; set { if (_dmpdo4 != value) { _dmpdo4 = value; NotifyPropertyChanged();} } }
	private DateTime? _dmpda5;
	public DateTime? dmpda5 { get => _dmpda5; set { if (_dmpda5 != value) { _dmpda5 = value; NotifyPropertyChanged();} } }
	private string? _dmpdo5;
	public string? dmpdo5 { get => _dmpdo5; set { if (_dmpdo5 != value) { _dmpdo5 = value; NotifyPropertyChanged();} } }
	private DateTime? _dmpda6;
	public DateTime? dmpda6 { get => _dmpda6; set { if (_dmpda6 != value) { _dmpda6 = value; NotifyPropertyChanged();} } }
	private string? _dmpdo6;
	public string? dmpdo6 { get => _dmpdo6; set { if (_dmpdo6 != value) { _dmpdo6 = value; NotifyPropertyChanged();} } }
	private DateTime? _dmpda7;
	public DateTime? dmpda7 { get => _dmpda7; set { if (_dmpda7 != value) { _dmpda7 = value; NotifyPropertyChanged();} } }
	private string? _dmpdo7;
	public string? dmpdo7 { get => _dmpdo7; set { if (_dmpdo7 != value) { _dmpdo7 = value; NotifyPropertyChanged();} } }
	private DateTime? _dmpda8;
	public DateTime? dmpda8 { get => _dmpda8; set { if (_dmpda8 != value) { _dmpda8 = value; NotifyPropertyChanged();} } }
	private string? _dmpdo8;
	public string? dmpdo8 { get => _dmpdo8; set { if (_dmpdo8 != value) { _dmpdo8 = value; NotifyPropertyChanged();} } }
	private DateTime? _dmpda9;
	public DateTime? dmpda9 { get => _dmpda9; set { if (_dmpda9 != value) { _dmpda9 = value; NotifyPropertyChanged();} } }
	private string? _dmpdo9;
	public string? dmpdo9 { get => _dmpdo9; set { if (_dmpdo9 != value) { _dmpdo9 = value; NotifyPropertyChanged();} } }
	private DateTime? _dmpda10;
	public DateTime? dmpda10 { get => _dmpda10; set { if (_dmpda10 != value) { _dmpda10 = value; NotifyPropertyChanged();} } }
	private string? _dmpdo10;
	public string? dmpdo10 { get => _dmpdo10; set { if (_dmpdo10 != value) { _dmpdo10 = value; NotifyPropertyChanged();} } }
	private DateTime? _dmpda11;
	public DateTime? dmpda11 { get => _dmpda11; set { if (_dmpda11 != value) { _dmpda11 = value; NotifyPropertyChanged();} } }
	private string? _dmpdo11;
	public string? dmpdo11 { get => _dmpdo11; set { if (_dmpdo11 != value) { _dmpdo11 = value; NotifyPropertyChanged();} } }
	private DateTime? _dmpda12;
	public DateTime? dmpda12 { get => _dmpda12; set { if (_dmpda12 != value) { _dmpda12 = value; NotifyPropertyChanged();} } }
	private string? _dmpdo12;
	public string? dmpdo12 { get => _dmpdo12; set { if (_dmpdo12 != value) { _dmpdo12 = value; NotifyPropertyChanged();} } }
	private DateTime? _dmpda13;
	public DateTime? dmpda13 { get => _dmpda13; set { if (_dmpda13 != value) { _dmpda13 = value; NotifyPropertyChanged();} } }
	private string? _dmpdo13;
	public string? dmpdo13 { get => _dmpdo13; set { if (_dmpdo13 != value) { _dmpdo13 = value; NotifyPropertyChanged();} } }
	private DateTime? _dmpda14;
	public DateTime? dmpda14 { get => _dmpda14; set { if (_dmpda14 != value) { _dmpda14 = value; NotifyPropertyChanged();} } }
	private string? _dmpdo14;
	public string? dmpdo14 { get => _dmpdo14; set { if (_dmpdo14 != value) { _dmpdo14 = value; NotifyPropertyChanged();} } }
	private DateTime? _dmpda15;
	public DateTime? dmpda15 { get => _dmpda15; set { if (_dmpda15 != value) { _dmpda15 = value; NotifyPropertyChanged();} } }
	private string? _dmpdo15;
	public string? dmpdo15 { get => _dmpdo15; set { if (_dmpdo15 != value) { _dmpdo15 = value; NotifyPropertyChanged();} } }
	private string? _dmppiu;
	public string? dmppiu { get => _dmppiu; set { if (_dmppiu != value) { _dmppiu = value; NotifyPropertyChanged();} } }
	private decimal? _dmpitl;
	public decimal? dmpitl { get => _dmpitl; set { if (_dmpitl != value) { _dmpitl = value; NotifyPropertyChanged();} } }
	private decimal? _dmpite;
	public decimal? dmpite { get => _dmpite; set { if (_dmpite != value) { _dmpite = value; NotifyPropertyChanged();} } }
	private int? _dmptof;
	public int? dmptof { get => _dmptof; set { if (_dmptof != value) { _dmptof = value; NotifyPropertyChanged();} } }
	private DateTime? _dmpdval;
	public DateTime? dmpdval { get => _dmpdval; set { if (_dmpdval != value) { _dmpdval = value; NotifyPropertyChanged();} } }
	private string? _dmpccn;
	public string? dmpccn { get => _dmpccn; set { if (_dmpccn != value) { _dmpccn = value; NotifyPropertyChanged();} } }
	private string? _dmpiban;
	public string? dmpiban { get => _dmpiban; set { if (_dmpiban != value) { _dmpiban = value; NotifyPropertyChanged();} } }
	private string? _dmpbban;
	public string? dmpbban { get => _dmpbban; set { if (_dmpbban != value) { _dmpbban = value; NotifyPropertyChanged();} } }
	private string? _dmpbic;
	public string? dmpbic { get => _dmpbic; set { if (_dmpbic != value) { _dmpbic = value; NotifyPropertyChanged();} } }
	private string? _dmpcin;
	public string? dmpcin { get => _dmpcin; set { if (_dmpcin != value) { _dmpcin = value; NotifyPropertyChanged();} } }
}