namespace VulpesX.Models.Default;
 
public partial class MezzidiTrasporto : Base 
{
	private string _MTCOD = null!;
	public required string MTCOD { get => _MTCOD; set { if (_MTCOD != value) { _MTCOD = value; NotifyPropertyChanged();} } }
	private string _MTDES = null!;
	public required string MTDES { get => _MTDES; set { if (_MTDES != value) { _MTDES = value; NotifyPropertyChanged();} } }
}