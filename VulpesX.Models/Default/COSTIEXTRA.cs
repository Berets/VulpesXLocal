namespace VulpesX.Models.Default;
 
public partial class COSTIEXTRA : Base 
{
	private string _cecsoc = null!;
	public required string cecsoc { get => _cecsoc; set { if (_cecsoc != value) { _cecsoc = value; NotifyPropertyChanged();} } }
	private string _ceccod = null!;
	public required string ceccod { get => _ceccod; set { if (_ceccod != value) { _ceccod = value; NotifyPropertyChanged();} } }
	private int _cecsede;
	public int cecsede { get => _cecsede; set { if (_cecsede != value) { _cecsede = value; NotifyPropertyChanged();} } }
	private string? _cecdes;
	public string? cecdes { get => _cecdes; set { if (_cecdes != value) { _cecdes = value; NotifyPropertyChanged();} } }
}