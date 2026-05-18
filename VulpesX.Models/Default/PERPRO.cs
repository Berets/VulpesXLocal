namespace VulpesX.Models.Default;
 
public partial class PERPRO : Base 
{
	private string _Persob = null!;
	public required string Persob { get => _Persob; set { if (_Persob != value) { _Persob = value; NotifyPropertyChanged();} } }
	private int _Peran1;
	public int Peran1 { get => _Peran1; set { if (_Peran1 != value) { _Peran1 = value; NotifyPropertyChanged();} } }
	private string? _permab;
	public string? permab { get => _permab; set { if (_permab != value) { _permab = value; NotifyPropertyChanged();} } }
	private string? _permai;
	public string? permai { get => _permai; set { if (_permai != value) { _permai = value; NotifyPropertyChanged();} } }
	private int? _pergio;
	public int? pergio { get => _pergio; set { if (_pergio != value) { _pergio = value; NotifyPropertyChanged();} } }
	private int? _perggo;
	public int? perggo { get => _perggo; set { if (_perggo != value) { _perggo = value; NotifyPropertyChanged();} } }
	private int? _pergga;
	public int? pergga { get => _pergga; set { if (_pergga != value) { _pergga = value; NotifyPropertyChanged();} } }
	private int? _perult;
	public int? perult { get => _perult; set { if (_perult != value) { _perult = value; NotifyPropertyChanged();} } }
	private int? _perrda;
	public int? perrda { get => _perrda; set { if (_perrda != value) { _perrda = value; NotifyPropertyChanged();} } }
	private int? _permps;
	public int? permps { get => _permps; set { if (_permps != value) { _permps = value; NotifyPropertyChanged();} } }
	private int? _permrp;
	public int? permrp { get => _permrp; set { if (_permrp != value) { _permrp = value; NotifyPropertyChanged();} } }
	private string? _perscp;
	public string? perscp { get => _perscp; set { if (_perscp != value) { _perscp = value; NotifyPropertyChanged();} } }
	private string? _percar;
	public string? percar { get => _percar; set { if (_percar != value) { _percar = value; NotifyPropertyChanged();} } }
	private string? _percaf;
	public string? percaf { get => _percaf; set { if (_percaf != value) { _percaf = value; NotifyPropertyChanged();} } }
	private string? _perscf;
	public string? perscf { get => _perscf; set { if (_perscf != value) { _perscf = value; NotifyPropertyChanged();} } }
	private string? _perscr;
	public string? perscr { get => _perscr; set { if (_perscr != value) { _perscr = value; NotifyPropertyChanged();} } }
	private string? _percap;
	public string? percap { get => _percap; set { if (_percap != value) { _percap = value; NotifyPropertyChanged();} } }
	private string? _percab;
	public string? percab { get => _percab; set { if (_percab != value) { _percab = value; NotifyPropertyChanged();} } }
	private string? _pertar;
	public string? pertar { get => _pertar; set { if (_pertar != value) { _pertar = value; NotifyPropertyChanged();} } }
	private string? _pertem;
	public string? pertem { get => _pertem; set { if (_pertem != value) { _pertem = value; NotifyPropertyChanged();} } }
	private string? _pertrd;
	public string? pertrd { get => _pertrd; set { if (_pertrd != value) { _pertrd = value; NotifyPropertyChanged();} } }
	private string? _perfin;
	public string? perfin { get => _perfin; set { if (_perfin != value) { _perfin = value; NotifyPropertyChanged();} } }
}