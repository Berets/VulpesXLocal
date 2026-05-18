namespace VulpesX.Models.Default;
 
public partial class CAUANNORD : Base 
{
	private string _anncod = null!;
	public required string anncod { get => _anncod; set { if (_anncod != value) { _anncod = value; NotifyPropertyChanged();} } }
	private string? _anntip;
	public string? anntip { get => _anntip; set { if (_anntip != value) { _anntip = value; NotifyPropertyChanged();} } }
	private string? _anndes;
	public string? anndes { get => _anndes; set { if (_anndes != value) { _anndes = value; NotifyPropertyChanged();} } }
}