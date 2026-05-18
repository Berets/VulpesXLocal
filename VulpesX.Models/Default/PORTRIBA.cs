namespace VulpesX.Models.Default;
 
public partial class PORTRIBA : Base 
{
	private string _ribade = null!;
	public required string ribade { get => _ribade; set { if (_ribade != value) { _ribade = value; NotifyPropertyChanged();} } }
}