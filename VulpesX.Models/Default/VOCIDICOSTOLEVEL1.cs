namespace VulpesX.Models.Default;
 
public partial class VOCIDICOSTOLEVEL1 : Base 
{
	private string _VdCcod = null!;
	public required string VdCcod { get => _VdCcod; set { if (_VdCcod != value) { _VdCcod = value; NotifyPropertyChanged();} } }
	private string _Vdcecod = null!;
	public required string Vdcecod { get => _Vdcecod; set { if (_Vdcecod != value) { _Vdcecod = value; NotifyPropertyChanged();} } }
	private string _Vdctipo = null!;
	public required string Vdctipo { get => _Vdctipo; set { if (_Vdctipo != value) { _Vdctipo = value; NotifyPropertyChanged();} } }
	private decimal _Vdcper;
	public decimal Vdcper { get => _Vdcper; set { if (_Vdcper != value) { _Vdcper = value; NotifyPropertyChanged();} } }
}