namespace VulpesX.Models.Default;
 
public partial class DWSCP00F : Base 
{
	private string _DwsCod = null!;
	public required string DwsCod { get => _DwsCod; set { if (_DwsCod != value) { _DwsCod = value; NotifyPropertyChanged();} } }
	private string? _DWSDes;
	public string? DWSDes { get => _DWSDes; set { if (_DWSDes != value) { _DWSDes = value; NotifyPropertyChanged();} } }
}