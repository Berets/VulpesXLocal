namespace VulpesX.Models.Default;
 
public partial class ZONE : Base 
{
	private string _zoncod = null!;
	public required string zoncod { get => _zoncod; set { if (_zoncod != value) { _zoncod = value; NotifyPropertyChanged();} } }
	private string _zondes = null!;
	public required string zondes { get => _zondes; set { if (_zondes != value) { _zondes = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}