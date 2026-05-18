namespace VulpesX.Models.Default;
 
public partial class DWCAUCOMMESSE : Base 
{
	private string _SOMCOD = null!;
	public required string SOMCOD { get => _SOMCOD; set { if (_SOMCOD != value) { _SOMCOD = value; NotifyPropertyChanged();} } }
	private string _CauComme = null!;
	public required string CauComme { get => _CauComme; set { if (_CauComme != value) { _CauComme = value; NotifyPropertyChanged();} } }
	private string? _caucomde;
	public string? caucomde { get => _caucomde; set { if (_caucomde != value) { _caucomde = value; NotifyPropertyChanged();} } }
}