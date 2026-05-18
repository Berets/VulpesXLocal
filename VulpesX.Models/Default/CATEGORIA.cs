namespace VulpesX.Models.Default;
 
public partial class CATEGORIA : Base 
{
	private string _catcod = null!;
	public required string catcod { get => _catcod; set { if (_catcod != value) { _catcod = value; NotifyPropertyChanged();} } }
	private string _catdes = null!;
	public required string catdes { get => _catdes; set { if (_catdes != value) { _catdes = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}