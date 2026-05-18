namespace VulpesX.Models.Default;
 
public partial class POLITICHE : Base 
{
	private string _polsoc = null!;
	public required string polsoc { get => _polsoc; set { if (_polsoc != value) { _polsoc = value; NotifyPropertyChanged();} } }
	private string _Polcod = null!;
	public required string Polcod { get => _Polcod; set { if (_Polcod != value) { _Polcod = value; NotifyPropertyChanged();} } }
	private string? _poldes;
	public string? poldes { get => _poldes; set { if (_poldes != value) { _poldes = value; NotifyPropertyChanged();} } }
}