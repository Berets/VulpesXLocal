namespace VulpesX.Models.Default;
 
public partial class DWTIPMOV : Base 
{
	private string _Moviment = null!;
	public required string Moviment { get => _Moviment; set { if (_Moviment != value) { _Moviment = value; NotifyPropertyChanged();} } }
	private string? _movimede;
	public string? movimede { get => _movimede; set { if (_movimede != value) { _movimede = value; NotifyPropertyChanged();} } }
}