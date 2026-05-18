namespace VulpesX.Models.Default;
 
public partial class SOCIETA : Base 
{
	private string _soctip = null!;
	public required string soctip { get => _soctip; set { if (_soctip != value) { _soctip = value; NotifyPropertyChanged();} } }
	private string? _socdes;
	public string? socdes { get => _socdes; set { if (_socdes != value) { _socdes = value; NotifyPropertyChanged();} } }
	private string? _soctpg;
	public string? soctpg { get => _soctpg; set { if (_soctpg != value) { _soctpg = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}