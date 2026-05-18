namespace VulpesX.Models.Default;
 
public partial class CONSEGNA_LINGUA : Base 
{
	private string _concod = null!;
	public required string concod { get => _concod; set { if (_concod != value) { _concod = value; NotifyPropertyChanged();} } }
	private string _lincod = null!;
	public required string lincod { get => _lincod; set { if (_lincod != value) { _lincod = value; NotifyPropertyChanged();} } }
	private string? _condes;
	public string? condes { get => _condes; set { if (_condes != value) { _condes = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}