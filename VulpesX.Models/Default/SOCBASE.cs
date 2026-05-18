namespace VulpesX.Models.Default;
 
public partial class SOCBASE : Base 
{
	private string _SOMCOD = null!;
	public required string SOMCOD { get => _SOMCOD; set { if (_SOMCOD != value) { _SOMCOD = value; NotifyPropertyChanged();} } }
	private string _SOMDES = null!;
	public required string SOMDES { get => _SOMDES; set { if (_SOMDES != value) { _SOMDES = value; NotifyPropertyChanged();} } }
	private string? _SOMF01;
	public string? SOMF01 { get => _SOMF01; set { if (_SOMF01 != value) { _SOMF01 = value; NotifyPropertyChanged();} } }
	private string? _SOMF02;
	public string? SOMF02 { get => _SOMF02; set { if (_SOMF02 != value) { _SOMF02 = value; NotifyPropertyChanged();} } }
	private string? _SOMF05;
	public string? SOMF05 { get => _SOMF05; set { if (_SOMF05 != value) { _SOMF05 = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
	private Guid? _SOCUID;
	public Guid? SOCUID { get => _SOCUID; set { if (_SOCUID != value) { _SOCUID = value; NotifyPropertyChanged();} } }
}