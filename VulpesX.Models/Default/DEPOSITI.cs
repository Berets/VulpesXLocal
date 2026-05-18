namespace VulpesX.Models.Default;
 
public partial class DEPOSITI : Base 
{
	private string _depcod = null!;
	public required string depcod { get => _depcod; set { if (_depcod != value) { _depcod = value; NotifyPropertyChanged();} } }
	private string _depdes = null!;
	public required string depdes { get => _depdes; set { if (_depdes != value) { _depdes = value; NotifyPropertyChanged();} } }
	private decimal? _deppvg;
	public decimal? deppvg { get => _deppvg; set { if (_deppvg != value) { _deppvg = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}