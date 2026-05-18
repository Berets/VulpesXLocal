namespace VulpesX.Models.Default;
 
public partial class INCOM00F1 : Base 
{
	private string _ICOSOC = null!;
	public required string ICOSOC { get => _ICOSOC; set { if (_ICOSOC != value) { _ICOSOC = value; NotifyPropertyChanged();} } }
	private string _ICOPIV = null!;
	public required string ICOPIV { get => _ICOPIV; set { if (_ICOPIV != value) { _ICOPIV = value; NotifyPropertyChanged();} } }
	private int _ICORIG;
	public int ICORIG { get => _ICORIG; set { if (_ICORIG != value) { _ICORIG = value; NotifyPropertyChanged();} } }
	private DateTime? _ICODAT;
	public DateTime? ICODAT { get => _ICODAT; set { if (_ICODAT != value) { _ICODAT = value; NotifyPropertyChanged();} } }
	private string? _ttdcod;
	public string? ttdcod { get => _ttdcod; set { if (_ttdcod != value) { _ttdcod = value; NotifyPropertyChanged();} } }
	private string? _ICONOT;
	public string? ICONOT { get => _ICONOT; set { if (_ICONOT != value) { _ICONOT = value; NotifyPropertyChanged();} } }
}