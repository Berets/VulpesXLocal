namespace VulpesX.Models.Default;
 
public partial class CFI001P : Base 
{
	private string _cfisoc = null!;
	public required string cfisoc { get => _cfisoc; set { if (_cfisoc != value) { _cfisoc = value; NotifyPropertyChanged();} } }
	private string _cficfi = null!;
	public required string cficfi { get => _cficfi; set { if (_cficfi != value) { _cficfi = value; NotifyPropertyChanged();} } }
	private int _cficod;
	public int cficod { get => _cficod; set { if (_cficod != value) { _cficod = value; NotifyPropertyChanged();} } }
	private int _cfipro;
	public int cfipro { get => _cfipro; set { if (_cfipro != value) { _cfipro = value; NotifyPropertyChanged();} } }
	private DateTime? _cfiinv;
	public DateTime? cfiinv { get => _cfiinv; set { if (_cfiinv != value) { _cfiinv = value; NotifyPropertyChanged();} } }
	private DateTime? _Cfirit;
	public DateTime? Cfirit { get => _Cfirit; set { if (_Cfirit != value) { _Cfirit = value; NotifyPropertyChanged();} } }
}