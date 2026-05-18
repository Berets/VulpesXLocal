namespace VulpesX.Models.Default;
 
public partial class MACFAMOFFE : Base 
{
	private string _MFcod = null!;
	public required string MFcod { get => _MFcod; set { if (_MFcod != value) { _MFcod = value; NotifyPropertyChanged();} } }
	private string? _MFdes;
	public string? MFdes { get => _MFdes; set { if (_MFdes != value) { _MFdes = value; NotifyPropertyChanged();} } }
}