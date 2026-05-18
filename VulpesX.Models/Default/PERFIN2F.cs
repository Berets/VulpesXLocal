namespace VulpesX.Models.Default;
 
public partial class PERFIN2F : Base 
{
	private string _FINSOC = null!;
	public required string FINSOC { get => _FINSOC; set { if (_FINSOC != value) { _FINSOC = value; NotifyPropertyChanged();} } }
	private string _FINCOD = null!;
	public required string FINCOD { get => _FINCOD; set { if (_FINCOD != value) { _FINCOD = value; NotifyPropertyChanged();} } }
	private int _FINRIG;
	public int FINRIG { get => _FINRIG; set { if (_FINRIG != value) { _FINRIG = value; NotifyPropertyChanged();} } }
	private string _FINTCF = null!;
	public required string FINTCF { get => _FINTCF; set { if (_FINTCF != value) { _FINTCF = value; NotifyPropertyChanged();} } }
	private int _FINCOF;
	public int FINCOF { get => _FINCOF; set { if (_FINCOF != value) { _FINCOF = value; NotifyPropertyChanged();} } }
	private string _FINDEF = null!;
	public required string FINDEF { get => _FINDEF; set { if (_FINDEF != value) { _FINDEF = value; NotifyPropertyChanged();} } }
}