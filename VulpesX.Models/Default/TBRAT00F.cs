namespace VulpesX.Models.Default;
 
public partial class TBRAT00F : Base 
{
	private string _tbrcod = null!;
	public required string tbrcod { get => _tbrcod; set { if (_tbrcod != value) { _tbrcod = value; NotifyPropertyChanged();} } }
	private string? _tbrdes;
	public string? tbrdes { get => _tbrdes; set { if (_tbrdes != value) { _tbrdes = value; NotifyPropertyChanged();} } }
}