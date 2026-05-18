namespace VulpesX.Models.Default;
 
public partial class CAUSCA : Base 
{
	private string _scmsoc = null!;
	public required string scmsoc { get => _scmsoc; set { if (_scmsoc != value) { _scmsoc = value; NotifyPropertyChanged();} } }
	private string _scmcod = null!;
	public required string scmcod { get => _scmcod; set { if (_scmcod != value) { _scmcod = value; NotifyPropertyChanged();} } }
	private string? _scmdes;
	public string? scmdes { get => _scmdes; set { if (_scmdes != value) { _scmdes = value; NotifyPropertyChanged();} } }
}