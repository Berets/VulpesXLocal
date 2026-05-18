namespace VulpesX.Models.Default;
 
public partial class KOMMESS : Base 
{
	private string _SOMCOD = null!;
	public required string SOMCOD { get => _SOMCOD; set { if (_SOMCOD != value) { _SOMCOD = value; NotifyPropertyChanged();} } }
	private string _kommess = null!;
	public required string kommess { get => _kommess; set { if (_kommess != value) { _kommess = value; NotifyPropertyChanged();} } }
	private string? _CommesDe;
	public string? CommesDe { get => _CommesDe; set { if (_CommesDe != value) { _CommesDe = value; NotifyPropertyChanged();} } }
	private string? _CommeSta;
	public string? CommeSta { get => _CommeSta; set { if (_CommeSta != value) { _CommeSta = value; NotifyPropertyChanged();} } }
}