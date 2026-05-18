namespace VulpesX.Models.Default;
 
public partial class LINGUA : Base 
{
	private string _lincod = null!;
	public required string lincod { get => _lincod; set { if (_lincod != value) { _lincod = value; NotifyPropertyChanged();} } }
	private string _lindes = null!;
	public required string lindes { get => _lindes; set { if (_lindes != value) { _lindes = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
	private byte[]? _linreport;
	public byte[]? linreport { get => _linreport; set { if (_linreport != value) { _linreport = value; NotifyPropertyChanged();} } }
}