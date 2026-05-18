namespace VulpesX.Models.Default;
 
public partial class FASLAV : Base 
{
	private string _lavsoc = null!;
	public required string lavsoc { get => _lavsoc; set { if (_lavsoc != value) { _lavsoc = value; NotifyPropertyChanged();} } }
	private string _lavcod = null!;
	public required string lavcod { get => _lavcod; set { if (_lavcod != value) { _lavcod = value; NotifyPropertyChanged();} } }
	private string? _lavdes;
	public string? lavdes { get => _lavdes; set { if (_lavdes != value) { _lavdes = value; NotifyPropertyChanged();} } }
}