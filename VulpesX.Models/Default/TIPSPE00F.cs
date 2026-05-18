namespace VulpesX.Models.Default;
 
public partial class TIPSPE00F : Base 
{
	private string _tspcod = null!;
	public required string tspcod { get => _tspcod; set { if (_tspcod != value) { _tspcod = value; NotifyPropertyChanged();} } }
	private string? _tspdes;
	public string? tspdes { get => _tspdes; set { if (_tspdes != value) { _tspdes = value; NotifyPropertyChanged();} } }
	private string? _tpsgru;
	public string? tpsgru { get => _tpsgru; set { if (_tpsgru != value) { _tpsgru = value; NotifyPropertyChanged();} } }
	private string? _tpscon;
	public string? tpscon { get => _tpscon; set { if (_tpscon != value) { _tpscon = value; NotifyPropertyChanged();} } }
	private string? _tpssot;
	public string? tpssot { get => _tpssot; set { if (_tpssot != value) { _tpssot = value; NotifyPropertyChanged();} } }
	private string? _tpclfo;
	public string? tpclfo { get => _tpclfo; set { if (_tpclfo != value) { _tpclfo = value; NotifyPropertyChanged();} } }
	private string? _tpsegn;
	public string? tpsegn { get => _tpsegn; set { if (_tpsegn != value) { _tpsegn = value; NotifyPropertyChanged();} } }
	private int? _tpwebcod;
	public int? tpwebcod { get => _tpwebcod; set { if (_tpwebcod != value) { _tpwebcod = value; NotifyPropertyChanged();} } }
}