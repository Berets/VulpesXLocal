namespace VulpesX.Models.Default;
 
public partial class BILANPER : Base 
{
	private string _bilasoc = null!;
	public required string bilasoc { get => _bilasoc; set { if (_bilasoc != value) { _bilasoc = value; NotifyPropertyChanged();} } }
	private string _bilacau = null!;
	public required string bilacau { get => _bilacau; set { if (_bilacau != value) { _bilacau = value; NotifyPropertyChanged();} } }
}