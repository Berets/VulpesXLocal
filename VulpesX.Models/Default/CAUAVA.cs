namespace VulpesX.Models.Default;
 
public partial class CAUAVA : Base 
{
	private string _avasoc = null!;
	public required string avasoc { get => _avasoc; set { if (_avasoc != value) { _avasoc = value; NotifyPropertyChanged();} } }
	private string _avacod = null!;
	public required string avacod { get => _avacod; set { if (_avacod != value) { _avacod = value; NotifyPropertyChanged();} } }
	private string? _avades;
	public string? avades { get => _avades; set { if (_avades != value) { _avades = value; NotifyPropertyChanged();} } }
	private string? _avatip;
	public string? avatip { get => _avatip; set { if (_avatip != value) { _avatip = value; NotifyPropertyChanged();} } }
}