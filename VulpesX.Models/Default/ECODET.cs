namespace VulpesX.Models.Default;
 
public partial class ECODET : Base 
{
	private string _ecosoc = null!;
	public required string ecosoc { get => _ecosoc; set { if (_ecosoc != value) { _ecosoc = value; NotifyPropertyChanged();} } }
	private int _ecocod;
	public int ecocod { get => _ecocod; set { if (_ecocod != value) { _ecocod = value; NotifyPropertyChanged();} } }
	private int _detpos;
	public int detpos { get => _detpos; set { if (_detpos != value) { _detpos = value; NotifyPropertyChanged();} } }
	private int? _detcod;
	public int? detcod { get => _detcod; set { if (_detcod != value) { _detcod = value; NotifyPropertyChanged();} } }
	private decimal? _detprl;
	public decimal? detprl { get => _detprl; set { if (_detprl != value) { _detprl = value; NotifyPropertyChanged();} } }
	private decimal? _detpre;
	public decimal? detpre { get => _detpre; set { if (_detpre != value) { _detpre = value; NotifyPropertyChanged();} } }
	private decimal? _detpel;
	public decimal? detpel { get => _detpel; set { if (_detpel != value) { _detpel = value; NotifyPropertyChanged();} } }
	private decimal? _detpee;
	public decimal? detpee { get => _detpee; set { if (_detpee != value) { _detpee = value; NotifyPropertyChanged();} } }
	private decimal? _detfil;
	public decimal? detfil { get => _detfil; set { if (_detfil != value) { _detfil = value; NotifyPropertyChanged();} } }
	private decimal? _detfie;
	public decimal? detfie { get => _detfie; set { if (_detfie != value) { _detfie = value; NotifyPropertyChanged();} } }
	private decimal? _detp01;
	public decimal? detp01 { get => _detp01; set { if (_detp01 != value) { _detp01 = value; NotifyPropertyChanged();} } }
	private decimal? _detp02;
	public decimal? detp02 { get => _detp02; set { if (_detp02 != value) { _detp02 = value; NotifyPropertyChanged();} } }
	private decimal? _detp03;
	public decimal? detp03 { get => _detp03; set { if (_detp03 != value) { _detp03 = value; NotifyPropertyChanged();} } }
	private decimal? _detbul;
	public decimal? detbul { get => _detbul; set { if (_detbul != value) { _detbul = value; NotifyPropertyChanged();} } }
	private decimal? _detbue;
	public decimal? detbue { get => _detbue; set { if (_detbue != value) { _detbue = value; NotifyPropertyChanged();} } }
	private decimal? _detp04;
	public decimal? detp04 { get => _detp04; set { if (_detp04 != value) { _detp04 = value; NotifyPropertyChanged();} } }
	private int? _PCFLAG;
	public int? PCFLAG { get => _PCFLAG; set { if (_PCFLAG != value) { _PCFLAG = value; NotifyPropertyChanged();} } }
	private string? _detso1;
	public string? detso1 { get => _detso1; set { if (_detso1 != value) { _detso1 = value; NotifyPropertyChanged();} } }
}