namespace VulpesX.Models.Default;
 
public partial class STACQ00F : Base 
{
	private string _STQSOC = null!;
	public required string STQSOC { get => _STQSOC; set { if (_STQSOC != value) { _STQSOC = value; NotifyPropertyChanged();} } }
	private int _STQANN;
	public int STQANN { get => _STQANN; set { if (_STQANN != value) { _STQANN = value; NotifyPropertyChanged();} } }
	private int _STQANO;
	public int STQANO { get => _STQANO; set { if (_STQANO != value) { _STQANO = value; NotifyPropertyChanged();} } }
	private int _STQNUO;
	public int STQNUO { get => _STQNUO; set { if (_STQNUO != value) { _STQNUO = value; NotifyPropertyChanged();} } }
	private int _STQPOO;
	public int STQPOO { get => _STQPOO; set { if (_STQPOO != value) { _STQPOO = value; NotifyPropertyChanged();} } }
	private int _STQPRO;
	public int STQPRO { get => _STQPRO; set { if (_STQPRO != value) { _STQPRO = value; NotifyPropertyChanged();} } }
	private string _STQTPR = null!;
	public required string STQTPR { get => _STQTPR; set { if (_STQTPR != value) { _STQTPR = value; NotifyPropertyChanged();} } }
	private string? _STQTPRDES;
	public string? STQTPRDES { get => _STQTPRDES; set { if (_STQTPRDES != value) { _STQTPRDES = value; NotifyPropertyChanged();} } }
	private string _STQART = null!;
	public required string STQART { get => _STQART; set { if (_STQART != value) { _STQART = value; NotifyPropertyChanged();} } }
	private int _STQFOR;
	public int STQFOR { get => _STQFOR; set { if (_STQFOR != value) { _STQFOR = value; NotifyPropertyChanged();} } }
	private string? _STQARTDES;
	public string? STQARTDES { get => _STQARTDES; set { if (_STQARTDES != value) { _STQARTDES = value; NotifyPropertyChanged();} } }
	private string? _STQFORDES;
	public string? STQFORDES { get => _STQFORDES; set { if (_STQFORDES != value) { _STQFORDES = value; NotifyPropertyChanged();} } }
	private decimal? _STQQT3;
	public decimal? STQQT3 { get => _STQQT3; set { if (_STQQT3 != value) { _STQQT3 = value; NotifyPropertyChanged();} } }
	private decimal? _STQPR3;
	public decimal? STQPR3 { get => _STQPR3; set { if (_STQPR3 != value) { _STQPR3 = value; NotifyPropertyChanged();} } }
	private int? _STQNUD;
	public int? STQNUD { get => _STQNUD; set { if (_STQNUD != value) { _STQNUD = value; NotifyPropertyChanged();} } }
	private DateTime? _STQDAO;
	public DateTime? STQDAO { get => _STQDAO; set { if (_STQDAO != value) { _STQDAO = value; NotifyPropertyChanged();} } }
	private decimal? _STQQT4;
	public decimal? STQQT4 { get => _STQQT4; set { if (_STQQT4 != value) { _STQQT4 = value; NotifyPropertyChanged();} } }
	private DateTime? _STQDAE;
	public DateTime? STQDAE { get => _STQDAE; set { if (_STQDAE != value) { _STQDAE = value; NotifyPropertyChanged();} } }
	private decimal? _STQPR5;
	public decimal? STQPR5 { get => _STQPR5; set { if (_STQPR5 != value) { _STQPR5 = value; NotifyPropertyChanged();} } }
	private decimal? _STQPR2;
	public decimal? STQPR2 { get => _STQPR2; set { if (_STQPR2 != value) { _STQPR2 = value; NotifyPropertyChanged();} } }
	private string _Stqnaz = null!;
	public required string Stqnaz { get => _Stqnaz; set { if (_Stqnaz != value) { _Stqnaz = value; NotifyPropertyChanged();} } }
	private string _stqbofo = null!;
	public required string stqbofo { get => _stqbofo; set { if (_stqbofo != value) { _stqbofo = value; NotifyPropertyChanged();} } }
	private DateTime _stqdabo;
	public DateTime stqdabo { get => _stqdabo; set { if (_stqdabo != value) { _stqdabo = value; NotifyPropertyChanged();} } }
	private string _stqtibo = null!;
	public required string stqtibo { get => _stqtibo; set { if (_stqtibo != value) { _stqtibo = value; NotifyPropertyChanged();} } }
}