namespace VulpesX.Models.Default;
 
public partial class COSMAC : Base 
{
	private string _macsoc = null!;
	public required string macsoc { get => _macsoc; set { if (_macsoc != value) { _macsoc = value; NotifyPropertyChanged();} } }
	private string _maccod = null!;
	public required string maccod { get => _maccod; set { if (_maccod != value) { _maccod = value; NotifyPropertyChanged();} } }
	private int _macann;
	public int macann { get => _macann; set { if (_macann != value) { _macann = value; NotifyPropertyChanged();} } }
	private int _macmes;
	public int macmes { get => _macmes; set { if (_macmes != value) { _macmes = value; NotifyPropertyChanged();} } }
	private decimal? _maccos;
	public decimal? maccos { get => _maccos; set { if (_maccos != value) { _maccos = value; NotifyPropertyChanged();} } }
}