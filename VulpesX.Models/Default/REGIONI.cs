namespace VulpesX.Models.Default;
 
public partial class REGIONI : Base 
{
	private string _regcod = null!;
	public required string regcod { get => _regcod; set { if (_regcod != value) { _regcod = value; NotifyPropertyChanged();} } }
	private string _regdes = null!;
	public required string regdes { get => _regdes; set { if (_regdes != value) { _regdes = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}