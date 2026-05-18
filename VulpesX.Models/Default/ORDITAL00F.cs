namespace VulpesX.Models.Default;
 
public partial class ORDITAL00F : Base 
{
	private string _otasoci = null!;
	public required string otasoci { get => _otasoci; set { if (_otasoci != value) { _otasoci = value; NotifyPropertyChanged();} } }
	private int _OTAANNO;
	public int OTAANNO { get => _OTAANNO; set { if (_OTAANNO != value) { _OTAANNO = value; NotifyPropertyChanged();} } }
	private int _OTANUOR;
	public int OTANUOR { get => _OTANUOR; set { if (_OTANUOR != value) { _OTANUOR = value; NotifyPropertyChanged();} } }
	private Guid _OTAUID;
	public Guid OTAUID { get => _OTAUID; set { if (_OTAUID != value) { _OTAUID = value; NotifyPropertyChanged();} } }
	private string _OTANAME = null!;
	public required string OTANAME { get => _OTANAME; set { if (_OTANAME != value) { _OTANAME = value; NotifyPropertyChanged();} } }
	private long? _OTASIZE;
	public long? OTASIZE { get => _OTASIZE; set { if (_OTASIZE != value) { _OTASIZE = value; NotifyPropertyChanged();} } }
	private DateTime _added;
	public DateTime added { get => _added; set { if (_added != value) { _added = value; NotifyPropertyChanged();} } }
	private string _add_user = null!;
	public required string add_user { get => _add_user; set { if (_add_user != value) { _add_user = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}