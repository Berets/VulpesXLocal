namespace VulpesX.Models.Default;
 
public partial class ARTNOT : Base 
{
	private string _ARTCOD = null!;
	public required string ARTCOD { get => _ARTCOD; set { if (_ARTCOD != value) { _ARTCOD = value; NotifyPropertyChanged();} } }
	private int _ntorig;
	public int ntorig { get => _ntorig; set { if (_ntorig != value) { _ntorig = value; NotifyPropertyChanged();} } }
	private string? _desnot;
	public string? desnot { get => _desnot; set { if (_desnot != value) { _desnot = value; NotifyPropertyChanged();} } }
	private string? _artflg;
	public string? artflg { get => _artflg; set { if (_artflg != value) { _artflg = value; NotifyPropertyChanged();} } }
	private string? _artnot;
	public string? artnot { get => _artnot; set { if (_artnot != value) { _artnot = value; NotifyPropertyChanged();} } }
}