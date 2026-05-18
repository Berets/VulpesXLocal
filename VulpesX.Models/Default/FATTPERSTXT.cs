namespace VulpesX.Models.Default;
 
public partial class FATTPERSTXT : Base 
{
	private string _txtsoci = null!;
	public required string txtsoci { get => _txtsoci; set { if (_txtsoci != value) { _txtsoci = value; NotifyPropertyChanged();} } }
	private int _txtid;
	public int txtid { get => _txtid; set { if (_txtid != value) { _txtid = value; NotifyPropertyChanged();} } }
	private string? _txtdes;
	public string? txtdes { get => _txtdes; set { if (_txtdes != value) { _txtdes = value; NotifyPropertyChanged();} } }
}