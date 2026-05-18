namespace VulpesX.Models.Default;
 
public partial class TIPPREBL : Base 
{
	private string _tipblsoc = null!;
	public required string tipblsoc { get => _tipblsoc; set { if (_tipblsoc != value) { _tipblsoc = value; NotifyPropertyChanged();} } }
	private string _tipblcod = null!;
	public required string tipblcod { get => _tipblcod; set { if (_tipblcod != value) { _tipblcod = value; NotifyPropertyChanged();} } }
	private string? _tipblper;
	public string? tipblper { get => _tipblper; set { if (_tipblper != value) { _tipblper = value; NotifyPropertyChanged();} } }
	private string? _tipblnom;
	public string? tipblnom { get => _tipblnom; set { if (_tipblnom != value) { _tipblnom = value; NotifyPropertyChanged();} } }
	private int? _tipblann;
	public int? tipblann { get => _tipblann; set { if (_tipblann != value) { _tipblann = value; NotifyPropertyChanged();} } }
	private int? _tipblpue;
	public int? tipblpue { get => _tipblpue; set { if (_tipblpue != value) { _tipblpue = value; NotifyPropertyChanged();} } }
	private string? _tipblcfi;
	public string? tipblcfi { get => _tipblcfi; set { if (_tipblcfi != value) { _tipblcfi = value; NotifyPropertyChanged();} } }
	private DateTime? _tipbldti;
	public DateTime? tipbldti { get => _tipbldti; set { if (_tipbldti != value) { _tipbldti = value; NotifyPropertyChanged();} } }
	private string? _tipblnia;
	public string? tipblnia { get => _tipblnia; set { if (_tipblnia != value) { _tipblnia = value; NotifyPropertyChanged();} } }
	private string _tipbluso = null!;
	public required string tipbluso { get => _tipbluso; set { if (_tipbluso != value) { _tipbluso = value; NotifyPropertyChanged();} } }
}