namespace VulpesX.Models.Default;
 
public partial class MCOTB00F : Base 
{
	private string _McoSoc = null!;
	public required string McoSoc { get => _McoSoc; set { if (_McoSoc != value) { _McoSoc = value; NotifyPropertyChanged();} } }
	private string _McoNum = null!;
	public required string McoNum { get => _McoNum; set { if (_McoNum != value) { _McoNum = value; NotifyPropertyChanged();} } }
	private int? _McoAnn;
	public int? McoAnn { get => _McoAnn; set { if (_McoAnn != value) { _McoAnn = value; NotifyPropertyChanged();} } }
	private DateTime? _McoDip;
	public DateTime? McoDip { get => _McoDip; set { if (_McoDip != value) { _McoDip = value; NotifyPropertyChanged();} } }
	private DateTime? _McoDie;
	public DateTime? McoDie { get => _McoDie; set { if (_McoDie != value) { _McoDie = value; NotifyPropertyChanged();} } }
	private DateTime? _McoDfp;
	public DateTime? McoDfp { get => _McoDfp; set { if (_McoDfp != value) { _McoDfp = value; NotifyPropertyChanged();} } }
	private DateTime? _McoDfe;
	public DateTime? McoDfe { get => _McoDfe; set { if (_McoDfe != value) { _McoDfe = value; NotifyPropertyChanged();} } }
	private string? _McoRes;
	public string? McoRes { get => _McoRes; set { if (_McoRes != value) { _McoRes = value; NotifyPropertyChanged();} } }
	private string? _McoTip;
	public string? McoTip { get => _McoTip; set { if (_McoTip != value) { _McoTip = value; NotifyPropertyChanged();} } }
	private string? _McoNot;
	public string? McoNot { get => _McoNot; set { if (_McoNot != value) { _McoNot = value; NotifyPropertyChanged();} } }
	private string? _McoSta;
	public string? McoSta { get => _McoSta; set { if (_McoSta != value) { _McoSta = value; NotifyPropertyChanged();} } }
	private int? _McoAbeCod;
	public int? McoAbeCod { get => _McoAbeCod; set { if (_McoAbeCod != value) { _McoAbeCod = value; NotifyPropertyChanged();} } }
}