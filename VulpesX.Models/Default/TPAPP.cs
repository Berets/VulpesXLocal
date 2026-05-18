namespace VulpesX.Models.Default;
 
public partial class TPAPP : Base 
{
	private string _tpacod = null!;
	public required string tpacod { get => _tpacod; set { if (_tpacod != value) { _tpacod = value; NotifyPropertyChanged();} } }
	private string? _tpades;
	public string? tpades { get => _tpades; set { if (_tpades != value) { _tpades = value; NotifyPropertyChanged();} } }
}