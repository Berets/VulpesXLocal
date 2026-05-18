namespace VulpesX.Models.Default;
 
public partial class TPERSONALEDETTAGLIO : Base 
{
	private string _prssoc = null!;
	public required string prssoc { get => _prssoc; set { if (_prssoc != value) { _prssoc = value; NotifyPropertyChanged();} } }
	private string _prsid1 = null!;
	public required string prsid1 { get => _prsid1; set { if (_prsid1 != value) { _prsid1 = value; NotifyPropertyChanged();} } }
	private string _prsid2 = null!;
	public required string prsid2 { get => _prsid2; set { if (_prsid2 != value) { _prsid2 = value; NotifyPropertyChanged();} } }
	private string _prsid3 = null!;
	public required string prsid3 { get => _prsid3; set { if (_prsid3 != value) { _prsid3 = value; NotifyPropertyChanged();} } }
	private int _prsanno;
	public int prsanno { get => _prsanno; set { if (_prsanno != value) { _prsanno = value; NotifyPropertyChanged();} } }
	private int _prsmese;
	public int prsmese { get => _prsmese; set { if (_prsmese != value) { _prsmese = value; NotifyPropertyChanged();} } }
	private decimal? _prsaccd;
	public decimal? prsaccd { get => _prsaccd; set { if (_prsaccd != value) { _prsaccd = value; NotifyPropertyChanged();} } }
	private decimal? _prsstid;
	public decimal? prsstid { get => _prsstid; set { if (_prsstid != value) { _prsstid = value; NotifyPropertyChanged();} } }
	private decimal? _prstfr;
	public decimal? prstfr { get => _prstfr; set { if (_prstfr != value) { _prstfr = value; NotifyPropertyChanged();} } }
	private string? _prsmesedes;
	public string? prsmesedes { get => _prsmesedes; set { if (_prsmesedes != value) { _prsmesedes = value; NotifyPropertyChanged();} } }
}