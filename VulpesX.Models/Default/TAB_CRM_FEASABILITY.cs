namespace VulpesX.Models.Default;
 
public partial class TAB_CRM_FEASABILITY : Base 
{
	private string _tdecod = null!;
	public required string tdecod { get => _tdecod; set { if (_tdecod != value) { _tdecod = value; NotifyPropertyChanged();} } }
	private string _tdedes = null!;
	public required string tdedes { get => _tdedes; set { if (_tdedes != value) { _tdedes = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}