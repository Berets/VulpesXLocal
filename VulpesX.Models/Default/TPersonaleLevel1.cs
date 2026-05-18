namespace VulpesX.Models.Default;
 
public partial class TPersonaleLevel1 : Base 
{
	private string _prssoc = null!;
	public required string prssoc { get => _prssoc; set { if (_prssoc != value) { _prssoc = value; NotifyPropertyChanged();} } }
	private string _prsid1 = null!;
	public required string prsid1 { get => _prsid1; set { if (_prsid1 != value) { _prsid1 = value; NotifyPropertyChanged();} } }
	private string _prsid2 = null!;
	public required string prsid2 { get => _prsid2; set { if (_prsid2 != value) { _prsid2 = value; NotifyPropertyChanged();} } }
	private string _prsid3 = null!;
	public required string prsid3 { get => _prsid3; set { if (_prsid3 != value) { _prsid3 = value; NotifyPropertyChanged();} } }
	private int _prsann;
	public int prsann { get => _prsann; set { if (_prsann != value) { _prsann = value; NotifyPropertyChanged();} } }
	private int _prsmes;
	public int prsmes { get => _prsmes; set { if (_prsmes != value) { _prsmes = value; NotifyPropertyChanged();} } }
	private int _prsprog;
	public int prsprog { get => _prsprog; set { if (_prsprog != value) { _prsprog = value; NotifyPropertyChanged();} } }
	private decimal? _prsaccmes;
	public decimal? prsaccmes { get => _prsaccmes; set { if (_prsaccmes != value) { _prsaccmes = value; NotifyPropertyChanged();} } }
	private decimal? _prsstimes;
	public decimal? prsstimes { get => _prsstimes; set { if (_prsstimes != value) { _prsstimes = value; NotifyPropertyChanged();} } }
	private int? _prsnanno;
	public int? prsnanno { get => _prsnanno; set { if (_prsnanno != value) { _prsnanno = value; NotifyPropertyChanged();} } }
	private int? _persnregi;
	public int? persnregi { get => _persnregi; set { if (_persnregi != value) { _persnregi = value; NotifyPropertyChanged();} } }
	private string? _prsstato;
	public string? prsstato { get => _prsstato; set { if (_prsstato != value) { _prsstato = value; NotifyPropertyChanged();} } }
}