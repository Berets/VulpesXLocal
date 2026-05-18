namespace VulpesX.Models.Default;
 
public partial class TABANNO : Base 
{
	private int _BGANNO;
	public int BGANNO { get => _BGANNO; set { if (_BGANNO != value) { _BGANNO = value; NotifyPropertyChanged();} } }
	private string? _Bgdes;
	public string? Bgdes { get => _Bgdes; set { if (_Bgdes != value) { _Bgdes = value; NotifyPropertyChanged();} } }
}