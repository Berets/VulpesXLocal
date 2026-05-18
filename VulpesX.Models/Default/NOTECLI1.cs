namespace VulpesX.Models.Default;
 
public partial class NOTECLI1 : Base 
{
	private int _NOTCLI;
	public int NOTCLI { get => _NOTCLI; set { if (_NOTCLI != value) { _NOTCLI = value; NotifyPropertyChanged();} } }
	private string _NOTETI = null!;
	public required string NOTETI { get => _NOTETI; set { if (_NOTETI != value) { _NOTETI = value; NotifyPropertyChanged();} } }
	private int _notrig;
	public int notrig { get => _notrig; set { if (_notrig != value) { _notrig = value; NotifyPropertyChanged();} } }
	private string? _NOTRAG;
	public string? NOTRAG { get => _NOTRAG; set { if (_NOTRAG != value) { _NOTRAG = value; NotifyPropertyChanged();} } }
	private string? _notdes;
	public string? notdes { get => _notdes; set { if (_notdes != value) { _notdes = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}