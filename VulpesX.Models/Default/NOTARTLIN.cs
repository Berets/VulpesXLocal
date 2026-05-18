namespace VulpesX.Models.Default;
 
public partial class NOTARTLIN : Base 
{
	private string _liacod = null!;
	public required string liacod { get => _liacod; set { if (_liacod != value) { _liacod = value; NotifyPropertyChanged();} } }
	private string _lialin = null!;
	public required string lialin { get => _lialin; set { if (_lialin != value) { _lialin = value; NotifyPropertyChanged();} } }
	private int _linrig;
	public int linrig { get => _linrig; set { if (_linrig != value) { _linrig = value; NotifyPropertyChanged();} } }
	private string? _linnot;
	public string? linnot { get => _linnot; set { if (_linnot != value) { _linnot = value; NotifyPropertyChanged();} } }
	private string? _linflg;
	public string? linflg { get => _linflg; set { if (_linflg != value) { _linflg = value; NotifyPropertyChanged();} } }
}