namespace VulpesX.Models.Default;
 
public partial class NOTASPESE : Base 
{
	private string _nossoc = null!;
	public required string nossoc { get => _nossoc; set { if (_nossoc != value) { _nossoc = value; NotifyPropertyChanged();} } }
	private string _nosdip = null!;
	public required string nosdip { get => _nosdip; set { if (_nosdip != value) { _nosdip = value; NotifyPropertyChanged();} } }
	private DateTime _nosdat;
	public DateTime nosdat { get => _nosdat; set { if (_nosdat != value) { _nosdat = value; NotifyPropertyChanged();} } }
	private string? _nosmez;
	public string? nosmez { get => _nosmez; set { if (_nosmez != value) { _nosmez = value; NotifyPropertyChanged();} } }
	private string? _nosdes;
	public string? nosdes { get => _nosdes; set { if (_nosdes != value) { _nosdes = value; NotifyPropertyChanged();} } }
	private int? _nostri;
	public int? nostri { get => _nostri; set { if (_nostri != value) { _nostri = value; NotifyPropertyChanged();} } }
	private int? _nosann;
	public int? nosann { get => _nosann; set { if (_nosann != value) { _nosann = value; NotifyPropertyChanged();} } }
	private int? _nosmes;
	public int? nosmes { get => _nosmes; set { if (_nosmes != value) { _nosmes = value; NotifyPropertyChanged();} } }
	private int? _noscli;
	public int? noscli { get => _noscli; set { if (_noscli != value) { _noscli = value; NotifyPropertyChanged();} } }
	private string? _nosfir;
	public string? nosfir { get => _nosfir; set { if (_nosfir != value) { _nosfir = value; NotifyPropertyChanged();} } }
	private string? _nosfi2;
	public string? nosfi2 { get => _nosfi2; set { if (_nosfi2 != value) { _nosfi2 = value; NotifyPropertyChanged();} } }
	private string? _noscom;
	public string? noscom { get => _noscom; set { if (_noscom != value) { _noscom = value; NotifyPropertyChanged();} } }
	private string? _nosflg;
	public string? nosflg { get => _nosflg; set { if (_nosflg != value) { _nosflg = value; NotifyPropertyChanged();} } }
	private string? _nosdir;
	public string? nosdir { get => _nosdir; set { if (_nosdir != value) { _nosdir = value; NotifyPropertyChanged();} } }
	private DateTime? _nosdad;
	public DateTime? nosdad { get => _nosdad; set { if (_nosdad != value) { _nosdad = value; NotifyPropertyChanged();} } }
	private string? _noscos;
	public string? noscos { get => _noscos; set { if (_noscos != value) { _noscos = value; NotifyPropertyChanged();} } }
	private decimal? _nostol;
	public decimal? nostol { get => _nostol; set { if (_nostol != value) { _nostol = value; NotifyPropertyChanged();} } }
	private decimal? _nosanl;
	public decimal? nosanl { get => _nosanl; set { if (_nosanl != value) { _nosanl = value; NotifyPropertyChanged();} } }
}