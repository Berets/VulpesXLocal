namespace VulpesX.Models.Default;
 
public partial class ANALISIDATLEVEL1 : Base 
{
	private string _Csoc = null!;
	public required string Csoc { get => _Csoc; set { if (_Csoc != value) { _Csoc = value; NotifyPropertyChanged();} } }
	private string _Cdc = null!;
	public required string Cdc { get => _Cdc; set { if (_Cdc != value) { _Cdc = value; NotifyPropertyChanged();} } }
	private int _CAnno;
	public int CAnno { get => _CAnno; set { if (_CAnno != value) { _CAnno = value; NotifyPropertyChanged();} } }
	private int _CMese;
	public int CMese { get => _CMese; set { if (_CMese != value) { _CMese = value; NotifyPropertyChanged();} } }
	private string _CComm = null!;
	public required string CComm { get => _CComm; set { if (_CComm != value) { _CComm = value; NotifyPropertyChanged();} } }
	private string _CTcom = null!;
	public required string CTcom { get => _CTcom; set { if (_CTcom != value) { _CTcom = value; NotifyPropertyChanged();} } }
	private decimal _COrlav;
	public decimal COrlav { get => _COrlav; set { if (_COrlav != value) { _COrlav = value; NotifyPropertyChanged();} } }
	private decimal _CQnt;
	public decimal CQnt { get => _CQnt; set { if (_CQnt != value) { _CQnt = value; NotifyPropertyChanged();} } }
}