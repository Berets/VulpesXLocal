namespace VulpesX.Models.Default;
 
public partial class VDCONT : Base 
{
	private string _VdSocc = null!;
	public required string VdSocc { get => _VdSocc; set { if (_VdSocc != value) { _VdSocc = value; NotifyPropertyChanged();} } }
	private string _VdS = null!;
	public required string VdS { get => _VdS; set { if (_VdS != value) { _VdS = value; NotifyPropertyChanged();} } }
	private string _VdGrup = null!;
	public required string VdGrup { get => _VdGrup; set { if (_VdGrup != value) { _VdGrup = value; NotifyPropertyChanged();} } }
	private string _Vdcca = null!;
	public required string Vdcca { get => _Vdcca; set { if (_Vdcca != value) { _Vdcca = value; NotifyPropertyChanged();} } }
	private string _VdSot = null!;
	public required string VdSot { get => _VdSot; set { if (_VdSot != value) { _VdSot = value; NotifyPropertyChanged();} } }
	private string _VdT = null!;
	public required string VdT { get => _VdT; set { if (_VdT != value) { _VdT = value; NotifyPropertyChanged();} } }
	private DateTime _VdDat;
	public DateTime VdDat { get => _VdDat; set { if (_VdDat != value) { _VdDat = value; NotifyPropertyChanged();} } }
}