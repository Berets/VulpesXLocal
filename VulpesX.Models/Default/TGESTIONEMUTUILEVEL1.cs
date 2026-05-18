namespace VulpesX.Models.Default;
 
public partial class TGESTIONEMUTUILEVEL1 : Base 
{
	private string _mutsoc = null!;
	public required string mutsoc { get => _mutsoc; set { if (_mutsoc != value) { _mutsoc = value; NotifyPropertyChanged();} } }
	private int _mutnum;
	public int mutnum { get => _mutnum; set { if (_mutnum != value) { _mutnum = value; NotifyPropertyChanged();} } }
	private DateTime _mutscad;
	public DateTime mutscad { get => _mutscad; set { if (_mutscad != value) { _mutscad = value; NotifyPropertyChanged();} } }
	private decimal? _mutirat;
	public decimal? mutirat { get => _mutirat; set { if (_mutirat != value) { _mutirat = value; NotifyPropertyChanged();} } }
	private decimal? _mutcap;
	public decimal? mutcap { get => _mutcap; set { if (_mutcap != value) { _mutcap = value; NotifyPropertyChanged();} } }
	private decimal? _mutint;
	public decimal? mutint { get => _mutint; set { if (_mutint != value) { _mutint = value; NotifyPropertyChanged();} } }
	private decimal? _mutdeb;
	public decimal? mutdeb { get => _mutdeb; set { if (_mutdeb != value) { _mutdeb = value; NotifyPropertyChanged();} } }
	private DateTime? _mutdpt;
	public DateTime? mutdpt { get => _mutdpt; set { if (_mutdpt != value) { _mutdpt = value; NotifyPropertyChanged();} } }
	private int? _mutnumr;
	public int? mutnumr { get => _mutnumr; set { if (_mutnumr != value) { _mutnumr = value; NotifyPropertyChanged();} } }
	private string? _mutconta;
	public string? mutconta { get => _mutconta; set { if (_mutconta != value) { _mutconta = value; NotifyPropertyChanged();} } }
	private decimal? _mutspese;
	public decimal? mutspese { get => _mutspese; set { if (_mutspese != value) { _mutspese = value; NotifyPropertyChanged();} } }
}