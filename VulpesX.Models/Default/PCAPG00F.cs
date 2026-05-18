namespace VulpesX.Models.Default;
 
public partial class PCAPG00F : Base 
{
	private string _VGSOCI = null!;
	public required string VGSOCI { get => _VGSOCI; set { if (_VGSOCI != value) { _VGSOCI = value; NotifyPropertyChanged();} } }
	private int _VGVOCE;
	public int VGVOCE { get => _VGVOCE; set { if (_VGVOCE != value) { _VGVOCE = value; NotifyPropertyChanged();} } }
	private string? _VGTIPO;
	public string? VGTIPO { get => _VGTIPO; set { if (_VGTIPO != value) { _VGTIPO = value; NotifyPropertyChanged();} } }
	private string? _VGDESC;
	public string? VGDESC { get => _VGDESC; set { if (_VGDESC != value) { _VGDESC = value; NotifyPropertyChanged();} } }
	private int? _VGTO01;
	public int? VGTO01 { get => _VGTO01; set { if (_VGTO01 != value) { _VGTO01 = value; NotifyPropertyChanged();} } }
	private string? _VGSE01;
	public string? VGSE01 { get => _VGSE01; set { if (_VGSE01 != value) { _VGSE01 = value; NotifyPropertyChanged();} } }
	private int? _VGTO02;
	public int? VGTO02 { get => _VGTO02; set { if (_VGTO02 != value) { _VGTO02 = value; NotifyPropertyChanged();} } }
	private string? _VGSE02;
	public string? VGSE02 { get => _VGSE02; set { if (_VGSE02 != value) { _VGSE02 = value; NotifyPropertyChanged();} } }
	private int? _VGTO03;
	public int? VGTO03 { get => _VGTO03; set { if (_VGTO03 != value) { _VGTO03 = value; NotifyPropertyChanged();} } }
	private string? _VGSE03;
	public string? VGSE03 { get => _VGSE03; set { if (_VGSE03 != value) { _VGSE03 = value; NotifyPropertyChanged();} } }
}