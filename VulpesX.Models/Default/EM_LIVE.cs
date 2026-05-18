namespace VulpesX.Models.Default;
 
public partial class EM_LIVE : Base 
{
	private string _SocietaID = null!;
	public required string SocietaID { get => _SocietaID; set { if (_SocietaID != value) { _SocietaID = value; NotifyPropertyChanged();} } }
	private string _DeviceID = null!;
	public required string DeviceID { get => _DeviceID; set { if (_DeviceID != value) { _DeviceID = value; NotifyPropertyChanged();} } }
	private DateTime _Date;
	public DateTime Date { get => _Date; set { if (_Date != value) { _Date = value; NotifyPropertyChanged();} } }
	private decimal? _V;
	public decimal? V { get => _V; set { if (_V != value) { _V = value; NotifyPropertyChanged();} } }
	private decimal? _A;
	public decimal? A { get => _A; set { if (_A != value) { _A = value; NotifyPropertyChanged();} } }
	private decimal? _W;
	public decimal? W { get => _W; set { if (_W != value) { _W = value; NotifyPropertyChanged();} } }
	private decimal? _Wh;
	public decimal? Wh { get => _Wh; set { if (_Wh != value) { _Wh = value; NotifyPropertyChanged();} } }
	private decimal? _KWh;
	public decimal? KWh { get => _KWh; set { if (_KWh != value) { _KWh = value; NotifyPropertyChanged();} } }
	private decimal? _PF;
	public decimal? PF { get => _PF; set { if (_PF != value) { _PF = value; NotifyPropertyChanged();} } }
	private decimal? _Temp;
	public decimal? Temp { get => _Temp; set { if (_Temp != value) { _Temp = value; NotifyPropertyChanged();} } }
}