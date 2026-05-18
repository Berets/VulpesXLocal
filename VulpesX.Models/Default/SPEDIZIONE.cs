namespace VulpesX.Models.Default;
 
public partial class SPEDIZIONE : Base 
{
	private string _specod = null!;
	public required string specod { get => _specod; set { if (_specod != value) { _specod = value; NotifyPropertyChanged();} } }
	private string _spedes = null!;
	public required string spedes { get => _spedes; set { if (_spedes != value) { _spedes = value; NotifyPropertyChanged();} } }
	private int? _spetip;
	public int? spetip { get => _spetip; set { if (_spetip != value) { _spetip = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}