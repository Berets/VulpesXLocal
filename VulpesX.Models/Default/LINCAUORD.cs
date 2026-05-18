namespace VulpesX.Models.Default;
 
public partial class LINCAUORD : Base 
{
	private string _Lordcau = null!;
	public required string Lordcau { get => _Lordcau; set { if (_Lordcau != value) { _Lordcau = value; NotifyPropertyChanged();} } }
	private string _lordlin = null!;
	public required string lordlin { get => _lordlin; set { if (_lordlin != value) { _lordlin = value; NotifyPropertyChanged();} } }
	private string? _Lorddes;
	public string? Lorddes { get => _Lorddes; set { if (_Lorddes != value) { _Lorddes = value; NotifyPropertyChanged();} } }
}