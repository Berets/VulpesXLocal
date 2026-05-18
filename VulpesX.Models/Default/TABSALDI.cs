namespace VulpesX.Models.Default;
 
public partial class TABSALDI : Base 
{
	private string _salsoc = null!;
	public required string salsoc { get => _salsoc; set { if (_salsoc != value) { _salsoc = value; NotifyPropertyChanged();} } }
	private int _salann;
	public int salann { get => _salann; set { if (_salann != value) { _salann = value; NotifyPropertyChanged();} } }
}