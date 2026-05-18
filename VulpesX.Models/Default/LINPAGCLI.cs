namespace VulpesX.Models.Default;
 
public partial class LINPAGCLI : Base 
{
	private string _Lpagcod = null!;
	public required string Lpagcod { get => _Lpagcod; set { if (_Lpagcod != value) { _Lpagcod = value; NotifyPropertyChanged();} } }
	private string _lpaglin = null!;
	public required string lpaglin { get => _lpaglin; set { if (_lpaglin != value) { _lpaglin = value; NotifyPropertyChanged();} } }
	private string? _lpagdes;
	public string? lpagdes { get => _lpagdes; set { if (_lpagdes != value) { _lpagdes = value; NotifyPropertyChanged();} } }
}