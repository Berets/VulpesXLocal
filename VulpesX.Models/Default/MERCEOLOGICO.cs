namespace VulpesX.Models.Default;
 
public partial class MERCEOLOGICO : Base 
{
	private string _smecod = null!;
	public required string smecod { get => _smecod; set { if (_smecod != value) { _smecod = value; NotifyPropertyChanged();} } }
	private string _smedes = null!;
	public required string smedes { get => _smedes; set { if (_smedes != value) { _smedes = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}