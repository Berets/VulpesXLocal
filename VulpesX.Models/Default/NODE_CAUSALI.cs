namespace VulpesX.Models.Default;
 
public partial class NODE_CAUSALI : Base 
{
	private string _causoc = null!;
	public required string causoc { get => _causoc; set { if (_causoc != value) { _causoc = value; NotifyPropertyChanged();} } }
	private string _caucod = null!;
	public required string caucod { get => _caucod; set { if (_caucod != value) { _caucod = value; NotifyPropertyChanged();} } }
	private bool? _cauchi;
	public bool? cauchi { get => _cauchi; set { if (_cauchi != value) { _cauchi = value; NotifyPropertyChanged();} } }
	private string _causoccol = null!;
	public required string causoccol { get => _causoccol; set { if (_causoccol != value) { _causoccol = value; NotifyPropertyChanged();} } }
	private string _caucodcol = null!;
	public required string caucodcol { get => _caucodcol; set { if (_caucodcol != value) { _caucodcol = value; NotifyPropertyChanged();} } }
}