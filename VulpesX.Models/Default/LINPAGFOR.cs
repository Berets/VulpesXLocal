namespace VulpesX.Models.Default;
 
public partial class LINPAGFOR : Base 
{
	private string _Lpafcod = null!;
	public required string Lpafcod { get => _Lpafcod; set { if (_Lpafcod != value) { _Lpafcod = value; NotifyPropertyChanged();} } }
	private string _lpaflin = null!;
	public required string lpaflin { get => _lpaflin; set { if (_lpaflin != value) { _lpaflin = value; NotifyPropertyChanged();} } }
	private string? _lpafdes;
	public string? lpafdes { get => _lpafdes; set { if (_lpafdes != value) { _lpafdes = value; NotifyPropertyChanged();} } }
}