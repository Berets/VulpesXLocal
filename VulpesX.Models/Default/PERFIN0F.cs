namespace VulpesX.Models.Default;
 
public partial class PERFIN0F : Base 
{
	private string _FINSOC = null!;
	public required string FINSOC { get => _FINSOC; set { if (_FINSOC != value) { _FINSOC = value; NotifyPropertyChanged();} } }
	private string _FINCOD = null!;
	public required string FINCOD { get => _FINCOD; set { if (_FINCOD != value) { _FINCOD = value; NotifyPropertyChanged();} } }
	private string _FININ1 = null!;
	public required string FININ1 { get => _FININ1; set { if (_FININ1 != value) { _FININ1 = value; NotifyPropertyChanged();} } }
	private string _FININ2 = null!;
	public required string FININ2 { get => _FININ2; set { if (_FININ2 != value) { _FININ2 = value; NotifyPropertyChanged();} } }
	private string _FININ3 = null!;
	public required string FININ3 { get => _FININ3; set { if (_FININ3 != value) { _FININ3 = value; NotifyPropertyChanged();} } }
	private string _FININ4 = null!;
	public required string FININ4 { get => _FININ4; set { if (_FININ4 != value) { _FININ4 = value; NotifyPropertyChanged();} } }
	private string _FINPA1 = null!;
	public required string FINPA1 { get => _FINPA1; set { if (_FINPA1 != value) { _FINPA1 = value; NotifyPropertyChanged();} } }
	private string _FINPA2 = null!;
	public required string FINPA2 { get => _FINPA2; set { if (_FINPA2 != value) { _FINPA2 = value; NotifyPropertyChanged();} } }
	private string _FINCO1 = null!;
	public required string FINCO1 { get => _FINCO1; set { if (_FINCO1 != value) { _FINCO1 = value; NotifyPropertyChanged();} } }
	private int _FINMAX;
	public int FINMAX { get => _FINMAX; set { if (_FINMAX != value) { _FINMAX = value; NotifyPropertyChanged();} } }
	private string _FINSCA = null!;
	public required string FINSCA { get => _FINSCA; set { if (_FINSCA != value) { _FINSCA = value; NotifyPropertyChanged();} } }
	private string _FINPO1 = null!;
	public required string FINPO1 { get => _FINPO1; set { if (_FINPO1 != value) { _FINPO1 = value; NotifyPropertyChanged();} } }
	private string _FINCO2 = null!;
	public required string FINCO2 { get => _FINCO2; set { if (_FINCO2 != value) { _FINCO2 = value; NotifyPropertyChanged();} } }
}