namespace VulpesX.Models.Default;
 
public partial class TIPPREBLL1 : Base 
{
	private string _tipblsoc = null!;
	public required string tipblsoc { get => _tipblsoc; set { if (_tipblsoc != value) { _tipblsoc = value; NotifyPropertyChanged();} } }
	private string _tipblcod = null!;
	public required string tipblcod { get => _tipblcod; set { if (_tipblcod != value) { _tipblcod = value; NotifyPropertyChanged();} } }
	private int _tpblanno;
	public int tpblanno { get => _tpblanno; set { if (_tpblanno != value) { _tpblanno = value; NotifyPropertyChanged();} } }
	private int _tpblmese;
	public int tpblmese { get => _tpblmese; set { if (_tpblmese != value) { _tpblmese = value; NotifyPropertyChanged();} } }
}