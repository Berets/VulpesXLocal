namespace VulpesX.Models.Default;
 
public partial class COSTIEXTRALEVEL1 : Base 
{
	private string _cecsoc = null!;
	public required string cecsoc { get => _cecsoc; set { if (_cecsoc != value) { _cecsoc = value; NotifyPropertyChanged();} } }
	private string _ceccod = null!;
	public required string ceccod { get => _ceccod; set { if (_ceccod != value) { _ceccod = value; NotifyPropertyChanged();} } }
	private int _cecsede;
	public int cecsede { get => _cecsede; set { if (_cecsede != value) { _cecsede = value; NotifyPropertyChanged();} } }
	private int _cecanno;
	public int cecanno { get => _cecanno; set { if (_cecanno != value) { _cecanno = value; NotifyPropertyChanged();} } }
	private int _cecmese;
	public int cecmese { get => _cecmese; set { if (_cecmese != value) { _cecmese = value; NotifyPropertyChanged();} } }
	private decimal _cecimpo1;
	public decimal cecimpo1 { get => _cecimpo1; set { if (_cecimpo1 != value) { _cecimpo1 = value; NotifyPropertyChanged();} } }
	private decimal _cecimpo2;
	public decimal cecimpo2 { get => _cecimpo2; set { if (_cecimpo2 != value) { _cecimpo2 = value; NotifyPropertyChanged();} } }
	private decimal _cecimpoSS;
	public decimal cecimpoSS { get => _cecimpoSS; set { if (_cecimpoSS != value) { _cecimpoSS = value; NotifyPropertyChanged();} } }
	private decimal _cecimpoCI;
	public decimal cecimpoCI { get => _cecimpoCI; set { if (_cecimpoCI != value) { _cecimpoCI = value; NotifyPropertyChanged();} } }
	private decimal _cecimpoC2;
	public decimal cecimpoC2 { get => _cecimpoC2; set { if (_cecimpoC2 != value) { _cecimpoC2 = value; NotifyPropertyChanged();} } }
	private decimal _cecimpoC3;
	public decimal cecimpoC3 { get => _cecimpoC3; set { if (_cecimpoC3 != value) { _cecimpoC3 = value; NotifyPropertyChanged();} } }
	private decimal _cecimpoC4;
	public decimal cecimpoC4 { get => _cecimpoC4; set { if (_cecimpoC4 != value) { _cecimpoC4 = value; NotifyPropertyChanged();} } }
	private decimal _cecimpoDI;
	public decimal cecimpoDI { get => _cecimpoDI; set { if (_cecimpoDI != value) { _cecimpoDI = value; NotifyPropertyChanged();} } }
	private decimal _cecimpoCO;
	public decimal cecimpoCO { get => _cecimpoCO; set { if (_cecimpoCO != value) { _cecimpoCO = value; NotifyPropertyChanged();} } }
	private decimal _cecimpoCN;
	public decimal cecimpoCN { get => _cecimpoCN; set { if (_cecimpoCN != value) { _cecimpoCN = value; NotifyPropertyChanged();} } }
	private decimal _cecimpoIN;
	public decimal cecimpoIN { get => _cecimpoIN; set { if (_cecimpoIN != value) { _cecimpoIN = value; NotifyPropertyChanged();} } }
	private decimal _cecimpoAT;
	public decimal cecimpoAT { get => _cecimpoAT; set { if (_cecimpoAT != value) { _cecimpoAT = value; NotifyPropertyChanged();} } }
}