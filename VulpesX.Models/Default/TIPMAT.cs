namespace VulpesX.Models.Default;
 
public partial class TIPMAT : Base 
{
	private string _Timsoc = null!;
	public required string Timsoc { get => _Timsoc; set { if (_Timsoc != value) { _Timsoc = value; NotifyPropertyChanged();} } }
	private string _timcod = null!;
	public required string timcod { get => _timcod; set { if (_timcod != value) { _timcod = value; NotifyPropertyChanged();} } }
	private string? _timdes;
	public string? timdes { get => _timdes; set { if (_timdes != value) { _timdes = value; NotifyPropertyChanged();} } }
}