namespace VulpesX.Models.Default;
 
public partial class CAMBI : Base 
{
	private string _CAMLIS = null!;
	public required string CAMLIS { get => _CAMLIS; set { if (_CAMLIS != value) { _CAMLIS = value; NotifyPropertyChanged();} } }
	private string _CAMSWI = null!;
	public required string CAMSWI { get => _CAMSWI; set { if (_CAMSWI != value) { _CAMSWI = value; NotifyPropertyChanged();} } }
	private DateTime _CAMDAT;
	public DateTime CAMDAT { get => _CAMDAT; set { if (_CAMDAT != value) { _CAMDAT = value; NotifyPropertyChanged();} } }
	private string? _CAMDIV;
	public string? CAMDIV { get => _CAMDIV; set { if (_CAMDIV != value) { _CAMDIV = value; NotifyPropertyChanged();} } }
	private int? _CAMORA;
	public int? CAMORA { get => _CAMORA; set { if (_CAMORA != value) { _CAMORA = value; NotifyPropertyChanged();} } }
	private DateTime? _CAMSCA;
	public DateTime? CAMSCA { get => _CAMSCA; set { if (_CAMSCA != value) { _CAMSCA = value; NotifyPropertyChanged();} } }
	private decimal? _CAMACQ;
	public decimal? CAMACQ { get => _CAMACQ; set { if (_CAMACQ != value) { _CAMACQ = value; NotifyPropertyChanged();} } }
	private decimal? _CAMVEN;
	public decimal? CAMVEN { get => _CAMVEN; set { if (_CAMVEN != value) { _CAMVEN = value; NotifyPropertyChanged();} } }
	private string _CAMDES = null!;
	public required string CAMDES { get => _CAMDES; set { if (_CAMDES != value) { _CAMDES = value; NotifyPropertyChanged();} } }
	private byte[]? _rv;
	public byte[]? rv { get => _rv; set { if (_rv != value) { _rv = value; NotifyPropertyChanged();} } }
}