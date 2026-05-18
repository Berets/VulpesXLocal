namespace VulpesX.Models.Default;
 
public partial class BST001P : Base 
{
	private string _BSTSOC = null!;
	public required string BSTSOC { get => _BSTSOC; set { if (_BSTSOC != value) { _BSTSOC = value; NotifyPropertyChanged();} } }
	private int _BSTANN;
	public int BSTANN { get => _BSTANN; set { if (_BSTANN != value) { _BSTANN = value; NotifyPropertyChanged();} } }
	private int _BSTMES;
	public int BSTMES { get => _BSTMES; set { if (_BSTMES != value) { _BSTMES = value; NotifyPropertyChanged();} } }
	private string _BSTTIP = null!;
	public required string BSTTIP { get => _BSTTIP; set { if (_BSTTIP != value) { _BSTTIP = value; NotifyPropertyChanged();} } }
	private int _BSTCOD;
	public int BSTCOD { get => _BSTCOD; set { if (_BSTCOD != value) { _BSTCOD = value; NotifyPropertyChanged();} } }
	private decimal? _BSTIMT;
	public decimal? BSTIMT { get => _BSTIMT; set { if (_BSTIMT != value) { _BSTIMT = value; NotifyPropertyChanged();} } }
}