namespace VulpesX.Models.Default;
 
public partial class CGIMP00F : Base 
{
	private string _IMPSOC = null!;
	public required string IMPSOC { get => _IMPSOC; set { if (_IMPSOC != value) { _IMPSOC = value; NotifyPropertyChanged();} } }
	private int _IMPCOD;
	public int IMPCOD { get => _IMPCOD; set { if (_IMPCOD != value) { _IMPCOD = value; NotifyPropertyChanged();} } }
	private string? _IMPDES;
	public string? IMPDES { get => _IMPDES; set { if (_IMPDES != value) { _IMPDES = value; NotifyPropertyChanged();} } }
	private string? _IMPGRU;
	public string? IMPGRU { get => _IMPGRU; set { if (_IMPGRU != value) { _IMPGRU = value; NotifyPropertyChanged();} } }
	private string? _IMPCON;
	public string? IMPCON { get => _IMPCON; set { if (_IMPCON != value) { _IMPCON = value; NotifyPropertyChanged();} } }
	private string? _IMPSOT;
	public string? IMPSOT { get => _IMPSOT; set { if (_IMPSOT != value) { _IMPSOT = value; NotifyPropertyChanged();} } }
	private string? _IMPFLA;
	public string? IMPFLA { get => _IMPFLA; set { if (_IMPFLA != value) { _IMPFLA = value; NotifyPropertyChanged();} } }
	private string? _IMPIRA;
	public string? IMPIRA { get => _IMPIRA; set { if (_IMPIRA != value) { _IMPIRA = value; NotifyPropertyChanged();} } }
	private string? _IMPIRP;
	public string? IMPIRP { get => _IMPIRP; set { if (_IMPIRP != value) { _IMPIRP = value; NotifyPropertyChanged();} } }
	private string? _IMPSEG;
	public string? IMPSEG { get => _IMPSEG; set { if (_IMPSEG != value) { _IMPSEG = value; NotifyPropertyChanged();} } }
	private decimal? _IMPP01;
	public decimal? IMPP01 { get => _IMPP01; set { if (_IMPP01 != value) { _IMPP01 = value; NotifyPropertyChanged();} } }
	private decimal? _IMPP02;
	public decimal? IMPP02 { get => _IMPP02; set { if (_IMPP02 != value) { _IMPP02 = value; NotifyPropertyChanged();} } }
	private decimal? _IMPPRL;
	public decimal? IMPPRL { get => _IMPPRL; set { if (_IMPPRL != value) { _IMPPRL = value; NotifyPropertyChanged();} } }
	private decimal? _IMPPRE;
	public decimal? IMPPRE { get => _IMPPRE; set { if (_IMPPRE != value) { _IMPPRE = value; NotifyPropertyChanged();} } }
	private decimal? _IMPPEL;
	public decimal? IMPPEL { get => _IMPPEL; set { if (_IMPPEL != value) { _IMPPEL = value; NotifyPropertyChanged();} } }
	private decimal? _IMPPEE;
	public decimal? IMPPEE { get => _IMPPEE; set { if (_IMPPEE != value) { _IMPPEE = value; NotifyPropertyChanged();} } }
	private decimal? _IMPBUL;
	public decimal? IMPBUL { get => _IMPBUL; set { if (_IMPBUL != value) { _IMPBUL = value; NotifyPropertyChanged();} } }
	private decimal? _IMPBUE;
	public decimal? IMPBUE { get => _IMPBUE; set { if (_IMPBUE != value) { _IMPBUE = value; NotifyPropertyChanged();} } }
	private decimal? _IMPGRL;
	public decimal? IMPGRL { get => _IMPGRL; set { if (_IMPGRL != value) { _IMPGRL = value; NotifyPropertyChanged();} } }
	private decimal? _IMPGRE;
	public decimal? IMPGRE { get => _IMPGRE; set { if (_IMPGRE != value) { _IMPGRE = value; NotifyPropertyChanged();} } }
	private decimal? _IMPGEL;
	public decimal? IMPGEL { get => _IMPGEL; set { if (_IMPGEL != value) { _IMPGEL = value; NotifyPropertyChanged();} } }
	private decimal? _IMPGEE;
	public decimal? IMPGEE { get => _IMPGEE; set { if (_IMPGEE != value) { _IMPGEE = value; NotifyPropertyChanged();} } }
	private decimal? _IMPGUL;
	public decimal? IMPGUL { get => _IMPGUL; set { if (_IMPGUL != value) { _IMPGUL = value; NotifyPropertyChanged();} } }
	private decimal? _IMPGUE;
	public decimal? IMPGUE { get => _IMPGUE; set { if (_IMPGUE != value) { _IMPGUE = value; NotifyPropertyChanged();} } }
}