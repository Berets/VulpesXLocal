namespace VulpesX.Models.Default;
 
public partial class CLAZIONE : Base 
{
	private string _csfcod = null!;
	public required string csfcod { get => _csfcod; set { if (_csfcod != value) { _csfcod = value; NotifyPropertyChanged();} } }
	private string _csfdes = null!;
	public required string csfdes { get => _csfdes; set { if (_csfdes != value) { _csfdes = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}