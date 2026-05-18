namespace VulpesX.Models.Default;
 
public partial class TIPMON : Base 
{
	private string _monsoc = null!;
	public required string monsoc { get => _monsoc; set { if (_monsoc != value) { _monsoc = value; NotifyPropertyChanged();} } }
	private string _moncod = null!;
	public required string moncod { get => _moncod; set { if (_moncod != value) { _moncod = value; NotifyPropertyChanged();} } }
	private string? _mondes;
	public string? mondes { get => _mondes; set { if (_mondes != value) { _mondes = value; NotifyPropertyChanged();} } }
}