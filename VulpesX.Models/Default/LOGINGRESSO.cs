namespace VulpesX.Models.Default;
 
public partial class LOGINGRESSO : Base 
{
	private string _LIUTE = null!;
	public required string LIUTE { get => _LIUTE; set { if (_LIUTE != value) { _LIUTE = value; NotifyPropertyChanged();} } }
	private DateTime _LIDATA;
	public DateTime LIDATA { get => _LIDATA; set { if (_LIDATA != value) { _LIDATA = value; NotifyPropertyChanged();} } }
	private string _LISOC = null!;
	public required string LISOC { get => _LISOC; set { if (_LISOC != value) { _LISOC = value; NotifyPropertyChanged();} } }
}