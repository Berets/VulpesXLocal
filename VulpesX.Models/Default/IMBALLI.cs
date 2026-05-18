namespace VulpesX.Models.Default;
 
public partial class IMBALLI : Base 
{
	private string _imbcod = null!;
	public required string imbcod { get => _imbcod; set { if (_imbcod != value) { _imbcod = value; NotifyPropertyChanged();} } }
	private string _imbdes = null!;
	public required string imbdes { get => _imbdes; set { if (_imbdes != value) { _imbdes = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}