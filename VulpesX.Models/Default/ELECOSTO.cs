namespace VulpesX.Models.Default;
 
public partial class ELECOSTO : Base 
{
	private string _ecSoc = null!;
	public required string ecSoc { get => _ecSoc; set { if (_ecSoc != value) { _ecSoc = value; NotifyPropertyChanged();} } }
	private int _ecProg;
	public int ecProg { get => _ecProg; set { if (_ecProg != value) { _ecProg = value; NotifyPropertyChanged();} } }
	private int _ecAnno;
	public int ecAnno { get => _ecAnno; set { if (_ecAnno != value) { _ecAnno = value; NotifyPropertyChanged();} } }
	private string _ecDocu = null!;
	public required string ecDocu { get => _ecDocu; set { if (_ecDocu != value) { _ecDocu = value; NotifyPropertyChanged();} } }
	private DateTime _ecDaDo;
	public DateTime ecDaDo { get => _ecDaDo; set { if (_ecDaDo != value) { _ecDaDo = value; NotifyPropertyChanged();} } }
	private int _ecArCo;
	public int ecArCo { get => _ecArCo; set { if (_ecArCo != value) { _ecArCo = value; NotifyPropertyChanged();} } }
	private int _ecNrCo;
	public int ecNrCo { get => _ecNrCo; set { if (_ecNrCo != value) { _ecNrCo = value; NotifyPropertyChanged();} } }
	private decimal _ecImpCost;
	public decimal ecImpCost { get => _ecImpCost; set { if (_ecImpCost != value) { _ecImpCost = value; NotifyPropertyChanged();} } }
	private string _ecVdSC = null!;
	public required string ecVdSC { get => _ecVdSC; set { if (_ecVdSC != value) { _ecVdSC = value; NotifyPropertyChanged();} } }
	private string _ecCdCo = null!;
	public required string ecCdCo { get => _ecCdCo; set { if (_ecCdCo != value) { _ecCdCo = value; NotifyPropertyChanged();} } }
	private int _ecMdCo;
	public int ecMdCo { get => _ecMdCo; set { if (_ecMdCo != value) { _ecMdCo = value; NotifyPropertyChanged();} } }
	private int _ecRigpn;
	public int ecRigpn { get => _ecRigpn; set { if (_ecRigpn != value) { _ecRigpn = value; NotifyPropertyChanged();} } }
	private string _ecSegno = null!;
	public required string ecSegno { get => _ecSegno; set { if (_ecSegno != value) { _ecSegno = value; NotifyPropertyChanged();} } }
}