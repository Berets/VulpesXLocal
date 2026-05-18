namespace VulpesX.Models.Default;
 
public partial class TIPRDA : Base 
{
	private string _trdcod = null!;
	public required string trdcod { get => _trdcod; set { if (_trdcod != value) { _trdcod = value; NotifyPropertyChanged();} } }
	private string? _trddes;
	public string? trddes { get => _trddes; set { if (_trddes != value) { _trddes = value; NotifyPropertyChanged();} } }
	private string? _trdges;
	public string? trdges { get => _trdges; set { if (_trdges != value) { _trdges = value; NotifyPropertyChanged();} } }
	private string? _trdacq;
	public string? trdacq { get => _trdacq; set { if (_trdacq != value) { _trdacq = value; NotifyPropertyChanged();} } }
	private string? _trdcau;
	public string? trdcau { get => _trdcau; set { if (_trdcau != value) { _trdcau = value; NotifyPropertyChanged();} } }
}