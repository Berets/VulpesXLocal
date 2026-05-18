namespace VulpesX.Models.Default;
 
public partial class CTTCO00F : Base 
{
	private string _CttNum = null!;
	public required string CttNum { get => _CttNum; set { if (_CttNum != value) { _CttNum = value; NotifyPropertyChanged();} } }
	private int? _CLIENT;
	public int? CLIENT { get => _CLIENT; set { if (_CLIENT != value) { _CLIENT = value; NotifyPropertyChanged();} } }
	private DateTime? _CttDat;
	public DateTime? CttDat { get => _CttDat; set { if (_CttDat != value) { _CttDat = value; NotifyPropertyChanged();} } }
	private int? _CttAdc;
	public int? CttAdc { get => _CttAdc; set { if (_CttAdc != value) { _CttAdc = value; NotifyPropertyChanged();} } }
	private int? _CttMdc;
	public int? CttMdc { get => _CttMdc; set { if (_CttMdc != value) { _CttMdc = value; NotifyPropertyChanged();} } }
	private string? _CttNot;
	public string? CttNot { get => _CttNot; set { if (_CttNot != value) { _CttNot = value; NotifyPropertyChanged();} } }
	private string? _CttCdz;
	public string? CttCdz { get => _CttCdz; set { if (_CttCdz != value) { _CttCdz = value; NotifyPropertyChanged();} } }
	private string? _CttValcod;
	public string? CttValcod { get => _CttValcod; set { if (_CttValcod != value) { _CttValcod = value; NotifyPropertyChanged();} } }
	private string? _CttValdiv;
	public string? CttValdiv { get => _CttValdiv; set { if (_CttValdiv != value) { _CttValdiv = value; NotifyPropertyChanged();} } }
	private string? _CttAtt;
	public string? CttAtt { get => _CttAtt; set { if (_CttAtt != value) { _CttAtt = value; NotifyPropertyChanged();} } }
	private string? _CttTpc;
	public string? CttTpc { get => _CttTpc; set { if (_CttTpc != value) { _CttTpc = value; NotifyPropertyChanged();} } }
	private string? _cttpag;
	public string? cttpag { get => _cttpag; set { if (_cttpag != value) { _cttpag = value; NotifyPropertyChanged();} } }
	private int? _cttnupr;
	public int? cttnupr { get => _cttnupr; set { if (_cttnupr != value) { _cttnupr = value; NotifyPropertyChanged();} } }
	private int? _cttanpr;
	public int? cttanpr { get => _cttanpr; set { if (_cttanpr != value) { _cttanpr = value; NotifyPropertyChanged();} } }
}