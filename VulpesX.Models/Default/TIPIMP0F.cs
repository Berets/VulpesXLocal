namespace VulpesX.Models.Default;
 
public partial class TIPIMP0F : Base 
{
	private string _TIICOD = null!;
	public required string TIICOD { get => _TIICOD; set { if (_TIICOD != value) { _TIICOD = value; NotifyPropertyChanged();} } }
	private string _TIIDES = null!;
	public required string TIIDES { get => _TIIDES; set { if (_TIIDES != value) { _TIIDES = value; NotifyPropertyChanged();} } }
	private string _TIITIP = null!;
	public required string TIITIP { get => _TIITIP; set { if (_TIITIP != value) { _TIITIP = value; NotifyPropertyChanged();} } }
}