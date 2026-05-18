namespace VulpesX.Models.Default;
 
public partial class INVENTARIO : Base 
{
	private string _Invesoc = null!;
	public required string Invesoc { get => _Invesoc; set { if (_Invesoc != value) { _Invesoc = value; NotifyPropertyChanged();} } }
	private DateTime _invedai;
	public DateTime invedai { get => _invedai; set { if (_invedai != value) { _invedai = value; NotifyPropertyChanged();} } }
	private string _invemag = null!;
	public required string invemag { get => _invemag; set { if (_invemag != value) { _invemag = value; NotifyPropertyChanged();} } }
	private string _inveart = null!;
	public required string inveart { get => _inveart; set { if (_inveart != value) { _inveart = value; NotifyPropertyChanged();} } }
	private decimal? _inveqtae;
	public decimal? inveqtae { get => _inveqtae; set { if (_inveqtae != value) { _inveqtae = value; NotifyPropertyChanged();} } }
	private decimal? _inveqtar;
	public decimal? inveqtar { get => _inveqtar; set { if (_inveqtar != value) { _inveqtar = value; NotifyPropertyChanged();} } }
	private decimal? _invedif;
	public decimal? invedif { get => _invedif; set { if (_invedif != value) { _invedif = value; NotifyPropertyChanged();} } }
	private string? _inveseg;
	public string? inveseg { get => _inveseg; set { if (_inveseg != value) { _inveseg = value; NotifyPropertyChanged();} } }
	private int? _inveann;
	public int? inveann { get => _inveann; set { if (_inveann != value) { _inveann = value; NotifyPropertyChanged();} } }
	private DateTime? _invedac;
	public DateTime? invedac { get => _invedac; set { if (_invedac != value) { _invedac = value; NotifyPropertyChanged();} } }
	private int? _invenur;
	public int? invenur { get => _invenur; set { if (_invenur != value) { _invenur = value; NotifyPropertyChanged();} } }
	private string? _inveflg1;
	public string? inveflg1 { get => _inveflg1; set { if (_inveflg1 != value) { _inveflg1 = value; NotifyPropertyChanged();} } }
	private string? _inveflg2;
	public string? inveflg2 { get => _inveflg2; set { if (_inveflg2 != value) { _inveflg2 = value; NotifyPropertyChanged();} } }
	private decimal? _inveqtse;
	public decimal? inveqtse { get => _inveqtse; set { if (_inveqtse != value) { _inveqtse = value; NotifyPropertyChanged();} } }
	private decimal? _inveqtsr;
	public decimal? inveqtsr { get => _inveqtsr; set { if (_inveqtsr != value) { _inveqtsr = value; NotifyPropertyChanged();} } }
	private decimal? _invedifs;
	public decimal? invedifs { get => _invedifs; set { if (_invedifs != value) { _invedifs = value; NotifyPropertyChanged();} } }
}