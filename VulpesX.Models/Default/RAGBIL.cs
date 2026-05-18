namespace VulpesX.Models.Default;
 
public partial class RAGBIL : Base 
{
	private string _rabsoc = null!;
	public required string rabsoc { get => _rabsoc; set { if (_rabsoc != value) { _rabsoc = value; NotifyPropertyChanged();} } }
	private string _rabcod = null!;
	public required string rabcod { get => _rabcod; set { if (_rabcod != value) { _rabcod = value; NotifyPropertyChanged();} } }
	private string? _rabdes;
	public string? rabdes { get => _rabdes; set { if (_rabdes != value) { _rabdes = value; NotifyPropertyChanged();} } }
	private string? _rabtip;
	public string? rabtip { get => _rabtip; set { if (_rabtip != value) { _rabtip = value; NotifyPropertyChanged();} } }
}