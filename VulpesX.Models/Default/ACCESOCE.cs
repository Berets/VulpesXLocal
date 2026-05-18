namespace VulpesX.Models.Default;
using VulpesX.Shared;
public partial class ACCESOCE : Base 
{
	private string _acssoc = null!;
	public required string acssoc { get => _acssoc; set { if (_acssoc != value) { _acssoc = value; NotifyPropertyChanged();} } }
	private string _acsute = null!;
	public required string acsute { get => _acsute; set { if (_acsute != value) { _acsute = value; NotifyPropertyChanged();} } }
	private string? _acsmas;
	public string? acsmas { get => _acsmas; set { if (_acsmas != value) { _acsmas = value; NotifyPropertyChanged();} } }
	private string? _acspsw;
	public string? acspsw { get => _acspsw; set { if (_acspsw != value) { _acspsw = value; NotifyPropertyChanged();} } }
	private bool? _acsadmin;
	public bool? acsadmin { get => _acsadmin; set { if (_acsadmin != value) { _acsadmin = value; NotifyPropertyChanged();} } }
	private string? _acsmenux;
	public string? acsmenux { get => _acsmenux; set { if (_acsmenux != value) { _acsmenux = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}