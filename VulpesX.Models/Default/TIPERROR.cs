namespace VulpesX.Models.Default;
 
public partial class TIPERROR : Base 
{
	private string _errcod = null!;
	public required string errcod { get => _errcod; set { if (_errcod != value) { _errcod = value; NotifyPropertyChanged();} } }
	private string? _errdes;
	public string? errdes { get => _errdes; set { if (_errdes != value) { _errdes = value; NotifyPropertyChanged();} } }
}