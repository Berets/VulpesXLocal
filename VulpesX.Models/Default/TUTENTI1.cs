namespace VulpesX.Models.Default;
 
public partial class TUTENTI1 : Base 
{
	private string _UTECOD = null!;
	public required string UTECOD { get => _UTECOD; set { if (_UTECOD != value) { _UTECOD = value; NotifyPropertyChanged();} } }
	private string _UTEAPA = null!;
	public required string UTEAPA { get => _UTEAPA; set { if (_UTEAPA != value) { _UTEAPA = value; NotifyPropertyChanged();} } }
	private DateTime? _UTEDIA;
	public DateTime? UTEDIA { get => _UTEDIA; set { if (_UTEDIA != value) { _UTEDIA = value; NotifyPropertyChanged();} } }
	private DateTime? _UTEDFA;
	public DateTime? UTEDFA { get => _UTEDFA; set { if (_UTEDFA != value) { _UTEDFA = value; NotifyPropertyChanged();} } }
	private string? _UTETAU;
	public string? UTETAU { get => _UTETAU; set { if (_UTETAU != value) { _UTETAU = value; NotifyPropertyChanged();} } }
	private string? _UTEIMA;
	public string? UTEIMA { get => _UTEIMA; set { if (_UTEIMA != value) { _UTEIMA = value; NotifyPropertyChanged();} } }
}