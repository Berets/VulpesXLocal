namespace VulpesX.Models.Default;
 
public partial class AREE : Base 
{
	private string _arecod = null!;
	public required string arecod { get => _arecod; set { if (_arecod != value) { _arecod = value; NotifyPropertyChanged();} } }
	private string _aredes = null!;
	public required string aredes { get => _aredes; set { if (_aredes != value) { _aredes = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
	private string? _arecap;
	public string? arecap { get => _arecap; set { if (_arecap != value) { _arecap = value; NotifyPropertyChanged();} } }
}