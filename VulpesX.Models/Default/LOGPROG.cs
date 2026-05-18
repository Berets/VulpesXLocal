namespace VulpesX.Models.Default;
 
public partial class LOGPROG : Base 
{
	private string _LPUTE = null!;
	public required string LPUTE { get => _LPUTE; set { if (_LPUTE != value) { _LPUTE = value; NotifyPropertyChanged();} } }
	private DateTime _LPDATA;
	public DateTime LPDATA { get => _LPDATA; set { if (_LPDATA != value) { _LPDATA = value; NotifyPropertyChanged();} } }
	private string _LPSOC = null!;
	public required string LPSOC { get => _LPSOC; set { if (_LPSOC != value) { _LPSOC = value; NotifyPropertyChanged();} } }
	private string _LPDES = null!;
	public required string LPDES { get => _LPDES; set { if (_LPDES != value) { _LPDES = value; NotifyPropertyChanged();} } }
	private string _LPPARA = null!;
	public required string LPPARA { get => _LPPARA; set { if (_LPPARA != value) { _LPPARA = value; NotifyPropertyChanged();} } }
}