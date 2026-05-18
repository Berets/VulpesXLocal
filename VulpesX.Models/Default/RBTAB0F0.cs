namespace VulpesX.Models.Default;
 
public partial class RBTAB0F0 : Base 
{
	private string _RBTSOC = null!;
	public required string RBTSOC { get => _RBTSOC; set { if (_RBTSOC != value) { _RBTSOC = value; NotifyPropertyChanged();} } }
	private string _RBTCOD = null!;
	public required string RBTCOD { get => _RBTCOD; set { if (_RBTCOD != value) { _RBTCOD = value; NotifyPropertyChanged();} } }
	private string? _RBTATT;
	public string? RBTATT { get => _RBTATT; set { if (_RBTATT != value) { _RBTATT = value; NotifyPropertyChanged();} } }
	private int? _RBTAZI;
	public int? RBTAZI { get => _RBTAZI; set { if (_RBTAZI != value) { _RBTAZI = value; NotifyPropertyChanged();} } }
	private string? _RBTUTI;
	public string? RBTUTI { get => _RBTUTI; set { if (_RBTUTI != value) { _RBTUTI = value; NotifyPropertyChanged();} } }
	private string? _RBTUTV;
	public string? RBTUTV { get => _RBTUTV; set { if (_RBTUTV != value) { _RBTUTV = value; NotifyPropertyChanged();} } }
	private string? _RBTTER;
	public string? RBTTER { get => _RBTTER; set { if (_RBTTER != value) { _RBTTER = value; NotifyPropertyChanged();} } }
	private string? _RBTORI;
	public string? RBTORI { get => _RBTORI; set { if (_RBTORI != value) { _RBTORI = value; NotifyPropertyChanged();} } }
	private string? _RBTORV;
	public string? RBTORV { get => _RBTORV; set { if (_RBTORV != value) { _RBTORV = value; NotifyPropertyChanged();} } }
	private string? _RBTIPO;
	public string? RBTIPO { get => _RBTIPO; set { if (_RBTIPO != value) { _RBTIPO = value; NotifyPropertyChanged();} } }
	private string? _RBTDES;
	public string? RBTDES { get => _RBTDES; set { if (_RBTDES != value) { _RBTDES = value; NotifyPropertyChanged();} } }
}