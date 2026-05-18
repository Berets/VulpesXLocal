namespace VulpesX.Models.Default;
 
public partial class CCLEG00 : Base 
{
	private string _cctsoc = null!;
	public required string cctsoc { get => _cctsoc; set { if (_cctsoc != value) { _cctsoc = value; NotifyPropertyChanged();} } }
	private string _cctmod = null!;
	public required string cctmod { get => _cctmod; set { if (_cctmod != value) { _cctmod = value; NotifyPropertyChanged();} } }
	private int _ccdpos;
	public int ccdpos { get => _ccdpos; set { if (_ccdpos != value) { _ccdpos = value; NotifyPropertyChanged();} } }
	private string _cdccod = null!;
	public required string cdccod { get => _cdccod; set { if (_cdccod != value) { _cdccod = value; NotifyPropertyChanged();} } }
	private string? _cccseg;
	public string? cccseg { get => _cccseg; set { if (_cccseg != value) { _cccseg = value; NotifyPropertyChanged();} } }
	private string? _ccsoci;
	public string? ccsoci { get => _ccsoci; set { if (_ccsoci != value) { _ccsoci = value; NotifyPropertyChanged();} } }
}