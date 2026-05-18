namespace VulpesX.Models.Default;
 
public partial class CONSEGNA : Base 
{
	private string _concod = null!;
	public required string concod { get => _concod; set { if (_concod != value) { _concod = value; NotifyPropertyChanged();} } }
	private string _condes = null!;
	public required string condes { get => _condes; set { if (_condes != value) { _condes = value; NotifyPropertyChanged();} } }
	private string? _conint;
	public string? conint { get => _conint; set { if (_conint != value) { _conint = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}