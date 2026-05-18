namespace VulpesX.Models.Default;
 
public partial class MACCHINE : Base 
{
	private string _macsoc = null!;
	public required string macsoc { get => _macsoc; set { if (_macsoc != value) { _macsoc = value; NotifyPropertyChanged();} } }
	private string _maccod = null!;
	public required string maccod { get => _maccod; set { if (_maccod != value) { _maccod = value; NotifyPropertyChanged();} } }
	private string? _macdes;
	public string? macdes { get => _macdes; set { if (_macdes != value) { _macdes = value; NotifyPropertyChanged();} } }
	private int? _macren;
	public int? macren { get => _macren; set { if (_macren != value) { _macren = value; NotifyPropertyChanged();} } }
	private decimal? _maccin;
	public decimal? maccin { get => _maccin; set { if (_maccin != value) { _maccin = value; NotifyPropertyChanged();} } }
	private decimal? _mactst;
	public decimal? mactst { get => _mactst; set { if (_mactst != value) { _mactst = value; NotifyPropertyChanged();} } }
	private decimal? _mactpr;
	public decimal? mactpr { get => _mactpr; set { if (_mactpr != value) { _mactpr = value; NotifyPropertyChanged();} } }
	private int? _macntu;
	public int? macntu { get => _macntu; set { if (_macntu != value) { _macntu = value; NotifyPropertyChanged();} } }
	private int? _macnad;
	public int? macnad { get => _macnad; set { if (_macnad != value) { _macnad = value; NotifyPropertyChanged();} } }
}