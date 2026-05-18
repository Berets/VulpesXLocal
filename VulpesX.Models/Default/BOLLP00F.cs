namespace VulpesX.Models.Default;
 
public partial class BOLLP00F : Base 
{
	private string _bolsoc = null!;
	public required string bolsoc { get => _bolsoc; set { if (_bolsoc != value) { _bolsoc = value; NotifyPropertyChanged();} } }
	private int _BTANNO;
	public int BTANNO { get => _BTANNO; set { if (_BTANNO != value) { _BTANNO = value; NotifyPropertyChanged();} } }
	private int _BTBOLL;
	public int BTBOLL { get => _BTBOLL; set { if (_BTBOLL != value) { _BTBOLL = value; NotifyPropertyChanged();} } }
	private int _bocass;
	public int bocass { get => _bocass; set { if (_bocass != value) { _bocass = value; NotifyPropertyChanged();} } }
	private string? _BPFL01;
	public string? BPFL01 { get => _BPFL01; set { if (_BPFL01 != value) { _BPFL01 = value; NotifyPropertyChanged();} } }
	private decimal? _BPPESO;
	public decimal? BPPESO { get => _BPPESO; set { if (_BPPESO != value) { _BPPESO = value; NotifyPropertyChanged();} } }
	private decimal? _BPPES2;
	public decimal? BPPES2 { get => _BPPES2; set { if (_BPPES2 != value) { _BPPES2 = value; NotifyPropertyChanged();} } }
	private string? _BPDESC;
	public string? BPDESC { get => _BPDESC; set { if (_BPDESC != value) { _BPDESC = value; NotifyPropertyChanged();} } }
	private int? _BPNUME;
	public int? BPNUME { get => _BPNUME; set { if (_BPNUME != value) { _BPNUME = value; NotifyPropertyChanged();} } }
	private int? _bpdim1;
	public int? bpdim1 { get => _bpdim1; set { if (_bpdim1 != value) { _bpdim1 = value; NotifyPropertyChanged();} } }
	private int? _bpdim2;
	public int? bpdim2 { get => _bpdim2; set { if (_bpdim2 != value) { _bpdim2 = value; NotifyPropertyChanged();} } }
	private int? _bpdim3;
	public int? bpdim3 { get => _bpdim3; set { if (_bpdim3 != value) { _bpdim3 = value; NotifyPropertyChanged();} } }
}