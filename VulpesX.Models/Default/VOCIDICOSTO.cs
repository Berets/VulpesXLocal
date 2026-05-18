namespace VulpesX.Models.Default;
 
public partial class VOCIDICOSTO : Base 
{
	private string _VdCcod = null!;
	public required string VdCcod { get => _VdCcod; set { if (_VdCcod != value) { _VdCcod = value; NotifyPropertyChanged();} } }
	private string _VdcDes = null!;
	public required string VdcDes { get => _VdcDes; set { if (_VdcDes != value) { _VdcDes = value; NotifyPropertyChanged();} } }
}