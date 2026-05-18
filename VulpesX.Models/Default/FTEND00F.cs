namespace VulpesX.Models.Default;
 
public partial class FTEND00F : Base 
{
	private string _TASOCI = null!;
	public required string TASOCI { get => _TASOCI; set { if (_TASOCI != value) { _TASOCI = value; NotifyPropertyChanged();} } }
	private string _TACOAR = null!;
	public required string TACOAR { get => _TACOAR; set { if (_TACOAR != value) { _TACOAR = value; NotifyPropertyChanged();} } }
	private int _TACOCL;
	public int TACOCL { get => _TACOCL; set { if (_TACOCL != value) { _TACOCL = value; NotifyPropertyChanged();} } }
	private string? _TATEND;
	public string? TATEND { get => _TATEND; set { if (_TATEND != value) { _TATEND = value; NotifyPropertyChanged();} } }
	private decimal? _TAQPRE;
	public decimal? TAQPRE { get => _TAQPRE; set { if (_TAQPRE != value) { _TAQPRE = value; NotifyPropertyChanged();} } }
	private DateTime? _TADAUL;
	public DateTime? TADAUL { get => _TADAUL; set { if (_TADAUL != value) { _TADAUL = value; NotifyPropertyChanged();} } }
	private int? _taanno;
	public int? taanno { get => _taanno; set { if (_taanno != value) { _taanno = value; NotifyPropertyChanged();} } }
}