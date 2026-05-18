namespace VulpesX.Models.Default;
 
public partial class FATTAL00F : Base 
{
	private string _FTASOCI = null!;
	public required string FTASOCI { get => _FTASOCI; set { if (_FTASOCI != value) { _FTASOCI = value; NotifyPropertyChanged();} } }
	private int _FTAANNO;
	public int FTAANNO { get => _FTAANNO; set { if (_FTAANNO != value) { _FTAANNO = value; NotifyPropertyChanged();} } }
	private int _FTANUOR;
	public int FTANUOR { get => _FTANUOR; set { if (_FTANUOR != value) { _FTANUOR = value; NotifyPropertyChanged();} } }
	private Guid _FTAUID;
	public Guid FTAUID { get => _FTAUID; set { if (_FTAUID != value) { _FTAUID = value; NotifyPropertyChanged();} } }
	private string _FTANAME = null!;
	public required string FTANAME { get => _FTANAME; set { if (_FTANAME != value) { _FTANAME = value; NotifyPropertyChanged();} } }
	private long? _FTASIZE;
	public long? FTASIZE { get => _FTASIZE; set { if (_FTASIZE != value) { _FTASIZE = value; NotifyPropertyChanged();} } }
	private DateTime _added;
	public DateTime added { get => _added; set { if (_added != value) { _added = value; NotifyPropertyChanged();} } }
	private string _add_user = null!;
	public required string add_user { get => _add_user; set { if (_add_user != value) { _add_user = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}