namespace VulpesX.Models.Default;
 
public partial class DWAMM00F : Base 
{
	private string _DwaCod = null!;
	public required string DwaCod { get => _DwaCod; set { if (_DwaCod != value) { _DwaCod = value; NotifyPropertyChanged();} } }
	private string? _DwaDes;
	public string? DwaDes { get => _DwaDes; set { if (_DwaDes != value) { _DwaDes = value; NotifyPropertyChanged();} } }
}