namespace VulpesX.Models.Default;
 
public partial class TABMESE : Base 
{
	private int _BGMESE;
	public int BGMESE { get => _BGMESE; set { if (_BGMESE != value) { _BGMESE = value; NotifyPropertyChanged();} } }
	private string? _Bgdesme;
	public string? Bgdesme { get => _Bgdesme; set { if (_Bgdesme != value) { _Bgdesme = value; NotifyPropertyChanged();} } }
}