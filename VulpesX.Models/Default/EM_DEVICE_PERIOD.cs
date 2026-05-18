namespace VulpesX.Models.Default;
 
public partial class EM_DEVICE_PERIOD : Base 
{
	private string _SocietaID = null!;
	public required string SocietaID { get => _SocietaID; set { if (_SocietaID != value) { _SocietaID = value; NotifyPropertyChanged();} } }
	private string _DeviceID = null!;
	public required string DeviceID { get => _DeviceID; set { if (_DeviceID != value) { _DeviceID = value; NotifyPropertyChanged();} } }
	private DateTime _Mese;
	public DateTime Mese { get => _Mese; set { if (_Mese != value) { _Mese = value; NotifyPropertyChanged();} } }
	private decimal? _Costo;
	public decimal? Costo { get => _Costo; set { if (_Costo != value) { _Costo = value; NotifyPropertyChanged();} } }
}