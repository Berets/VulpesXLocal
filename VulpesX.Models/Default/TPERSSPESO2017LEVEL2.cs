namespace VulpesX.Models.Default;
 
public partial class TPERSSPESO2017LEVEL2 : Base 
{
	private string _socspeso = null!;
	public required string socspeso { get => _socspeso; set { if (_socspeso != value) { _socspeso = value; NotifyPropertyChanged();} } }
	private string _spcauacq = null!;
	public required string spcauacq { get => _spcauacq; set { if (_spcauacq != value) { _spcauacq = value; NotifyPropertyChanged();} } }
	private string _spcauacqdes = null!;
	public required string spcauacqdes { get => _spcauacqdes; set { if (_spcauacqdes != value) { _spcauacqdes = value; NotifyPropertyChanged();} } }
}