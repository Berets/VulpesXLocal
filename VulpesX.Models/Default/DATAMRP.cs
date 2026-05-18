namespace VulpesX.Models.Default;
 
public partial class DATAMRP : Base 
{
	private string _DRPSOC = null!;
	public required string DRPSOC { get => _DRPSOC; set { if (_DRPSOC != value) { _DRPSOC = value; NotifyPropertyChanged();} } }
	private DateTime _DRPINI;
	public DateTime DRPINI { get => _DRPINI; set { if (_DRPINI != value) { _DRPINI = value; NotifyPropertyChanged();} } }
	private DateTime? _DRPORI;
	public DateTime? DRPORI { get => _DRPORI; set { if (_DRPORI != value) { _DRPORI = value; NotifyPropertyChanged();} } }
	private DateTime? _DRPFIN;
	public DateTime? DRPFIN { get => _DRPFIN; set { if (_DRPFIN != value) { _DRPFIN = value; NotifyPropertyChanged();} } }
	private DateTime? _DRPSCA;
	public DateTime? DRPSCA { get => _DRPSCA; set { if (_DRPSCA != value) { _DRPSCA = value; NotifyPropertyChanged();} } }
	private int? _DRPFOR;
	public int? DRPFOR { get => _DRPFOR; set { if (_DRPFOR != value) { _DRPFOR = value; NotifyPropertyChanged();} } }
}