namespace VulpesX.Models.Default;
 
public partial class PERFIN1F : Base 
{
	private string _FINSOC = null!;
	public required string FINSOC { get => _FINSOC; set { if (_FINSOC != value) { _FINSOC = value; NotifyPropertyChanged();} } }
	private string _FINCOD = null!;
	public required string FINCOD { get => _FINCOD; set { if (_FINCOD != value) { _FINCOD = value; NotifyPropertyChanged();} } }
	private string _FINCAU = null!;
	public required string FINCAU { get => _FINCAU; set { if (_FINCAU != value) { _FINCAU = value; NotifyPropertyChanged();} } }
	private string _FINTIP = null!;
	public required string FINTIP { get => _FINTIP; set { if (_FINTIP != value) { _FINTIP = value; NotifyPropertyChanged();} } }
	private string _FINCAD = null!;
	public required string FINCAD { get => _FINCAD; set { if (_FINCAD != value) { _FINCAD = value; NotifyPropertyChanged();} } }
}