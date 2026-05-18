namespace VulpesX.Models.Default;
 
public partial class ARTLING : Base 
{
	private string _liacod = null!;
	public required string liacod { get => _liacod; set { if (_liacod != value) { _liacod = value; NotifyPropertyChanged();} } }
	private string _lialin = null!;
	public required string lialin { get => _lialin; set { if (_lialin != value) { _lialin = value; NotifyPropertyChanged();} } }
	private string? _liades;
	public string? liades { get => _liades; set { if (_liades != value) { _liades = value; NotifyPropertyChanged();} } }
	private string? _liasoc;
	public string? liasoc { get => _liasoc; set { if (_liasoc != value) { _liasoc = value; NotifyPropertyChanged();} } }
}