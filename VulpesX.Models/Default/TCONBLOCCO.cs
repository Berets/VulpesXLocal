namespace VulpesX.Models.Default;
 
public partial class TCONBLOCCO : Base 
{
	private string _cobazi = null!;
	public required string cobazi { get => _cobazi; set { if (_cobazi != value) { _cobazi = value; NotifyPropertyChanged();} } }
	private int _cobann;
	public int cobann { get => _cobann; set { if (_cobann != value) { _cobann = value; NotifyPropertyChanged();} } }
	private int _cobnum;
	public int cobnum { get => _cobnum; set { if (_cobnum != value) { _cobnum = value; NotifyPropertyChanged();} } }
	private DateTime _cobini;
	public DateTime cobini { get => _cobini; set { if (_cobini != value) { _cobini = value; NotifyPropertyChanged();} } }
	private DateTime? _cobfin;
	public DateTime? cobfin { get => _cobfin; set { if (_cobfin != value) { _cobfin = value; NotifyPropertyChanged();} } }
	private string? _cobcab;
	public string? cobcab { get => _cobcab; set { if (_cobcab != value) { _cobcab = value; NotifyPropertyChanged();} } }
	private string? _cobnot;
	public string? cobnot { get => _cobnot; set { if (_cobnot != value) { _cobnot = value; NotifyPropertyChanged();} } }
	private string? _cobusr;
	public string? cobusr { get => _cobusr; set { if (_cobusr != value) { _cobusr = value; NotifyPropertyChanged();} } }
	private string? _cobcas;
	public string? cobcas { get => _cobcas; set { if (_cobcas != value) { _cobcas = value; NotifyPropertyChanged();} } }
	private int? _cobcli;
	public int? cobcli { get => _cobcli; set { if (_cobcli != value) { _cobcli = value; NotifyPropertyChanged();} } }
	private string? _cobuss;
	public string? cobuss { get => _cobuss; set { if (_cobuss != value) { _cobuss = value; NotifyPropertyChanged();} } }
	private string? _cobusp;
	public string? cobusp { get => _cobusp; set { if (_cobusp != value) { _cobusp = value; NotifyPropertyChanged();} } }
	private decimal? _cobfid;
	public decimal? cobfid { get => _cobfid; set { if (_cobfid != value) { _cobfid = value; NotifyPropertyChanged();} } }
	private decimal? _cobord;
	public decimal? cobord { get => _cobord; set { if (_cobord != value) { _cobord = value; NotifyPropertyChanged();} } }
	private decimal? _cobesp;
	public decimal? cobesp { get => _cobesp; set { if (_cobesp != value) { _cobesp = value; NotifyPropertyChanged();} } }
}