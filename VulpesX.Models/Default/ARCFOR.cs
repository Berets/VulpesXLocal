namespace VulpesX.Models.Default;
 
public partial class ARCFOR : Base 
{
	private string _Arfsoc = null!;
	public required string Arfsoc { get => _Arfsoc; set { if (_Arfsoc != value) { _Arfsoc = value; NotifyPropertyChanged();} } }
	private int _arffor;
	public int arffor { get => _arffor; set { if (_arffor != value) { _arffor = value; NotifyPropertyChanged();} } }
	private string _arftip = null!;
	public required string arftip { get => _arftip; set { if (_arftip != value) { _arftip = value; NotifyPropertyChanged();} } }
	private int _arfanno;
	public int arfanno { get => _arfanno; set { if (_arfanno != value) { _arfanno = value; NotifyPropertyChanged();} } }
	private int _arfrig;
	public int arfrig { get => _arfrig; set { if (_arfrig != value) { _arfrig = value; NotifyPropertyChanged();} } }
	private byte[]? _arfdoc;
	public byte[]? arfdoc { get => _arfdoc; set { if (_arfdoc != value) { _arfdoc = value; NotifyPropertyChanged();} } }
	private DateTime _arfdat;
	public DateTime arfdat { get => _arfdat; set { if (_arfdat != value) { _arfdat = value; NotifyPropertyChanged();} } }
	private string _arfute = null!;
	public required string arfute { get => _arfute; set { if (_arfute != value) { _arfute = value; NotifyPropertyChanged();} } }
	private string _arfdes = null!;
	public required string arfdes { get => _arfdes; set { if (_arfdes != value) { _arfdes = value; NotifyPropertyChanged();} } }
	private string _arfest = null!;
	public required string arfest { get => _arfest; set { if (_arfest != value) { _arfest = value; NotifyPropertyChanged();} } }
	private string _arfper = null!;
	public required string arfper { get => _arfper; set { if (_arfper != value) { _arfper = value; NotifyPropertyChanged();} } }
}