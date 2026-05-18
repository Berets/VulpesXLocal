namespace VulpesX.Models.Default;
 
public partial class TABPERCRM : Base 
{
	private string _Crmsoc = null!;
	public required string Crmsoc { get => _Crmsoc; set { if (_Crmsoc != value) { _Crmsoc = value; NotifyPropertyChanged();} } }
}