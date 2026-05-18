namespace VulpesX.Models.Default;
 
public partial class CTRA00F : Base 
{
	private string _CttNum = null!;
	public required string CttNum { get => _CttNum; set { if (_CttNum != value) { _CttNum = value; NotifyPropertyChanged();} } }
	private int _ctarig;
	public int ctarig { get => _ctarig; set { if (_ctarig != value) { _ctarig = value; NotifyPropertyChanged();} } }
	private int? _CtrMac;
	public int? CtrMac { get => _CtrMac; set { if (_CtrMac != value) { _CtrMac = value; NotifyPropertyChanged();} } }
	private int? _CtrAac;
	public int? CtrAac { get => _CtrAac; set { if (_CtrAac != value) { _CtrAac = value; NotifyPropertyChanged();} } }
	private DateTime? _CtrDac;
	public DateTime? CtrDac { get => _CtrDac; set { if (_CtrDac != value) { _CtrDac = value; NotifyPropertyChanged();} } }
	private decimal? _CtrCac;
	public decimal? CtrCac { get => _CtrCac; set { if (_CtrCac != value) { _CtrCac = value; NotifyPropertyChanged();} } }
	private decimal? _CtrPve;
	public decimal? CtrPve { get => _CtrPve; set { if (_CtrPve != value) { _CtrPve = value; NotifyPropertyChanged();} } }
	private string? _ARTCOD;
	public string? ARTCOD { get => _ARTCOD; set { if (_ARTCOD != value) { _ARTCOD = value; NotifyPropertyChanged();} } }
	private string? _CtrNot;
	public string? CtrNot { get => _CtrNot; set { if (_CtrNot != value) { _CtrNot = value; NotifyPropertyChanged();} } }
	private int? _ctrrio;
	public int? ctrrio { get => _ctrrio; set { if (_ctrrio != value) { _ctrrio = value; NotifyPropertyChanged();} } }
	private int? _ctrnuo;
	public int? ctrnuo { get => _ctrnuo; set { if (_ctrnuo != value) { _ctrnuo = value; NotifyPropertyChanged();} } }
	private int? _ctrano;
	public int? ctrano { get => _ctrano; set { if (_ctrano != value) { _ctrano = value; NotifyPropertyChanged();} } }
	private string? _ctrnew;
	public string? ctrnew { get => _ctrnew; set { if (_ctrnew != value) { _ctrnew = value; NotifyPropertyChanged();} } }
	private int? _ctanum;
	public int? ctanum { get => _ctanum; set { if (_ctanum != value) { _ctanum = value; NotifyPropertyChanged();} } }
	private string? _ctaval;
	public string? ctaval { get => _ctaval; set { if (_ctaval != value) { _ctaval = value; NotifyPropertyChanged();} } }
	private string? _ctadiv;
	public string? ctadiv { get => _ctadiv; set { if (_ctadiv != value) { _ctadiv = value; NotifyPropertyChanged();} } }
	private decimal? _ctacam;
	public decimal? ctacam { get => _ctacam; set { if (_ctacam != value) { _ctacam = value; NotifyPropertyChanged();} } }
}