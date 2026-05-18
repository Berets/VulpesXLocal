namespace VulpesX.Models.Default;
 
public partial class AQORTN0F : Base 
{
	private string _otsoco = null!;
	public required string otsoco { get => _otsoco; set { if (_otsoco != value) { _otsoco = value; NotifyPropertyChanged();} } }
	private int _ONANNO;
	public int ONANNO { get => _ONANNO; set { if (_ONANNO != value) { _ONANNO = value; NotifyPropertyChanged();} } }
	private int _ONNORP;
	public int ONNORP { get => _ONNORP; set { if (_ONNORP != value) { _ONNORP = value; NotifyPropertyChanged();} } }
	private string? _ONNOT1;
	public string? ONNOT1 { get => _ONNOT1; set { if (_ONNOT1 != value) { _ONNOT1 = value; NotifyPropertyChanged();} } }
	private string? _ONNOT2;
	public string? ONNOT2 { get => _ONNOT2; set { if (_ONNOT2 != value) { _ONNOT2 = value; NotifyPropertyChanged();} } }
	private string? _ONNOT3;
	public string? ONNOT3 { get => _ONNOT3; set { if (_ONNOT3 != value) { _ONNOT3 = value; NotifyPropertyChanged();} } }
	private string? _ONNOT4;
	public string? ONNOT4 { get => _ONNOT4; set { if (_ONNOT4 != value) { _ONNOT4 = value; NotifyPropertyChanged();} } }
	private string? _ONNOT;
	public string? ONNOT { get => _ONNOT; set { if (_ONNOT != value) { _ONNOT = value; NotifyPropertyChanged();} } }
}