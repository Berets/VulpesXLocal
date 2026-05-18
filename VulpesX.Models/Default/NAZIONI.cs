namespace VulpesX.Models.Default;
 
public partial class NAZIONI : Base 
{
	private string _nazcod = null!;
	public required string nazcod { get => _nazcod; set { if (_nazcod != value) { _nazcod = value; NotifyPropertyChanged();} } }
	private string? _nazdes;
	public string? nazdes { get => _nazdes; set { if (_nazdes != value) { _nazdes = value; NotifyPropertyChanged();} } }
	private string? _naztip;
	public string? naztip { get => _naztip; set { if (_naztip != value) { _naztip = value; NotifyPropertyChanged();} } }
	private int? _nazest;
	public int? nazest { get => _nazest; set { if (_nazest != value) { _nazest = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}