namespace VulpesX.Models.Default;
 
public partial class AQORDN0F : Base 
{
	private string _otsoco = null!;
	public required string otsoco { get => _otsoco; set { if (_otsoco != value) { _otsoco = value; NotifyPropertyChanged();} } }
	private int _OWANNO;
	public int OWANNO { get => _OWANNO; set { if (_OWANNO != value) { _OWANNO = value; NotifyPropertyChanged();} } }
	private int _OWNORP;
	public int OWNORP { get => _OWNORP; set { if (_OWNORP != value) { _OWNORP = value; NotifyPropertyChanged();} } }
	private int _OWNUPP;
	public int OWNUPP { get => _OWNUPP; set { if (_OWNUPP != value) { _OWNUPP = value; NotifyPropertyChanged();} } }
	private string? _ONFLAG;
	public string? ONFLAG { get => _ONFLAG; set { if (_ONFLAG != value) { _ONFLAG = value; NotifyPropertyChanged();} } }
	private string? _ONNOTE;
	public string? ONNOTE { get => _ONNOTE; set { if (_ONNOTE != value) { _ONNOTE = value; NotifyPropertyChanged();} } }
	private string? _ONNOTA;
	public string? ONNOTA { get => _ONNOTA; set { if (_ONNOTA != value) { _ONNOTA = value; NotifyPropertyChanged();} } }
}