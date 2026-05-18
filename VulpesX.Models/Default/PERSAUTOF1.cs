namespace VulpesX.Models.Default;
 
public partial class PERSAUTOF1 : Base 
{
	private string _PAuSoc = null!;
	public required string PAuSoc { get => _PAuSoc; set { if (_PAuSoc != value) { _PAuSoc = value; NotifyPropertyChanged();} } }
	private string _pCAUCONT = null!;
	public required string pCAUCONT { get => _pCAUCONT; set { if (_pCAUCONT != value) { _pCAUCONT = value; NotifyPropertyChanged();} } }
	private string? _PAuCaF;
	public string? PAuCaF { get => _PAuCaF; set { if (_PAuCaF != value) { _PAuCaF = value; NotifyPropertyChanged();} } }
}