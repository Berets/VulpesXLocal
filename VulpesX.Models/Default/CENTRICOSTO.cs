namespace VulpesX.Models.Default;
 
public partial class CENTRICOSTO : Base 
{
	private string _ccccod = null!;
	public required string ccccod { get => _ccccod; set { if (_ccccod != value) { _ccccod = value; NotifyPropertyChanged();} } }
	private string? _cccdes;
	public string? cccdes { get => _cccdes; set { if (_cccdes != value) { _cccdes = value; NotifyPropertyChanged();} } }
	private decimal _cccore;
	public decimal cccore { get => _cccore; set { if (_cccore != value) { _cccore = value; NotifyPropertyChanged();} } }
}