namespace VulpesX.Models.Default;
 
public partial class CAUFERMAC : Base 
{
	private string _cfmcod = null!;
	public required string cfmcod { get => _cfmcod; set { if (_cfmcod != value) { _cfmcod = value; NotifyPropertyChanged();} } }
	private string? _cfmdes;
	public string? cfmdes { get => _cfmdes; set { if (_cfmdes != value) { _cfmdes = value; NotifyPropertyChanged();} } }
}