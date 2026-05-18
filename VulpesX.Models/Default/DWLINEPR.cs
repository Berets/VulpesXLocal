namespace VulpesX.Models.Default;
 
public partial class DWLINEPR : Base 
{
	private string _linepr = null!;
	public required string linepr { get => _linepr; set { if (_linepr != value) { _linepr = value; NotifyPropertyChanged();} } }
	private string? _lidescr;
	public string? lidescr { get => _lidescr; set { if (_lidescr != value) { _lidescr = value; NotifyPropertyChanged();} } }
}