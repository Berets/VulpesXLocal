namespace VulpesX.Models.Default;
 
public partial class AQORTNH0F : Base 
{
	private string _otsoco = null!;
	public required string otsoco { get => _otsoco; set { if (_otsoco != value) { _otsoco = value; NotifyPropertyChanged();} } }
	private int _OyANNO;
	public int OyANNO { get => _OyANNO; set { if (_OyANNO != value) { _OyANNO = value; NotifyPropertyChanged();} } }
	private int _OHNORP;
	public int OHNORP { get => _OHNORP; set { if (_OHNORP != value) { _OHNORP = value; NotifyPropertyChanged();} } }
	private string? _OHNOT1;
	public string? OHNOT1 { get => _OHNOT1; set { if (_OHNOT1 != value) { _OHNOT1 = value; NotifyPropertyChanged();} } }
	private string? _OHNOT2;
	public string? OHNOT2 { get => _OHNOT2; set { if (_OHNOT2 != value) { _OHNOT2 = value; NotifyPropertyChanged();} } }
	private string? _OHNOT3;
	public string? OHNOT3 { get => _OHNOT3; set { if (_OHNOT3 != value) { _OHNOT3 = value; NotifyPropertyChanged();} } }
	private string? _OHNOT4;
	public string? OHNOT4 { get => _OHNOT4; set { if (_OHNOT4 != value) { _OHNOT4 = value; NotifyPropertyChanged();} } }
	private string? _OHNOTE;
	public string? OHNOTE { get => _OHNOTE; set { if (_OHNOTE != value) { _OHNOTE = value; NotifyPropertyChanged();} } }
}