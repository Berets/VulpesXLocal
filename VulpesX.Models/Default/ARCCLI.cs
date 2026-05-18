namespace VulpesX.Models.Default;
 
public partial class ARCCLI : Base 
{
	private string _Arcsoc = null!;
	public required string Arcsoc { get => _Arcsoc; set { if (_Arcsoc != value) { _Arcsoc = value; NotifyPropertyChanged();} } }
	private int _arccli;
	public int arccli { get => _arccli; set { if (_arccli != value) { _arccli = value; NotifyPropertyChanged();} } }
	private string _arctip = null!;
	public required string arctip { get => _arctip; set { if (_arctip != value) { _arctip = value; NotifyPropertyChanged();} } }
	private int _arcanno;
	public int arcanno { get => _arcanno; set { if (_arcanno != value) { _arcanno = value; NotifyPropertyChanged();} } }
	private int _arcrig;
	public int arcrig { get => _arcrig; set { if (_arcrig != value) { _arcrig = value; NotifyPropertyChanged();} } }
	private byte[]? _arcdoc;
	public byte[]? arcdoc { get => _arcdoc; set { if (_arcdoc != value) { _arcdoc = value; NotifyPropertyChanged();} } }
	private DateTime _arcdat;
	public DateTime arcdat { get => _arcdat; set { if (_arcdat != value) { _arcdat = value; NotifyPropertyChanged();} } }
	private string _arcute = null!;
	public required string arcute { get => _arcute; set { if (_arcute != value) { _arcute = value; NotifyPropertyChanged();} } }
	private string _arcdes = null!;
	public required string arcdes { get => _arcdes; set { if (_arcdes != value) { _arcdes = value; NotifyPropertyChanged();} } }
	private string _arcest = null!;
	public required string arcest { get => _arcest; set { if (_arcest != value) { _arcest = value; NotifyPropertyChanged();} } }
	private string _arcper = null!;
	public required string arcper { get => _arcper; set { if (_arcper != value) { _arcper = value; NotifyPropertyChanged();} } }
}