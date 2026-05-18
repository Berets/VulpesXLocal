namespace VulpesX.Models.Default;
 
public partial class MRPDATE : Base 
{
	private string _MRPDSO = null!;
	public required string MRPDSO { get => _MRPDSO; set { if (_MRPDSO != value) { _MRPDSO = value; NotifyPropertyChanged();} } }
	private DateTime _MRPDDA;
	public DateTime MRPDDA { get => _MRPDDA; set { if (_MRPDDA != value) { _MRPDDA = value; NotifyPropertyChanged();} } }
	private int? _MRPDSE;
	public int? MRPDSE { get => _MRPDSE; set { if (_MRPDSE != value) { _MRPDSE = value; NotifyPropertyChanged();} } }
}