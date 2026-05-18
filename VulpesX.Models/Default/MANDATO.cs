namespace VulpesX.Models.Default;
 
public partial class MANDATO : Base 
{
	private string _mancod = null!;
	public required string mancod { get => _mancod; set { if (_mancod != value) { _mancod = value; NotifyPropertyChanged();} } }
	private string? _mandes;
	public string? mandes { get => _mandes; set { if (_mandes != value) { _mandes = value; NotifyPropertyChanged();} } }
	private string? _mantip;
	public string? mantip { get => _mantip; set { if (_mantip != value) { _mantip = value; NotifyPropertyChanged();} } }
	private string? _manpag;
	public string? manpag { get => _manpag; set { if (_manpag != value) { _manpag = value; NotifyPropertyChanged();} } }

    private byte[]? _rv;
    public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged(); } } }
}