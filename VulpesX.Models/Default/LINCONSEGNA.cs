namespace VulpesX.Models.Default;
 
public partial class LINCONSEGNA : Base 
{
	private string _Lconcod = null!;
	public required string Lconcod { get => _Lconcod; set { if (_Lconcod != value) { _Lconcod = value; NotifyPropertyChanged();} } }
	private string _lconlin = null!;
	public required string lconlin { get => _lconlin; set { if (_lconlin != value) { _lconlin = value; NotifyPropertyChanged();} } }
	private string? _Lcondes;
	public string? Lcondes { get => _Lcondes; set { if (_Lcondes != value) { _Lcondes = value; NotifyPropertyChanged();} } }
}