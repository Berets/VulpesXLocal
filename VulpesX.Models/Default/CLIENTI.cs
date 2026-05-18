namespace VulpesX.Models.Default;
 
public partial class CLIENTI : Base 
{
	private int _CLIENT;
	public int CLIENT { get => _CLIENT; set { if (_CLIENT != value) { _CLIENT = value; NotifyPropertyChanged();} } }
	private string? _CLNOME;
	public string? CLNOME { get => _CLNOME; set { if (_CLNOME != value) { _CLNOME = value; NotifyPropertyChanged();} } }
	private string? _CLNUTE;
	public string? CLNUTE { get => _CLNUTE; set { if (_CLNUTE != value) { _CLNUTE = value; NotifyPropertyChanged();} } }
	private string? _CLNUFA;
	public string? CLNUFA { get => _CLNUFA; set { if (_CLNUFA != value) { _CLNUFA = value; NotifyPropertyChanged();} } }
	private string? _classo;
	public string? classo { get => _classo; set { if (_classo != value) { _classo = value; NotifyPropertyChanged();} } }
	private string? _classa;
	public string? classa { get => _classa; set { if (_classa != value) { _classa = value; NotifyPropertyChanged();} } }
	private int? _solcod;
	public int? solcod { get => _solcod; set { if (_solcod != value) { _solcod = value; NotifyPropertyChanged();} } }
	private string? _CLREDI;
	public string? CLREDI { get => _CLREDI; set { if (_CLREDI != value) { _CLREDI = value; NotifyPropertyChanged();} } }
	private string? _CLRECO;
	public string? CLRECO { get => _CLRECO; set { if (_CLRECO != value) { _CLRECO = value; NotifyPropertyChanged();} } }
	private string? _CLREAC;
	public string? CLREAC { get => _CLREAC; set { if (_CLREAC != value) { _CLREAC = value; NotifyPropertyChanged();} } }
	private string? _CLCCOL;
	public string? CLCCOL { get => _CLCCOL; set { if (_CLCCOL != value) { _CLCCOL = value; NotifyPropertyChanged();} } }
	private string? _ammcod;
	public string? ammcod { get => _ammcod; set { if (_ammcod != value) { _ammcod = value; NotifyPropertyChanged();} } }
	private string? _climail;
	public string? climail { get => _climail; set { if (_climail != value) { _climail = value; NotifyPropertyChanged();} } }
	private string? _Clisit;
	public string? Clisit { get => _Clisit; set { if (_Clisit != value) { _Clisit = value; NotifyPropertyChanged();} } }
	private string? _CLSOSP;
	public string? CLSOSP { get => _CLSOSP; set { if (_CLSOSP != value) { _CLSOSP = value; NotifyPropertyChanged();} } }
	private string? _clicel;
	public string? clicel { get => _clicel; set { if (_clicel != value) { _clicel = value; NotifyPropertyChanged();} } }
	private string? _cliperi;
	public string? cliperi { get => _cliperi; set { if (_cliperi != value) { _cliperi = value; NotifyPropertyChanged();} } }
	private string? _clcouff;
	public string? clcouff { get => _clcouff; set { if (_clcouff != value) { _clcouff = value; NotifyPropertyChanged();} } }
	private string? _clpaym;
	public string? clpaym { get => _clpaym; set { if (_clpaym != value) { _clpaym = value; NotifyPropertyChanged();} } }
	private string? _clcoddest;
	public string? clcoddest { get => _clcoddest; set { if (_clcoddest != value) { _clcoddest = value; NotifyPropertyChanged();} } }
	private string? _clpec;
	public string? clpec { get => _clpec; set { if (_clpec != value) { _clpec = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}