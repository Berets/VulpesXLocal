namespace VulpesX.Models.Default;
 
public partial class TRATIOS1 : Base 
{
	private string _ratsoc = null!;
	public required string ratsoc { get => _ratsoc; set { if (_ratsoc != value) { _ratsoc = value; NotifyPropertyChanged();} } }
	private string _ratcod = null!;
	public required string ratcod { get => _ratcod; set { if (_ratcod != value) { _ratcod = value; NotifyPropertyChanged();} } }
	private string? _ratdes;
	public string? ratdes { get => _ratdes; set { if (_ratdes != value) { _ratdes = value; NotifyPropertyChanged();} } }
	private string? _ratflg;
	public string? ratflg { get => _ratflg; set { if (_ratflg != value) { _ratflg = value; NotifyPropertyChanged();} } }
}