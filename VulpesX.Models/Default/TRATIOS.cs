namespace VulpesX.Models.Default;
 
public partial class TRATIOS : Base 
{
	private string _ratcod = null!;
	public required string ratcod { get => _ratcod; set { if (_ratcod != value) { _ratcod = value; NotifyPropertyChanged();} } }
	private int _ratprog;
	public int ratprog { get => _ratprog; set { if (_ratprog != value) { _ratprog = value; NotifyPropertyChanged();} } }
	private string? _ratgru;
	public string? ratgru { get => _ratgru; set { if (_ratgru != value) { _ratgru = value; NotifyPropertyChanged();} } }
	private string? _ratcon;
	public string? ratcon { get => _ratcon; set { if (_ratcon != value) { _ratcon = value; NotifyPropertyChanged();} } }
	private string? _ratsott;
	public string? ratsott { get => _ratsott; set { if (_ratsott != value) { _ratsott = value; NotifyPropertyChanged();} } }
	private string? _ratsegn;
	public string? ratsegn { get => _ratsegn; set { if (_ratsegn != value) { _ratsegn = value; NotifyPropertyChanged();} } }
	private string? _ratsegnalt;
	public string? ratsegnalt { get => _ratsegnalt; set { if (_ratsegnalt != value) { _ratsegnalt = value; NotifyPropertyChanged();} } }
	private string _ratsoc = null!;
	public required string ratsoc { get => _ratsoc; set { if (_ratsoc != value) { _ratsoc = value; NotifyPropertyChanged();} } }
	private decimal? _ratval;
	public decimal? ratval { get => _ratval; set { if (_ratval != value) { _ratval = value; NotifyPropertyChanged();} } }
}