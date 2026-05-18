namespace VulpesX.Models.Default;
 
public partial class FIRR : Base 
{
	private int _fAnno;
	public int fAnno { get => _fAnno; set { if (_fAnno != value) { _fAnno = value; NotifyPropertyChanged();} } }
	private string _fTipoage = null!;
	public required string fTipoage { get => _fTipoage; set { if (_fTipoage != value) { _fTipoage = value; NotifyPropertyChanged();} } }
	private decimal _fPerc1;
	public decimal fPerc1 { get => _fPerc1; set { if (_fPerc1 != value) { _fPerc1 = value; NotifyPropertyChanged();} } }
	private decimal _fImp1;
	public decimal fImp1 { get => _fImp1; set { if (_fImp1 != value) { _fImp1 = value; NotifyPropertyChanged();} } }
	private decimal _fPerc2;
	public decimal fPerc2 { get => _fPerc2; set { if (_fPerc2 != value) { _fPerc2 = value; NotifyPropertyChanged();} } }
	private decimal _fImp2;
	public decimal fImp2 { get => _fImp2; set { if (_fImp2 != value) { _fImp2 = value; NotifyPropertyChanged();} } }
	private decimal _fPerc3;
	public decimal fPerc3 { get => _fPerc3; set { if (_fPerc3 != value) { _fPerc3 = value; NotifyPropertyChanged();} } }
	private decimal _fImp3;
	public decimal fImp3 { get => _fImp3; set { if (_fImp3 != value) { _fImp3 = value; NotifyPropertyChanged();} } }
	private decimal _fPerc4;
	public decimal fPerc4 { get => _fPerc4; set { if (_fPerc4 != value) { _fPerc4 = value; NotifyPropertyChanged();} } }
	private decimal _fImp4;
	public decimal fImp4 { get => _fImp4; set { if (_fImp4 != value) { _fImp4 = value; NotifyPropertyChanged();} } }
}