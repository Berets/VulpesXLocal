namespace VulpesX.Models.Default;
 
public partial class MPSDATE : Base 
{
	private string _MPSDSO = null!;
	public required string MPSDSO { get => _MPSDSO; set { if (_MPSDSO != value) { _MPSDSO = value; NotifyPropertyChanged();} } }
	private DateTime _MPSDDA;
	public DateTime MPSDDA { get => _MPSDDA; set { if (_MPSDDA != value) { _MPSDDA = value; NotifyPropertyChanged();} } }
	private int? _MPSDSE;
	public int? MPSDSE { get => _MPSDSE; set { if (_MPSDSE != value) { _MPSDSE = value; NotifyPropertyChanged();} } }
}