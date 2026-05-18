namespace VulpesX.Models.Default;
 
public partial class PDCANNI : Base 
{
	private string _P1SOCI = null!;
	public required string P1SOCI { get => _P1SOCI; set { if (_P1SOCI != value) { _P1SOCI = value; NotifyPropertyChanged();} } }
	private string _P1GRUP = null!;
	public required string P1GRUP { get => _P1GRUP; set { if (_P1GRUP != value) { _P1GRUP = value; NotifyPropertyChanged();} } }
	private string _P2CONT = null!;
	public required string P2CONT { get => _P2CONT; set { if (_P2CONT != value) { _P2CONT = value; NotifyPropertyChanged();} } }
	private string _P3SOTC = null!;
	public required string P3SOTC { get => _P3SOTC; set { if (_P3SOTC != value) { _P3SOTC = value; NotifyPropertyChanged();} } }
	private int _P4ANNO;
	public int P4ANNO { get => _P4ANNO; set { if (_P4ANNO != value) { _P4ANNO = value; NotifyPropertyChanged();} } }
	private string? _P1CCHI;
	public string? P1CCHI { get => _P1CCHI; set { if (_P1CCHI != value) { _P1CCHI = value; NotifyPropertyChanged();} } }
	private decimal? _P4DAPE;
	public decimal? P4DAPE { get => _P4DAPE; set { if (_P4DAPE != value) { _P4DAPE = value; NotifyPropertyChanged();} } }
	private decimal? _P4AVPE;
	public decimal? P4AVPE { get => _P4AVPE; set { if (_P4AVPE != value) { _P4AVPE = value; NotifyPropertyChanged();} } }
	private decimal? _P4DAES;
	public decimal? P4DAES { get => _P4DAES; set { if (_P4DAES != value) { _P4DAES = value; NotifyPropertyChanged();} } }
	private decimal? _P4AVES;
	public decimal? P4AVES { get => _P4AVES; set { if (_P4AVES != value) { _P4AVES = value; NotifyPropertyChanged();} } }
	private int? _P4PAM2;
	public int? P4PAM2 { get => _P4PAM2; set { if (_P4PAM2 != value) { _P4PAM2 = value; NotifyPropertyChanged();} } }
	private string? _P4CORA;
	public string? P4CORA { get => _P4CORA; set { if (_P4CORA != value) { _P4CORA = value; NotifyPropertyChanged();} } }
	private DateTime? _P4DATA;
	public DateTime? P4DATA { get => _P4DATA; set { if (_P4DATA != value) { _P4DATA = value; NotifyPropertyChanged();} } }
	private decimal? _p4dpe;
	public decimal? p4dpe { get => _p4dpe; set { if (_p4dpe != value) { _p4dpe = value; NotifyPropertyChanged();} } }
	private decimal? _p4ape;
	public decimal? p4ape { get => _p4ape; set { if (_p4ape != value) { _p4ape = value; NotifyPropertyChanged();} } }
	private decimal? _p4dee;
	public decimal? p4dee { get => _p4dee; set { if (_p4dee != value) { _p4dee = value; NotifyPropertyChanged();} } }
	private decimal? _p4aee;
	public decimal? p4aee { get => _p4aee; set { if (_p4aee != value) { _p4aee = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}