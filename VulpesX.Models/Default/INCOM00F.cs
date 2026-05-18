namespace VulpesX.Models.Default;
 
public partial class INCOM00F : Base 
{
	private string _ICOSOC = null!;
	public required string ICOSOC { get => _ICOSOC; set { if (_ICOSOC != value) { _ICOSOC = value; NotifyPropertyChanged();} } }
	private string _ICOPIV = null!;
	public required string ICOPIV { get => _ICOPIV; set { if (_ICOPIV != value) { _ICOPIV = value; NotifyPropertyChanged();} } }
	private string? _ICORAG;
	public string? ICORAG { get => _ICORAG; set { if (_ICORAG != value) { _ICORAG = value; NotifyPropertyChanged();} } }
	private string? _ICOIND;
	public string? ICOIND { get => _ICOIND; set { if (_ICOIND != value) { _ICOIND = value; NotifyPropertyChanged();} } }
	private string? _ICOLOC;
	public string? ICOLOC { get => _ICOLOC; set { if (_ICOLOC != value) { _ICOLOC = value; NotifyPropertyChanged();} } }
	private string? _ICOPRO;
	public string? ICOPRO { get => _ICOPRO; set { if (_ICOPRO != value) { _ICOPRO = value; NotifyPropertyChanged();} } }
	private int? _ICOCAP;
	public int? ICOCAP { get => _ICOCAP; set { if (_ICOCAP != value) { _ICOCAP = value; NotifyPropertyChanged();} } }
	private string? _ICOTSO;
	public string? ICOTSO { get => _ICOTSO; set { if (_ICOTSO != value) { _ICOTSO = value; NotifyPropertyChanged();} } }
	private string? _tbrcod;
	public string? tbrcod { get => _tbrcod; set { if (_tbrcod != value) { _tbrcod = value; NotifyPropertyChanged();} } }
	private string? _ICODIR;
	public string? ICODIR { get => _ICODIR; set { if (_ICODIR != value) { _ICODIR = value; NotifyPropertyChanged();} } }
	private string? _ICOCOG;
	public string? ICOCOG { get => _ICOCOG; set { if (_ICOCOG != value) { _ICOCOG = value; NotifyPropertyChanged();} } }
	private string? _ICONOM;
	public string? ICONOM { get => _ICONOM; set { if (_ICONOM != value) { _ICONOM = value; NotifyPropertyChanged();} } }
	private string? _ICOCFI;
	public string? ICOCFI { get => _ICOCFI; set { if (_ICOCFI != value) { _ICOCFI = value; NotifyPropertyChanged();} } }
}