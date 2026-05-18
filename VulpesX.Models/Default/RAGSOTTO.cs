namespace VulpesX.Models.Default;
 
public partial class RAGSOTTO : Base 
{
	private string _R1GRUP = null!;
	public required string R1GRUP { get => _R1GRUP; set { if (_R1GRUP != value) { _R1GRUP = value; NotifyPropertyChanged();} } }
	private string _R2CONT = null!;
	public required string R2CONT { get => _R2CONT; set { if (_R2CONT != value) { _R2CONT = value; NotifyPropertyChanged();} } }
	private string _R3SOTC = null!;
	public required string R3SOTC { get => _R3SOTC; set { if (_R3SOTC != value) { _R3SOTC = value; NotifyPropertyChanged();} } }
	private string? _R3DES1;
	public string? R3DES1 { get => _R3DES1; set { if (_R3DES1 != value) { _R3DES1 = value; NotifyPropertyChanged();} } }
	private string? _R3DES2;
	public string? R3DES2 { get => _R3DES2; set { if (_R3DES2 != value) { _R3DES2 = value; NotifyPropertyChanged();} } }
	private string? _R3OBCP;
	public string? R3OBCP { get => _R3OBCP; set { if (_R3OBCP != value) { _R3OBCP = value; NotifyPropertyChanged();} } }
	private string? _R3CLFO;
	public string? R3CLFO { get => _R3CLFO; set { if (_R3CLFO != value) { _R3CLFO = value; NotifyPropertyChanged();} } }
	private string? _r3coni;
	public string? r3coni { get => _r3coni; set { if (_r3coni != value) { _r3coni = value; NotifyPropertyChanged();} } }
	private int? _r3este;
	public int? r3este { get => _r3este; set { if (_r3este != value) { _r3este = value; NotifyPropertyChanged();} } }
	private string? _r3soci;
	public string? r3soci { get => _r3soci; set { if (_r3soci != value) { _r3soci = value; NotifyPropertyChanged();} } }
	private string? _r3cee1;
	public string? r3cee1 { get => _r3cee1; set { if (_r3cee1 != value) { _r3cee1 = value; NotifyPropertyChanged();} } }
	private string? _r3cee2;
	public string? r3cee2 { get => _r3cee2; set { if (_r3cee2 != value) { _r3cee2 = value; NotifyPropertyChanged();} } }
	private string? _r3cee3;
	public string? r3cee3 { get => _r3cee3; set { if (_r3cee3 != value) { _r3cee3 = value; NotifyPropertyChanged();} } }
	private string? _r3cee4;
	public string? r3cee4 { get => _r3cee4; set { if (_r3cee4 != value) { _r3cee4 = value; NotifyPropertyChanged();} } }
	private string? _r3cee5;
	public string? r3cee5 { get => _r3cee5; set { if (_r3cee5 != value) { _r3cee5 = value; NotifyPropertyChanged();} } }
	private string? _r3cove;
	public string? r3cove { get => _r3cove; set { if (_r3cove != value) { _r3cove = value; NotifyPropertyChanged();} } }
	private string? _r3cee6;
	public string? r3cee6 { get => _r3cee6; set { if (_r3cee6 != value) { _r3cee6 = value; NotifyPropertyChanged();} } }
	private string? _r3cee7;
	public string? r3cee7 { get => _r3cee7; set { if (_r3cee7 != value) { _r3cee7 = value; NotifyPropertyChanged();} } }
}