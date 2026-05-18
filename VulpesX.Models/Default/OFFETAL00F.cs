namespace VulpesX.Models.Default;
 
public partial class OFFETAL00F : Base 
{
	private string _oftasoci = null!;
	public required string oftasoci { get => _oftasoci; set { if (_oftasoci != value) { _oftasoci = value; NotifyPropertyChanged();} } }
	private int _OFTAANNO;
	public int OFTAANNO { get => _OFTAANNO; set { if (_OFTAANNO != value) { _OFTAANNO = value; NotifyPropertyChanged();} } }
	private int _OFTANUOR;
	public int OFTANUOR { get => _OFTANUOR; set { if (_OFTANUOR != value) { _OFTANUOR = value; NotifyPropertyChanged();} } }
	private Guid _OFTAUID;
	public Guid OFTAUID { get => _OFTAUID; set { if (_OFTAUID != value) { _OFTAUID = value; NotifyPropertyChanged();} } }
	private string _OFTANAME = null!;
	public required string OFTANAME { get => _OFTANAME; set { if (_OFTANAME != value) { _OFTANAME = value; NotifyPropertyChanged();} } }
	private long? _OFTASIZE;
	public long? OFTASIZE { get => _OFTASIZE; set { if (_OFTASIZE != value) { _OFTASIZE = value; NotifyPropertyChanged();} } }
	private DateTime _added;
	public DateTime added { get => _added; set { if (_added != value) { _added = value; NotifyPropertyChanged();} } }
	private string _add_user = null!;
	public required string add_user { get => _add_user; set { if (_add_user != value) { _add_user = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}